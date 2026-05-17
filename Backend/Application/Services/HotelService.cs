using Application.Exceptions;
using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using Application.Interfaces.Hotel;
using Application.Validators.Hotel;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelService> _logger;
        private readonly IMemoryCache _cache;
        private readonly HotelCreateValidator _createValidator;
        private readonly HotelUpdateValidator _updateValidator;

        // Cache constants
        private const string HotelCacheKeyPrefix = "hotel_";
        private const string HotelsListCacheKey = "all_hotels";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public HotelService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<HotelService> logger,
            IMemoryCache cache,
            HotelCreateValidator createValidator,
            HotelUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<HotelResponse> CreateAsync(HotelCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to create new hotel with name {HotelName} at location ({Latitude}, {Longitude})",
                request.HotelName, request.Latitude, request.Longitude);

            // Validate request
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Hotel creation validation failed for {HotelName}: {Errors}",
                    request.HotelName, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                // Check for duplicate
                var exists = await _unitOfWork.Hotels
                    .Query()
                    .AnyAsync(h => h.HotelName == request.HotelName
                                && h.Latitude == request.Latitude
                                && h.Longitude == request.Longitude,
                        cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Duplicate hotel attempt: {HotelName} at location ({Latitude}, {Longitude})",
                        request.HotelName, request.Latitude, request.Longitude);
                    throw new ConflictException($"A hotel with name '{request.HotelName}' already exists at this location");
                }

                var entity = _mapper.Map<Hotel>(request);
                entity.CreatedAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = DateTime.UtcNow;


                await _unitOfWork.Hotels.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateHotelCache();

                var response = _mapper.Map<HotelResponse>(entity);

                _logger.LogInformation("Successfully created hotel {HotelId} with name {HotelName}",
                    entity.Id, entity.HotelName);

                return response;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogError(ex, "Database unique constraint violation while creating hotel {HotelName}",
                    request.HotelName);
                throw new ConflictException($"Hotel '{request.HotelName}' already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating hotel {HotelName}", request.HotelName);
                throw new ServiceException($"Failed to create hotel: {ex.Message}", ex);
            }
        }

        public async Task<HotelResponse> UpdateAsync(int id, HotelUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID", nameof(id));

            _logger.LogInformation("Attempting to update hotel with ID {HotelId}", id);

            // Validate request
            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Hotel update validation failed for ID {HotelId}: {Errors}",
                    id, string.Join(", ", validationResult.Errors));
                throw new ValidationException(validationResult.Errors);
            }

            try
            {
                var entity = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found for update", id);
                    throw new NotFoundException($"Hotel with ID '{id}' not found");
                }

                // Check for duplicate name/location if changed
                if (entity.HotelName != request.HotelName ||
                    entity.Latitude != request.Latitude ||
                    entity.Longitude != request.Longitude)
                {
                    var exists = await _unitOfWork.Hotels
                        .Query()
                        .AnyAsync(h => h.HotelName == request.HotelName
                                    && h.Latitude == request.Latitude
                                    && h.Longitude == request.Longitude
                                    && h.Id != id,
                            cancellationToken);

                    if (exists)
                    {
                        _logger.LogWarning("Duplicate hotel name/location attempt for ID {HotelId}", id);
                        throw new ConflictException($"A hotel with name '{request.HotelName}' already exists at this location");
                    }
                }

                _mapper.Map(request, entity);
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.Hotels.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateHotelCache(id);

                var response = _mapper.Map<HotelResponse>(entity);

                _logger.LogInformation("Successfully updated hotel {HotelId}", id);

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
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while updating hotel {HotelId}", id);
                throw new ConcurrencyException("The hotel was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating hotel {HotelId}", id);
                throw new ServiceException($"Failed to update hotel: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID", nameof(id));

            _logger.LogInformation("Attempting to delete hotel with ID {HotelId}", id);

            try
            {
                var entity = await _unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found for deletion", id);
                    return false;
                }

                // Check for dependencies (bookings)
                //var hasActiveBookings = await _unitOfWork.Bookings
                //    .Query()
                //    .AnyAsync(b => b.HotelId == id && b.Status != "Cancelled" && b.Status != "Completed",
                //        cancellationToken);

                //if (hasActiveBookings)
                //{
                //    _logger.LogWarning("Cannot delete hotel {HotelId} with active bookings", id);
                //    throw new BusinessRuleException("Cannot delete hotel with active bookings");
                //}

                _unitOfWork.Hotels.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Invalidate cache
                InvalidateHotelCache(id);

                _logger.LogInformation("Successfully deleted hotel {HotelId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting hotel {HotelId}", id);
                throw new ServiceException($"Failed to delete hotel: {ex.Message}", ex);
            }
        }

        public async Task<HotelResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid hotel ID", nameof(id));

            _logger.LogDebug("Retrieving hotel with ID {HotelId}", id);

            // Try cache first
            var cacheKey = $"{HotelCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out HotelResponse? cachedHotel) && cachedHotel != null)
            {
                _logger.LogDebug("Cache hit for hotel {HotelId}", id);
                return cachedHotel;
            }

            try
            {
                var entity = await _unitOfWork.Hotels
                    .Query()
                    .IgnoreQueryFilters()
                    .Where(h => h.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("Hotel with ID {HotelId} not found", id);
                    throw new NotFoundException($"Hotel with ID {id} not found");
                }

                var response = _mapper.Map<HotelResponse>(entity);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved hotel {HotelId}", id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving hotel {HotelId}", id);
                throw new ServiceException($"Failed to retrieve hotel: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<HotelResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving all hotels");

            // Try cache
            if (_cache.TryGetValue(HotelsListCacheKey, out IReadOnlyList<HotelResponse>? cachedHotels) && cachedHotels != null)
            {
                _logger.LogDebug("Cache hit for all hotels");
                return cachedHotels;
            }

            try
            {
                var entities = await _unitOfWork.Hotels
                    .Query()
                    .OrderBy(h => h.HotelName)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<HotelResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(HotelsListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} hotels", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all hotels");
                throw new ServiceException($"Failed to retrieve hotels: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<HotelResponse>> FilterAsync(
            int? cityId = null,
            int? minStarRating = null,
            int? maxStarRating = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Filtering hotels with parameters - CityId: {CityId}, MinStarRating: {MinStarRating}, " +
                             "MaxStarRating: {MaxStarRating}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                cityId?.ToString() ?? "Any",
                minStarRating?.ToString() ?? "Any",
                maxStarRating?.ToString() ?? "Any",
                minPrice?.ToString() ?? "Any",
                maxPrice?.ToString() ?? "Any");

            try
            {
                var query = _unitOfWork.Hotels.Query();


                // Apply city filter
                if (cityId.HasValue && cityId.Value > 0)
                {
                    query = query.Where(h => h.CityId == cityId.Value);
                    _logger.LogDebug("Applied city filter: {CityId}", cityId.Value);
                }

                // Apply star rating range filters with validation
                if (minStarRating.HasValue)
                {
                    var minRating = Math.Max(1, Math.Min(5, minStarRating.Value)); // Clamp between 1-5
                    query = query.Where(h => h.StarRating >= minRating);
                    _logger.LogDebug("Applied min star rating filter: {MinStarRating}", minRating);
                }

                if (maxStarRating.HasValue)
                {
                    var maxRating = Math.Max(1, Math.Min(5, maxStarRating.Value)); // Clamp between 1-5
                    query = query.Where(h => h.StarRating <= maxRating);
                    _logger.LogDebug("Applied max star rating filter: {MaxStarRating}", maxRating);
                }

                // Validate and apply price range filters
                if (minPrice.HasValue && minPrice.Value > 0)
                {
                    query = query.Where(h => h.PricePerNight >= minPrice.Value);
                    _logger.LogDebug("Applied min price filter: {MinPrice:C}", minPrice.Value);
                }

                if (maxPrice.HasValue && maxPrice.Value > 0)
                {
                    if (minPrice.HasValue && maxPrice.Value < minPrice.Value)
                    {
                        _logger.LogWarning("Invalid price range: MinPrice ({MinPrice}) > MaxPrice ({MaxPrice})",
                            minPrice.Value, maxPrice.Value);
                        return new List<HotelResponse>(); // Return empty list for invalid range
                    }

                    query = query.Where(h => h.PricePerNight <= maxPrice.Value);
                    _logger.LogDebug("Applied max price filter: {MaxPrice:C}", maxPrice.Value);
                }

                // Execute query with ordering and limits
                var entities = await query
                    .AsNoTracking() // Improve performance for read-only queries
                    .OrderByDescending(h => h.StarRating) // Higher rated first
                    .ThenBy(h => h.PricePerNight)         // Then cheaper first
                    .ThenBy(h => h.HotelName)             // Then alphabetically
                    .Take(50) // Limit search results for performance
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<HotelResponse>>(entities);

                _logger.LogInformation("Hotel filter completed. Found {Count} hotels matching criteria", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering hotels with parameters - CityId: {CityId}, MinStarRating: {MinStarRating}, " +
                                    "MaxStarRating: {MaxStarRating}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                    cityId?.ToString() ?? "Any",
                    minStarRating?.ToString() ?? "Any",
                    maxStarRating?.ToString() ?? "Any",
                    minPrice?.ToString() ?? "Any",
                    maxPrice?.ToString() ?? "Any");
                throw new ServiceException($"Failed to filter hotels: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _unitOfWork.Hotels
                    .Query()
                    .AnyAsync(h => h.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of hotel {HotelId}", id);
                throw new ServiceException($"Failed to check hotel existence: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        private void InvalidateHotelCache(int? specificHotelId = null)
        {
            if (specificHotelId.HasValue)
            {
                var cacheKey = $"{HotelCacheKeyPrefix}{specificHotelId.Value}";
                _cache.Remove(cacheKey);
                _logger.LogDebug("Invalidated cache for hotel {HotelId}", specificHotelId.Value);
            }

            // Always invalidate the list cache when any hotel changes
            _cache.Remove(HotelsListCacheKey);
            _logger.LogDebug("Invalidated all hotels list cache");
        }

        #endregion
    }
}