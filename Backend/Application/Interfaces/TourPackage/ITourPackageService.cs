using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;
using Domain.Enums;

namespace Application.Interfaces.TourPackage
{
    public interface ITourPackageService
    {
        Task<TourPackageResponse> CreateAsync(TourPackageCreateRequest request, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> GetAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<PaginatedResponse<TourPackageResponse>> GetMineAsync(int page = 1, int pageSize = 20, TourPackageStatus? status = null, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> GetMineAsync(int id, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> UpdateAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> RepublishAsync(int id, TourPackageUpdateRequest request, CancellationToken cancellationToken = default);

        Task<ProgramStatusResponse> CancelAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> GetUnApprovedAsync(CancellationToken cancellationToken = default);

        Task<ProgramStatusResponse> ApproveAsync(int id, CancellationToken cancellationToken = default);

        Task<ProgramStatusResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> FilterAsync(
            int? countryId = null,
            int? cityId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated completed tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<CompletedTourPackageResponse>> GetMineCompletedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated active tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<ActiveTourPackageResponse>> GetMineActiveAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated cancelled tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<CancelledTourPackageResponse>> GetMineCancelledAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves paginated rejected tour packages for the authenticated company.</summary>
        Task<PaginatedResponse<RejectedTourPackageResponse>> GetMineRejectedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

        /// <summary>Retrieves package statistics for the authenticated company's dashboard.</summary>
        Task<PackageStatsResponse> GetPackageStatsAsync(CancellationToken cancellationToken = default);
    }
}
