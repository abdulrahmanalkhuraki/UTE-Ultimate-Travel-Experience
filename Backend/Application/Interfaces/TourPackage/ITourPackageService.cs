using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Domain.Enums;

namespace Application.Interfaces.TourPackage
{
    public interface ITourPackageService
    {
        Task<TourPackageResponse> CreateAsync(TourPackageCreateRequest request, CancellationToken cancellationToken);

        Task<TourPackageResponse> GetAsync(int id, CancellationToken cancellationToken);

        Task<PaginatedResponse<TourPackageResponse>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        Task<PaginatedResponse<TourPackageResponse>> GetMineAsync(int page = 1, int pageSize = 20, TourPackageStatus? status = null, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> GetMineAsync(int id, CancellationToken cancellationToken);

        Task<TourPackageResponse> UpdateAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken);

        Task<TourPackageResponse> RepublishAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken);

        Task<ProgramStatusResponse> CancelAsync(int id, CancellationToken cancellationToken);

        Task<ProgramStatusResponse> ApproveAsync(int id, CancellationToken cancellationToken);

        Task<ProgramStatusResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<PaginatedResponse<TourPackageResponse>> FilterAsync(int? countryId = null, 
            int? cityId = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> GetMostWantedPackagesAsync(CancellationToken cancellationToken);

        /// <summary>Retrieves paginated completed tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<CompletedTourPackageResponse>> GetMineCompletedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated active tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<ActiveTourPackageResponse>> GetMineActiveAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated cancelled tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<CancelledTourPackageResponse>> GetMineCancelledAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated rejected tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<RejectedTourPackageResponse>> GetMineRejectedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves package statistics for the authenticated company's dashboard.</summary>
        Task<PackageStatsResponse> GetPackageStatsAsync(CancellationToken cancellationToken);

        /// <summary>Retrieves aggregated rating and review statistics for the authenticated company.</summary>
        Task<RateAndReviewStatsResponse> GetRateAndReviewStatsAsync(CancellationToken cancellationToken);
        /// <summary>Retrieves tourist statistics for the authenticated company's dashboard.</summary>
        Task<TouristStatsResponse> GetTouristStatsAsync(CancellationToken cancellationToken);

        /// <summary>Retrieves paginated tour packages filtered by status for the admin dashboard.</summary>
        Task<PaginatedResponse<TourPackageResponse>> GetByStatusAsync(TourPackageStatus status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves the count of tour packages for each status.</summary>
        Task<TourPackageStatusCountsResponse> GetStatusCountsAsync(CancellationToken cancellationToken);
    }
}
