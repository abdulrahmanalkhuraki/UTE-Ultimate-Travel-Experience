using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.TourPackage;
using Application.DTOs.TourPackage.Request;
using Application.DTOs.TourPackage.Response;

namespace Application.Interfaces.TourPackage
{
    public interface ITourPackageService
    {
        /// <summary>Creates a program owned by the company of <paramref name="ownerUserId"/>.</summary>
        Task<TourPackageResponse> CreateAsync(int ownerUserId, TourPackageCreateRequest request, CancellationToken cancellationToken = default);

        Task<TourPackageResponse> GetAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>Lists every program owned by the company of <paramref name="ownerUserId"/>.</summary>
        Task<IReadOnlyList<TourPackageResponse>> GetMineAsync(int ownerUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists the company's own programs filtered to one dashboard tab:
        /// current (الحالية), previous (السابقة), or cancelled (الملغاة).
        /// </summary>
        Task<IReadOnlyList<TourPackageResponse>> GetMineByTimelineAsync(int ownerUserId, ProgramTimeline timeline, CancellationToken cancellationToken = default);

        /// <summary>Updates a program; only its owning company may do so.</summary>
        Task<TourPackageResponse> UpdateAsync(int id, int ownerUserId, TourPackageUpdateRequest request, CancellationToken cancellationToken = default);

        /// <summary>Cancels a program (sets its status to cancelled); only its owning company may do so.</summary>
        Task<TourPackageResponse> CancelAsync(int id, int ownerUserId, CancellationToken cancellationToken = default);

        /// <summary>Deletes a program; only its owning company may do so.</summary>
        Task<bool> DeleteAsync(int id, int ownerUserId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> FilterAsync(
            int? countryId = null,
            int? cityId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool publishedOnly = true,
            CancellationToken cancellationToken = default);
    }
}
