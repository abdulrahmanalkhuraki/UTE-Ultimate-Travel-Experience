using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.Companion;
using Application.Interfaces.User;
using Application.Validators.Companion;
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
        private const string ObjectName = "Companion";

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
                throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Create", ObjectName, userId);

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

                _logger.SuccessfulOperation("Create", ObjectName);

                return await BuildResponseAsync(companion.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<CompanionResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            var userId = _currentUser.UserId ??
                throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Retrieve", ObjectName, id, userId);

            var cacheKey = $"{CompanionCacheKeyPrefix}{id}";
            if(_cache.TryGetValue(cacheKey, out CompanionResponse? cached) && cached is not null)
            {
                _logger.LogInformation("cache hit for companion with id {id}", id);
                return cached;
            }

            await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            var response = await BuildResponseAsync(id, cancellationToken);
            _logger.SuccessfulOperation("Retrieve", ObjectName);

            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = SlidingCacheDuration,
                Priority = CacheItemPriority.Normal
            });

            return response;
        }

        public async Task<PaginatedResponse<CompanionResponseSummary>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            if(page < 1 || pageSize < 1 || pageSize > 100)
            {
                throw new ValidationException(ExceptionMessages.InvalidPagination());
            }

            var userId = _currentUser.UserId ??
                throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Retrieve All", ObjectName, userId);

            var cacheKey = $"companions_page{page}_size{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResponse<CompanionResponseSummary>? cached) && cached is not null)
            {
                _logger.LogInformation($"cache hit for companions page {page}| page size {pageSize}");
                return cached;
            }

            var entities = await QueryWithGraph()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Person.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .ToListAsync(cancellationToken);


            var items = _mapper.Map<IReadOnlyList<CompanionResponseSummary>>(entities);

            var paginationMetadata = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = items.Count
            };

            var response = new PaginatedResponse<CompanionResponseSummary> { Items = items,Pagination = paginationMetadata};

            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                Priority = CacheItemPriority.Low
            });

            _logger.LogInformation($"{items.Count} companion(s) successfully retrived");
            return response;
        }

        public async Task<CompanionResponse> UpdateAsync(int id, CompanionUpdateRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            ArgumentNullException.ThrowIfNull(request, nameof(request));
            var userId = _currentUser.UserId ??
                throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Update", ObjectName, id, userId);

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
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));

                await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

                await ApplyPartialUpdateAsync(entity, request, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Update", ObjectName);

                InvalidateBookingCache(id);

                return await BuildResponseFromEntity(entity, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException)
            {
                _logger.ServerError("Update", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("update", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            var userId = _currentUser.UserId ??
                    throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Delete", ObjectName, id, userId);

            await _EnsureCompanionBelongsToUserAsync(id, userId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.Companions
                    .Query()
                    .Include(c => c.Person)
                    .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.EntityNotFound(ObjectName, id);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
                }

                _unitOfWork.Persons.Remove(entity.Person);
                _unitOfWork.Companions.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Delete", ObjectName);
                InvalidateBookingCache(id);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException)
            {
                _logger.ServerError("Delete", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
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
                throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
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
                throw new ForbiddenException(ExceptionMessages.Forbidden("access", ObjectName));
        }

        private async Task _EnsureCountryExistsAsync(int countryId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);

            if (!exists)
            {
                _logger.EntityNotFound("Country", countryId);
                throw new NotFoundException(ExceptionMessages.NotFound("Country", countryId));
            }
        }

        private async Task _EnsureCityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == cityId, cancellationToken);

            if (!exists)
            {
                _logger.EntityNotFound("City", cityId);
                throw new NotFoundException(ExceptionMessages.NotFound("City", cityId));
            }
        }

        #endregion
    }
}
