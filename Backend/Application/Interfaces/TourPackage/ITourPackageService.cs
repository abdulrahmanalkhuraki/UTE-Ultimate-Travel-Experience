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

        /// <summary>Aggregate counts of the company's own programs for the dashboard stats card.</summary>
        Task<CompanyProgramStatsResponse> GetMyStatsAsync(int ownerUserId, CancellationToken cancellationToken = default);

        /// <summary>Updates a program; only its owning company may do so.</summary>
        Task<TourPackageResponse> UpdateAsync(int id, int ownerUserId, TourPackageUpdateRequest request, CancellationToken cancellationToken = default);

        /// <summary>Cancels a program (sets its status to cancelled); only its owning company may do so.</summary>
        Task<ProgramStatusResponse> CancelAsync(int id, int ownerUserId, CancellationToken cancellationToken = default);

        /// <summary>Admin view: lists all programs awaiting moderation (قيد الانتظار), oldest first.</summary>
        Task<IReadOnlyList<TourPackageResponse>> GetPendingAsync(CancellationToken cancellationToken = default);

        /// <summary>Admin action: accepts a program (المقبولة) and notifies the owning company.</summary>
        Task<ProgramStatusResponse> AcceptAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Admin action: rejects a program (المرفوضة) with a reason and notifies the owning company.</summary>
        Task<ProgramStatusResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default);

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
