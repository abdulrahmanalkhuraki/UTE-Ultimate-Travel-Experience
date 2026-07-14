using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.TourPackage;
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

        Task<IReadOnlyList<TourPackageResponse>> GetMineAsync(TourPackageStatus? status = null, CancellationToken cancellationToken = default);

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
    }
}
