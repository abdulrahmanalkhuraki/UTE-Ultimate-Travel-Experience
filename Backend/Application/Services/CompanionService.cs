using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.Companion;
using Application.Interfaces.User;
using Application.Validators.Companion;
using AutoMapper;
using Azure;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class CompanionService : ICompanionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanionService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly IMemoryCache _cache;
        private readonly CompanionCreateValidator _createValidator;
        private readonly CompanionUpdateValidator _updateValidator;

        private const string CompanionImageFolder = "companion-images";
        private const string CompanionCacheKeyPrefix = "companion_";
        private const string CompanionListCacheKey = "all_companions";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public CompanionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CompanionService> logger,
            IFileStorage fileStorage,
            ICurrentUserService currentUser,
            IMemoryCache cache,
            CompanionCreateValidator createValidator,
            CompanionUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<CompanionResponse> CreateAsync(CompanionCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var userId = _currentUser.UserId ??
                throw new AuthException("You must be logged in to proform this action.");

            _logger.LogInformation("user with id {userId} is Attemping to create new Companion", userId);

            await _EnsureCountryExistsAsync(request.NationalityCountryId, cancellationToken);
            await _EnsureCityExistsAsync(request.ResidentialCityId, cancellationToken);

            try
            {
                var person = _mapper.Map<Person>(request);
                person.CreatedAtUtc = DateTime.UtcNow;
                person.UpdatedAtUtc = DateTime.UtcNow;

                if (request.NationalIdCard is not null)
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, CompanionImageFolder, cancellationToken);
                if (request.PassportScan is not null)
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, CompanionImageFolder, cancellationToken);
                if (request.ResidencyCard is not null)
                    person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, CompanionImageFolder, cancellationToken);
                if (request.ProfileImage is not null)
                    person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, CompanionImageFolder, cancellationToken);

                var companion = new Companion
                {
                    Relationship = request.Relationship,
                    UserId = userId,
                    Person = person,
                };

                await _unitOfWork.Companions.AddAsync(companion, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} successfully created for user {UserId}", companion.Id, userId);

                return await BuildResponseAsync(companion.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException)
            {
                _logger.LogError(ex, "Error creating companion for user {UserId}", userId);
                throw new ServiceException($"Failed to create companion: {ex.Message}", ex);
            }
        }

        public async Task<CompanionResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            var userId = _currentUser.UserId ??
                throw new AuthException("You must be logged in to proform this action.");

            _logger.LogInformation("user with id {userId} is Attemping to retrive Companion with id {companionId}"
                , userId, id);

            var cacheKey = $"{CompanionCacheKeyPrefix}{id}";
            if(_cache.TryGetValue(cacheKey, out CompanionResponse? cached) && cached is not null)
            {
                _logger.LogInformation("cache hit for companion with id {id}", id);
                return cached;
            }

            await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            var response = await BuildResponseAsync(id, cancellationToken);
            _logger.LogInformation($"Companion with id {id} successfully retrived");

            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = SlidingCacheDuration,
                Priority = CacheItemPriority.Normal
            });

            return response;
        }

        public async Task<IReadOnlyList<CompanionResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId ??
                throw new AuthException("You must be logged in to proform this action.");

            _logger.LogInformation("user with id {userId} is Attemping to retrive all Companions", userId);

            if (_cache.TryGetValue(CompanionListCacheKey, out IReadOnlyList<CompanionResponse>? cached) && cached is not null)
            {
                _logger.LogInformation("cache hit for all companions");
                return cached;
            }

            var entities = await QueryWithGraph()
                .Where(c => c.UserId == userId)
                .ToListAsync(cancellationToken);

            var responses = new List<CompanionResponse>(entities.Count);
            foreach (var entity in entities)
            {
                responses.Add(await BuildResponseFromEntity(entity, cancellationToken));
            }

            _cache.Set(CompanionListCacheKey, responses, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                Priority = CacheItemPriority.Low
            });

            _logger.LogInformation($"{entities.Count} companion(s) successfully retrived");
            return responses.AsReadOnly();
        }

        public async Task<CompanionResponse> UpdateAsync(int id, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            ArgumentNullException.ThrowIfNull(request, nameof(request));
            var userId = _currentUser.UserId ??
    throw new AuthException("You must be logged in to proform this action.");

            _logger.LogInformation("user with id {userId} is Attemping to Update Companion with id {companionId}"
    , userId, id);

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (request.NationalityCountryId.HasValue)
                await _EnsureCountryExistsAsync(request.NationalityCountryId.Value, cancellationToken);
            if (request.ResidentialCityId.HasValue)
                await _EnsureCityExistsAsync(request.ResidentialCityId.Value, cancellationToken);





            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.Person)
                    .Include(c => c.CompanionBookings)
                        .ThenInclude(cb => cb.Booking)
                            .ThenInclude(b => b.TourPackage)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Companion with ID {id} not found");

                await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

                await ApplyPartialUpdateAsync(entity, request, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} updated", id);

                InvalidateBookingCache(id);

                return await BuildResponseFromEntity(entity, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException)
            {
                _logger.LogError(ex, "Error updating companion {CompanionId}", id);
                throw new ServiceException($"Failed to update companion: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid companion ID", nameof(id));

            var userId = _currentUser.UserId ??
                    throw new AuthException("You must be logged in to proform this action.");

            _logger.LogInformation("user with id {userId} is Attemping to Delete Companion with id {companionId}"
                , userId, id);

            await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.Person)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Companion with id {id} not found.", id);
                    throw new NotFoundException($"Companion with id {id} not found.");
                }

                _unitOfWork.Persons.Remove(entity.Person);
                _unitOfWork.Companions.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Companion {CompanionId} Successfully deleted", id);
                InvalidateBookingCache(id);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException)
            {
                _logger.LogError(ex, "Error deleting companion {CompanionId}", id);
                throw new ServiceException($"Failed to delete companion: {ex.Message}", ex);
            }
        }

        #region Helpers

        private void InvalidateBookingCache(int? specificCompanionId = null)
        {
            if (specificCompanionId.HasValue)
            {
                var cacheKey = $"{CompanionCacheKeyPrefix}{specificCompanionId.Value}";
                _cache.Remove(cacheKey);
            }

            _cache.Remove(CompanionListCacheKey);
        }

        private async Task ApplyPartialUpdateAsync(Companion entity, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            _mapper.Map(request, entity.Person);

            if (request.NationalityCountryId.HasValue)
                entity.Person.NationalityCountryId = request.NationalityCountryId.Value;
            if (request.Relationship.HasValue)
                entity.Relationship = request.Relationship.Value;

            entity.Person.UpdatedAtUtc = DateTime.UtcNow;

            if (request.NationalIdCard is not null)
                entity.Person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, CompanionImageFolder, cancellationToken);
            if (request.PassportScan is not null)
                entity.Person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, CompanionImageFolder, cancellationToken);
            if (request.ResidencyCard is not null)
                entity.Person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, CompanionImageFolder, cancellationToken);
            if (request.ProfileImage is not null)
                entity.Person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, CompanionImageFolder, cancellationToken);
        }

        private IQueryable<Companion> QueryWithGraph() =>
            _unitOfWork.Companions
                .Query()
                .AsNoTracking()
                .Include(c => c.Person)
                    .ThenInclude(p => p.ResidentialCity)              
                .Include(c => c.Person)
                    .ThenInclude(p => p.NationalityCountry)              
                .Include(c => c.CompanionBookings)
                    .ThenInclude(cb => cb.Booking)
                        .ThenInclude(b => b.TourPackage);

        private async Task<CompanionResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Companion with ID {id} not found");
            return await BuildResponseFromEntity(entity, cancellationToken);
        }

        private async Task<CompanionResponse> BuildResponseFromEntity(Companion entity, CancellationToken cancellationToken)
        {
            var response = _mapper.Map<CompanionResponse>(entity);

            response.RegistrationDate = DateOnly.FromDateTime(entity.Person.CreatedAtUtc);

            var CompletedCompanionBookings = entity.CompanionBookings.Where(cb => cb.Booking.Status == BookingStatus.Completed).ToList();

            if (CompletedCompanionBookings != null &&
                CompletedCompanionBookings.Count != 0)
            {

                response.JoinedPackagesCount = entity.CompanionBookings
                    .Select(cb => cb.Booking.TourPackageId)
                    .Distinct()
                    .Count();

                response.TotalAmountSpent = entity.CompanionBookings
                    .Where(cb => cb.Booking.TourPackage != null)
                    .Sum(cb => cb.Booking.TourPackage.PricePerPerson);

                var lastBooking = entity.CompanionBookings
                    .Where(cb => cb.Booking.TourPackage != null)
                    .OrderByDescending(cb => cb.Booking.TourPackage.StartDate)
                    .FirstOrDefault();

                if (lastBooking?.Booking.TourPackage != null)
                    response.LastTourPackage = _mapper.Map<TourPackageResponse>(lastBooking.Booking.TourPackage);
            }

            return response;
        }

        private async Task _EnsureCompanionBelongsToUserAsync(int companionId, int userId, CancellationToken cancellationToken)
        {
            var belongs = await _unitOfWork.Companions
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == companionId && c.UserId == userId, cancellationToken);

            if (!belongs)
                throw new ForbiddenException("This companion does not belong to you.");
        }

        private async Task _EnsureCountryExistsAsync(int countryId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("Country With Id {id} not found", countryId);
                throw new NotFoundException($"Country with ID {countryId} not found");
            }
        }

        private async Task _EnsureCityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == cityId, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("city With Id {id} not found", cityId);
                throw new NotFoundException($"City with ID {cityId} not found");
            }
        }

        #endregion
    }
}
