using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using Application.Exceptions;
using Application.Interfaces.Booking;
using Application.Interfaces.Notifications;
using Application.Interfaces.User;
using Application.Validators.Booking;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        private readonly IMemoryCache _cache;
        private readonly BookingCreateValidator _createValidator;
        private readonly BookingUpdateValidator _updateValidator;
        private readonly ICurrentUserService _currentUser;
        private readonly INotificationService _notificationService;

        private const string BookingCacheKeyPrefix = "booking_";
        private const string BookingsListCacheKey = "all_bookings";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public BookingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BookingService> logger,
            IMemoryCache cache,
            BookingCreateValidator createValidator,
            BookingUpdateValidator updateValidator,
            ICurrentUserService currentUser,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        /// <summary>Creates a new booking with payment and companion links.</summary>
        /// <param name="request">The booking creation payload.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The newly created <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ValidationException">Thrown when request validation fails.</exception>
        /// <exception cref="NotFoundException">Thrown when the package or a companion ID is not found.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ConflictException">Thrown when the user attempts to book a package whose dates overlap with one of their existing bookings.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> CreateAsync(BookingCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new booking");

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Booking creation validation failed: {Errors}",
                    string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            await EnsureProfileCompletedAsync(cancellationToken);

            var transactionStarted = false;

            try
            {
                var package = await _unitOfWork.TourPackages
                    .Query()
                    .FirstOrDefaultAsync(x => x.Id == request.PackageId, cancellationToken);

                // check if package exists
                if (package is null)
                {
                    _logger.LogWarning("Package with ID {PackageId} not found", request.PackageId);
                    throw new NotFoundException($"Package with ID {request.PackageId} not found");
                }

                var companionIds = request.CompanionIds.ToHashSet();
                var existingCompanions = await _unitOfWork.Companions
                    .Query()
                    .Where(c => companionIds.Contains(c.Id))
                    .ToListAsync(cancellationToken);

                var foundIds = existingCompanions.Select(c => c.Id).ToHashSet();
                var missingIds = companionIds.Except(foundIds).ToList();
                if (missingIds.Count != 0)
                {
                    _logger.LogWarning("Companions with IDs [{MissingIds}] not found",
                        string.Join(", ", missingIds));
                    throw new NotFoundException($"Companions with IDs [{string.Join(", ", missingIds)}] not found");
                }

                var adultCompanions = existingCompanions.Count(c => IsAdult(c.DateOfBirth));
                var childrenCompanions = existingCompanions.Count - adultCompanions;

                var userId = _currentUser.UserId
                    ?? throw new AuthException("You must be logged in to create a booking");

                // check if this booking conflict with other bookings
                var UserBookingsPackages = await _unitOfWork.Bookings
                    .Query()
                    .Where(b => b.UserId == userId)
                    .Where(b => b.Status != BookingStatus.Completed &&
                                b.Status != BookingStatus.Cancelled &&
                                b.Status != BookingStatus.Rejected_By_Company &&
                                b.Status != BookingStatus.Rejected_By_Tourist &&
                                b.Status != BookingStatus.No_Show)
                    .Include(b => b.TourPackage)
                    .Select(b => b.TourPackage)
                    .ToListAsync();

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

                    _logger.LogWarning($"User {userId} attempted to book package '{package.PackageName}' " +
                        $"(from {package.StartDate:d} to {package.EndDate:d})" +
                        " but it conflicts with their existing booking for package " +
                        $"'{p.PackageName}' (from {p.StartDate:d} to {p.EndDate:d}).");
                    throw new ConflictException($"You already have a booking that overlaps with this package's dates.");
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                transactionStarted = true;

                var totalAmount = package.PricePerPerson * (adultCompanions + 1 + childrenCompanions);

                var payment = new Payment();
                payment.UserId = userId;
                payment.Amount = totalAmount;
                payment.PaymentDate = DateTime.UtcNow;
                payment.PaymentStatus = PaymentStatus.Pending;
                await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

                var booking = _mapper.Map<Booking>(request);
                booking.UserId = userId;
                booking.TourPackageId = package.Id;
                booking.TourPackage = package;
                booking.TotalCost = totalAmount;
                booking.Payment = payment;
                booking.BookingDate = DateTime.UtcNow;
                booking.Status = BookingStatus.Pending;
                booking.NumberOfAdults = adultCompanions + 1;
                booking.NumberOfChildren = childrenCompanions;
                booking.CreatedAtUtc = DateTime.UtcNow;
                booking.UpdatedAtUtc = DateTime.UtcNow;


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

                var response = _mapper.Map<BookingResponse>(booking);

                _logger.LogInformation("Successfully created booking {BookingId}", booking.Id);

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
                _logger.LogError(ex, "Unexpected error while creating booking");
                throw new ServiceException($"Failed to create booking: {ex.Message}", ex);
            }
        }

        /// <summary>Retrieves a booking by its ID, including payment and companion details.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The matching <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogDebug("Retrieving booking with ID {BookingId}", id);

            var cacheKey = $"{BookingCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out BookingResponse? cached) && cached is not null)
            {
                _logger.LogDebug("Cache hit for booking {BookingId}", id);
                return cached;
            }

            await EnsureProfileCompletedAsync(cancellationToken);

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.TourPackage)
                    .Include(b => b.Payment)
                    .Include(b => b.CompanionBookings)
                        .ThenInclude(cb => cb.Companion)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogDebug("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                if (entity.UserId != _currentUser.UserId)
                {
                    _logger.LogWarning($"User with Id {_currentUser.UserId} Cannot Get booking with Id {entity.Id} " +
                           $"because it belongs to another user.");
                    throw new ForbiddenException("You do not have permission to access this booking. This booking belongs to another user.");
                }

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

        /// <summary>Retrieves all bookings ordered by booking date descending.</summary>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>A read-only list of <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all bookings");

            if (_cache.TryGetValue(BookingsListCacheKey, out IReadOnlyList<BookingResponse>? cached) && cached is not null)
            {
                _logger.LogDebug("Cache hit for all bookings");
                return cached;
            }

            await EnsureProfileCompletedAsync(cancellationToken);

            try
            {
                var entities = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.TourPackage)
                    .Include(b => b.Payment)
                    .Include(b => b.CompanionBookings)
                        .ThenInclude(cb => cb.Companion)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<BookingResponse>>(entities);

                _cache.Set(BookingsListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} bookings", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                throw new ServiceException($"Failed to retrieve bookings: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Retrieves pending bookings for the current tour company's packages.
        /// </summary>
        /// <param name="packageId">Optional. If provided, only pending bookings for this specific package are returned.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>A read-only list of pending <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="packageId"/> is zero or negative.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ForbiddenException">Thrown when the current user is not associated with any tour company.</exception>
        /// <exception cref="NotFoundException">Thrown when the specified <paramref name="packageId"/> does not exist for the current company.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<IReadOnlyList<BookingResponse>> GetUnApprovedAsync(int? packageId, CancellationToken cancellationToken)
        {
            if (packageId.HasValue && packageId <= 0)
                throw new ArgumentException("Invalid package ID", nameof(packageId));

            await EnsureProfileCompletedAsync(cancellationToken);

            var userId = _currentUser.UserId
                ?? throw new AuthException("You must be logged in to perform this action.");

            var company = await _unitOfWork.TourCompanies
                .FirstOrDefaultAsync(tc => tc.UserId == userId, cancellationToken);

            if (company is null)
                throw new ForbiddenException("You are not associated with any tour company.");

            if (packageId.HasValue)
                _logger.LogDebug("Retrieving Pending Bookings For Package {PackageId}", packageId);
            else
                _logger.LogDebug("Retrieving All Pending Bookings");

            try
            {
                List<Booking> entities;
                if (packageId.HasValue)
                {
                    bool exists = await _unitOfWork.TourPackages
                        .AnyAsync(t => t.Id == packageId.Value && t.CompanyId == company.Id, cancellationToken);

                    if (!exists)
                    {
                        _logger.LogWarning("Tour Package With ID = {PackageId} Not Found For Your Company.", packageId);
                        throw new NotFoundException($"Tour Package With ID = {packageId} not found for your company.");
                    }

                    entities = await _unitOfWork.Bookings
                            .Query()
                            .Where(b => b.TourPackage.CompanyId == company.Id)
                            .Where(b => b.TourPackageId == packageId)
                            .Where(b => b.Status == BookingStatus.Pending)
                            .Include(b => b.TourPackage)
                            .Include(b => b.User)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                            .OrderByDescending(b => b.BookingDate)
                            .ToListAsync(cancellationToken);
                }
                else
                {
                    entities = await _unitOfWork.Bookings
                            .Query()
                            .Where(b => b.TourPackage.CompanyId == company.Id)
                            .Where(b => b.Status == BookingStatus.Pending)
                            .Include(b => b.TourPackage)
                            .Include(b => b.User)
                            .Include(b => b.Payment)
                            .Include(b => b.CompanionBookings)
                                .ThenInclude(cb => cb.Companion)
                            .OrderByDescending(b => b.BookingDate)
                            .ToListAsync(cancellationToken);
                }

                var response = _mapper.Map<IReadOnlyList<BookingResponse>>(entities);
                _logger.LogDebug("Successfully retrieved {Count} bookings", response.Count);
                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Pending bookings");
                throw new ServiceException($"Failed to retrieve bookings: {ex.Message}", ex);
            }
        }

        /// <summary>Approves or conditionally accepts a pending booking on behalf of the tour company.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="approveRequest">The approval payload. Must include <see cref="BookingApproveRequest.NewCalculatedCost"/> when the tourist specified preferences.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ForbiddenException">Thrown when the current user's company does not own the booking's tour package.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="BusinessRuleException">Thrown when the booking status is not <see cref="BookingStatus.Pending"/> or when required data is missing.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> ApproveAsync(int id, BookingApproveRequest approveRequest, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            await EnsureProfileCompletedAsync(cancellationToken);

            _logger.LogInformation("Attempting to approve booking with ID {BookingId}", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                            .Include(b => b.User)
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

                if (company is null || booking.TourPackage.CompanyId != company.Id)
                    throw new ForbiddenException("You do not have permission to approve this booking.");

                if (booking.Status != BookingStatus.Pending)
                {
                    _logger.LogWarning("Booking {BookingId} is not pending. Current status: {Status}", id, booking.Status);
                    throw new BusinessRuleException($"Cannot approve booking {id}. Current status is '{booking.Status}'. " +
                        $"Approvals are only allowed when status is '{BookingStatus.Pending}'.");
                }

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
                        throw new BusinessRuleException("Cannot confirm booking with preferences. The new calculated cost is required because additional charges or adjustments may apply based on the tourist's requests. Please provide the updated total cost.");
                    }

                    booking.Status = BookingStatus.Accepted_By_Company;
                    booking.TotalCost = approveRequest.NewCalculatedCost.Value;

                    // Payment status remains pending until user accepts the new cost
                    userNotificationMessage = $"Booking #{booking.Id} has been accepted by the tour company with an updated total cost of {approveRequest.NewCalculatedCost.Value:C}. Please review and confirm to finalize your booking.";
                }

                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(id);

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

        /// <summary>Rejects a pending booking  with a reason.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="rejectRequest">The rejection payload. Must include a non-empty <see cref="BookingRejectRequest.RejectReason"/>.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive or the rejection reason is missing.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ForbiddenException">Thrown when the current user's company does not own the booking's tour package.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="BusinessRuleException">Thrown when the booking status is not <see cref="BookingStatus.Pending"/>.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> RejectAsync(int id, BookingRejectRequest rejectRequest, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            if (string.IsNullOrWhiteSpace(rejectRequest?.RejectReason))
                throw new ArgumentException("Rejection reason is required", nameof(rejectRequest));

            await EnsureProfileCompletedAsync(cancellationToken);

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

                if (company is null || booking.TourPackage.CompanyId != company.Id)
                    throw new ForbiddenException("You do not have permission to reject this booking.");

                if (booking.Status != BookingStatus.Pending)
                {
                    _logger.LogWarning("Booking {BookingId} is Not Pending", id);
                    throw new BusinessRuleException($"Cannot Reject booking {id}. Current status is '{booking.Status}'. " +
                        $"Rejections are only allowed when status is '{BookingStatus.Pending}'.");
                }

                booking.Status = BookingStatus.Rejected_By_Company;
                booking.RejectReason = rejectRequest.RejectReason;

                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(id);

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

        /// <summary>Confirms a booking after the company has accepted it (tourist action).</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ForbiddenException">Thrown when the current user is not the owner of this booking.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="BusinessRuleException">Thrown when the booking status is not <see cref="BookingStatus.Accepted_By_Company"/>.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> ConfirmAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            await EnsureProfileCompletedAsync(cancellationToken);

            _logger.LogInformation("Attempting to Confirm booking with ID {BookingId} by tourist", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                                .ThenInclude(b => b.Company)
                            .Include(b => b.Payment)
                            .Include(b => b.User)
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

                if (booking.UserId != userId)
                    throw new ForbiddenException("You do not have permission to confirm this booking.");

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

                InvalidateBookingCache(id);

                var tourCompanyNotificationMessage = $"✓ Booking #{booking.Id} " +
                    $"confirmed by {booking.User.Fullname}. " +
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

        /// <summary>Declines a booking after the company has accepted it (tourist action).</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="AuthException">Thrown when the user is not authenticated.</exception>
        /// <exception cref="ForbiddenException">Thrown when the current user is not the owner of this booking.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="BusinessRuleException">Thrown when the booking status is not <see cref="BookingStatus.Accepted_By_Company"/>.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<BookingResponse> DeclineAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            await EnsureProfileCompletedAsync(cancellationToken);

            _logger.LogInformation("Attempting to Decline booking with ID {BookingId} by tourist", id);

            try
            {
                var booking = await _unitOfWork.Bookings
                            .Query()
                            .Include(b => b.TourPackage)
                                .ThenInclude(b => b.Company)
                            .Include(b => b.User)
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

                if (booking.UserId != userId)
                    throw new ForbiddenException("You do not have permission to decline this booking.");

                if (booking.Status != BookingStatus.Accepted_By_Company)
                {
                    _logger.LogWarning("Booking {BookingId} is Not Approved By Tour Company", id);
                    throw new BusinessRuleException($"Cannot Decline booking {id}. Current status is '{booking.Status}'. " +
                        $"Declines are only allowed when status is '{BookingStatus.Accepted_By_Company}'.");
                }

                booking.Status = BookingStatus.Rejected_By_Tourist;
                booking.Payment.PaymentStatus = PaymentStatus.Cancelled;

                _unitOfWork.Bookings.Update(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(id);

                var tourCompanyNotificationMessage = $"✗ Booking #{booking.Id} " +
                    $"declined by {booking.User.Fullname}. Booking cancelled.";

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when Decline booking {BookingId}", id);
                throw new ServiceException($"Failed to Decline booking: {ex.Message}", ex);
            }
        }

        /// <summary>Updates an existing booking's optional fields, companions, and flight type.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="request">The booking update payload.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ValidationException">Thrown when request validation fails.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="ConcurrencyException">Thrown when a concurrency conflict is detected.</exception>
        /// <exception cref="BusinessRuleException">Thrown when Update Operation Not Allowed.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
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

            await EnsureProfileCompletedAsync(cancellationToken);

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.CompanionBookings)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found for update", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
                }

                if (entity.UserId != _currentUser.UserId)
                {
                    _logger.LogWarning($"User with Id {_currentUser.UserId} Cannot Update booking with Id {entity.Id} " +
                           $"because it belongs to another user.");
                    throw new ForbiddenException("You do not have permission to Update this booking. This booking belongs to another user.");
                }

                // prevent editing if the booking status isn't pending
                if (entity.Status != BookingStatus.Pending)
                {
                    _logger.LogWarning("Update attempted on booking {BookingId} with status {CurrentStatus}. " +
                                       "Only pending bookings can be updated.",
                                       id, entity.Status);

                    throw new BusinessRuleException(
                        $"Cannot update booking {id}. Current status is '{entity.Status}'. " +
                        $"Updates are only allowed when status is '{BookingStatus.Pending}'.");
                }

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

                InvalidateBookingCache(id);

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

        /// <summary>Cancels a booking if it's pending.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns><see langword="true"/> if the booking was cancelled; <see langword="false"/> if it was already cancelled or not found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="BusinessRuleException">Thrown when attempting to cancel not pending booking.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            await EnsureProfileCompletedAsync(cancellationToken);

            _logger.LogInformation("Attempting to cancel booking with ID {BookingId}", id);

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(e => e.Payment)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);


                if (entity is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found for cancellation", id);
                    throw new NotFoundException($"Booking with ID {id} not found for cancellation");
                }

                if (entity.UserId != _currentUser.UserId)
                {
                    _logger.LogWarning($"User with Id {_currentUser.UserId} Cannot Cancel booking with Id {entity.Id} " +
                           $"because it belongs to another user.");
                    throw new ForbiddenException("You do not have permission to Cancel this booking. This booking belongs to another user.");
                }

                if (entity.Status != BookingStatus.Pending)
                {
                    _logger.LogWarning("Booking {BookingId} is already cancelled", id);
                    throw new BusinessRuleException($"Cannot Cancel booking {id}. Current status is '{entity.Status}'. " +
                        $"Cancellation are only allowed when status is '{BookingStatus.Pending}'.");
                }

                entity.Status = BookingStatus.Cancelled;
                entity.Payment.PaymentStatus = PaymentStatus.Cancelled;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Bookings.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateBookingCache(id);

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

        private async Task EnsureProfileCompletedAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId
                ?? throw new AuthException("You must be logged in to perform this action.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user is null || !user.IsProfileCompleted)
                throw new BusinessRuleException("You must complete your profile before performing this action.");
        }

        private static bool IsAdult(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age >= 18;
        }

        private static bool Conflict(
            DateOnly firstBookingStartDate,
            DateOnly firstBookingEndDate,
            DateOnly secondBookingStartDate,
            DateOnly secondBookingEndDate)
        {
            return !(firstBookingEndDate < secondBookingStartDate
                || firstBookingStartDate > secondBookingEndDate);
        }

        private void InvalidateBookingCache(int? specificBookingId = null)
        {
            if (specificBookingId.HasValue)
            {
                var cacheKey = $"{BookingCacheKeyPrefix}{specificBookingId.Value}";
                _cache.Remove(cacheKey);
            }

            _cache.Remove(BookingsListCacheKey);
        }
        #endregion
    }
}
