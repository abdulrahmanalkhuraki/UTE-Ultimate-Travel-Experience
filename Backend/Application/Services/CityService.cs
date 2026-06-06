using Application.DTOs.City.Response;
using Application.DTOs.Hotel.Response;
using Application.Exceptions;
using Application.Interfaces.City;
using Application.Validators.Hotel;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CityService> _logger;
        private readonly IMemoryCache _cache;

        // Cache constants
        private const string CityCacheKeyPrefix = "city_";
        private const string CitiesListCacheKey = "all_citys";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public CityService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CityService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CityResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid City ID", nameof(id));

            _logger.LogDebug("Retrieving City with ID {CityId}", id);

            // Try cache first
            var cacheKey = $"{CityCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out CityResponse? cachedCity) && cachedCity != null)
            {
                _logger.LogDebug("Cache hit for City {CityId}", id);
                return cachedCity;
            }

            try
            {
                var entity = await _unitOfWork.Cities
                    .Query()
                    .IgnoreQueryFilters()
                    .Where(h => h.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("City with ID {CityId} not found", id);
                    throw new NotFoundException($"City with ID {id} not found");
                }

                var response = _mapper.Map<CityResponse>(entity);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved City {CityId}", id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving City {CityId}", id);
                throw new ServiceException($"Failed to retrieve City: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all cities");

            // Try cache
            if (_cache.TryGetValue(CitiesListCacheKey, out IReadOnlyList<CityResponse>? cachedCitys) && cachedCitys != null)
            {
                _logger.LogDebug("Cache hit for all cities");
                return cachedCitys;
            }

            try
            {
                var entities = await _unitOfWork.Cities
                    .Query()
                    .Include(c => c.Country)
                    .Include(c => c.Hotels)
                    .Include(c => c.Attractions)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<CityResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(CitiesListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} cities", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all cities");
                throw new ServiceException($"Failed to retrieve cities: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _unitOfWork.Cities
                    .Query()
                    .AnyAsync(h => h.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of city {CityId}", id);
                throw new ServiceException($"Failed to check city existence: {ex.Message}", ex);
            }
        }
    }
}
