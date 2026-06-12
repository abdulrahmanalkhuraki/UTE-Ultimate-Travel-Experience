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

            var transactionStarted = false;

            try
            {
                var package = await _unitOfWork.TourPackages
                    .Query()
                    .Select(p => new { p.Id, p.PackageName, p.CompanyId , p.PricePerPerson, p.StartDate,p.EndDate })
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
                booking.Payment = payment;
                booking.BookingDate = DateTime.UtcNow;
                booking.Status = BookingStatus.Pending;
                booking.NumberOfAdults = adultCompanions + 1;
                booking.NumberOfChildren = childrenCompanions;
                booking.CreatedAtUtc = DateTime.UtcNow;
                booking.UpdatedAtUtc = DateTime.UtcNow;

                foreach (var companionId in request.CompanionIds)
                {
                    booking.CompanionBookings.Add(new CompanionBooking
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

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .Include(b => b.Payment)
                    .Include(b => b.CompanionBookings)
                        .ThenInclude(cb => cb.Companion)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogDebug("Booking with ID {BookingId} not found", id);
                    throw new NotFoundException($"Booking with ID {id} not found");
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

            try
            {
                var entities = await _unitOfWork.Bookings
                    .Query()
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

        /// <summary>Updates an existing booking's optional fields, companions, and flight type.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="request">The booking update payload.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="BookingResponse"/>.</returns>
        /// <exception cref="ValidationException">Thrown when request validation fails.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="NotFoundException">Thrown when no booking with the given ID exists.</exception>
        /// <exception cref="ConcurrencyException">Thrown when a concurrency conflict is detected.</exception>
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

                _mapper.Map(request, entity);
                entity.UpdatedAtUtc = DateTime.UtcNow;

                if (request.CompanionIds is not null)
                {
                    entity.CompanionBookings.Clear();
                    foreach (var companionId in request.CompanionIds)
                    {
                        entity.CompanionBookings.Add(new CompanionBooking
                        {
                            BookingId = id,
                            CompanionId = companionId
                        });
                    }
                }

                //if (request.FlightType != null)
                //    entity.FlightType = request.FlightType.Value;

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

        /// <summary>Cancels a booking if it is not already cancelled or completed.</summary>
        /// <param name="id">The booking ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns><see langword="true"/> if the booking was cancelled; <see langword="false"/> if it was already cancelled or not found.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is not positive.</exception>
        /// <exception cref="BusinessRuleException">Thrown when attempting to cancel a completed booking.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<bool> CancelAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid booking ID", nameof(id));

            _logger.LogInformation("Attempting to cancel booking with ID {BookingId}", id);

            try
            {
                var entity = await _unitOfWork.Bookings
                    .Query()
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Booking with ID {BookingId} not found for cancellation", id);
                    return false;
                }

                if (entity.Status == BookingStatus.Cancelled)
                {
                    _logger.LogWarning("Booking {BookingId} is already cancelled", id);
                    return false;
                }

                if (entity.Status == BookingStatus.Completed)
                {
                    _logger.LogWarning("Cannot cancel completed booking {BookingId}", id);
                    throw new BusinessRuleException("Cannot cancel a completed booking");
                }

                entity.Status = BookingStatus.Cancelled;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while cancelling booking {BookingId}", id);
                throw new ServiceException($"Failed to cancel booking: {ex.Message}", ex);
            }
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
    }
}
