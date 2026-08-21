using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using Application.DTOs.Pagination;
using Application.Exceptions;
using Application.Interfaces.Booking;
using Application.Interfaces.Localization;
using Application.Interfaces.Notifications;
using Application.Interfaces.User;
using Application.Validators.Booking;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;
namespace Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILanguageContext _language;
        private readonly ILogger<BookingService> _logger;
        private readonly IMemoryCache _cache;
        private readonly BookingCreateValidator _createValidator;
        private readonly BookingUpdateValidator _updateValidator;
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationService _notificationService;

        private const string BookingCacheKeyPrefix = "booking_";
        private const string BookingsListCacheKeyPrefix = "all_bookings_";
        private const string FilteredCacheKeyPrefix = "filtered_";
        private const string UnapprovedCacheKeyPrefix = "unapproved_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan FilterCacheDuration = TimeSpan.FromMinutes(3);
        private static readonly TimeSpan UnapprovedCacheDuration = TimeSpan.FromMinutes(2);

        // Logging Messages constant
        private const string ObjectName = "Booking";

        public BookingService(
            IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
            ILanguageContext language,
            ILogger<BookingService> logger,
            IMemoryCache cache,
            BookingCreateValidator createValidator,
            BookingUpdateValidator updateValidator,
            ICurrentUserService currentUser,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _language = language ?? throw new ArgumentNullException(nameof(language));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public async Task<BookingResponse> CreateAsync(BookingCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var userId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Create", ObjectName, userId);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Create", ObjectName, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            var transactionStarted = false;

            try
            {
                var package = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.Translations)
                    .Include(p => p.Country).ThenInclude(c => c.Translations)
                    .Include(p => p.Company)
                    .Include(p => p.PackageAttractions).ThenInclude(pa => pa.Attraction)
                        .ThenInclude(a => a.City).ThenInclude(c => c.Translations)
                    .Include(p => p.PackageItineraries).ThenInclude(d => d.Translations)
                    .Include(p => p.PackageItineraries).ThenInclude(d => d.Activities)
                        .ThenInclude(a => a.Translations)
                    .FirstOrDefaultAsync(x => x.Id == request.PackageId, cancellationToken);

                // check if package exists
                if (package is null)
                {
                    _logger.EntityNotFound("Tour Package", request.PackageId);
                    throw new NotFoundException(ExceptionMessages.NotFound("Tour Package", request.PackageId));
                }

                var companionIds = request.CompanionIds.ToHashSet();
                var existingCompanions = await _unitOfWork.Companions
                    .Query()
                    .Where(c => companionIds.Contains(c.Id))
                    .Include(c => c.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                    .Include(c => c.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations)
                    .ToListAsync(cancellationToken);

                var adultCompanions = existingCompanions.Count(c => c.Person.Age >= 6);
                var childrenCompanions = existingCompanions.Count - adultCompanions;
                var totalSeatesNeeded = adultCompanions + 1; // user and his/her adult companions

                _EnsureCompanionsExists(existingCompanions, companionIds);
                _EnsureSeatAvailability(package, totalSeatesNeeded);
                await _EnsureNoBookingConflicts(package, cancellationToken);


                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                transactionStarted = true;

                var totalAmount = package.PricePerPerson * totalSeatesNeeded;

                // create payment record
                var payment = new Payment();
                payment.UserId = userId;
                payment.Amount = totalAmount;
                payment.PaymentDate = DateTime.UtcNow;
                payment.PaymentStatus = PaymentStatus.Pending;

                // create booking record
                var booking = _mapper.Map<Booking>(request);
                booking.UserId = userId;
                booking.TourPackageId = package.Id;
                booking.TourPackage = package;
                booking.TotalCost = totalAmount;
                booking.Payment = payment;
                booking.Status = BookingStatus.Pending;
                booking.NumberOfAdults = adultCompanions + 1;
                booking.NumberOfChildren = childrenCompanions;

                foreach (var companionId in request.CompanionIds)
                {
                    booking.CompanionBookings.Add(new Companion_Booking
                    {
                        CompanionId = companionId
                    });
                }

                await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                transactionStarted = false;

                var createdBooking = await QueryWithGraph()
                    .FirstOrDefaultAsync(b => b.Id == booking.Id, cancellationToken)
                    ?? throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, booking.Id));

                var response = _mapper.Map<BookingResponse>(createdBooking);

                _logger.SuccessfulOperation(userId, "Create", ObjectName, booking.Id);

                InvalidateBookingCache(userId: userId, companyId: package.CompanyId);

                // notify user
                await _notificationService.NotifyAsync(
                    userId,
                    $"Your booking for {package.PackageName} has been created successfully. Awaiting acceptance by the tour company.",
                    NotificationType.NewBooking,
                    cancellationToken);

                // notify company
                await _notificationService.NotifyAsync(
                    package.CompanyId,
                    $"New booking received for {package.PackageName}.",
                    NotificationType.NewBooking,
                    cancellationToken);

                return response;
            }
            catch (NotFoundException)
            {
                if (transactionStarted)
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (AuthException)
            {
                if (transactionStarted)
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (ConflictException)
            {
                if (transactionStarted)
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                if (transactionStarted)
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("Creating", ObjectName, ex.Message), ex);
            }
        }

        public async Task<BookingResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogDebug("Retrieving booking with ID {BookingId}", id);

            var cacheKey = $"{BookingCacheKeyPrefix}{id}_{_language.LanguageCode}";
            if (_cache.TryGetValue(cacheKey, out BookingResponse? cached) && cached is not null)
            {
                _logger.LogDebug("Cache hit for booking {BookingId}", id);
                return cached;
            }

            try
            {
                var entity = await QueryWithGraph()
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogDebug("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                _EnsureBookingBelongsToCurrentUser(entity);

                var response = _mapper.Map<BookingResponse>(entity);

                _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogDebug("Successfully retrieved booking {BookingId}", id);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
                throw new ServiceException($"Failed to retrieve booking: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedUserBookingsResponse> GetAllAsync(int userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(ExceptionMessages.InvalidPagination());

            _logger.LogDebug($"Retrieving all bookings for user with Id = {userId}");

            var listCacheKey = $"{BookingsListCacheKeyPrefix}{userId}_page{page}_size{pageSize}_{_language.LanguageCode}";

            if (_cache.TryGetValue(listCacheKey, out PaginatedUserBookingsResponse? cached) && cached is not null)
            {
                _logger.LogDebug("Cache hit for all bookings of user {UserId}", userId);
                return cached;
            }

            try
            {
                var query = _unitOfWork.Bookings
                    .Query()
                    .Where(b => b.UserId == userId);

                var totalAmountSpent = (await query
                    .AsNoTracking()
                    .Where(b => b.Status != BookingStatus.Pending &&
                    b.Status != BookingStatus.Cancelled &&
                    b.Status != BookingStatus.Rejected_By_Company &&
                    b.Status != BookingStatus.Rejected_By_Tourist)
                    .SumAsync(b => b.TotalCost, cancellationToken)) ?? 0m;

                var totalItemsCount = await query
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                var entities = await query
                    .Include(b => b.TourPackage)
                    .Include(b => b.Payment)
                    .Include(b => b.CompanionBookings)
                        .ThenInclude(cb => cb.Companion)
                            .ThenInclude(cb => cb.Person)
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<BookingResponse>>(entities);

                var response = new PaginatedUserBookingsResponse
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItemsCount
                    },
                    TotalAmountSpent = totalAmountSpent
                };

                _cache.Set(listCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} bookings", items.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                throw new ServiceException($"Failed to retrieve bookings: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedResponse<BookingResponse>> FilterAsync(BookingStatus? status, int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(ExceptionMessages.InvalidPagination());

            _logger.LogDebug("Retrieving bookings with status filter");

            var userId = _currentUser.UserId ?? throw new AuthException("You must be loged in to preform this action.");

            var filterCacheKey = status.HasValue
                ? $"{FilteredCacheKeyPrefix}{userId}_{status.Value}_page{page}_size{pageSize}_{_language.LanguageCode}"
                : $"{FilteredCacheKeyPrefix}{userId}_all_page{page}_size{pageSize}_{_language.LanguageCode}";

            if (_cache.TryGetValue(filterCacheKey, out PaginatedResponse<BookingResponse>? cached) && cached is not null)
            {
                _logger.LogDebug("Cache hit for filtered bookings of user {UserId} with status {Status}", userId, status);
                return cached;
            }

            try
            {
                var query = _unitOfWork.Bookings
                    .Query()
                    .Where(b => b.UserId == userId);

                if (status.HasValue)
                {
                    query = query.Where(b => b.Status == status.Value);
                }

                var totalItemsCount = await query
                    .AsNoTracking()
                    .CountAsync(cancellationToken);

                var entities = await query
                    .Include(b => b.TourPackage)
                    .Include(b => b.Payment)
                    .Include(b => b.CompanionBookings)
                        .ThenInclude(cb => cb.Companion)
                    .AsNoTracking()
                    .OrderByDescending(b => b.BookingDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<BookingResponse>>(entities);

                var response = new PaginatedResponse<BookingResponse>
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItemsCount
                    }
                };

                _cache.Set(filterCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = FilterCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogDebug("Successfully retrieved {Count} bookings with filter {Status}", items.Count, status);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings with filter {Status}", status);
                throw new ServiceException($"Failed to retrieve bookings: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedResponse<BookingResponse>> GetUnApprovedAsync(int page, int pageSize,
            int? packageId, CancellationToken cancellationToken)
        {
            if (packageId.HasValue && packageId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("Tour Package"), nameof(packageId));

            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(ExceptionMessages.InvalidPagination());

            var userId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());

            if (packageId.HasValue)
                _logger.LogDebug("Retrieving Pending Bookings For Package {PackageId}", packageId);
            else
                _logger.LogDebug("Retrieving All Pending Bookings");

            try
            {
                var company = await _unitOfWork.TourCompanies
                .FirstOrDefaultAsync(tc => tc.UserId == userId, cancellationToken);

                if (company is null)
                {
                    _logger.LogWarning("User with id {userId} is not associated with any tour company", userId);
                    throw new ForbiddenException("You are not associated with any tour company.");
                }

                var unapprovedCacheKey = packageId.HasValue
                    ? $"{UnapprovedCacheKeyPrefix}{company.Id}_{packageId.Value}_page{page}_size{pageSize}_{_language.LanguageCode}"
                    : $"{UnapprovedCacheKeyPrefix}{company.Id}_all_page{page}_size{pageSize}_{_language.LanguageCode}";

                if (_cache.TryGetValue(unapprovedCacheKey, out PaginatedResponse<BookingResponse>? cached) && cached is not null)
                {
                    _logger.LogDebug("Cache hit for unapproved bookings of company {CompanyId}", company.Id);
                    return cached;
                }

                var query = _unitOfWork.Bookings
                            .Query()
                            .Where(b => b.TourPackage.CompanyId == company.Id)
                            .Where(b => b.Status == BookingStatus.Pending);

                if (packageId.HasValue)
                {
                    bool exists = await _unitOfWork.TourPackages
                        .AnyAsync(t => t.Id == packageId.Value && t.CompanyId == company.Id, cancellationToken);

                    if (!exists)
                    {
                        _logger.LogWarning("Tour Package With ID = {PackageId} Not Found For Your Company.", packageId);
                        throw new NotFoundException($"Tour Package With ID = {packageId} not found for your company.");
                    }

                    query = query.Where(b => b.TourPackageId == packageId);
                }

                var entities = await query
                            .Include(b => b.TourPackage)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.NationalityCountry)
                                        .ThenInclude(n => n.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.ResidentialCity)
                                        .ThenInclude(c => c.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Role)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                                    .ThenInclude(c => c.Person)
                            .OrderByDescending(b => b.BookingDate)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                var totalItemsCount = await query.CountAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<BookingResponse>>(entities);

                var paginationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItemsCount
                };

                var response = new PaginatedResponse<BookingResponse> { Items = items, Pagination = paginationMetadata };

                _cache.Set(unapprovedCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = UnapprovedCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.SuccessfulOperation("retrieved", $"UnApproved {ObjectName}s");
                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Pending bookings");
                throw new ServiceException($"Failed to retrieve bookings: {ex.Message}", ex);
            }
        }

        public async Task<BookingResponse> ApproveAsync(int id, BookingApproveRequest approveRequest, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogInformation("Attempting to approve booking with ID {BookingId}", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.NationalityCountry)
                                        .ThenInclude(n => n.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.ResidentialCity)
                                        .ThenInclude(c => c.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Role)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                                    .ThenInclude(c => c.Person)
                            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (booking is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                var userId = _currentUser.UserId
                    ?? throw new AuthException("You must be logged in to perform this action.");


                var company = await _unitOfWork.TourCompanies
                    .FirstOrDefaultAsync(tc => tc.UserId == userId, cancellationToken);

                if (company is null)
                {
                    _logger.LogWarning("User with id {userId} is not associated with any tour company", userId);
                    throw new ForbiddenException("You are not associated with any tour company.");
                }

                if (booking.TourPackage.CompanyId != company.Id)
                {
                    _logger.LogWarning(
                        "Authorization failed: Company '{CompanyId}' ('{CompanyName}') attempted to approve booking '{BookingId}' " +
                        "which belongs to company '{BookingCompanyId}' ('{BookingCompanyName}'). " +
                        "Access denied due to company mismatch. User: '{UserId}'",
                        company.Id,
                        company.Name ?? "Unknown",
                        booking.Id,
                        booking.TourPackage.CompanyId,
                        booking.TourPackage.Company?.Name ?? "Unknown",
                        _currentUser.UserId);

                    throw new ForbiddenException(
                        $"You do not have permission to approve booking '{booking.Id}'. " +
                        $"This booking belongs to a different tour company.");
                }

                _EnsureBookingIsPending(booking, BookingOperation.Approve);

                // Validate payment exists
                if (booking.Payment is null)
                {
                    _logger.LogError("Booking {BookingId} has no associated payment record", id);
                    throw new BusinessRuleException($"Cannot approve booking {id}: No payment record found.");
                }

                string userNotificationMessage;

                // Booking without Preferences, No Additional Costs
                if (booking.RoomTypePreference == null &&
                    booking.DietaryRequirements == null &&
                    booking.SpecialRequests == null)
                {
                    booking.Status = BookingStatus.Confirmed;
                    booking.Payment.PaymentStatus = PaymentStatus.Completed;
                    userNotificationMessage = $"Booking #{booking.Id} has been approved" +
                        $". Thank you for booking with us.";
                }
                else // booking with Preferences
                {
                    if (!approveRequest.NewCalculatedCost.HasValue)
                    {
                        _logger.LogWarning("NewCalculatedCost is missing but required because the tourist provided preferences (RoomTypePreference: {RoomPref}, DietaryRequirements: {Dietary}, SpecialRequests: {SpecialRequests}) for booking {BookingId}",
                            booking.RoomTypePreference, booking.DietaryRequirements, booking.SpecialRequests, id);
                        throw new BusinessRuleException("Cannot approve booking with preferences. The new calculated cost is required because additional charges or adjustments may apply based on the tourist's requests. Please provide the updated total cost.");
                    }

                    booking.Status = BookingStatus.Accepted_By_Company;
                    booking.TotalCost = approveRequest.NewCalculatedCost.Value;

                    // Payment status remains pending until user accepts the new cost
                    userNotificationMessage = $"Booking #{booking.Id} has been accepted by the tour company with an updated total cost of {approveRequest.NewCalculatedCost.Value:C}. Please review and confirm to finalize your booking.";
                }

                // Deduct total seats: 1 for the primary booker plus all companions
                booking.TourPackage.AvailableSeats -= (booking.CompanionBookings.Count + 1);

                _unitOfWork.TourPackages.Update(booking.TourPackage);
                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(booking.Id, booking.UserId, booking.TourPackage.CompanyId, booking.TourPackageId);

                await _notificationService.NotifyAsync(booking.UserId, userNotificationMessage, NotificationType.BookingApproved, cancellationToken);

                _logger.LogInformation("Booking {BookingId} successfully approved with status {Status}", id, booking.Status);
                return _mapper.Map<BookingResponse>(booking);
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while approving booking {BookingId}", id);
                throw new ServiceException($"Failed to approve booking: {ex.Message}", ex);
            }
        }

        public async Task<BookingResponse> RejectAsync(int id, BookingRejectRequest rejectRequest, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            if (string.IsNullOrWhiteSpace(rejectRequest?.RejectReason))
                throw new ArgumentException("Rejection reason is required", nameof(rejectRequest));

            _logger.LogInformation("Attempting to Reject booking with ID {BookingId}", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (booking is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                var userId = _currentUser.UserId
                    ?? throw new AuthException("You must be logged in to perform this action.");

                var company = await _unitOfWork.TourCompanies
                    .FirstOrDefaultAsync(tc => tc.UserId == userId, cancellationToken);

                if (company is null)
                {
                    _logger.LogWarning("User with id {userId} is not associated with any tour company", userId);
                    throw new ForbiddenException("You are not associated with any tour company.");
                }

                if (booking.TourPackage.CompanyId != company.Id)
                {
                    _logger.LogWarning(
                        "Authorization failed: Company '{CompanyId}' ('{CompanyName}') attempted to reject booking '{BookingId}' " +
                        "which belongs to company '{BookingCompanyId}' ('{BookingCompanyName}'). " +
                        "Access denied due to company mismatch. User: '{UserId}'",
                        company.Id,
                        company.Name ?? "Unknown",
                        booking.Id,
                        booking.TourPackage.CompanyId,
                        booking.TourPackage.Company?.Name ?? "Unknown",
                        _currentUser.UserId);

                    throw new ForbiddenException(
                        $"You do not have permission to reject booking '{booking.Id}'. " +
                        $"This booking belongs to a different tour company.");
                }


                _EnsureBookingIsPending(booking, BookingOperation.Reject);

                booking.Status = BookingStatus.Rejected_By_Company;
                booking.RejectReason = rejectRequest.RejectReason;

                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(booking.Id, booking.UserId, booking.TourPackage.CompanyId, booking.TourPackageId);

                var userNotificationMessage = $"Booking #{booking.Id} has been rejected by the tour company. " +
                    $"Reason: {rejectRequest.RejectReason}. " +
                    $"Please contact support for further assistance or to make a new booking.";

                await _notificationService.NotifyAsync(booking.UserId, userNotificationMessage, NotificationType.BookingRejected, cancellationToken);

                _logger.LogInformation("booking {BookingId} Successfully Rejected", id);
                return _mapper.Map<BookingResponse>(booking);
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while Rejecting booking {BookingId}", id);
                throw new ServiceException($"Failed to Reject booking: {ex.Message}", ex);
            }
        }

        public async Task<BookingResponse> ConfirmAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));


            _logger.LogInformation("Attempting to Confirm booking with ID {BookingId} by tourist", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                                .ThenInclude(b => b.Company)
                            .Include(b => b.Payment)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.NationalityCountry)
                                        .ThenInclude(n => n.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.ResidentialCity)
                                        .ThenInclude(c => c.Translations)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
                if (booking is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                _EnsureBookingBelongsToCurrentUser(booking);

                if (booking.Status != BookingStatus.Accepted_By_Company)
                {
                    _logger.LogWarning("Booking {BookingId} is Not Approved By Tour Company", id);
                    throw new BusinessRuleException($"Cannot Accept booking {id}. Current status is '{booking.Status}'. " +
                        $"Acceptance only allowed when status is '{BookingStatus.Accepted_By_Company}'.");
                }

                booking.Status = BookingStatus.Confirmed;
                booking.Payment.PaymentStatus = PaymentStatus.Completed;

                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(booking.Id, booking.UserId, booking.TourPackage.CompanyId, booking.TourPackageId);

                var tourCompanyNotificationMessage = $"✓ Booking #{booking.Id} " +
                    $"confirmed by {booking.User.Person?.Fullname}. " +
                    $"Tour: {booking.TourPackage.PackageName}. " +
                    $"Total: {booking.TotalCost:C}. Ready to proceed.";

                await _notificationService.NotifyAsync(booking.TourPackage.Company.UserId,
                    tourCompanyNotificationMessage,
                    NotificationType.BookingConfirmed,
                    cancellationToken);

                _logger.LogInformation("booking {BookingId} Successfully Confirmed", id);
                return _mapper.Map<BookingResponse>(booking);
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while Confirming booking {BookingId}", id);
                throw new ServiceException($"Failed to Confirm booking: {ex.Message}", ex);
            }
        }

        public async Task<BookingResponse> DeclineAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));


            _logger.LogInformation("Attempting to Decline booking with ID {BookingId} by tourist", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                                .ThenInclude(b => b.Company)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.NationalityCountry)
                                        .ThenInclude(n => n.Translations)
                            .Include(b => b.User)
                                .ThenInclude(u => u.Person)
                                    .ThenInclude(p => p.ResidentialCity)
                                        .ThenInclude(c => c.Translations)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (booking is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                _EnsureBookingBelongsToCurrentUser(booking);

                if (booking.Status != BookingStatus.Accepted_By_Company)
                {
                    _logger.LogWarning("Booking {BookingId} is Not Approved By Tour Company", id);
                    throw new BusinessRuleException($"Cannot Decline booking {id}. Current status is '{booking.Status}'. " +
                        $"Declines are only allowed when status is '{BookingStatus.Accepted_By_Company}'.");
                }

                booking.Status = BookingStatus.Rejected_By_Tourist;
                booking.Payment.PaymentStatus = PaymentStatus.Cancelled;

                booking.TourPackage.AvailableSeats += (booking.CompanionBookings.Count + 1);

                _unitOfWork.TourPackages.Update(booking.TourPackage);
                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(booking.Id, booking.UserId, booking.TourPackage.CompanyId, booking.TourPackageId);

                var tourCompanyNotificationMessage = $"✗ Booking #{booking.Id} " +
                    $"declined by {booking.User.Person?.Fullname}. Booking cancelled.";

                await _notificationService.NotifyAsync(
                    booking.TourPackage.Company.UserId,
                    tourCompanyNotificationMessage,
                    NotificationType.BookingDeclined,
                    cancellationToken);

                _logger.LogInformation("booking {BookingId} Successfully Declined", id);
                return _mapper.Map<BookingResponse>(booking);
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not BusinessRuleException)
            {
                _logger.LogError(ex, "Unexpected error when Decline booking {BookingId}", id);
                throw new ServiceException($"Failed to Decline booking: {ex.Message}", ex);
            }
        }

        public async Task<BookingResponse> UpdateAsync(int id, BookingUpdateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogInformation("Attempting to update booking with ID {BookingId}", id);

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Booking update validation failed for ID {BookingId}: {Errors}",
                    id, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }


            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.CompanionBookings)
                    .Include(b => b.TourPackage)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found for update", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                _EnsureBookingBelongsToCurrentUser(entity);

                // prevent editing if the booking status isn't pending
                _EnsureBookingIsPending(entity, BookingOperation.Update);


                var companionIds = request.CompanionIds.ToHashSet();
                var existingCompanions = await _unitOfWork.Companions
                    .Query()
                    .Where(c => companionIds.Contains(c.Id))
                    .Include(c => c.Person)
                    .ToListAsync(cancellationToken);


                _EnsureCompanionsExists(existingCompanions, companionIds);
                _EnsureSeatAvailability(entity.TourPackage, entity.CompanionBookings.Count + 1);

                _mapper.Map(request, entity);
                entity.UpdatedAtUtc = DateTime.UtcNow;

                if (request.CompanionIds is not null)
                {
                    entity.CompanionBookings.Clear();
                    foreach (var companionId in request.CompanionIds)
                    {
                        entity.CompanionBookings.Add(new Companion_Booking
                        {
                            BookingId = id,
                            CompanionId = companionId
                        });
                    }
                }

                _unitOfWork.Bookings.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(entity.Id, entity.UserId, entity.TourPackage?.CompanyId);

                var response = _mapper.Map<BookingResponse>(entity);

                _logger.LogInformation("Successfully updated booking {BookingId}", id);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while updating booking {BookingId}", id);
                throw new ConcurrencyException("The booking was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating booking {BookingId}", id);
                throw new ServiceException($"Failed to update booking: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogInformation("Attempting to cancel booking with ID {BookingId}", id);

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(e => e.Payment)
                    .Include(b => b.TourPackage)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);


                if (entity is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found for cancellation", id);
                    throw new NotFoundException($"Booking with ID {id} not found for cancellation");
                }

                _EnsureBookingBelongsToCurrentUser(entity);
                _EnsureBookingIsPending(entity, BookingOperation.Cancel);

                entity.Status = BookingStatus.Cancelled;
                entity.Payment.PaymentStatus = PaymentStatus.Cancelled;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Bookings.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(entity.Id, entity.UserId, entity.TourPackage?.CompanyId);

                _logger.LogInformation("Successfully cancelled booking {BookingId}", id);

                return true;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling booking {BookingId}", id);
                throw new ServiceException($"Failed to cancel booking: {ex.Message}", ex);
            }
        }


        #region Helpers

        private IQueryable<Booking> QueryWithGraph() =>
            _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(b => b.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.NationalityCountry).ThenInclude(n => n.Translations)
                .Include(b => b.User).ThenInclude(u => u.Person)
                    .ThenInclude(p => p.ResidentialCity).ThenInclude(c => c.Translations)
                .Include(b => b.User).ThenInclude(u => u.Role)
                .Include(b => b.Payment)
                .Include(b => b.TourPackage).ThenInclude(p => p.Translations)
                .Include(b => b.TourPackage).ThenInclude(p => p.CabinClasses)
                .Include(b => b.TourPackage).ThenInclude(p => p.Media)
                .Include(b => b.TourPackage).ThenInclude(p => p.TourPackageGuides)
                .ThenInclude(tg => tg.TouristGuide).ThenInclude(tg => tg.Person)
                .Include(b => b.TourPackage).ThenInclude(p => p.Country)
                    .ThenInclude(c => c.Translations)
                .Include(b => b.TourPackage).ThenInclude(p => p.Company)
                .Include(b => b.TourPackage).ThenInclude(p => p.PackageAttractions)
                    .ThenInclude(pa => pa.Attraction).ThenInclude(a => a.City)
                        .ThenInclude(c => c.Translations)
                .Include(b => b.TourPackage).ThenInclude(p => p.PackageItineraries)
                    .ThenInclude(d => d.Translations)
                .Include(b => b.TourPackage).ThenInclude(p => p.PackageItineraries)
                    .ThenInclude(d => d.Activities).ThenInclude(a => a.Translations)
                .Include(b => b.CompanionBookings).ThenInclude(cb => cb.Companion)
                    .ThenInclude(c => c.Person).ThenInclude(p => p.NationalityCountry)
                        .ThenInclude(n => n.Translations)
                .Include(b => b.CompanionBookings).ThenInclude(cb => cb.Companion)
                    .ThenInclude(c => c.Person).ThenInclude(p => p.ResidentialCity)
                        .ThenInclude(c => c.Translations);

        private static bool Conflict(
            DateOnly firstBookingStartDate,
            DateOnly firstBookingEndDate,
            DateOnly secondBookingStartDate,
            DateOnly secondBookingEndDate)
        {
            return !(firstBookingEndDate < secondBookingStartDate
                || firstBookingStartDate > secondBookingEndDate);
        }

        private void InvalidateBookingCache(int? bookingId = null, int? userId = null, int? companyId = null, int? packageId = null)
        {
            foreach (var lang in LanguageCodes.Supported)
            {
                if (bookingId.HasValue)
                {
                    _cache.Remove($"{BookingCacheKeyPrefix}{bookingId.Value}_{lang}");
                }

                if (userId.HasValue)
                {
                    _cache.Remove($"{BookingsListCacheKeyPrefix}{userId.Value}_{lang}");
                    _cache.Remove($"{FilteredCacheKeyPrefix}{userId.Value}_all_{lang}");
                }
            }

            if (companyId.HasValue)
            {
                // Unapproved cache keys are paginated; clear every combination.
                foreach (var lang in LanguageCodes.Supported)
                {
                    for (var page = 1; page <= 100; page++)
                    {
                        for (var size = 1; size <= 100; size++)
                        {
                            var unapprovedKey = packageId.HasValue
                                ? $"{UnapprovedCacheKeyPrefix}{companyId.Value}_{packageId.Value}_page{page}_size{size}_{lang}"
                                : $"{UnapprovedCacheKeyPrefix}{companyId.Value}_all_page{page}_size{size}_{lang}";
                            _cache.Remove(unapprovedKey);
                        }
                    }
                }
            }
        }

        private void _EnsureBookingIsPending(Booking entity, BookingOperation operation)
        {
            // Validate input
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!Enum.IsDefined(typeof(BookingOperation), operation))
                throw new ArgumentException($"Invalid operation: {operation}", nameof(operation));

            // Check booking status
            if (entity.Status != BookingStatus.Pending)
            {
                var operationName = GetOperationDisplayName(operation);

                _logger.LogWarning(
                    "Business rule violation: Attempted to perform '{Operation}' on booking '{BookingId}' with status '{CurrentStatus}'. " +
                    "This operation is only allowed for bookings in 'Pending' status. " +
                    "User: '{UserId}', Timestamp: {Timestamp}",
                    operationName,
                    entity.Id,
                    entity.Status,
                    _currentUser.UserId,
                    DateTime.UtcNow);

                throw new BusinessRuleException(
                    $"Cannot {operationName.ToLower()} booking '{entity.Id}'. " +
                    $"The booking is currently in '{entity.Status}' status. " +
                    $"Only bookings with 'Pending' status can be {operationName.ToLower()}ed.");
            }
        }

        private string GetOperationDisplayName(BookingOperation operation)
        {
            return operation switch
            {
                BookingOperation.Update => "Update",
                BookingOperation.Reject => "Reject",
                BookingOperation.Approve => "Approve",
                BookingOperation.Cancel => "Cancel",
                _ => throw new ArgumentOutOfRangeException(nameof(operation), $"Unknown operation: {operation}")
            };
        }

        private void _EnsureBookingBelongsToCurrentUser(Booking entity)
        {
            int userId = _currentUser.UserId ??
                throw new AuthException("You must be logged in to perform this action.");

            if (entity.UserId != userId)
            {
                _logger.LogWarning(
                    "Authorization failed: Current user '{CurrentUserId}' attempted to access booking '{BookingId}' which belongs to user '{BookingUserId}'. " +
                    "Access denied due to ownership mismatch.",
                    userId,
                    entity.Id,
                    entity.UserId);

                throw new ForbiddenException(
                    $"You do not have permission to perform this operation on booking '{entity.Id}'. " +
                    "The booking belongs to a different user.");
            }
        }

        private void _EnsureCompanionsExists(List<Companion> existingCompanions, HashSet<int> companionIds)
        {
            var foundIds = existingCompanions.Select(c => c.Id).ToHashSet();
            var missingIds = companionIds.Except(foundIds).ToList();
            if (missingIds.Count != 0)
            {
                _logger.LogWarning("Companions with IDs [{MissingIds}] not found",
                    string.Join(", ", missingIds));
                throw new NotFoundException($"Companions with IDs [{string.Join(", ", missingIds)}] not found");
            }
        }

        private void _EnsureSeatAvailability(TourPackage package, int totalSeatsNeeded)
        {
            // Validate inputs
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            if (totalSeatsNeeded <= 0)
                throw new ArgumentException($"Number of seats needed must be positive. Received: {totalSeatsNeeded}", nameof(totalSeatsNeeded));

            // Check seat availability
            if (package.AvailableSeats < totalSeatsNeeded)
            {
                _logger.LogWarning(
                    "Insufficient seat availability: Requested {RequestedSeats} seats for tour package '{PackageId}' ('{PackageName}'), " +
                    "but only {AvailableSeats} seats are available. Shortage: {Shortage} seats.",
                    totalSeatsNeeded,
                    package.Id,
                    package.PackageName,
                    package.AvailableSeats,
                    totalSeatsNeeded - package.AvailableSeats);

                throw new BusinessRuleException(
                    $"Not enough seats available for tour package '{package.Id}'. " +
                    $"You requested {totalSeatsNeeded} seat(s), but only {package.AvailableSeats} seat(s) are available. " +
                    $"Please reduce the number of seats or choose a different package.");
            }
        }

        private async Task _EnsureNoBookingConflicts(TourPackage package, CancellationToken ct)
        {
            // check if this booking conflict with other bookings
            var UserBookingsPackages = await _unitOfWork.Bookings
                .Query()
                .Where(b => b.UserId == _currentUser.UserId)
                .Where(b => b.Status != BookingStatus.Completed &&
                            b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.Rejected_By_Company &&
                            b.Status != BookingStatus.Rejected_By_Tourist &&
                            b.Status != BookingStatus.No_Show)
                .Include(b => b.TourPackage)
                .Select(b => b.TourPackage)
                .ToListAsync(ct);

            // user bookings packages that conflict with the created booking package
            var PackageConflicts = UserBookingsPackages.Where(
                p => Conflict(
                package.StartDate,
                package.EndDate,
                p.StartDate,
                p.EndDate));

            if (PackageConflicts.Any())
            {
                var p = PackageConflicts.First();

                _logger.LogWarning($"User {_currentUser.UserId} attempted to book package '{package.PackageName}' " +
                    $"(from {package.StartDate:d} to {package.EndDate:d})" +
                    " but it conflicts with their existing booking for package " +
                    $"'{p.PackageName}' (from {p.StartDate:d} to {p.EndDate:d}).");
                throw new ConflictException($"You already have a booking that overlaps with this package's dates.");
            }
        }

        #endregion
    }

    public enum BookingOperation
    {
        Update = 0,
        Reject = 1,
        Approve = 2,
        Cancel = 3
    }
}
