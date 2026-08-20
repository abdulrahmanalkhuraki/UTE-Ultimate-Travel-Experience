using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.City.Response;
using Application.Exceptions;
using Application.Interfaces.City;
using Application.Interfaces.Localization;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizedMapper _mapper;
        private readonly ILanguageContext _language;
        private readonly ILogger<CityService> _logger;
        private readonly IMemoryCache _cache;
        private const string ObjectName = "City";

        // Cache constants
        private const string CityCacheKeyPrefix = "city_";
        private const string CitiesListCacheKey = "all_citys";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public CityService(IUnitOfWork unitOfWork,
            ILocalizedMapper mapper,
            ILanguageContext language,
            ILogger<CityService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _language = language;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CityResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            _logger.StartOperation("Retrieve", ObjectName, id, 0);

            // Try cache first
            var cacheKey = $"{CityCacheKeyPrefix}{id}_{_language.LanguageCode}";
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
                    .Include(c => c.Country)
                        .ThenInclude(c => c.Translations)
                    .Include(c => c.Translations)
                    .Include(c => c.Attractions)
                        .ThenInclude(a => a.Translations)
                    .Where(h => h.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.EntityNotFound(ObjectName, id);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
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

                _logger.SuccessfulOperation("Retrieve", ObjectName);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all cities");

            // Try cache
            var cacheKey = $"{CitiesListCacheKey}_{_language.LanguageCode}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<CityResponse>? cachedCitys) && cachedCitys != null)
            {
                _logger.LogDebug("Cache hit for all cities");
                return cachedCitys;
            }

            try
            {
                var entities = await _unitOfWork.Cities
                    .Query()
                    .Include(c => c.Country)
                        .ThenInclude(c => c.Translations)
                    .Include(c => c.Translations)
                    .Include(c => c.Attractions)
                        .ThenInclude(a => a.Translations)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<CityResponse>>(entities);

                // Cache the result with lower priority
                _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} cities", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve All", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
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
                _logger.ServerError("Check Existence", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("check existence of", ObjectName, ex.Message), ex);
            }
        }
    }
}
