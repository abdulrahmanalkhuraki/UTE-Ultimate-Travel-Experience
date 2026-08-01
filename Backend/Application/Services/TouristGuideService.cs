using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Pagination;
using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;
using Application.Exceptions;
using Application.Interfaces.TouristGuide;
using Application.Interfaces.User;
using Application.Validators.TouristGuide;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class TouristGuideService : ITouristGuideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TouristGuideService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly IMemoryCache _cache;
        private readonly TouristGuideCreateValidator _createValidator;
        private readonly TouristGuideUpdateValidator _updateValidator;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        private const string GuideImageFolder = "guide-images";
        private const string ObjectName = "Tourist Guide";

        public TouristGuideService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TouristGuideService> logger,
            IFileStorage fileStorage,
            ICurrentUserService currentUser,
            IMemoryCache cache,
            TouristGuideCreateValidator createValidator,
            TouristGuideUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _cache = cache ?? throw new ArgumentException(nameof(cache));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<TouristGuideResponse> CreateAsync(TouristGuideCreateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Create", ObjectName, userId);

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Create",ObjectName,string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var companyId = await ResolveCompanyIdAsync(userId, cancellationToken);
                await EnsureCountryExistsAsync(request.NationalityCountryId, cancellationToken);
                await EnsureCityExistsAsync(request.ResidentialCityId, cancellationToken);

                var person = _mapper.Map<Person>(request);

                if (request.ProfileImage is not null)
                    person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken);
                if (request.NationalIdCard is not null)
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdCard, GuideImageFolder, cancellationToken);
                if (request.PassportScan is not null)
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportScan, GuideImageFolder, cancellationToken);
                if (request.ResidencyCard is not null)
                    person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, GuideImageFolder, cancellationToken);

                var entity = new TouristGuide
                {
                    Email = request.Email.Trim(),
                    YearsOfExperiance = request.YearsOfExperiance,
                    Bio = request.Bio.Trim(),
                    Languages = request.Languages?.Trim(),
                    IsAvailable = true,
                    Person = person,
                };

                entity.CompanyGuides.Add(new Company_TouristGuide { CompanyId = companyId });

                await _unitOfWork.TouristGuides.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation(userId,"Create",ObjectName,entity.Id);

                return _mapper.Map<TouristGuideResponse>(entity);
            }
            catch (Exception ex) when (ex is not ValidationException
            and not NotFoundException
            and not ForbiddenException
            and not ConflictException)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TouristGuideResponse> UpdateAsync(int id, TouristGuideUpdateRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Update", ObjectName, id, 0);

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.ValidationFailed("Update", ObjectName, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }


            try
            {
                var companyId = await ResolveCompanyIdAsync(userId, cancellationToken);

                if (request.NationalityCountryId.HasValue)
                    await EnsureCountryExistsAsync(request.NationalityCountryId.Value, cancellationToken);
                if (request.ResidentialCityId.HasValue)
                    await EnsureCityExistsAsync(request.ResidentialCityId.Value, cancellationToken);

                var entity = await _unitOfWork.TouristGuides
                    .Query()
                    .Include(g => g.Person)
                    .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));

                await EnsureGuideBelongsToCompanyAsync(id, companyId, cancellationToken);

                _mapper.Map(request, entity);
                _mapper.Map(request, entity.Person);
                entity.Person.UpdatedAtUtc = DateTime.UtcNow;

                if (request.ProfileImage is not null)
                    entity.Person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken);

                if (request.NationalIdImage is not null)
                    entity.Person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdImage, GuideImageFolder, cancellationToken);

                if (request.PassportImage is not null)
                    entity.Person.PassportScan = await _fileStorage.SaveAsync(request.PassportImage, GuideImageFolder, cancellationToken);

                if (request.ResidencyCard is not null)
                    entity.Person.ResidencyCard = await _fileStorage.SaveAsync(request.ResidencyCard, GuideImageFolder, cancellationToken);


                _unitOfWork.TouristGuides.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Update", ObjectName);

                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException
            and not NotFoundException
            and not ForbiddenException
            and not ConflictException)
            {
                _logger.ServerError("Update", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("update", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            try
            {
                var companyId = await ResolveCompanyIdAsync(userId, cancellationToken);

                var entity = await _unitOfWork.TouristGuides
                    .Query()
                    .Include(g => g.CompanyGuides)
                    .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

                if (entity is null)
                    return false;

                var link = entity.CompanyGuides.FirstOrDefault(cg => cg.CompanyId == companyId);
                if (link is null)
                    throw new ForbiddenException(ExceptionMessages.Forbidden("delete", ObjectName));

                var isAssigned = await _unitOfWork.TourPackage_TouristGuide
                    .Query()
                    .AnyAsync(pg => pg.TouristGuideId == id && pg.Package.CompanyId == companyId, cancellationToken);
                if (isAssigned)
                    throw new BusinessRuleException(ExceptionMessages.BusinessRule("Cannot delete a guide assigned to one of your programs."));

                _unitOfWork.Company_TouristGuide.Remove(link);

                var otherLinks = entity.CompanyGuides.Any(cg => cg.CompanyId != companyId);
                if (!otherLinks)
                    _unitOfWork.TouristGuides.Remove(entity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.SuccessfulOperation("Delete", ObjectName);

                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException and not ArgumentException)
            {
                _logger.ServerError("Delete", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TouristGuideResponse> GetAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(id));

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            _logger.StartOperation("Retrieve", ObjectName, id, userId);

            try
            {
                var companyId = await ResolveCompanyIdAsync(userId, cancellationToken);
                await EnsureGuideBelongsToCompanyAsync(id, companyId, cancellationToken);
                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException and not ArgumentException)
            {
                _logger.ServerError("Delete", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<TouristGuideResponseSummary>> GetMineAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                throw new ValidationException(ExceptionMessages.InvalidPagination());
            }

            var userId = _currentUser.UserId ?? throw new AuthException(ExceptionMessages.Auth());

            var cacheKey = $"mine_page{page}_size{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResponse<TouristGuideResponseSummary>? cached) && cached is not null)
            {
                _logger.LogInformation("Cache hit for Tourist Guide Page {page} | page size {pageSize}", page, pageSize);
                return cached;
            }

            try
            {
                var companyId = await ResolveCompanyIdAsync(userId, cancellationToken);

                var entities = await QueryWithGraph()
                    .Where(g => g.CompanyGuides.Any(cg => cg.CompanyId == companyId))
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<TouristGuideResponseSummary>>(entities);

                var paginationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = items.Count
                };

                var response = new PaginatedResponse<TouristGuideResponseSummary> { Items = items, Pagination = paginationMetadata };

                _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                return response;
            }
            catch (Exception ex) when (ex is not ForbiddenException)
            {
                _logger.ServerError("retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        #region Helpers

        private IQueryable<TouristGuide> QueryWithGraph() =>
            _unitOfWork.TouristGuides
                .Query()
                .AsNoTracking()
                .Include(g => g.NatinalityCountry)
                .Include(g => g.Person)
                    .ThenInclude(p => p.ResidentialCity)
                .Include(g => g.TourPackageGuides);

        private async Task<TouristGuideResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
            return _mapper.Map<TouristGuideResponse>(entity);
        }

        private async Task EnsureGuideBelongsToCompanyAsync(int guideId, int companyId, CancellationToken cancellationToken)
        {
            var linked = await _unitOfWork.Company_TouristGuide
                .Query().AsNoTracking()
                .AnyAsync(cg => cg.TouristGuideId == guideId && cg.CompanyId == companyId, cancellationToken);
            if (!linked)
                throw new ForbiddenException(ExceptionMessages.Forbidden("access", ObjectName));
        }

        private async Task<int> ResolveCompanyIdAsync(int ownerUserId, CancellationToken cancellationToken)
        {
            if (ownerUserId <= 0)
                throw new ForbiddenException("You must be signed in as a tour company.");

            var company = await _unitOfWork.TourCompanies
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == ownerUserId, cancellationToken);

            if (company is null)
                throw new ForbiddenException("You must have a registered tour company to manage guides.");

            return company.Id;
        }

        private async Task EnsureCountryExistsAsync(int countryId, CancellationToken cancellationToken)
        {
            var countryExists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);
            if (!countryExists)
                throw new NotFoundException(ExceptionMessages.NotFound("Country", countryId));
        }

        private async Task EnsureCityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            var cityExists = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == cityId, cancellationToken);
            if (!cityExists)
                throw new NotFoundException(ExceptionMessages.NotFound("City", cityId));
        }

        #endregion
    }
}
