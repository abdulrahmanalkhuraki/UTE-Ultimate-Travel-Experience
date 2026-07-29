using Application.Common;
using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.Notifications;
using Application.Interfaces.TourPackage;
using Application.Interfaces.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design;
using System.Globalization;
using System.Security.Authentication;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public partial class TourPackageService : ITourPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TourPackageService> _logger;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUser;
        private readonly TourPackageCreateValidator _createValidator;
        private readonly TourPackageUpdateValidator _updateValidator;
        private readonly INotificationService _notificationService;
        private readonly IMemoryCache _cache;

        private const string MediaFolder = "package-media";
        private const string ActivityImageFolder = "package-activities";

        private const string CacheKeyPrefix = "tp_";
        private const string AllCacheKey = "all_tourpackages";
        private const string UnapprovedCacheKey = "unapproved_tourpackages";
        private const string MineCacheKeyPrefix = "mine_";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan UnapprovedCacheDuration = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan MineCacheDuration = TimeSpan.FromMinutes(3);
        private const string ObjectName = "Tour Package";

        public TourPackageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TourPackageService> logger,
            IFileStorage fileStorage,
            ICurrentUserService currentUser,
            TourPackageCreateValidator createValidator,
            TourPackageUpdateValidator updateValidator,
            INotificationService notificationService,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<TourPackageResponse> CreateAsync(TourPackageCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Tour package create validation failed: {Errors}", validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);
            await EnsureCountryExistsAsync(request.CountryId, cancellationToken);
            await EnsureGuidesBelongToCompanyAsync(request.TouristGuideIds, companyId, cancellationToken);

            _logger.StartOperation("Create", ObjectName, currentUserId);

            try
            {
                var entity = new TourPackage
                {
                    PackageName = request.PackageName.Trim(),
                    Description = request.Description.Trim(),
                    MeetingPoint = request.MeetingPoint.Trim(),
                    Currency = request.Currency.Trim(),
                    DurationInDays = request.DurationInDays,
                    TotalCapacity = request.TotalCapacity,
                    AvailableSeats = request.TotalCapacity,
                    CountryId = request.CountryId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    RegistrationDeadline = request.RegistrationDeadline,
                    ServiceLevel = request.ServiceLevel,
                    CompanyId = companyId,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                };

                foreach (var mediaRequest in request.Media)
                    entity.Media.Add(new TourPackageMedia
                    {
                        MediaUrl = await _fileStorage.SaveAsync(mediaRequest.Media, MediaFolder, cancellationToken),
                        MediaType = mediaRequest.Type,
                        DisplayOrder = mediaRequest.DisplayOrder,
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow,
                    });

                foreach (var guideId in request.TouristGuideIds.Distinct())
                    entity.TourPackageGuides.Add(new TourPackage_TouristGuide { TouristGuideId = guideId });

                if (request.CabinClasses is { Count: > 0 })
                {
                    var defaultClass = request.CabinClasses.First(c => c.IsDefault);
                    entity.PricePerPerson = defaultClass.Price;

                    foreach (var cc in request.CabinClasses)
                        entity.CabinClasses.Add(new TourPackageCabinClass
                        {
                            CabinClass = cc.CabinClass,
                            Price = cc.Price,
                            IsDefault = cc.IsDefault,
                        });
                }
                else
                {
                    entity.PricePerPerson = request.PricePerPerson;
                    entity.CabinClasses.Add(new TourPackageCabinClass
                    {
                        CabinClass = FlightCabinClass.Economy,
                        Price = request.PricePerPerson,
                        IsDefault = true,
                    });
                }

                foreach (var day in request.Days)
                    entity.PackageItineraries.Add(await BuildDayAsync(day, cancellationToken));

                await _unitOfWork.TourPackages.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(companyId: companyId);

                _logger.LogInformation("Created tour package {PackageId} ({PackageName}) for company {CompanyId} | {CabinClassCount} cabin classes, {DayCount} days, {MediaCount} media",
                    entity.Id, entity.PackageName, companyId,
                    entity.CabinClasses.Count, entity.PackageItineraries.Count, entity.Media.Count);

                if (entity.Status == TourPackageStatus.Active)
                    await NotifyFavoritingUsersAsync(companyId, entity, cancellationToken);

                return await BuildResponseAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException)
            {
                _logger.ServerError("Create", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("create", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TourPackageResponse> UpdateAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Tour package update validation failed for package {PackageId}: {Errors}", id, validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            // Partial update: only validate the foreign keys that are actually being changed.
            if (request.CountryId.HasValue)
                await EnsureCountryExistsAsync(request.CountryId.Value, cancellationToken);
            if (request.TouristGuideIds is not null)
                await EnsureGuidesBelongToCompanyAsync(request.TouristGuideIds, companyId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageAttractions)
                    .Include(p => p.TourPackageGuides)
                    .Include(p => p.CabinClasses)
                    .Include(p => p.Media)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.Activities)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.EntityNotFound(ObjectName, id);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
                }

                if (entity.CompanyId != companyId)
                {
                    _logger.ForbiddenAction(currentUserId, "Update", ObjectName, id);
                    throw new ForbiddenException(ExceptionMessages.Forbidden("update", ObjectName));
                }

                if (entity.Status == TourPackageStatus.Completed)
                {
                    _logger.LogWarning("Attempted to update completed tour package {PackageId}", id);
                    throw new BusinessRuleException("Cannot update a completed tour package.");
                }

                var oldStatus = entity.Status;
                var (oldClassPrices, oldPricePerPerson) = await ApplyUpdateAsync(entity, request, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Updated tour package {PackageId} | status {OldStatus}→{NewStatus}, {ChangedFields} fields",
                    id, oldStatus, entity.Status,
                    CountUpdatedFields(request));

                // Notify wishlisted users about price drops
                var pricePerPersonDropped = request.PricePerPerson.HasValue && request.PricePerPerson.Value < oldPricePerPerson;
                var cabinClassPriceDropped = false;
                if (request.CabinClasses is { Count: > 0 })
                {
                    cabinClassPriceDropped = request.CabinClasses.Any(cc =>
                        oldClassPrices.TryGetValue(cc.CabinClass, out var oldPrice) && cc.Price < oldPrice);
                }

                if (pricePerPersonDropped || cabinClassPriceDropped)
                {
                    _logger.LogInformation("Price drop detected for package {PackageId}, triggering wishlist notifications", id);
                    await NotifyWishlistUsersAboutPriceDropAsync(entity, cancellationToken);
                }

                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not ConflictException and not BusinessRuleException)
            {
                _logger.ServerError("Update", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("update", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TourPackageResponse> RepublishAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Tour package republish validation failed for package {PackageId}: {Errors}", id, validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var currentUserId = _currentUser.UserId
                ?? throw new AuthException("User must be authenticated");
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            if (request.CountryId.HasValue)
                await EnsureCountryExistsAsync(request.CountryId.Value, cancellationToken);
            if (request.TouristGuideIds is not null)
                await EnsureGuidesBelongToCompanyAsync(request.TouristGuideIds, companyId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageAttractions)
                    .Include(p => p.TourPackageGuides)
                    .Include(p => p.CabinClasses)
                    .Include(p => p.Media)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.Activities)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Republish target tour package {PackageId} not found", id);
                    throw new NotFoundException($"Tour package with ID {id} not found");
                }

                if (entity.CompanyId != companyId)
                {
                    _logger.LogWarning("User {UserId} tried to republish tour package {PackageId} belonging to company {CompanyId}",
                        currentUserId, id, entity.CompanyId);
                    throw new ForbiddenException("You can only republish your own tour packages.");
                }

                if (entity.Status == TourPackageStatus.Pending || entity.Status == TourPackageStatus.Active)
                {
                    _logger.LogWarning("Attempted to republish tour package {PackageId} with status {Status}", id, entity.Status);
                    throw new BusinessRuleException("Only completed, cancelled, or rejected packages can be republished.");
                }

                var oldStatus = entity.Status;

                if (request.StartDate is null)
                {
                    _logger.LogWarning("Republish tour package {PackageId}: start date is required", id);
                    throw new ValidationException("Start date is required when republishing.");
                }

                if (request.EndDate is null)
                {
                    _logger.LogWarning("Republish tour package {PackageId}: end date is required", id);
                    throw new ValidationException("End date is required when republishing.");
                }

                if (request.StartDate.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    _logger.LogWarning("Republish tour package {PackageId}: start date {StartDate} is not in the future", id, request.StartDate.Value);
                    throw new ValidationException("Start date must be in the future.");
                }

                if (request.StartDate.Value >= request.EndDate.Value)
                {
                    _logger.LogWarning("Republish tour package {PackageId}: start date {StartDate} is not before end date {EndDate}",
                        id, request.StartDate.Value, request.EndDate.Value);
                    throw new ValidationException("Start date must be before end date.");
                }

                entity.Status = TourPackageStatus.Pending;
                entity.RejectionReason = null;
                entity.PublishCount++;
                entity.AvailableSeats = entity.TotalCapacity;

                await ApplyUpdateAsync(entity, request, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Republished tour package {PackageId} | old status {OldStatus}, publish count now {PublishCount}",
                    id, oldStatus, entity.PublishCount);

                return await BuildResponseAsync(id, cancellationToken);
            }
            catch (Exception ex) when (ex is not ValidationException and not NotFoundException and not ForbiddenException and not BusinessRuleException)
            {
                _logger.LogError(ex, "Unexpected error while republishing tour package {PackageId}", id);
                throw new ServiceException($"Failed to republish tour package: {ex.Message}", ex);
            }
        }

        public async Task<ProgramStatusResponse> CancelAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.EntityNotFound(ObjectName, id);
                    throw new NotFoundException(ExceptionMessages.NotFound(ObjectName, id));
                }

                if (entity.CompanyId != companyId)
                {
                    _logger.ForbiddenAction(currentUserId, "cancel", ObjectName, id);
                    throw new ForbiddenException(ExceptionMessages.Forbidden("cancel", ObjectName));
                }

                if (entity.Status == TourPackageStatus.Cancelled)
                {
                    _logger.BusinessRuleViolated(ObjectName, "already cancelled");
                    throw new BusinessRuleException(ExceptionMessages.BusinessRule("This tour package is already cancelled."));
                }

                var oldStatus = entity.Status;
                entity.Status = TourPackageStatus.Cancelled;
                entity.CancelledAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Cancelled tour package {PackageId} | status {OldStatus}→Cancelled", id, oldStatus);

                return new ProgramStatusResponse
                {
                    Id = entity.Id,
                    PackageName = entity.PackageName,
                    Status = entity.Status,
                    RejectionReason = entity.RejectionReason,
                    UpdatedAtUtc = entity.UpdatedAtUtc,
                };
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException)
            {
                _logger.ServerError("Cancel", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("cancel", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<TourPackageResponse>> GetUnApprovedAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(UnapprovedCacheKey, out IReadOnlyList<TourPackageResponse>? cached) && cached is not null)
                return cached;

            var entities = await QueryWithGraph()
                .Where(p => p.Status == TourPackageStatus.Pending)
                .OrderBy(p => p.CreatedAtUtc) // longest-waiting first
                .ToListAsync(cancellationToken);

            var response = _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);

            _cache.Set(UnapprovedCacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = UnapprovedCacheDuration,
                Priority = CacheItemPriority.Normal
            });

            return response;
        }

        public async Task<ProgramStatusResponse> ApproveAsync(int id, CancellationToken cancellationToken = default)
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
                {
                    _logger.LogWarning("Approve target tour package {PackageId} not found", id);
                    throw new NotFoundException($"Tour package with ID {id} not found");
                }

                var oldStatus = entity.Status;
                entity.Status = TourPackageStatus.Active;
                entity.RejectionReason = null;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Tour package {PackageId} approved | status {OldStatus}→Active", id, oldStatus);

                var ownerUserId = entity.Company?.UserId ?? 0;
                if (ownerUserId > 0)
                {
                    try
                    {
                        await _notificationService.NotifyAsync(ownerUserId, PackageApprovalMessages.Accepted, NotificationType.PackageAccepted, cancellationToken);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogWarning(notifyEx, "Failed to notify owner {UserId} of tour package {PackageId} approval", ownerUserId, id);
                    }
                }

                await NotifyFavoritingUsersAsync(entity.CompanyId, entity, cancellationToken);

                return new ProgramStatusResponse
                {
                    Id = entity.Id,
                    PackageName = entity.PackageName,
                    Status = entity.Status,
                    RejectionReason = entity.RejectionReason,
                    UpdatedAtUtc = entity.UpdatedAtUtc,
                };
            }
            catch (Exception ex) when (ex is not NotFoundException and not ArgumentException)
            {
                _logger.ServerError("Approve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("approve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<ProgramStatusResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            if (string.IsNullOrWhiteSpace(reason))
            {
                _logger.LogWarning("Reject tour package {PackageId}: rejection reason is required", id);
                throw new ArgumentException("Rejection reason is required", nameof(reason));
            }

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.Company)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Reject target tour package {PackageId} not found", id);
                    throw new NotFoundException($"Tour package with ID {id} not found");
                }

                var oldStatus = entity.Status;
                entity.Status = TourPackageStatus.Rejected;
                entity.RejectionReason = reason;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Tour package {PackageId} rejected | status {OldStatus}→Rejected", id, oldStatus);

                var ownerUserId = entity.Company?.UserId ?? 0;
                if (ownerUserId > 0)
                {
                    try
                    {
                        var message = $"{PackageApprovalMessages.Rejected} السبب: {reason}";
                        await _notificationService.NotifyAsync(ownerUserId, message, NotificationType.PackageRejected, cancellationToken);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogWarning(notifyEx, "Failed to notify owner {UserId} of tour package {PackageId} rejection", ownerUserId, id);
                    }
                }

                return new ProgramStatusResponse
                {
                    Id = entity.Id,
                    PackageName = entity.PackageName,
                    Status = entity.Status,
                    RejectionReason = entity.RejectionReason,
                    UpdatedAtUtc = entity.UpdatedAtUtc,
                };
            }
            catch (Exception ex) when (ex is not NotFoundException and not ArgumentException)
            {
                _logger.ServerError("Reject", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("reject", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            _logger.StartOperation("Delete", ObjectName, id, currentUserId);

            try
            {
                var entity = await _unitOfWork.TourPackages
                    .Query()
                    .Include(p => p.PackageAttractions)
                    .Include(p => p.Media)
                    .Include(p => p.PackageItineraries)
                        .ThenInclude(d => d.Activities)
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("Delete target tour package {PackageId} not found", id);
                    return false;
                }

                if (entity.CompanyId != companyId)
                {
                    _logger.ForbiddenAction(currentUserId, "delete", ObjectName, id);
                    throw new ForbiddenException(ExceptionMessages.Forbidden("delete", ObjectName));
                }

                var hasBookings = await _unitOfWork.TourPackages
                    .Query()
                    .Where(p => p.Id == id)
                    .SelectMany(p => p.Bookings)
                    .AnyAsync(cancellationToken);
                if (hasBookings)
                {
                    _logger.LogWarning("Attempted to delete tour package {PackageId} which has bookings", id);
                    throw new BusinessRuleException("Cannot delete a tour package that already has bookings.");
                }

                var activityCount = entity.PackageItineraries.SelectMany(d => d.Activities).Count();
                var mediaCount = entity.Media.Count;
                var attractionsCount = entity.PackageAttractions.Count;
                var itineraryCount = entity.PackageItineraries.Count;

                var activities = entity.PackageItineraries.SelectMany(d => d.Activities).ToList();
                _unitOfWork.Activities.RemoveRange(activities);
                _unitOfWork.Itineraries.RemoveRange(entity.PackageItineraries);
                _unitOfWork.TourPackage_Attraction.RemoveRange(entity.PackageAttractions);
                _unitOfWork.Media.RemoveRange(entity.Media);
                _unitOfWork.TourPackages.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourPackageCache(id, entity.CompanyId);

                _logger.LogInformation("Deleted tour package {PackageId} ({PackageName}) | cleaned {AttractionsCount} attractions, {ItineraryCount} days, {ActivityCount} activities, {MediaCount} media",
                    id, entity.PackageName, attractionsCount, itineraryCount, activityCount, mediaCount);
                return true;
            }
            catch (Exception ex) when (ex is not NotFoundException and not ForbiddenException and not BusinessRuleException)
            {
                _logger.ServerError("Delete", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("delete", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TourPackageResponse> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            var cacheKey = $"{CacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out TourPackageResponse? cached) && cached is not null)
                return cached;

            var entity = await WherePubliclyVisible(QueryWithGraph())
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (entity is null)
            {
                _logger.LogWarning("tour package {PackageId} not found", id);
                throw new NotFoundException($"Tour package with ID {id} not found");
            }

            var response = _mapper.Map<TourPackageResponse>(entity);

            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration,
                SlidingExpiration = SlidingCacheDuration,
                Priority = CacheItemPriority.Normal
            });

            return response;
        }

        public async Task<PaginatedResponse<TourPackageResponse>> GetAllAsync(int page = 1, int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{AllCacheKey}_page{page}_pageSize{pageSize}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResponse<TourPackageResponse>? cached) && cached is not null)
                return cached;

            try
            {
                var query = WherePubliclyVisible(QueryWithGraph());

                var entities = await query
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var packageResponses = _mapper.Map<IReadOnlyCollection<TourPackageResponse>>(entities);
                var paginationMetadata = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = await query.CountAsync(cancellationToken)
                };

                var response = new PaginatedResponse<TourPackageResponse>
                {
                    Items = packageResponses,
                    Pagination = paginationMetadata
                };

                _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve All", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<PaginatedResponse<TourPackageResponse>> GetMineAsync(int page = 1, int pageSize = 20,
            TourPackageStatus? status = null, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthException("User must be authenticated");

            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var mineKey = status.HasValue
                    ? $"{MineCacheKeyPrefix}{companyId}_{status.Value}"
                    : $"{MineCacheKeyPrefix}{companyId}_all";

                mineKey = mineKey + $"page{page}_pageSize{pageSize}"; 

                if (_cache.TryGetValue(mineKey, out PaginatedResponse<TourPackageResponse>? cached) && cached is not null)
                    return cached;

                var query = QueryWithGraph()
                    .Where(p => p.CompanyId == companyId);

                if (status.HasValue)
                    query = query.Where(p => p.Status == status.Value);

                var entities = await query
                    .Skip((page-1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var packageResponses = _mapper.Map<IReadOnlyList<TourPackageResponse>>(entities);
                var paginationMetadata = new PaginationMetadata()
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = await query.CountAsync(cancellationToken)
                };
                var paginatedResponse = new PaginatedResponse<TourPackageResponse>()
                {
                    Items = packageResponses,
                    Pagination = paginationMetadata
                };


                _cache.Set(mineKey, paginatedResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = MineCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Mine", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<TourPackageResponse> GetMineAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour package ID", nameof(id));

            int userId = _currentUser.UserId ?? throw new AuthException("User must be authenticated");
            int companyId = await ResolveCompanyIdAsync(userId, cancellationToken);
            try
            {

                var entity = await QueryWithGraph()
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (entity is null)
                {
                    _logger.LogWarning("tour package {PackageId} not found", id);
                    throw new NotFoundException($"Tour package with ID {id} not found");
                }

                if (entity.CompanyId != companyId)
                {
                    _logger.LogWarning(
                        "Authorization failed: User {UserId} from Company {UserCompanyId} attempted to access Package" +
                        " {PackageId} belonging to Company {PackageCompanyId}",
                        userId,
                        companyId,
                        entity.Id,
                        entity.CompanyId);

                    throw new ForbiddenException("Access denied: You do not have permission to access " +
                        $"package '{entity.Id}' as it belongs to a different company.");
                }

                var response = _mapper.Map<TourPackageResponse>(entity);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.ServerError("Retrieve Mine By Id", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<IReadOnlyList<TourPackageResponse>> FilterAsync(
            int? countryId = null,
            int? cityId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            var query = WherePubliclyVisible(QueryWithGraph());

            if (countryId is > 0)
                query = query.Where(p => p.CountryId == countryId);
            if (cityId is > 0)
                query = query.Where(p => p.PackageAttractions.Any(pa => pa.Attraction.CityId == cityId));
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

        public async Task<PackageStatsResponse> GetPackageStatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthenticationException(ExceptionMessages.AuthFailure("not authenticated"));

            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            var raw = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                .GroupBy(p => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Active = g.Count(p => p.Status == TourPackageStatus.Active),
                    Rejected = g.Count(p => p.Status == TourPackageStatus.Rejected),
                    Completed = g.Count(p => p.Status == TourPackageStatus.Completed),
                    Cancelled = g.Count(p => p.Status == TourPackageStatus.Cancelled),
                })
                .FirstOrDefaultAsync(cancellationToken);

            var monthlyRaw = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && !p.IsDeleted && p.PublishedAtUtc != null)
                .GroupBy(p => new { p.PublishedAtUtc!.Value.Year, p.PublishedAtUtc!.Value.Month })
                .Select(m => new { m.Key.Year, m.Key.Month, Count = m.Count() })
                .ToListAsync(cancellationToken);

            var monthlyLookup = monthlyRaw.ToDictionary(
                m => (Year: m.Year, Month: m.Month),
                m => m.Count);

            var today = DateTime.UtcNow;
            var monthlyPublished = new List<MonthlyPackageCount>(12);
            for (var i = 11; i >= 0; i--)
            {
                var date = today.AddMonths(-i);
                var year = date.Year;
                var month = date.Month;
                monthlyPublished.Add(new MonthlyPackageCount
                {
                    Year = year,
                    Month = month,
                    MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month),
                    PublishedCount = monthlyLookup.GetValueOrDefault((Year: year, Month: month), 0)
                });
            }

            return new PackageStatsResponse
            {
                TotalPackages = raw?.Total ?? 0,
                ActivePackages = raw?.Active ?? 0,
                RejectedPackages = raw?.Rejected ?? 0,
                CompletedPackages = raw?.Completed ?? 0,
                CancelledPackages = raw?.Cancelled ?? 0,
                MonthlyPublished = monthlyPublished.AsReadOnly()
            };
        }

        public async Task<RateAndReviewStatsResponse> GetRateAndReviewStatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthenticationException(ExceptionMessages.AuthFailure("not authenticated"));

            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            var summary = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                .GroupBy(p => 1)
                .Select(g => new
                {
                    TotalRatings = g.Sum(p => p.Rates.Count),
                    TotalReviews = g.Sum(p => p.Reviews.Count),
                    AverageRating = g.SelectMany(p => p.Rates).Select(r => (double)r.RateValue).DefaultIfEmpty().Average()
                })
                .FirstOrDefaultAsync(cancellationToken);

            var monthlyRatings = await _unitOfWork.Rates
                .Query()
                .AsNoTracking()
                .Where(r => r.Package.CompanyId == companyId && !r.Package.IsDeleted)
                .GroupBy(r => new { r.CreatedAtUtc.Year, r.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var monthlyReviews = await _unitOfWork.Reviews
                .Query()
                .AsNoTracking()
                .Where(r => r.Package.CompanyId == companyId && !r.Package.IsDeleted)
                .GroupBy(r => new { r.CreatedAtUtc.Year, r.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var ratingsLookup = monthlyRatings.ToDictionary(m => (m.Year, m.Month), m => m.Count);
            var reviewsLookup = monthlyReviews.ToDictionary(m => (m.Year, m.Month), m => m.Count);

            var today = DateTime.UtcNow;
            var monthlyStats = new List<MonthlyRateReviewCount>(12);
            for (var i = 11; i >= 0; i--)
            {
                var date = today.AddMonths(-i);
                monthlyStats.Add(new MonthlyRateReviewCount
                {
                    Year = date.Year,
                    Month = date.Month,
                    MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(date.Month),
                    RatingCount = ratingsLookup.GetValueOrDefault((date.Year, date.Month), 0),
                    ReviewCount = reviewsLookup.GetValueOrDefault((date.Year, date.Month), 0)
                });
            }

            return new RateAndReviewStatsResponse
            {
                AverageRating = summary is not null ? Math.Round(summary.AverageRating, 2) : 0.0,
                TotalRatings = summary?.TotalRatings ?? 0,
                TotalReviews = summary?.TotalReviews ?? 0,
                MonthlyStats = monthlyStats.AsReadOnly()
            };
        }

        public async Task<TouristStatsResponse> GetTouristStatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthenticationException(ExceptionMessages.AuthFailure("not authenticated"));

            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            var statusFilter = new[]
            {
                BookingStatus.Rejected_By_Company,
                BookingStatus.Rejected_By_Tourist,
                BookingStatus.Cancelled
            };

            var totalUniqueTourists = await _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .Where(b => b.TourPackage.CompanyId == companyId
                         && !statusFilter.Contains(b.Status))
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var latestBookings = await _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .Where(b => b.TourPackage.CompanyId == companyId
                         && !statusFilter.Contains(b.Status))
                .OrderByDescending(b => b.BookingDate)
                .ThenByDescending(b => b.Id)
                .Take(10)
                .Select(b => new LatestBookingItem
                {
                    Id = b.Id,
                    TouristName = b.User.Person!.FirstName + " " + b.User.Person!.LastName,
                    TouristImage = b.User.Person!.ProfileImage,
                    BookingDate = b.BookingDate,
                    PackageName = b.TourPackage.PackageName
                })
                .ToListAsync(cancellationToken);

            var monthlyRaw = await _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .Where(b => b.TourPackage.CompanyId == companyId
                         && !statusFilter.Contains(b.Status))
                .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var lookup = monthlyRaw.ToDictionary(m => (m.Year, m.Month), m => m.Count);

            var today = DateTime.UtcNow;
            var monthlyBookings = new List<MonthlyBookingCount>(12);
            for (var i = 11; i >= 0; i--)
            {
                var date = today.AddMonths(-i);
                monthlyBookings.Add(new MonthlyBookingCount
                {
                    Year = date.Year,
                    Month = date.Month,
                    MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(date.Month),
                    BookingCount = lookup.GetValueOrDefault((date.Year, date.Month), 0)
                });
            }

            return new TouristStatsResponse
            {
                TotalUniqueTourists = totalUniqueTourists,
                LatestBookings = latestBookings.AsReadOnly(),
                MonthlyBookings = monthlyBookings.AsReadOnly()
            };
        }

        #region Helpers

        /// <summary>
        /// Applies scalar, media, guide, cabin class, and itinerary updates from
        /// <paramref name="request"/> onto <paramref name="entity"/>.
        /// Returns the old cabin-class prices so callers can detect drops for notifications.
        /// </summary>
        private async Task<(Dictionary<FlightCabinClass, decimal> OldClassPrices, decimal OldPricePerPerson)> ApplyUpdateAsync(
            TourPackage entity, TourPackageUpdateRequest request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Applying update to tour package {PackageId}", entity.Id);

            var oldClassPrices = entity.CabinClasses.ToDictionary(c => c.CabinClass, c => c.Price);
            var oldPricePerPerson = entity.PricePerPerson;

            // Scalars — partial update: apply only the fields that were actually sent
            if (request.PackageName is not null) entity.PackageName = request.PackageName.Trim();
            if (request.Description is not null) entity.Description = request.Description.Trim();
            if (request.MeetingPoint is not null) entity.MeetingPoint = request.MeetingPoint.Trim();
            if (request.Currency is not null) entity.Currency = request.Currency.Trim();
            if (request.DurationInDays.HasValue) entity.DurationInDays = request.DurationInDays.Value;
            if (request.TotalCapacity.HasValue)
            {
                if (request.TotalCapacity.Value < entity.AvailableSeats)
                {
                    _logger.LogWarning("Update tour package {PackageId}: cannot reduce total capacity ({NewCapacity}) below available seats ({AvailableSeats})",
                        entity.Id, request.TotalCapacity.Value, entity.AvailableSeats);
                    throw new BusinessRuleException("Total capacity cannot be reduced below the number of available seats.");
                }

                var delta = request.TotalCapacity.Value - entity.TotalCapacity;
                entity.TotalCapacity = request.TotalCapacity.Value;
                entity.AvailableSeats += delta;
            }
            if (request.CountryId.HasValue) entity.CountryId = request.CountryId.Value;
            if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) entity.EndDate = request.EndDate.Value;
            if (request.RegistrationDeadline.HasValue) entity.RegistrationDeadline = request.RegistrationDeadline.Value;
            if (request.ServiceLevel.HasValue) entity.ServiceLevel = request.ServiceLevel.Value;

            entity.UpdatedAtUtc = DateTime.UtcNow;

            // Handle media updates (only when sent).
            if (request.ExistingMedia is not null || request.Media is not null)
            {
                var updatedIds = request.ExistingMedia?
                    .Where(m => m.Id.HasValue && m.Id.Value > 0)
                    .Select(m => m.Id!.Value)
                    .ToHashSet() ?? [];

                var toRemove = entity.Media.Where(m => !updatedIds.Contains(m.Id)).ToList();
                _unitOfWork.Media.RemoveRange(toRemove);

                foreach (var existing in request.ExistingMedia ?? [])
                {
                    var mediaEntity = entity.Media.FirstOrDefault(m => m.Id == existing.Id);
                    if (mediaEntity is null) continue;

                    if (existing.Media is not null)
                        mediaEntity.MediaUrl = await _fileStorage.SaveAsync(existing.Media, MediaFolder, cancellationToken);
                    if (existing.Type.HasValue)
                        mediaEntity.MediaType = existing.Type.Value;
                    if (existing.DisplayOrder.HasValue)
                        mediaEntity.DisplayOrder = existing.DisplayOrder.Value;
                    mediaEntity.UpdatedAtUtc = DateTime.UtcNow;
                }

                foreach (var newMedia in request.Media ?? [])
                {
                    entity.Media.Add(new TourPackageMedia
                    {
                        TourPackageId = entity.Id,
                        MediaUrl = await _fileStorage.SaveAsync(newMedia.Media, MediaFolder, cancellationToken),
                        MediaType = newMedia.Type,
                        DisplayOrder = newMedia.DisplayOrder,
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow,
                    });
                }
            }

            // Replace assigned guides (only when sent).
            if (request.TouristGuideIds is not null)
            {
                _unitOfWork.TourPackage_TouristGuide.RemoveRange(entity.TourPackageGuides);
                entity.TourPackageGuides = request.TouristGuideIds.Distinct()
                    .Select(guideId => new TourPackage_TouristGuide { TouristGuideId = guideId })
                    .ToList();
            }

            // Replace available cabin classes (only when sent).
            if (request.CabinClasses is not null)
            {
                _unitOfWork.TourPackageCabinClasses.RemoveRange(entity.CabinClasses);
                entity.CabinClasses.Clear();

                if (request.CabinClasses.Count > 0)
                {
                    var defaultClass = request.CabinClasses.First(c => c.IsDefault);
                    entity.PricePerPerson = defaultClass.Price;

                    foreach (var cc in request.CabinClasses)
                        entity.CabinClasses.Add(new TourPackageCabinClass
                        {
                            CabinClass = cc.CabinClass,
                            Price = cc.Price,
                            IsDefault = cc.IsDefault,
                        });
                }
                else
                {
                    if (request.PricePerPerson.HasValue)
                        entity.PricePerPerson = request.PricePerPerson.Value;
                    entity.CabinClasses.Add(new TourPackageCabinClass
                    {
                        CabinClass = FlightCabinClass.Economy,
                        Price = entity.PricePerPerson,
                        IsDefault = true,
                    });
                }
            }

            // Replace the whole itinerary (days + activities) only when sent.
            if (request.Days is not null)
            {
                var oldActivities = entity.PackageItineraries.SelectMany(d => d.Activities).ToList();
                _unitOfWork.Activities.RemoveRange(oldActivities);
                _unitOfWork.Itineraries.RemoveRange(entity.PackageItineraries);

                var newDays = new List<Itinerary>();
                foreach (var day in request.Days)
                    newDays.Add(await BuildDayAsync(day, cancellationToken));
                entity.PackageItineraries = newDays;
            }

            return (oldClassPrices, oldPricePerPerson);
        }

        /// <summary>Returns a rough count of how many update-request fields were actually sent.</summary>
        private static int CountUpdatedFields(TourPackageUpdateRequest request)
        {
            var count = 0;
            if (request.PackageName is not null) count++;
            if (request.Description is not null) count++;
            if (request.MeetingPoint is not null) count++;
            if (request.Currency is not null) count++;
            if (request.DurationInDays.HasValue) count++;
            if (request.TotalCapacity.HasValue) count++;
            if (request.CountryId.HasValue) count++;
            if (request.StartDate.HasValue) count++;
            if (request.EndDate.HasValue) count++;
            if (request.RegistrationDeadline.HasValue) count++;
            if (request.ServiceLevel.HasValue) count++;
            if (request.PricePerPerson.HasValue) count++;
            if (request.CabinClasses is not null) count++;
            if (request.TouristGuideIds is not null) count++;
            if (request.ExistingMedia is not null) count++;
            if (request.Media is not null) count++;
            if (request.Days is not null) count++;
            return count;
        }

        /// <summary>Invalidates cached entries affected by a mutation to the given package.</summary>
        private void InvalidateTourPackageCache(int? packageId = null, int? companyId = null)
        {
            if (packageId.HasValue)
                _cache.Remove($"{CacheKeyPrefix}{packageId.Value}");

            _cache.Remove(AllCacheKey);
            _cache.Remove(UnapprovedCacheKey);

            if (companyId.HasValue)
            {
                _cache.Remove($"{MineCacheKeyPrefix}{companyId.Value}_all");
                foreach (var status in Enum.GetValues<TourPackageStatus>())
                {
                    _cache.Remove($"mine_{companyId}_{status}");
                }
            }
        }

        /// <summary>Base query that eager-loads everything needed to build a response.</summary>
        private IQueryable<TourPackage> QueryWithGraph() =>
            _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.Country)
                .Include(p => p.Company)
                .Include(p => p.Rates)
                .Include(p => p.TourPackageGuides).ThenInclude(g => g.TouristGuide)
                .Include(p => p.CabinClasses)
                .Include(p => p.Media)
                .Include(p => p.PackageAttractions).ThenInclude(pa => pa.Attraction)
                .Include(p => p.PackageItineraries).ThenInclude(d => d.Activities);


        /// <summary>
        /// Restricts a query to programs that should be visible to the public/tourists:
        /// active and not cancelled or pending. Used by the open
        /// (no-auth) endpoints so drafts, pending, rejected, or cancelled programs never leak.
        /// </summary>
        private static IQueryable<TourPackage> WherePubliclyVisible(IQueryable<TourPackage> query) =>
            query.Where(p => p.Status == TourPackageStatus.Active);

        private async Task<TourPackageResponse> BuildResponseAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await QueryWithGraph().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (entity is null)
            {
                _logger.LogWarning("BuildResponseAsync: tour package {PackageId} not found after save", id);
                throw new NotFoundException($"Tour package with ID {id} not found");
            }
            _logger.LogTrace("Built response for tour package {PackageId}", id);
            return _mapper.Map<TourPackageResponse>(entity);
        }

        private async Task<Itinerary> BuildDayAsync(TourPackageDayRequest day, CancellationToken cancellationToken)
        {
            var itinerary = new Itinerary
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

                itinerary.Activities.Add(new Activity
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
            {
                _logger.LogWarning("ResolveCompanyId: invalid user ID {UserId}", ownerUserId);
                throw new ForbiddenException("You must be signed in as a tour company.");
            }

            var company = await _unitOfWork.TourCompanies
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == ownerUserId, cancellationToken);

            if (company is null)
            {
                _logger.LogWarning("ResolveCompanyId: user {UserId} has no registered tour company", ownerUserId);
                throw new ForbiddenException("You must have a registered tour company to manage tour packages.");
            }

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



        /// <summary>
        /// Ensures every selected guide is linked to the owning company. Guarantees a
        /// company can only assign its own guides to a program (المرشدون التابعون للشركة).
        /// </summary>
        private async Task EnsureGuidesBelongToCompanyAsync(IReadOnlyCollection<int> guideIds, int companyId, CancellationToken cancellationToken)
        {
            var distinctIds = guideIds.Distinct().ToList();
            if (distinctIds.Count == 0)
                return; // a non-empty list is enforced by the validator

            var ownedCount = await _unitOfWork.Company_TouristGuide
                .Query().AsNoTracking()
                .CountAsync(cg => cg.CompanyId == companyId && distinctIds.Contains(cg.TouristGuideId), cancellationToken);

            if (ownedCount != distinctIds.Count)
            {
                _logger.LogWarning("EnsureGuidesBelongToCompany: company {CompanyId} does not own one or more of the requested guides {GuideIds}",
                    companyId, distinctIds);
                throw new ForbiddenException("One or more selected guides do not belong to your company.");
            }
        }





        private async Task NotifyFavoritingUsersAsync(int companyId, TourPackage package, CancellationToken cancellationToken)
        {
            try
            {
                var favoritingUserIds = await _unitOfWork.Favorites
                    .Query()
                    .AsNoTracking()
                    .Where(f => f.CompanyId == companyId)
                    .Select(f => f.UserId)
                    .ToListAsync(cancellationToken);

                if (favoritingUserIds.Count == 0)
                {
                    _logger.LogDebug("No favoriting users to notify for company {CompanyId}", companyId);
                    return;
                }

                var message = $"New package '{package.PackageName}' has been published by a company you follow!";
                var notifiedCount = 0;

                foreach (var userId in favoritingUserIds)
                {
                    try
                    {
                        await _notificationService.NotifyAsync(userId, message, NotificationType.NewPackage, cancellationToken);
                        notifiedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send notification to user {UserId} about new package {PackageId}",
                            userId, package.Id);
                    }
                }

                _logger.LogInformation("Successfully notified {NotifiedCount}/{TotalCount} users about new package {PackageId}",
                    notifiedCount, favoritingUserIds.Count, package.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying favoriting users about new package {PackageId}", package.Id);
            }
        }

        private async Task NotifyWishlistUsersAboutPriceDropAsync(TourPackage package, CancellationToken cancellationToken)
        {
            try
            {
                var wishlistUserIds = await _unitOfWork.Wishlists
                    .Query()
                    .AsNoTracking()
                    .Where(w => w.TourPackageId == package.Id)
                    .Select(w => w.UserId)
                    .ToListAsync(cancellationToken);

                if (wishlistUserIds.Count == 0)
                {
                    _logger.LogDebug("No wishlist users to notify for price drop on package {PackageId}", package.Id);
                    return;
                }

                var message = $"Great news! The price for '{package.PackageName}' has dropped! Check it out now.";
                var notifiedCount = 0;

                foreach (var userId in wishlistUserIds)
                {
                    try
                    {
                        await _notificationService.NotifyAsync(userId, message, NotificationType.PriceDrop, cancellationToken);
                        notifiedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send price-drop notification to user {UserId} for package {PackageId}",
                            userId, package.Id);
                    }
                }

                _logger.LogInformation("Notified {NotifiedCount}/{TotalCount} wishlist users about price drop on package {PackageId}",
                    notifiedCount, wishlistUserIds.Count, package.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying wishlist users about price drop on package {PackageId}", package.Id);
            }
        }



        #endregion
    }
}
