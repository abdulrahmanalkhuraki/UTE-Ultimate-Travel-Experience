using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.User;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <summary>
    /// Partial extension of TourPackageService containing status-specific query methods.
    /// Each method returns optimized DTOs for a specific package status.
    /// </summary>
    public partial class TourPackageService
    {
        private const string CompletedCacheKeyPrefix = "mine_completed_";
        private const string ActiveCacheKeyPrefix = "mine_active_";
        private const string CancelledCacheKeyPrefix = "mine_cancelled_";
        private const string RejectedCacheKeyPrefix = "mine_rejected_";

        /// <summary>
        /// Retrieves paginated completed tour packages for the authenticated company.
        /// Returns optimized CompletedTourPackageResponse with earning analytics.
        /// </summary>
        public async Task<PaginatedResponse<CompletedTourPackageResponse>> GetMineCompletedAsync(
            int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var cacheKey = $"{CompletedCacheKeyPrefix}{companyId}_page{page}_pageSize{pageSize}_{_language.LanguageCode}";

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<CompletedTourPackageResponse>? cached) && cached is not null)
                    return cached;

                var query = _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId && p.Status == TourPackageStatus.Completed);

                var totalItems = await query.CountAsync(cancellationToken);

                var entities = await query
                    .Include(p => p.Translations)
                    .Include(p => p.Media)
                    .Include(p => p.Bookings)
                    .Include(p => p.Rates)
                    .Include(p => p.Reviews)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<CompletedTourPackageResponse>>(entities);

                var paginatedResponse = new PaginatedResponse<CompletedTourPackageResponse>
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    }
                };

                _cache.Set(cacheKey, paginatedResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = MineCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogInformation(
                    "Retrieved {Count} completed packages for company {CompanyId} | page {Page}/{TotalPages}",
                    items.Count, companyId, page, (totalItems + pageSize - 1) / pageSize);

                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Mine Completed", ObjectName, ex);
                throw new ServiceException(
                    ExceptionMessages.ServiceException("retrieve completed", ObjectName, ex.Message), ex);
            }
        }

        /// <summary>
        /// Retrieves paginated active tour packages for the authenticated company.
        /// Returns optimized ActiveTourPackageResponse with time-sensitive information.
        /// </summary>
        public async Task<PaginatedResponse<ActiveTourPackageResponse>> GetMineActiveAsync(
            int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var cacheKey = $"{ActiveCacheKeyPrefix}{companyId}_page{page}_pageSize{pageSize}_{_language.LanguageCode}";

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<ActiveTourPackageResponse>? cached) && cached is not null)
                    return cached;

                var query = _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId && p.Status == TourPackageStatus.Active);

                var totalItems = await query.CountAsync(cancellationToken);

                var entities = await query
                    .Include(p => p.Translations)
                    .Include(p => p.Media)
                    .Include(p => p.Rates)
                    .Include(p => p.Bookings)
                        .ThenInclude(b => b.User)
                            .ThenInclude(u => u.Person)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<ActiveTourPackageResponse>>(entities);

                var paginatedResponse = new PaginatedResponse<ActiveTourPackageResponse>
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    }
                };

                _cache.Set(cacheKey, paginatedResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = MineCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogInformation(
                    "Retrieved {Count} active packages for company {CompanyId} | page {Page}/{TotalPages}",
                    items.Count, companyId, page, (totalItems + pageSize - 1) / pageSize);

                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Mine Active", ObjectName, ex);
                throw new ServiceException(
                    ExceptionMessages.ServiceException("retrieve active", ObjectName, ex.Message), ex);
            }
        }

        /// <summary>
        /// Retrieves paginated cancelled tour packages for the authenticated company.
        /// Returns optimized CancelledTourPackageResponse with cancellation timestamp.
        /// </summary>
        public async Task<PaginatedResponse<CancelledTourPackageResponse>> GetMineCancelledAsync(
            int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var cacheKey = $"{CancelledCacheKeyPrefix}{companyId}_page{page}_pageSize{pageSize}_{_language.LanguageCode}";

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<CancelledTourPackageResponse>? cached) && cached is not null)
                    return cached;

                var query = _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId && p.Status == TourPackageStatus.Cancelled);

                var totalItems = await query.CountAsync(cancellationToken);

                var entities = await query
                    .Include(p => p.Translations)
                    .Include(p => p.Media)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(p => p.CancelledAtUtc ?? p.UpdatedAtUtc)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<CancelledTourPackageResponse>>(entities);

                var paginatedResponse = new PaginatedResponse<CancelledTourPackageResponse>
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    }
                };

                _cache.Set(cacheKey, paginatedResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = MineCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogInformation(
                    "Retrieved {Count} cancelled packages for company {CompanyId} | page {Page}/{TotalPages}",
                    items.Count, companyId, page, (totalItems + pageSize - 1) / pageSize);

                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Mine Cancelled", ObjectName, ex);
                throw new ServiceException(
                    ExceptionMessages.ServiceException("retrieve cancelled", ObjectName, ex.Message), ex);
            }
        }

        /// <summary>
        /// Retrieves paginated rejected tour packages for the authenticated company.
        /// Returns optimized RejectedTourPackageResponse with rejection reason.
        /// </summary>
        public async Task<PaginatedResponse<RejectedTourPackageResponse>> GetMineRejectedAsync(
            int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new AuthException(ExceptionMessages.Auth());
            var companyId = await ResolveCompanyIdAsync(currentUserId, cancellationToken);

            try
            {
                var cacheKey = $"{RejectedCacheKeyPrefix}{companyId}_page{page}_pageSize{pageSize}_{_language.LanguageCode}";

                if (_cache.TryGetValue(cacheKey, out PaginatedResponse<RejectedTourPackageResponse>? cached) && cached is not null)
                    return cached;

                var query = _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId && p.Status == TourPackageStatus.Rejected);

                var totalItems = await query.CountAsync(cancellationToken);

                var entities = await query
                    .Include(p => p.Translations)
                    .Include(p => p.Media)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(p => p.CreatedAtUtc)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<IReadOnlyList<RejectedTourPackageResponse>>(entities);

                var paginatedResponse = new PaginatedResponse<RejectedTourPackageResponse>
                {
                    Items = items,
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    }
                };

                _cache.Set(cacheKey, paginatedResponse, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = MineCacheDuration,
                    Priority = CacheItemPriority.Normal
                });

                _logger.LogInformation(
                    "Retrieved {Count} rejected packages for company {CompanyId} | page {Page}/{TotalPages}",
                    items.Count, companyId, page, (totalItems + pageSize - 1) / pageSize);

                return paginatedResponse;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve Mine Rejected", ObjectName, ex);
                throw new ServiceException(
                    ExceptionMessages.ServiceException("retrieve rejected", ObjectName, ex.Message), ex);
            }
        }
    }
}
