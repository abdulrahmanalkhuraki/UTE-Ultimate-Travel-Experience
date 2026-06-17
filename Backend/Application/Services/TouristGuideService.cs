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
    /// <summary>
    /// CRUD for tour guides, scoped to the signed-in company. A guide is linked to
    /// the company through <see cref="Company_TouristGuide"/> (many-to-many), so a company
    /// only ever sees and manages its own guides.
    /// </summary>
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

            try
            {
                var entity = new TouristGuide
                {
                    //Firstname = request.Firstname.Trim(),
                    //Lastname = request.Lastname.Trim(),
                    //Phone = request.Phone.Trim(),
                    //Email = request.Email.Trim(),
                    //NationalityCountryId = request.NationalityCountryId,
                    //Gender = request.Gender,
                    //DateOfBirth = request.DateOfBirth,
                    //YearsOfExperiance = request.YearsOfExperiance,
                    //Bio = request.Bio.Trim(),
                    //ResidentialCountryId = request.PlaceOfResidence.Trim(),
                    //CurrentLocation = request.CurrentLocation?.Trim(),
                    //NationalNumber = request.NationalNumber.Trim(),
                    //PassportNumber = request.PassportNumber?.Trim(),
                    //Languages = request.Languages?.Trim(),
                    //IsAvailable = true,
                    //CreatedAtUtc = DateTime.UtcNow,
                    //UpdatedAtUtc = DateTime.UtcNow,
                };

                if (request.ProfileImage is not null)
                    //entity.ProfileImage = await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken);
                if (request.IdCardImage is not null)
                    //entity.NationalIdCard = await _fileStorage.SaveAsync(request.IdCardImage, GuideImageFolder, cancellationToken);
                if (request.PassportImage is not null)
                    //entity.PassportScan = await _fileStorage.SaveAsync(request.PassportImage, GuideImageFolder, cancellationToken);

                // Link the new guide to the creating company.
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

            // Partial update: only validate the country when it is actually being changed.
            if (request.NationalityCountryId.HasValue)
                await EnsureCountryExistsAsync(request.NationalityCountryId.Value, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TouristGuides
                    .Query()
                    .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Guide with ID {id} not found");

                await EnsureGuideBelongsToCompanyAsync(id, companyId, cancellationToken);

                // Partial update: apply only the fields that were actually sent (non-null).
                // Anything omitted keeps its current value.
                //if (request.Firstname is not null) entity.Firstname = request.Firstname.Trim();
                //if (request.Lastname is not null) entity.Lastname = request.Lastname.Trim();
                //if (request.Phone is not null) entity.Phone = request.Phone.Trim();
                //if (request.Email is not null) entity.Email = request.Email.Trim();
                if (request.NationalityCountryId.HasValue) entity.NationalityCountryId = request.NationalityCountryId.Value;
                //if (request.Gender.HasValue) entity.Gender = request.Gender.Value;
                //if (request.DateOfBirth.HasValue) entity.DateOfBirth = request.DateOfBirth.Value;
                if (request.YearsOfExperiance.HasValue) entity.YearsOfExperiance = request.YearsOfExperiance.Value;
                if (request.Bio is not null) entity.Bio = request.Bio.Trim();
                //if (request.PlaceOfResidence is not null) entity.ResidentialCountryId = request.PlaceOfResidence.Trim();
                if (request.CurrentLocation is not null) entity.CurrentLocation = request.CurrentLocation.Trim();
                //if (request.NationalNumber is not null) entity.NationalNumber = request.NationalNumber.Trim();
                //if (request.PassportNumber is not null) entity.PassportNumber = request.PassportNumber.Trim();
                if (request.Languages is not null) entity.Languages = request.Languages.Trim();
                if (request.IsAvailable.HasValue) entity.IsAvailable = request.IsAvailable.Value;
                //entity.UpdatedAtUtc = DateTime.UtcNow;

                //entity.ProfileImage = request.ProfileImage is not null
                    //? await _fileStorage.SaveAsync(request.ProfileImage, GuideImageFolder, cancellationToken)
                    //: request.ProfileImageUrl ?? entity.ProfileImage;
                //entity.NationalIdCard = request.IdCardImage is not null
                    //? await _fileStorage.SaveAsync(request.IdCardImage, GuideImageFolder, cancellationToken)
                    //: request.IdCardUrl ?? entity.NationalIdCard;
                //entity.PassportScan = request.PassportImage is not null
                    //? await _fileStorage.SaveAsync(request.PassportImage, GuideImageFolder, cancellationToken)
                    //: request.PassportScanUrl ?? entity.PassportScan;

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

                // Block deletion while the guide is assigned to any of this company's programs.
                var isAssigned = await _unitOfWork.TourPackageGuides
                    .Query()
                    .AnyAsync(pg => pg.TouristGuideId == id && pg.Package.CompanyId == companyId, cancellationToken);
                if (isAssigned)
                    throw new BusinessRuleException("Cannot delete a guide assigned to one of your programs.");

                // Unlink from this company.
                _unitOfWork.CompanyGuides.Remove(link);

                // If no other company keeps this guide, hard-delete the guide record.
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
                //.OrderBy(g => g.Firstname).ThenBy(g => g.Lastname)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TouristGuideResponse>>(entities);
        }

        #region Helpers

        /// <summary>Base query that eager-loads the lookups needed to build a response.</summary>
        private IQueryable<TouristGuide> QueryWithGraph() =>
            _unitOfWork.TouristGuides
                .Query()
                .AsNoTracking()
                .Include(g => g.NatinalityCountry);

        private async Task<TouristGuideResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Guide with ID {id} not found");
            return _mapper.Map<TouristGuideResponse>(entity);
        }

        private async Task EnsureGuideBelongsToCompanyAsync(int guideId, int companyId, CancellationToken cancellationToken)
        {
            var linked = await _unitOfWork.CompanyGuides
                .Query().AsNoTracking()
                .AnyAsync(cg => cg.TouristGuideId == guideId && cg.CompanyId == companyId, cancellationToken);
            if (!linked)
                throw new ForbiddenException("This guide does not belong to your company.");
        }

        /// <summary>Resolves the company owned by the JWT user, or throws if none.</summary>
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

        #endregion
    }
}
