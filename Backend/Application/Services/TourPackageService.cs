using Application.Common;
using Application.DTOs.TourPackage;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Notifications;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
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
        private readonly INotificationService _notificationService;

        private const string MainImageFolder = "package-images";
        private const string ActivityImageFolder = "package-activities";

        public TourPackageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TourPackageService> logger,
            IFileStorage fileStorage,
            TourPackageCreateValidator createValidator,
            TourPackageUpdateValidator updateValidator,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<TourPackageResponse> CreateAsync(int ownerUserId, TourPackageCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            await EnsureCountryAndCitiesExistAsync(request.CountryId, request.CityIds, cancellationToken);
            await EnsureGuidesBelongToCompanyAsync(request.TouristGuideIds, companyId, cancellationToken);

            try
            {
                var entity = new TourPackage
                {
                    PackageName = request.PackageName.Trim(),
                    Description = request.Description?.Trim(),
                    MeetingPoint = request.MeetingPoint.Trim(),
                    // Optional costs (اختياري): null/omitted is stored as 0.
                    PricePerPerson = request.PricePerPerson ?? 0,
                    EconomyClassPrice = request.EconomyClassPrice ?? 0,
                    PremiumClassPrice = request.PremiumClassPrice ?? 0,
                    BusinessClassPrice = request.BusinessClassPrice ?? 0,
                    Currency = request.Currency.Trim(),
                    DurationInDays = request.DurationInDays,
                    AvailableSeats = request.AvailableSeats,
                    CountryId = request.CountryId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    RegistrationDeadline = request.RegistrationDeadline,
                    ServiceLevel = request.ServiceLevel,
                    IsPublished = request.IsPublished,
                    // Publishing on creation counts as the first publish (المرة الأولى).
                    PublishCount = request.IsPublished ? 1 : 0,
                    PublishedAtUtc = request.IsPublished ? DateTime.UtcNow : null,
                    CompanyId = companyId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                };

                if (request.MainImage is not null)
                    entity.MainImageUrl = await _fileStorage.SaveAsync(request.MainImage, MainImageFolder, cancellationToken);

                foreach (var cityId in request.CityIds.Distinct())
                    entity.PackageCities.Add(new PackageCity { CityId = cityId });

                foreach (var guideId in request.TouristGuideIds.Distinct())
                    entity.TourPackageGuides.Add(new TourPackageGuide { TouristGuideId = guideId });

                foreach (var cabin in ResolveCabinClasses(request.AvailableCabinClasses))
                    entity.CabinClasses.Add(new TourPackageCabinClass { CabinClass = cabin });

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

            // Partial update: only validate the foreign keys that are actually being changed.
            if (request.CountryId.HasValue)
                await EnsureCountryExistsAsync(request.CountryId.Value, cancellationToken);
            if (request.CityIds is not null)
                await EnsureCitiesExistAsync(request.CityIds, cancellationToken);
            if (request.TouristGuideIds is not null)
                await EnsureGuidesBelongToCompanyAsync(request.TouristGuideIds, companyId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageCities)
                    .Include(p => p.TourPackageGuides)
                    .Include(p => p.CabinClasses)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.PackageItineraryAttractions)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Tour package with ID {id} not found");

                if (entity.CompanyId != companyId)
                    throw new ForbiddenException("You can only modify your own tour packages.");

                // Scalars — partial update: apply only the fields that were actually sent
                // (non-null). Anything omitted keeps its current value.
                if (request.PackageName is not null) entity.PackageName = request.PackageName.Trim();
                if (request.Description is not null) entity.Description = request.Description.Trim();
                if (request.MeetingPoint is not null) entity.MeetingPoint = request.MeetingPoint.Trim();
                if (request.PricePerPerson.HasValue) entity.PricePerPerson = request.PricePerPerson.Value;
                if (request.EconomyClassPrice.HasValue) entity.EconomyClassPrice = request.EconomyClassPrice.Value;
                if (request.PremiumClassPrice.HasValue) entity.PremiumClassPrice = request.PremiumClassPrice.Value;
                if (request.BusinessClassPrice.HasValue) entity.BusinessClassPrice = request.BusinessClassPrice.Value;
                if (request.Currency is not null) entity.Currency = request.Currency.Trim();
                if (request.DurationInDays.HasValue) entity.DurationInDays = request.DurationInDays.Value;
                if (request.AvailableSeats.HasValue) entity.AvailableSeats = request.AvailableSeats.Value;
                if (request.CountryId.HasValue) entity.CountryId = request.CountryId.Value;
                if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
                if (request.EndDate.HasValue) entity.EndDate = request.EndDate.Value;
                if (request.RegistrationDeadline.HasValue) entity.RegistrationDeadline = request.RegistrationDeadline.Value;
                if (request.ServiceLevel.HasValue) entity.ServiceLevel = request.ServiceLevel.Value;

                // Publish flag (only when sent). Count each unpublished→published transition
                // and stamp the publish time (drives "كم مرة نُشر" and "اديش صرلو منشور").
                if (request.IsPublished.HasValue)
                {
                    if (!entity.IsPublished && request.IsPublished.Value)
                    {
                        entity.PublishCount++;
                        entity.PublishedAtUtc = DateTime.UtcNow;
                    }
                    entity.IsPublished = request.IsPublished.Value;
                }

                entity.UpdatedAtUtc = DateTime.UtcNow;

                if (request.MainImage is not null)
                    entity.MainImageUrl = await _fileStorage.SaveAsync(request.MainImage, MainImageFolder, cancellationToken);
                // else: keep the existing MainImageUrl.

                // Replace visited cities (only when sent).
                if (request.CityIds is not null)
                {
                    _unitOfWork.PackageCities.RemoveRange(entity.PackageCities);
                    entity.PackageCities = request.CityIds.Distinct()
                        .Select(cityId => new PackageCity { CityId = cityId })
                        .ToList();
                }

                // Replace assigned guides (only when sent).
                if (request.TouristGuideIds is not null)
                {
                    _unitOfWork.TourPackageGuides.RemoveRange(entity.TourPackageGuides);
                    entity.TourPackageGuides = request.TouristGuideIds.Distinct()
                        .Select(guideId => new TourPackageGuide { TouristGuideId = guideId })
                        .ToList();
                }

                // Replace available cabin classes (only when sent; empty defaults to economy).
                if (request.AvailableCabinClasses is not null)
                {
                    _unitOfWork.TourPackageCabinClasses.RemoveRange(entity.CabinClasses);
                    entity.CabinClasses = ResolveCabinClasses(request.AvailableCabinClasses)
                        .Select(cabin => new TourPackageCabinClass { CabinClass = cabin })
                        .ToList();
                }

                // Replace the whole itinerary (days + activities) only when sent.
                if (request.Days is not null)
                {
                    var oldActivities = entity.PackageItineraries.SelectMany(d => d.PackageItineraryAttractions).ToList();
                    _unitOfWork.PackageItineraryAttractions.RemoveRange(oldActivities);
                    _unitOfWork.PackageItineraries.RemoveRange(entity.PackageItineraries);

                    var newDays = new List<PackageItinerary>();
                    foreach (var day in request.Days)
                        newDays.Add(await BuildDayAsync(day, cancellationToken));
                    entity.PackageItineraries = newDays;
                }

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

        public async Task<ProgramStatusResponse> CancelAsync(int id, int ownerUserId, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Tour package with ID {id} not found");

                if (entity.CompanyId != companyId)
                    throw new ForbiddenException("You can only cancel your own tour packages.");

                if (entity.Status == TourPackageStatus.Cancelled)
                    throw new BusinessRuleException("This program is already cancelled.");

                entity.Status = TourPackageStatus.Cancelled;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.TourPackages.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Cancelled tour package {PackageId}", id);

                return ToStatusResponse(entity);
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException)
            {
                _logger.LogError(ex, "Unexpected error while cancelling tour package {PackageId}", id);
                throw new ServiceException($"Failed to cancel tour package: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<TourPackageResponse>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            var entities = await QueryWithGraph()
                .Where(p => p.ApprovalStatus == ProgramApprovalStatus.Pending)
                .OrderBy(p => p.CreatedAtUtc) // longest-waiting first
                .ToListAsync(cancellationToken);

            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }

        public Task<ProgramStatusResponse> AcceptAsync(int id, CancellationToken cancellationToken = default)
            => SetApprovalAsync(id, ProgramApprovalStatus.Accepted, null, cancellationToken);

        public Task<ProgramStatusResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required", nameof(reason));

            return SetApprovalAsync(id, ProgramApprovalStatus.Rejected, reason, cancellationToken);
        }

        /// <summary>Shared admin-moderation path for accept/reject: updates the status, then best-effort notifies the owning company.</summary>
        private async Task<ProgramStatusResponse> SetApprovalAsync(int id, ProgramApprovalStatus approval, string? reason, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.Company)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                    throw new NotFoundException($"Tour package with ID {id} not found");

                // Reason is kept only for rejections; cleared on accept.
                var rejectionReason = approval == ProgramApprovalStatus.Rejected ? reason : null;

                entity.ApprovalStatus = approval;
                entity.RejectionReason = rejectionReason;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.TourPackages.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tour package {PackageId} approval set to {Approval}", id, approval);

                // Notify the owning company's user. A notification failure must not undo the
                // decision, so it is best-effort here (mirrors the TourCompany approve/reject flow).
                var (message, notificationType) = approval switch
                {
                    ProgramApprovalStatus.Accepted => (ProgramApprovalMessages.Accepted, NotificationType.PackageAccepted),
                    ProgramApprovalStatus.Rejected => ($"{ProgramApprovalMessages.Rejected} السبب: {reason}", NotificationType.PackageRejected),
                    _ => (string.Empty, NotificationType.General)
                };

                var ownerUserId = entity.Company?.UserId ?? 0;
                if (!string.IsNullOrEmpty(message) && ownerUserId > 0)
                {
                    try
                    {
                        await _notificationService.NotifyAsync(ownerUserId, message, notificationType, cancellationToken);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogWarning(notifyEx, "Failed to notify owner {UserId} of program {PackageId} approval change",
                            ownerUserId, id);
                    }
                }

                return ToStatusResponse(entity);
            }
            catch (Exception ex) when (ex is not NotFoundException and not ArgumentException)
            {
                _logger.LogError(ex, "Unexpected error while moderating tour package {PackageId}", id);
                throw new ServiceException($"Failed to update tour package approval: {ex.Message}", ex);
            }
        }

        /// <summary>Projects an entity to the lightweight status response returned by the status actions.</summary>
        private static ProgramStatusResponse ToStatusResponse(TourPackage entity) => new()
        {
            Id = entity.Id,
            PackageName = entity.PackageName,
            Status = entity.Status,
            ApprovalStatus = entity.ApprovalStatus,
            RejectionReason = entity.RejectionReason,
            UpdatedAtUtc = entity.UpdatedAtUtc,
        };

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
            // Public endpoint: only show published, admin-accepted, non-cancelled programs.
            var entities = await WherePubliclyVisible(QueryWithGraph())
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

        public async Task<IReadOnlyList<TourPackageResponse>> GetMineByTimelineAsync(int ownerUserId, ProgramTimeline timeline, CancellationToken cancellationToken = default)
        {
            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var query = QueryWithGraph().Where(p => p.CompanyId == companyId);

            query = timeline switch
            {
                // الحالية: active and not finished yet (ongoing or upcoming).
                ProgramTimeline.Current => query.Where(p => p.Status == TourPackageStatus.Active && p.EndDate >= today),
                // السابقة: active but already finished.
                ProgramTimeline.Previous => query.Where(p => p.Status == TourPackageStatus.Active && p.EndDate < today),
                // الملغاة: cancelled, regardless of dates.
                ProgramTimeline.Cancelled => query.Where(p => p.Status == TourPackageStatus.Cancelled),
                _ => query,
            };

            // Upcoming/cancelled read best soonest-first; finished programs newest-first.
            query = timeline == ProgramTimeline.Previous
                ? query.OrderByDescending(p => p.EndDate)
                : query.OrderBy(p => p.StartDate);

            var entities = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
        }

        public async Task<CompanyProgramStatsResponse> GetMyStatsAsync(int ownerUserId, CancellationToken cancellationToken = default)
        {
            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // One trip to the DB, only the columns the counts need.
            var rows = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId)
                .Select(p => new { p.Status, p.ApprovalStatus, p.EndDate })
                .ToListAsync(cancellationToken);

            return new CompanyProgramStatsResponse
            {
                Total = rows.Count,
                Current = rows.Count(r => r.Status == TourPackageStatus.Active && r.EndDate >= today),
                Accepted = rows.Count(r => r.ApprovalStatus == ProgramApprovalStatus.Accepted),
                Cancelled = rows.Count(r => r.Status == TourPackageStatus.Cancelled),
                Rejected = rows.Count(r => r.ApprovalStatus == ProgramApprovalStatus.Rejected),
            };
        }

        public async Task<int> GetMyPublishedCountAsync(int ownerUserId, CancellationToken cancellationToken = default)
        {
            var companyId = await ResolveCompanyIdAsync(ownerUserId, cancellationToken);

            // Published only (IsPublished == true). Counted in the DB (COUNT(*)), nothing is materialized.
            return await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .CountAsync(p => p.CompanyId == companyId && p.IsPublished, cancellationToken);
        }

        public async Task<IReadOnlyList<TourPackageResponse>> FilterAsync(
            int? countryId = null,
            int? cityId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool publishedOnly = true,
            CancellationToken cancellationToken = default)
        {
            // Public endpoint: always restrict to published, admin-accepted, non-cancelled
            // programs. The publishedOnly flag is kept for compatibility but visibility is
            // always enforced here so unapproved/cancelled programs never leak to tourists.
            var query = WherePubliclyVisible(QueryWithGraph());

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

        /// <summary>
        /// Resolves the available flight cabin classes (تذاكر الطيران المتاحة) to persist.
        /// Distinct, and defaults to economy (الدرجة الاقتصادية) when none are provided.
        /// </summary>
        private static IReadOnlyList<FlightCabinClass> ResolveCabinClasses(IEnumerable<FlightCabinClass> requested)
        {
            var classes = requested.Distinct().ToList();
            if (classes.Count == 0)
                classes.Add(FlightCabinClass.Economy);
            return classes;
        }

        /// <summary>Base query that eager-loads everything needed to build a response.</summary>
        private IQueryable<TourPackage> QueryWithGraph() =>
            _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Country)
                .Include(p => p.Company)
                .Include(p => p.PackageCities).ThenInclude(pc => pc.City)
                .Include(p => p.TourPackageGuides).ThenInclude(g => g.TouristGuide)
                .Include(p => p.CabinClasses)
                .Include(p => p.PackageItineraries).ThenInclude(d => d.PackageItineraryAttractions);

        /// <summary>
        /// Restricts a query to programs that should be visible to the public/tourists:
        /// published, admin-accepted (المقبولة), and not cancelled. Used by the open
        /// (no-auth) endpoints so drafts, pending, rejected, or cancelled programs never leak.
        /// </summary>
        private static IQueryable<TourPackage> WherePubliclyVisible(IQueryable<TourPackage> query) =>
            query.Where(p => p.IsPublished
                          && p.ApprovalStatus == ProgramApprovalStatus.Accepted
                          && p.Status == TourPackageStatus.Active);

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
            await EnsureCountryExistsAsync(countryId, cancellationToken);
            await EnsureCitiesExistAsync(cityIds, cancellationToken);
        }

        private async Task EnsureCountryExistsAsync(int countryId, CancellationToken cancellationToken)
        {
            var countryExists = await _unitOfWork.Countries
                .Query().AsNoTracking()
                .AnyAsync(c => c.Id == countryId, cancellationToken);
            if (!countryExists)
                throw new NotFoundException($"Country with ID {countryId} not found");
        }

        private async Task EnsureCitiesExistAsync(IReadOnlyCollection<int> cityIds, CancellationToken cancellationToken)
        {
            var distinctIds = cityIds.Distinct().ToList();
            if (distinctIds.Count == 0)
                return; // a non-empty list is enforced by the validator when sent

            var foundCount = await _unitOfWork.Cities
                .Query().AsNoTracking()
                .CountAsync(c => distinctIds.Contains(c.Id), cancellationToken);
            if (foundCount != distinctIds.Count)
                throw new NotFoundException("One or more selected cities do not exist.");
        }

        /// <summary>
        /// Ensures every selected guide is linked to the owning company. Guarantees a
        /// company can only assign its own guides to a program (المرشدون التابعون للشركة).
        /// </summary>
        private async Task EnsureGuidesBelongToCompanyAsync(IReadOnlyCollection<int> guideIds, int companyId, CancellationToken cancellationToken)
        {
            var distinctIds = guideIds.Distinct().ToList();
            if (distinctIds.Count == 0)
                return; // a non-empty list is enforced by the validator

            var ownedCount = await _unitOfWork.CompanyGuides
                .Query().AsNoTracking()
                .CountAsync(cg => cg.CompanyId == companyId && distinctIds.Contains(cg.TouristGuideId), cancellationToken);

            if (ownedCount != distinctIds.Count)
                throw new ForbiddenException("One or more selected guides do not belong to your company.");
        }

        #endregion
    }
}
