using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.Exceptions;
using Application.Interfaces.Companion;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CompanionService : ICompanionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanionService> _logger;
        private readonly IMemoryCache _cache;

        // Cache constants
        private const string CompanionCacheKeyPrefix = "companion_";
        private const string CompanionsListCacheKey = "all_companions";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public CompanionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CompanionService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>Creates a new companion for a user.</summary>
        /// <param name="request">The companion creation payload.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The newly created <see cref="CompanionResponse"/> with computed fields.</returns>
        /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the user, nationality country, or residential country is not found.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<CompanionResponse> CreateAsync(CompanionCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            //_logger.LogInformation("Attempting to create new companion for user {UserId}", request.UserId);

            try
            {
                // Verify user exists
                //var userExists = await _unitOfWork.Users
                    //.Query()
                    //.AnyAsync(u => u.Id == request.UserId, cancellationToken);

                //if (!userExists)
                //{
                    //_logger.LogWarning("User with ID {UserId} not found", request.UserId);
                    //throw new NotFoundException($"User with ID {request.UserId} not found");
                //}

                // Verify nationality country exists
                var nationalityCountryExists = await _unitOfWork.Countries
                    .Query()
                    .AnyAsync(c => c.Id == request.NationalityCountryId, cancellationToken);

                if (!nationalityCountryExists)
                {
                    _logger.LogWarning("Nationality country with ID {CountryId} not found", request.NationalityCountryId);
                    throw new NotFoundException($"Nationality country with ID {request.NationalityCountryId} not found");
                }

                // Verify residential country exists
                var residentialCountryExists = await _unitOfWork.Countries
                    .Query()
                    .AnyAsync(c => c.Id == request.ResidentialCountryId, cancellationToken);

                if (!residentialCountryExists)
                {
                    _logger.LogWarning("Residential country with ID {CountryId} not found", request.ResidentialCountryId);
                    throw new NotFoundException($"Residential country with ID {request.ResidentialCountryId} not found");
                }

                // Map request to entity
                var companion = _mapper.Map<Companion>(request);

                // Add to repository
                await _unitOfWork.Companions.AddAsync(companion, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion created successfully with ID {CompanionId}", companion.Id);

                // Build response with computed fields
                var response = await BuildCompanionResponseAsync(companion, cancellationToken);

                // Invalidate list cache
                _cache.Remove(CompanionsListCacheKey);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating companion");
                throw new ServiceException($"Failed to create companion: {ex.Message}", ex);
            }
        }

        /// <summary>Retrieves a companion by ID with computed fields.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The <see cref="CompanionResponse"/> with computed fields.</returns>
        /// <exception cref="ArgumentException">Thrown when id is invalid.</exception>
        /// <exception cref="NotFoundException">Thrown when the companion is not found.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<CompanionResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            _logger.LogDebug("Retrieving companion with ID {CompanionId}", id);

            // Try cache first
            var cacheKey = $"{CompanionCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out CompanionResponse? cachedCompanion) && cachedCompanion is not null)
            {
                _logger.LogDebug("Cache hit for companion {CompanionId}", id);
                return cachedCompanion;
            }

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.NationalityCountry)
                    //.Include(c => c.ResidentialCountry)
                    .Include(c => c.CompanionBookings)
                        .ThenInclude(cb => cb.Booking)
                            .ThenInclude(b => b.TourPackage)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogDebug("Companion with ID {CompanionId} not found", id);
                    throw new NotFoundException($"Companion with ID {id} not found");
                }

                var response = await BuildCompanionResponseAsync(entity, cancellationToken);

                // Cache the result with sliding expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    SlidingExpiration = SlidingCacheDuration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, response, cacheOptions);

                _logger.LogDebug("Successfully retrieved companion {CompanionId}", id);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving companion {CompanionId}", id);
                throw new ServiceException($"Failed to retrieve companion: {ex.Message}", ex);
            }
        }

        /// <summary>Retrieves all companions with computed fields.</summary>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>A read-only list of <see cref="CompanionResponse"/> with computed fields.</returns>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<IReadOnlyList<CompanionResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving all companions");

            // Try cache
            if (_cache.TryGetValue(CompanionsListCacheKey, out IReadOnlyList<CompanionResponse>? cachedCompanions) && cachedCompanions is not null)
            {
                _logger.LogDebug("Cache hit for all companions");
                return cachedCompanions;
            }

            try
            {
                var entities = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.NationalityCountry)
                    //.Include(c => c.ResidentialCountry)
                    .Include(c => c.CompanionBookings)
                        .ThenInclude(cb => cb.Booking)
                            .ThenInclude(b => b.TourPackage)
                    .ToListAsync(cancellationToken);

                var responses = new List<CompanionResponse>();
                foreach (var entity in entities)
                {
                    var response = await BuildCompanionResponseAsync(entity, cancellationToken);
                    responses.Add(response);
                }

                var result = responses.AsReadOnly();

                // Cache the result with lower priority
                _cache.Set(CompanionsListCacheKey, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} companions", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all companions");
                throw new ServiceException($"Failed to retrieve companions: {ex.Message}", ex);
            }
        }

        /// <summary>Updates an existing companion with computed fields.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="request">The companion update payload.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The updated <see cref="CompanionResponse"/> with computed fields.</returns>
        /// <exception cref="ArgumentException">Thrown when id is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the companion or countries are not found.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        public async Task<CompanionResponse> UpdateAsync(int id, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _logger.LogInformation("Attempting to update companion {CompanionId}", id);

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.NationalityCountry)
                    //.Include(c => c.ResidentialCountry)
                    .Include(c => c.CompanionBookings)
                        .ThenInclude(cb => cb.Booking)
                            .ThenInclude(b => b.TourPackage)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Companion with ID {CompanionId} not found", id);
                    throw new NotFoundException($"Companion with ID {id} not found");
                }

                // Verify new countries exist if provided
                if (request.NationalityCountryId.HasValue)
                {
                    var countryExists = await _unitOfWork.Countries
                        .Query()
                        .AnyAsync(c => c.Id == request.NationalityCountryId.Value, cancellationToken);

                    if (!countryExists)
                    {
                        _logger.LogWarning("Nationality country with ID {CountryId} not found", request.NationalityCountryId);
                        throw new NotFoundException($"Nationality country with ID {request.NationalityCountryId} not found");
                    }
                }

                if (request.ResidentialCountryId.HasValue)
                {
                    var countryExists = await _unitOfWork.Countries
                        .Query()
                        .AnyAsync(c => c.Id == request.ResidentialCountryId.Value, cancellationToken);

                    if (!countryExists)
                    {
                        _logger.LogWarning("Residential country with ID {CountryId} not found", request.ResidentialCountryId);
                        throw new NotFoundException($"Residential country with ID {request.ResidentialCountryId} not found");
                    }
                }

                // Map request to entity (partial update)
                _mapper.Map(request, entity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} updated successfully", id);

                // Build response with computed fields
                var response = await BuildCompanionResponseAsync(entity, cancellationToken);

                // Invalidate caches
                _cache.Remove($"{CompanionCacheKeyPrefix}{id}");
                _cache.Remove(CompanionsListCacheKey);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating companion {CompanionId}", id);
                throw new ServiceException($"Failed to update companion: {ex.Message}", ex);
            }
        }

        /// <summary>Deletes a companion by ID.</summary>
        /// <param name="id">The companion ID.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>True if deletion was successful; false if companion was not found.</returns>
        /// <exception cref="ArgumentException">Thrown when id is invalid.</exception>
        /// <exception cref="ServiceException">Thrown when an unexpected error occurs.</exception>
        //public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        //{
        //    if (id <= 0)
        //        throw new ArgumentException("Invalid companion ID", nameof(id));

        //    _logger.LogInformation("Attempting to delete companion {CompanionId}", id);
        //    return 1;
        //}

        /// <summary>Builds a CompanionResponse with computed fields from a Companion entity.</summary>
        /// <param name="companion">The companion entity.</param>
        /// <param name="cancellationToken">Propagates notification that the operation should be cancelled.</param>
        /// <returns>The populated <see cref="CompanionResponse"/>.</returns>
        private async Task<CompanionResponse> BuildCompanionResponseAsync(Companion companion, CancellationToken cancellationToken)
        {
            // Map basic properties
            var response = _mapper.Map<CompanionResponse>(companion);

            // Calculate RegistrationDate from CreatedAtUtc
            //response.RegistrationDate = DateOnly.FromDateTime(companion.CreatedAtUtc);

            // Calculate JoinedPackagesCount and related trip data
            if (companion.CompanionBookings != null && companion.CompanionBookings.Any())
            {
                // Count unique packages (through bookings)
                response.JoinedPackagesCount = companion.CompanionBookings
                    .Select(cb => cb.Booking.TourPackageId)
                    .Distinct()
                    .Count();

                // Get the last trip package ID (most recent booking)
                var lastBooking = companion.CompanionBookings
                    .OrderByDescending(cb => cb.Booking.BookingDate)
                    .FirstOrDefault();

                response.LastTripPackageId = lastBooking?.Booking.TourPackageId;

                // Calculate TotalAmountSpent (sum of default class prices)
                response.TotalAmountSpent = companion.CompanionBookings
                    .Where(cb => cb.Booking.TourPackage != null)
                    .Sum(cb => cb.Booking.TourPackage.PricePerPerson);
            }
            else
            {
                response.JoinedPackagesCount = 0;
                response.LastTripPackageId = null;
                response.TotalAmountSpent = 0;
            }

            return await Task.FromResult(response);
        }
    }
}
