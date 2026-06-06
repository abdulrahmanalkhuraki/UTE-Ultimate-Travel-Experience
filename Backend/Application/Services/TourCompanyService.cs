using Application.Common;
using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Notifications;
using Application.Interfaces.TourCompany;
using Application.Interfaces.User;
using Application.Validators.TourCompany;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TourCompanyEntity = Domain.Entities.TourCompany;
using ValidationException = Application.Exceptions.ValidationException;

namespace Application.Services
{
    public class TourCompanyService : ITourCompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TourCompanyService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IFileStorage _fileStorage;
        private readonly TourCompanyCreateValidator _createValidator;
        private readonly TourCompanyUpdateValidator _updateValidator;
        private readonly INotificationService _notificationService;

        // Cache constants
        private const string TourCompanyCacheKeyPrefix = "tourcompany_";
        private const string TourCompaniesListCacheKey = "all_tourcompanies";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan SlidingCacheDuration = TimeSpan.FromMinutes(2);

        public TourCompanyService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TourCompanyService> logger,
            IMemoryCache cache,
            IFileStorage fileStorage,
            TourCompanyCreateValidator createValidator,
            TourCompanyUpdateValidator updateValidator,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<TourCompanyResponse> CreateAsync(int ownerUserId, TourCompanyCreateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (ownerUserId <= 0)
                throw new ArgumentException("Invalid owner user ID", nameof(ownerUserId));

            _logger.LogInformation("Attempting to create tour company {CompanyName} for user {UserId}",
                request.Name, ownerUserId);

            // Validate request
            var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Tour company creation validation failed for {CompanyName}: {Errors}",
                    request.Name, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                // The owner must exist.
                var ownerExists = await _unitOfWork.Users
                    .Query()
                    .AnyAsync(u => u.Id == ownerUserId, cancellationToken);

                if (!ownerExists)
                {
                    _logger.LogWarning("Owner user {UserId} not found while creating tour company", ownerUserId);
                    throw new NotFoundException($"User with ID '{ownerUserId}' not found");
                }

                // One company per owner with the same name.
                var exists = await _unitOfWork.TourCompanies
                    .Query()
                    .AnyAsync(c => c.UserId == ownerUserId && c.Name == request.Name, cancellationToken);

                if (exists)
                {
                    _logger.LogWarning("Duplicate tour company attempt: {CompanyName} for user {UserId}",
                        request.Name, ownerUserId);
                    throw new ConflictException($"You already have a company named '{request.Name}'");
                }

                var entity = _mapper.Map<TourCompanyEntity>(request);
                entity.UserId = ownerUserId;
                // New companies await admin approval before becoming publicly visible.
                entity.Status = TourCompanyStatus.Pending;
                entity.CreatedAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = DateTime.UtcNow;

                // Persist uploaded images, if any.
                if (request.Logo is not null)
                    entity.Logo = await _fileStorage.SaveAsync(request.Logo, "company-logos", cancellationToken);

                if (request.TourismLicenseImage is not null)
                    entity.TourismLicenseImage = await _fileStorage.SaveAsync(request.TourismLicenseImage, "tourism-licenses", cancellationToken);

                await _unitOfWork.TourCompanies.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourCompanyCache();

                var response = _mapper.Map<TourCompanyResponse>(entity);

                _logger.LogInformation("Successfully created tour company {CompanyId} ({CompanyName}) for user {UserId}",
                    entity.Id, entity.Name, ownerUserId);

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogError(ex, "Database unique constraint violation while creating tour company {CompanyName}", request.Name);
                throw new ConflictException($"Tour company '{request.Name}' already exists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating tour company {CompanyName}", request.Name);
                throw new ServiceException($"Failed to create tour company: {ex.Message}", ex);
            }
        }

        public async Task<TourCompanyResponse> GetAsync(int id, int? requestingUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour company ID", nameof(id));

            _logger.LogDebug("Retrieving tour company with ID {CompanyId}", id);

            // Only approved companies are cached, since visibility of a non-approved
            // company depends on who is asking.
            var cacheKey = $"{TourCompanyCacheKeyPrefix}{id}";
            if (_cache.TryGetValue(cacheKey, out TourCompanyResponse? cached) && cached != null)
            {
                _logger.LogDebug("Cache hit for tour company {CompanyId}", id);
                return cached;
            }

            try
            {
                var entity = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                {
                    _logger.LogDebug("Tour company with ID {CompanyId} not found", id);
                    throw new NotFoundException($"Tour company with ID {id} not found");
                }

                // A company that is not approved is only visible to its owner or an admin.
                // To everyone else it behaves as if it does not exist.
                if (entity.Status != TourCompanyStatus.Approved
                    && !isAdmin
                    && entity.UserId != requestingUserId)
                {
                    _logger.LogDebug("Tour company {CompanyId} is {Status}; hidden from user {UserId}",
                        id, entity.Status, requestingUserId?.ToString() ?? "anonymous");
                    throw new NotFoundException($"Tour company with ID {id} not found");
                }

                var response = _mapper.Map<TourCompanyResponse>(entity);

                if (entity.Status == TourCompanyStatus.Approved)
                {
                    _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheDuration,
                        SlidingExpiration = SlidingCacheDuration,
                        Priority = CacheItemPriority.Normal
                    });
                }

                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tour company {CompanyId}", id);
                throw new ServiceException($"Failed to retrieve tour company: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<TourCompanyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving all tour companies");

            if (_cache.TryGetValue(TourCompaniesListCacheKey, out IReadOnlyList<TourCompanyResponse>? cached) && cached != null)
            {
                _logger.LogDebug("Cache hit for all tour companies");
                return cached;
            }

            try
            {
                var entities = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .Where(c => c.Status == TourCompanyStatus.Approved)
                    .OrderBy(c => c.Name)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<TourCompanyResponse>>(entities);

                _cache.Set(TourCompaniesListCacheKey, response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration,
                    Priority = CacheItemPriority.Low
                });

                _logger.LogDebug("Successfully retrieved {Count} tour companies", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tour companies");
                throw new ServiceException($"Failed to retrieve tour companies: {ex.Message}", ex);
            }
        }

        public async Task<TourCompanyResponse> UpdateAsync(int id, int requestingUserId, bool isAdmin, TourCompanyUpdateRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (id <= 0)
                throw new ArgumentException("Invalid tour company ID", nameof(id));

            _logger.LogInformation("Attempting to update tour company {CompanyId} by user {UserId}", id, requestingUserId);

            var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Tour company update validation failed for {CompanyId}: {Errors}",
                    id, string.Join(", ", validationResult.Errors));
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            try
            {
                var entity = await _unitOfWork.TourCompanies.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Tour company {CompanyId} not found for update", id);
                    throw new NotFoundException($"Tour company with ID '{id}' not found");
                }

                // Ownership: only the owner or an admin may update.
                if (!isAdmin && entity.UserId != requestingUserId)
                {
                    _logger.LogWarning("User {UserId} forbidden from updating tour company {CompanyId}", requestingUserId, id);
                    throw new ForbiddenException("You are not allowed to modify this company");
                }

                // Reject a duplicate name within the same owner's companies.
                if (request.Name is not null && request.Name != entity.Name)
                {
                    var exists = await _unitOfWork.TourCompanies
                        .Query()
                        .AnyAsync(c => c.UserId == entity.UserId && c.Name == request.Name && c.Id != id, cancellationToken);

                    if (exists)
                    {
                        _logger.LogWarning("Duplicate tour company name '{Name}' for user {UserId}", request.Name, entity.UserId);
                        throw new ConflictException($"You already have a company named '{request.Name}'");
                    }
                }

                _mapper.Map(request, entity);

                // Replace images only when a new file is uploaded.
                if (request.Logo is not null)
                    entity.Logo = await _fileStorage.SaveAsync(request.Logo, "company-logos", cancellationToken);

                if (request.TourismLicenseImage is not null)
                    entity.TourismLicenseImage = await _fileStorage.SaveAsync(request.TourismLicenseImage, "tourism-licenses", cancellationToken);

                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.TourCompanies.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourCompanyCache(id);

                var response = _mapper.Map<TourCompanyResponse>(entity);

                _logger.LogInformation("Successfully updated tour company {CompanyId}", id);

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
            catch (ConflictException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict while updating tour company {CompanyId}", id);
                throw new ConcurrencyException("The company was modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating tour company {CompanyId}", id);
                throw new ServiceException($"Failed to update tour company: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, int requestingUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour company ID", nameof(id));

            _logger.LogInformation("Attempting to delete tour company {CompanyId} by user {UserId}", id, requestingUserId);

            try
            {
                var entity = await _unitOfWork.TourCompanies.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Tour company {CompanyId} not found for deletion", id);
                    return false;
                }

                // Ownership: only the owner or an admin may delete.
                if (!isAdmin && entity.UserId != requestingUserId)
                {
                    _logger.LogWarning("User {UserId} forbidden from deleting tour company {CompanyId}", requestingUserId, id);
                    throw new ForbiddenException("You are not allowed to delete this company");
                }

                _unitOfWork.TourCompanies.Remove(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourCompanyCache(id);

                _logger.LogInformation("Successfully deleted tour company {CompanyId}", id);
                return true;
            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting tour company {CompanyId}", id);
                throw new ServiceException($"Failed to delete tour company: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<TourCompanyResponse>> FilterAsync(
            string? name = null,
            string? location = null,
            int? userId = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Filtering tour companies - Name: {Name}, Location: {Location}, UserId: {UserId}",
                name ?? "Any", location ?? "Any", userId?.ToString() ?? "Any");

            try
            {
                // Filter is a public search, so only approved companies are returned.
                var query = _unitOfWork.TourCompanies.Query().AsNoTracking()
                    .Where(c => c.Status == TourCompanyStatus.Approved);

                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(c => c.Name.Contains(name));

                if (!string.IsNullOrWhiteSpace(location))
                    query = query.Where(c => c.Location != null && c.Location.Contains(location));

                if (userId.HasValue && userId.Value > 0)
                    query = query.Where(c => c.UserId == userId.Value);

                var entities = await query
                    .OrderBy(c => c.Name)
                    .Take(50)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<TourCompanyResponse>>(entities);

                _logger.LogInformation("Tour company filter completed. Found {Count} companies", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering tour companies");
                throw new ServiceException($"Failed to filter tour companies: {ex.Message}", ex);
            }
        }

        public async Task<IReadOnlyList<TourCompanyResponse>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving tour companies pending approval");

            try
            {
                var entities = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .Where(c => c.Status == TourCompanyStatus.Pending)
                    .OrderBy(c => c.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<IReadOnlyList<TourCompanyResponse>>(entities);

                _logger.LogInformation("Found {Count} tour companies pending approval", response.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending tour companies");
                throw new ServiceException($"Failed to retrieve pending tour companies: {ex.Message}", ex);
            }
        }

        public Task<TourCompanyResponse> ApproveAsync(int id, CancellationToken cancellationToken = default)
            => SetStatusAsync(id, TourCompanyStatus.Approved, null, cancellationToken);

        public Task<TourCompanyResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Rejection reason is required", nameof(reason));

            return SetStatusAsync(id, TourCompanyStatus.Rejected, reason, cancellationToken);
        }

        private async Task<TourCompanyResponse> SetStatusAsync(int id, TourCompanyStatus status, string? reason, CancellationToken cancellationToken)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid tour company ID", nameof(id));

            _logger.LogInformation("Setting tour company {CompanyId} status to {Status}", id, status);

            try
            {
                var entity = await _unitOfWork.TourCompanies.GetByIdAsync(id, cancellationToken);
                if (entity == null)
                {
                    _logger.LogWarning("Tour company {CompanyId} not found for status change", id);
                    throw new NotFoundException($"Tour company with ID '{id}' not found");
                }

                // Skip only when the status AND (for rejections) the reason are unchanged.
                var rejectionReason = status == TourCompanyStatus.Rejected ? reason : null;
                if (entity.Status == status && entity.RejectionReason == rejectionReason)
                {
                    _logger.LogInformation("Tour company {CompanyId} is already {Status}; no change", id, status);
                    return _mapper.Map<TourCompanyResponse>(entity);
                }

                entity.Status = status;
                entity.RejectionReason = rejectionReason; // cleared on approve, set on reject
                entity.UpdatedAtUtc = DateTime.UtcNow;

                _unitOfWork.TourCompanies.Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                InvalidateTourCompanyCache(id);

                _logger.LogInformation("Tour company {CompanyId} status set to {Status}", id, status);

                // Notify the owner of the decision (saved + pushed via FCM). A notification
                // failure must not undo the approval, so it is best-effort here. Messages are
                // shared with the API response via TourCompanyStatusMessages.
                var (message, notificationType) = status switch
                {
                    TourCompanyStatus.Approved => (TourCompanyStatusMessages.Approved, NotificationType.CompanyApproved),
                    TourCompanyStatus.Rejected => ($"{TourCompanyStatusMessages.Rejected} السبب: {reason}", NotificationType.CompanyRejected),
                    _ => (string.Empty, NotificationType.General)
                };

                if (!string.IsNullOrEmpty(message))
                {
                    try
                    {
                        await _notificationService.NotifyAsync(entity.UserId, message, notificationType, cancellationToken);
                    }
                    catch (Exception notifyEx)
                    {
                        _logger.LogWarning(notifyEx, "Failed to notify owner {UserId} of company {CompanyId} status change",
                            entity.UserId, id);
                    }
                }

                return _mapper.Map<TourCompanyResponse>(entity);
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting tour company {CompanyId} status to {Status}", id, status);
                throw new ServiceException($"Failed to update tour company status: {ex.Message}", ex);
            }
        }

        #region Private Helper Methods

        private void InvalidateTourCompanyCache(int? specificId = null)
        {
            if (specificId.HasValue)
                _cache.Remove($"{TourCompanyCacheKeyPrefix}{specificId.Value}");

            _cache.Remove(TourCompaniesListCacheKey);
        }

        #endregion
    }
}
