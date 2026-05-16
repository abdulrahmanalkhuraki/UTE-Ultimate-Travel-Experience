using Application.DTOs.Flight.Request;
using Application.DTOs.Flight.Response;
using Application.Exceptions;
using Application.Interfaces.Flight;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FlightService> _logger;
        private readonly IMemoryCache _cache;
        private readonly FlightCreateValidator _createValidator;
        private readonly FlightUpdateValidator _updateValidator;

        // Cache constants
        private const string FlightCacheKeyPrefix = "flight_";
        private const string FlightsListCacheKey = "all_flights";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public FlightService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FlightService> logger,
            IMemoryCache cache,
            FlightCreateValidator createValidator,
            FlightUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<FlightResponse> CreateAsync(FlightCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new flight {FlightNumber} from City {DepartureCityId} to {ArrivalCityId}",
                request.FlightNumber, request.DepartureCityId, request.ArrivalCityId);

            // Validate request
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Flight creation validation failed for {FlightNumber}: {Errors}",
                    request.FlightNumber, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                // Check for duplicate flight (same flight number on same day)
                var exists = await _unitOfWork.Flights
                    .Query()
                    .AnyAsync(f => f.FlightNumber == request.FlightNumber
                                && f.Departure.Date == request.Departure.Date,
                        cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Duplicate flight attempt: {FlightNumber} on {DepartureDate}",
                        request.FlightNumber, request.Departure.Date);
                    throw new ConflictException($"A flight with number '{request.FlightNumber}' already exists on this date");
                }

                var entity = _mapper.Map<Flight>(request);
                entity.CreatedAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                await _unitOfWork.Flights.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);


                // Invalidate cache
                InvalidateFlightCache();

                var response = _mapper.Map<FlightResponse>(entity);

                _logger.LogInformation("Successfully created flight {FlightId} with number {FlightNumber}",
                    entity.Id, entity.FlightNumber);

                return response;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogError(ex, "Database unique constraint violation while creating flight {FlightNumber}",
                    request.FlightNumber);
                throw new ConflictException($"Flight '{request.FlightNumber}' already exists for this date");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating flight {FlightNumber}", request.FlightNumber);
                throw new ServiceException($"Failed to create flight: {ex.Message}", ex);
            }
        }

        public async Task<FlightResponse> UpdateAsync(int id, FlightUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (id <= 0)
                throw new ArgumentException("Invalid flight ID", nameof(id));

            _logger.LogInformation("Attempting to update flight with ID {FlightId}", id);

            // Validate request
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Flight update validation failed for ID {FlightId}: {Errors}",
                    id, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var entity = await _unitOfWork.Flights
                    .Query()
                    .Include(f => f.DepartureCity)
                    .Include(f => f.ArrivalCity)
                    .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("Flight with ID {FlightId} not found for update", id);
                    throw new NotFoundException($"Flight with ID '{id}' not found");
                }

                // Check for duplicate flight number on same date if changed
                if (entity.FlightNumber != request.FlightNumber ||
                    entity.Departure.Date != request.Departure.Date)
                {
                    var exists = await _unitOfWork.Flights
                        .Query()
                        .AnyAsync(f => f.FlightNumber == request.FlightNumber
                                    && f.Departure.Date == request.Departure.Date
                                    && f.Id != id,
                            cancellationToken);

                    if (exists)
                    {
                        _logger.LogWarning("Duplicate flight number/date attempt for ID {FlightId}", id);
                        throw new ConflictException($"A flight with number '{request.FlightNumber}' already exists on this date");
                    }
                }

                // Check if flight has any confirmed bookings
                //var hasConfirmedBookings = await _unitOfWork.Bookings
                //    .Query()
                //    .AnyAsync(b => b.FlightId == id && b.Status == "Confirmed", cancellationToken);

                //if (hasConfirmedBookings && (request.Departure != entity.Departure || request.Arrival != entity.Arrival))
                //{
                //    _logger.LogWarning("Attempting to change flight schedule with confirmed bookings for ID {FlightId}", id);
                //    throw new BusinessRuleException("Cannot change flight schedule because there are confirmed bookings. Please cancel bookings first.");
                //}

                _mapper.Map(request, entity);
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Flights.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateFlightCache(id);

                var response = _mapper.Map<FlightResponse>(entity);

                _logger.LogInformation("Successfully updated flight {FlightId}", id);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while updating flight {FlightId}", id);
                throw new ConcurrencyException("The flight was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating flight {FlightId}", id);
                throw new ServiceException($"Failed to update flight: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flight ID", nameof(id));

            _logger.LogInformation("Attempting to delete flight with ID {FlightId}", id);

            try
            {
                var entity = await _unitOfWork.Flights.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Flight with ID {FlightId} not found for deletion", id);
                    return false;
                }

                // Check for any bookings (not just confirmed)
                //var hasAnyBookings = await _unitOfWork.Bookings
                //    .Query()
                //    .AnyAsync(b => b.FlightId == id, cancellationToken);

                //if (hasAnyBookings)
                //{
                //    _logger.LogWarning("Cannot delete flight {FlightId} with existing bookings", id);
                //    throw new BusinessRuleException("Cannot delete flight with existing bookings. Please cancel or remove bookings first.");
                //}

                // Check if departure is in the past
                if (entity.Departure < DateTime.UtcNow)
                {
                    _logger.LogWarning("Cannot delete flight {FlightId} that has already departed", id);
                    throw new BusinessRuleException("Cannot delete a flight that has already departed.");
                }

                _unitOfWork.Flights.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateFlightCache(id);

                _logger.LogInformation("Successfully deleted flight {FlightId}", id);
                return true;
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting flight {FlightId}", id);
                throw new ServiceException($"Failed to delete flight: {ex.Message}", ex);
            }
        }

        public async Task<FlightResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flight ID", nameof(id));

            _logger.LogDebug("Retrieving flight with ID {FlightId}", id);

            // Try cache first
            var cacheKey = $"{FlightCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out FlightResponse? cachedFlight) && cachedFlight != null)
            {
                _logger.LogDebug("Cache hit for flight {FlightId}", id);
                return cachedFlight;
            }

            try
            {
                var entity = await _unitOfWork.Flights
                    .Query()
                    .IgnoreQueryFilters()
                    .Where(f => f.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("Flight with ID {FlightId} not found", id);
                    throw new NotFoundException($"Flight with ID {id} not found");
                }

                var response = _mapper.Map<FlightResponse>(entity);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved flight {FlightId}", id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flight {FlightId}", id);
                throw new ServiceException($"Failed to retrieve flight: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<FlightResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving all flights");

            // Try cache
            if (_cache.TryGetValue(FlightsListCacheKey, out IReadOnlyList<FlightResponse>? cachedFlights) && cachedFlights != null)
            {
                _logger.LogDebug("Cache hit for all flights");
                return cachedFlights;
            }

            try
            {
                var entities = await _unitOfWork.Flights
                    .Query()
                    .OrderBy(f => f.Departure)
                    .ThenBy(f => f.FlightNumber)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<FlightResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(FlightsListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} flights", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all flights");
                throw new ServiceException($"Failed to retrieve flights: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _unitOfWork.Flights
                    .Query()
                    .AnyAsync(f => f.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of flight {FlightId}", id);
                throw new ServiceException($"Failed to check flight existence: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<FlightResponse>> FilterAsync(
            string? airline = null,
            int? departureCityId = null,
            int? arrivalCityId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Filtering flights with parameters - Airline: {Airline}, DepartureCityId: {DepartureCityId}, " +
                             "ArrivalCityId: {ArrivalCityId}, FromDate: {FromDate}, ToDate: {ToDate}, " +
                             "MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                airline ?? "Any", departureCityId ?? 0, arrivalCityId ?? 0,
                fromDate?.ToString("yyyy-MM-dd") ?? "Any", toDate?.ToString("yyyy-MM-dd") ?? "Any",
                minPrice?.ToString() ?? "Any", maxPrice?.ToString() ?? "Any");

            try
            {
                var query = _unitOfWork.Flights.Query();


                // Apply filters
                if (!string.IsNullOrWhiteSpace(airline))
                {
                    query = query.Where(f => f.Airline.ToLower().Contains(airline.ToLower()));
                    _logger.LogDebug("Applied airline filter: {Airline}", airline);
                }

                if (departureCityId.HasValue && departureCityId.Value > 0)
                {
                    query = query.Where(f => f.DepartureCityId == departureCityId.Value);
                    _logger.LogDebug("Applied departure city filter: {DepartureCityId}", departureCityId.Value);
                }

                if (arrivalCityId.HasValue && arrivalCityId.Value > 0)
                {
                    query = query.Where(f => f.ArrivalCityId == arrivalCityId.Value);
                    _logger.LogDebug("Applied arrival city filter: {ArrivalCityId}", arrivalCityId.Value);
                }

                if (fromDate.HasValue)
                {
                    var fromDateTime = fromDate.Value.Date;
                    query = query.Where(f => f.Departure >= fromDateTime);
                    _logger.LogDebug("Applied from date filter: {FromDate}", fromDateTime);
                }

                if (toDate.HasValue)
                {
                    var toDateTime = toDate.Value.Date.AddDays(1); // Include the entire end date
                    query = query.Where(f => f.Departure <= toDateTime);
                    _logger.LogDebug("Applied to date filter: {ToDate}", toDate.Value);
                }

                if (minPrice.HasValue && minPrice.Value > 0)
                {
                    query = query.Where(f => f.Price >= minPrice.Value);
                    _logger.LogDebug("Applied min price filter: {MinPrice}", minPrice.Value);
                }

                if (maxPrice.HasValue && maxPrice.Value > 0)
                {
                    query = query.Where(f => f.Price <= maxPrice.Value);
                    _logger.LogDebug("Applied max price filter: {MaxPrice}", maxPrice.Value);
                }

                // Only show future flights that haven't departed yet
                 query = query.Where(f => f.Departure > DateTime.UtcNow);

                // Execute query with ordering
                var entities = await query
                    .OrderBy(f => f.Departure)          // Soonest flights first
                    .ThenBy(f => f.Price)               // Then cheapest flights
                    .ThenBy(f => f.FlightNumber)        // Then by flight number
                    .Take(100)                          // Limit results for performance
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<FlightResponse>>(entities);

                _logger.LogInformation("Filter completed. Found {Count} flights matching criteria", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering flights with parameters - Airline: {Airline}, DepartureCityId: {DepartureCityId}, " +
                                     "ArrivalCityId: {ArrivalCityId}, FromDate: {FromDate}, ToDate: {ToDate}",
                    airline ?? "Any", departureCityId ?? 0, arrivalCityId ?? 0,
                    fromDate?.ToString("yyyy-MM-dd") ?? "Any", toDate?.ToString("yyyy-MM-dd") ?? "Any");
                throw new ServiceException($"Failed to filter flights: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        private void InvalidateFlightCache(int? specificFlightId = null)
        {
            if (specificFlightId.HasValue)
            {
                var cacheKey = $"{FlightCacheKeyPrefix}{specificFlightId.Value}";
                _cache.Remove(cacheKey);
                _logger.LogDebug("Invalidated cache for flight {FlightId}", specificFlightId.Value);
            }

            // Always invalidate the list cache when any flight changes
            _cache.Remove(FlightsListCacheKey);
            _logger.LogDebug("Invalidated all flights list cache");
        }

        #endregion
    }
}