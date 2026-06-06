using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    /// <summary>
    /// CRUD for tour programs (TourPackage). A program is created/updated as one
    /// nested graph: the program, its visited cities, and a day-by-day itinerary
    /// whose activities each carry an uploaded image.
    /// </summary>
    public class TourPackageService : ITourPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TourPackageService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly TourPackageCreateValidator _createValidator;
        private readonly TourPackageUpdateValidator _updateValidator;

        private const string MainImageFolder = "package-images";
        private const string ActivityImageFolder = "package-activities";

        public TourPackageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TourPackageService> logger,
            IFileStorage fileStorage,
            TourPackageCreateValidator createValidator,
            TourPackageUpdateValidator updateValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        public async Task<TourPackageResponse> CreateAsync(int ownerUserId, TourPackageCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            await EnsureCountryAndCitiesExistAsync(request.CountryId, request.CityIds, cancellationToken);

            try
            {
                var entity = new TourPackage
                {
                    PackageName = request.PackageName.Trim(),
                    Description = request.Description?.Trim(),
                    PricePerPerson = request.PricePerPerson,
                    Currency = request.Currency.Trim(),
                    DurationInDays = request.DurationInDays,
                    AvailableSeats = request.AvailableSeats,
                    CountryId = request.CountryId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    RegistrationDeadline = request.RegistrationDeadline,
                    TourGuide = request.TourGuide?.Trim(),
                    IsPublished = request.IsPublished,
                    CompanyId = companyId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                };

                if (request.MainImage is not null)
                    entity.MainImageUrl = await _fileStorage.SaveAsync(request.MainImage, MainImageFolder, cancellationToken);

                foreach (var cityId in request.CityIds.Distinct())
                    entity.PackageCities.Add(new PackageCity { CityId = cityId });

                foreach (var day in request.Days)
                    entity.PackageItineraries.Add(await BuildDayAsync(day, cancellationToken));

                await _unitOfWork.TourPackages.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created tour package {PackageId} ({PackageName}) for company {CompanyId}",
                    entity.Id, entity.PackageName, companyId);

                return await BuildResponseAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException)
            {
                _logger.LogError(ex, "Unexpected error while creating tour package {PackageName}", request.PackageName);
                throw new ServiceException($"Failed to create tour package: {ex.Message}", ex);
            }
        }

        public async Task<TourPackageResponse> UpdateAsync(int id, int ownerUserId, TourPackageUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            await EnsureCountryAndCitiesExistAsync(request.CountryId, request.CityIds, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageCities)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.PackageItineraryAttractions)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Tour package with ID {id} not found");

                if (entity.CompanyId != companyId)
                    throw new ForbiddenException("You can only modify your own tour packages.");

                // Scalars.
                entity.PackageName = request.PackageName.Trim();
                entity.Description = request.Description?.Trim();
                entity.PricePerPerson = request.PricePerPerson;
                entity.Currency = request.Currency.Trim();
                entity.DurationInDays = request.DurationInDays;
                entity.AvailableSeats = request.AvailableSeats;
                entity.CountryId = request.CountryId;
                entity.StartDate = request.StartDate;
                entity.EndDate = request.EndDate;
                entity.RegistrationDeadline = request.RegistrationDeadline;
                entity.TourGuide = request.TourGuide?.Trim();
                entity.IsPublished = request.IsPublished;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                if (request.MainImage is not null)
                    entity.MainImageUrl = await _fileStorage.SaveAsync(request.MainImage, MainImageFolder, cancellationToken);
                // else: keep the existing MainImageUrl.

                // Replace visited cities.
                _unitOfWork.PackageCities.RemoveRange(entity.PackageCities);
                entity.PackageCities = request.CityIds.Distinct()
                    .Select(cityId => new PackageCity { CityId = cityId })
                    .ToList();

                // Replace the whole itinerary (days + activities).
                var oldActivities = entity.PackageItineraries.SelectMany(d => d.PackageItineraryAttractions).ToList();
                _unitOfWork.PackageItineraryAttractions.RemoveRange(oldActivities);
                _unitOfWork.PackageItineraries.RemoveRange(entity.PackageItineraries);

                var newDays = new List<PackageItinerary>();
                foreach (var day in request.Days)
                    newDays.Add(await BuildDayAsync(day, cancellationToken));
                entity.PackageItineraries = newDays;

                _unitOfWork.TourPackages.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated tour package {PackageId}", id);

                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException)
            {
                _logger.LogError(ex, "Unexpected error while updating tour package {PackageId}", id);
                throw new ServiceException($"Failed to update tour package: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int ownerUserId, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageCities)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.PackageItineraryAttractions)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                    return false;

                if (entity.CompanyId != companyId)
                    throw new ForbiddenException("You can only delete your own tour packages.");

                var hasBookings = await _unitOfWork.TourPackages
                    .Query()
                    .Where(p => p.Id == id)
                    .SelectMany(p => p.Bookings)
                    .AnyAsync(cancellationToken);
                if (hasBookings)
                    throw new BusinessRuleException("Cannot delete a program that already has bookings.");

                var activities = entity.PackageItineraries.SelectMany(d => d.PackageItineraryAttractions).ToList();
                _unitOfWork.PackageItineraryAttractions.RemoveRange(activities);
                _unitOfWork.PackageItineraries.RemoveRange(entity.PackageItineraries);
                _unitOfWork.PackageCities.RemoveRange(entity.PackageCities);
                _unitOfWork.TourPackages.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted tour package {PackageId}", id);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException)
            {
                _logger.LogError(ex, "Unexpected error while deleting tour package {PackageId}", id);
                throw new ServiceException($"Failed to delete tour package: {ex.Message}", ex);
            }
        }

        public async Task<TourPackageResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var entity = await QueryWithGraph()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (entity is null)
                throw new NotFoundException($"Tour package with ID {id} not found");

            return _mapper.Map<TourPackageResponse>(entity);
        }

        public async Task<IReadOnlyList<TourPackageResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await QueryWithGraph()
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }

        public async Task<IReadOnlyList<TourPackageResponse>> GetMineAsync(int ownerUserId, CancellationToken cancellationToken = default)
        {
            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            var entities = await QueryWithGraph()
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }

        public async Task<IReadOnlyList<TourPackageResponse>> FilterAsync(
            int? countryId = null,
            int? cityId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool publishedOnly = true,
            CancellationToken cancellationToken = default)
        {
            var query = QueryWithGraph();

            if (publishedOnly)
                query = query.Where(p => p.IsPublished);
            if (countryId is > 0)
                query = query.Where(p => p.CountryId == countryId);
            if (cityId is > 0)
                query = query.Where(p => p.PackageCities.Any(pc => pc.CityId == cityId));
            if (minPrice is > 0)
                query = query.Where(p => p.PricePerPerson >= minPrice);
            if (maxPrice is > 0)
                query = query.Where(p => p.PricePerPerson <= maxPrice);

            var entities = await query
                .OrderBy(p => p.StartDate)
                .ThenBy(p => p.PricePerPerson)
                .Take(100)
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }

        #region Helpers

        /// <summary>Base query that eager-loads everything needed to build a response.</summary>
        private IQueryable<TourPackage> QueryWithGraph() =>
            _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Country)
                .Include(p => p.PackageCities).ThenInclude(pc => pc.City)
                .Include(p => p.PackageItineraries).ThenInclude(d => d.PackageItineraryAttractions);

        private async Task<TourPackageResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (entity is null)
                throw new NotFoundException($"Tour package with ID {id} not found");
            return _mapper.Map<TourPackageResponse>(entity);
        }

        private async Task<PackageItinerary> BuildDayAsync(TourPackageDayRequest day, CancellationToken cancellationToken)
        {
            var itinerary = new PackageItinerary
            {
                DayNumber = day.DayNumber,
                DayTitle = day.DayTitle.Trim(),
                DayDescription = day.DayDescription?.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow,
            };

            foreach (var activity in day.Activities)
            {
                var imageUrl = activity.Image is not null
                    ? await _fileStorage.SaveAsync(activity.Image, ActivityImageFolder, cancellationToken)
                    : activity.ImageUrl;

                itinerary.PackageItineraryAttractions.Add(new PackageItineraryAttraction
                {
                    OrderNumber = activity.OrderNumber,
                    Title = activity.Title.Trim(),
                    Description = activity.Description?.Trim(),
                    ImageUrl = imageUrl,
                    StartTime = activity.StartTime,
                    EndTime = activity.EndTime,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                });
            }

            return itinerary;
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
                throw new ForbiddenException("You must have a registered tour company to manage programs.");

            return company.Id;
        }

        private async Task EnsureCountryAndCitiesExistAsync(int countryId, IReadOnlyCollection<int> cityIds, CancellationToken cancellationToken)
        {
            var countryExists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);
            if (!countryExists)
                throw new NotFoundException($"Country with ID {countryId} not found");

            var distinctIds = cityIds.Distinct().ToList();
            var foundCount = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .CountAsync(c => distinctIds.Contains(c.Id), cancellationToken);
            if (foundCount != distinctIds.Count)
                throw new NotFoundException("One or more selected cities do not exist.");
        }

        #endregion
    }
}
