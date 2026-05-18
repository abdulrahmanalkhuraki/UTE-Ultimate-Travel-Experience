using Application.DTOs.Country.Response;
using Application.Exceptions;
using Application.Interfaces.Country;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;
        private readonly IMemoryCache _cache;

        // Cache constants
        private const string CountryCacheKeyPrefix = "country_";
        private const string CountriesListCacheKey = "all_countries";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public CountryService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CountryService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CountryResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Country ID", nameof(id));

            _logger.LogDebug("Retrieving Country with ID {CountryId}", id);

            // Try cache first
            var cacheKey = $"{CountryCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out CountryResponse? cachedCountry) && cachedCountry != null)
            {
                _logger.LogDebug("Cache hit for Country {CountryId}", id);
                return cachedCountry;
            }

            try
            {
                var entity = await _unitOfWork.Countries
                    .Query()
                    .IgnoreQueryFilters()
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("Country with ID {CountryId} not found", id);
                    throw new NotFoundException($"Country with ID {id} not found");
                }

                var response = _mapper.Map<CountryResponse>(entity);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved Country {CountryId}", id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Country {CountryId}", id);
                throw new ServiceException($"Failed to retrieve Country: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<CountryResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all countries");

            // Try cache
            if (_cache.TryGetValue(CountriesListCacheKey, out IReadOnlyList<CountryResponse>? cachedCountries) && cachedCountries != null)
            {
                _logger.LogDebug("Cache hit for all countries");
                return cachedCountries;
            }

            try
            {
                var entities = await _unitOfWork.Countries
                    .Query()
                    .Include(c => c.Cities)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<CountryResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(CountriesListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} countries", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all countries");
                throw new ServiceException($"Failed to retrieve countries: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return false;

            try
            {
                return await _unitOfWork.Countries
                    .Query()
                    .AnyAsync(c => c.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking existence of country {CountryId}", id);
                throw new ServiceException($"Failed to check country existence: {ex.Message}", ex);
            }
        }
    }
}