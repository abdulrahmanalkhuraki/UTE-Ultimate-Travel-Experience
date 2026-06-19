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
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class TouristGuideService : ITouristGuideService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TouristGuideService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly TouristGuideCreateValidator _createValidator;
        private readonly TouristGuideUpdateValidator _updateValidator;

        private const string GuideImageFolder = "guide-images";

        public TouristGuideService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TouristGuideService> logger,
            IFileStorage fileStorage,
            TouristGuideCreateValidator createValidator,
            TouristGuideUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<TouristGuideResponse> CreateAsync(int ownerUserId, TouristGuideCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            await EnsureCountryExistsAsync(request.NationalityCountryId, cancellationToken);
            await EnsureCityExistsAsync(request.ResidentialCityId, cancellationToken);

            try
            {
                var person = _mapper.Map<Person>(request);
                person.CreatedAtUtc = DateTime.UtcNow;
                person.UpdatedAtUtc = DateTime.UtcNow;

                if (request.ProfileImage is not null)
                    person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken);
                if (request.NationalIdImage is not null)
                    person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdImage, GuideImageFolder, cancellationToken);
                if (request.PassportImage is not null)
                    person.PassportScan = await _fileStorage.SaveAsync(request.PassportImage, GuideImageFolder, cancellationToken);

                var entity = new TouristGuide
                {
                    Email = request.Email.Trim(),
                    NationalityCountryId = request.NationalityCountryId,
                    YearsOfExperiance = request.YearsOfExperiance,
                    Bio = request.Bio.Trim(),
                    Languages = request.Languages?.Trim(),
                    IsAvailable = true,
                    Person = person,
                };

                entity.CompanyGuides.Add(new Company_TouristGuide { CompanyId = companyId });

                await _unitOfWork.TouristGuides.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created guide {GuideId} for company {CompanyId}", entity.Id, companyId);

                return await BuildResponseAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException)
            {
                _logger.LogError(ex, "Unexpected error while creating guide {Email}", request.Email);
                throw new ServiceException($"Failed to create guide: {ex.Message}", ex);
            }
        }

        public async Task<TouristGuideResponse> UpdateAsync(int id, int ownerUserId, TouristGuideUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (id <= 0)
                throw new ArgumentException("Invalid guide ID", nameof(id));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            if (request.NationalityCountryId.HasValue)
                await EnsureCountryExistsAsync(request.NationalityCountryId.Value, cancellationToken);
            if (request.ResidentialCityId.HasValue)
                await EnsureCityExistsAsync(request.ResidentialCityId.Value, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TouristGuides
                    .Query()
                    .Include(g => g.Person)
                    .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Guide with ID {id} not found");

                await EnsureGuideBelongsToCompanyAsync(id, companyId, cancellationToken);

                if (request.FirstName is not null) entity.Person.FirstName = request.FirstName.Trim();
                if (request.LastName is not null) entity.Person.LastName = request.LastName.Trim();
                if (request.Phone is not null) entity.Person.Phone = request.Phone.Trim();
                if (request.Email is not null) entity.Email = request.Email.Trim();
                if (request.NationalityCountryId.HasValue) entity.NationalityCountryId = request.NationalityCountryId.Value;
                if (request.Gender is not null) entity.Person.Gender = request.Gender;
                if (request.DateOfBirth.HasValue) entity.Person.DateOfBirth = request.DateOfBirth.Value;
                if (request.YearsOfExperiance.HasValue) entity.YearsOfExperiance = request.YearsOfExperiance.Value;
                if (request.Bio is not null) entity.Bio = request.Bio.Trim();
                if (request.ResidentialCityId.HasValue) entity.Person.ResidentialCityId = request.ResidentialCityId.Value;
                if (request.NationalNumber is not null) entity.Person.NationalNumber = request.NationalNumber.Trim();
                if (request.PassportNumber is not null) entity.Person.PassportNumber = request.PassportNumber.Trim();
                if (request.Languages is not null) entity.Languages = request.Languages.Trim();
                if (request.IsAvailable.HasValue) entity.IsAvailable = request.IsAvailable.Value;
                entity.Person.UpdatedAtUtc = DateTime.UtcNow;

                if (request.ProfileImage is not null)
                    entity.Person.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken);
                else if (request.ProfileImageUrl is not null)
                    entity.Person.ProfileImage = request.ProfileImageUrl;

                if (request.NationalIdImage is not null)
                    entity.Person.NationalIdCard = await _fileStorage.SaveAsync(request.NationalIdImage, GuideImageFolder, cancellationToken);
                else if (request.NationalIdCardUrl is not null)
                    entity.Person.NationalIdCard = request.NationalIdCardUrl;

                if (request.PassportImage is not null)
                    entity.Person.PassportScan = await _fileStorage.SaveAsync(request.PassportImage, GuideImageFolder, cancellationToken);
                else if (request.PassportScanUrl is not null)
                    entity.Person.PassportScan = request.PassportScanUrl;

                _unitOfWork.TouristGuides.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated guide {GuideId}", id);

                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException)
            {
                _logger.LogError(ex, "Unexpected error while updating guide {GuideId}", id);
                throw new ServiceException($"Failed to update guide: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int ownerUserId, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid guide ID", nameof(id));

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TouristGuides
                    .Query()
                    .Include(g => g.CompanyGuides)
                    .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

                if (entity is null)
                    return false;

                var link = entity.CompanyGuides.FirstOrDefault(cg => cg.CompanyId == companyId);
                if (link is null)
                    throw new ForbiddenException("You can only delete guides that belong to your company.");

                var isAssigned = await _unitOfWork.TourPackage_TouristGuide
                    .Query()
                    .AnyAsync(pg => pg.TouristGuideId == id && pg.Package.CompanyId == companyId, cancellationToken);
                if (isAssigned)
                    throw new BusinessRuleException("Cannot delete a guide assigned to one of your programs.");

                _unitOfWork.Company_TouristGuide.Remove(link);

                var otherLinks = entity.CompanyGuides.Any(cg => cg.CompanyId != companyId);
                if (!otherLinks)
                    _unitOfWork.TouristGuides.Remove(entity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted/unlinked guide {GuideId} for company {CompanyId}", id, companyId);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException and not ArgumentException)
            {
                _logger.LogError(ex, "Unexpected error while deleting guide {GuideId}", id);
                throw new ServiceException($"Failed to delete guide: {ex.Message}", ex);
            }
        }

        public async Task<TouristGuideResponse> GetAsync(int id, int ownerUserId, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid guide ID", nameof(id));

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            await EnsureGuideBelongsToCompanyAsync(id, companyId, cancellationToken);

            return await BuildResponseAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<TouristGuideResponse>> GetMineAsync(int ownerUserId, CancellationToken cancellationToken = default)
        {
            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            var entities = await QueryWithGraph()
                .Where(g => g.CompanyGuides.Any(cg => cg.CompanyId == companyId))
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TouristGuideResponse>>(entities);
        }

        #region Helpers

        private IQueryable<TouristGuide> QueryWithGraph() =>
            _unitOfWork.TouristGuides
                .Query()
                .AsNoTracking()
                .Include(g => g.NatinalityCountry)
                .Include(g => g.Person)
                    .ThenInclude(p => p.ResidentialCity);

        private async Task<TouristGuideResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Guide with ID {id} not found");
            return _mapper.Map<TouristGuideResponse>(entity);
        }

        private async Task EnsureGuideBelongsToCompanyAsync(int guideId, int companyId, CancellationToken cancellationToken)
        {
            var linked = await _unitOfWork.Company_TouristGuide
                .Query().AsNoTracking()
                .AnyAsync(cg => cg.TouristGuideId == guideId && cg.CompanyId == companyId, cancellationToken);
            if (!linked)
                throw new ForbiddenException("This guide does not belong to your company.");
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
                throw new NotFoundException($"Country with ID {countryId} not found");
        }

        private async Task EnsureCityExistsAsync(int cityId, CancellationToken cancellationToken)
        {
            var cityExists = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == cityId, cancellationToken);
            if (!cityExists)
                throw new NotFoundException($"City with ID {cityId} not found");
        }

        #endregion
    }
}
