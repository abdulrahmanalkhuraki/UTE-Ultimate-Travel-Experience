using Application.DTOs.Pagination;
using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.TouristGuide
{
    /// <summary>
    /// CRUD for tour guides scoped to the signed-in company. Guides are linked to
    /// the company (many-to-many) so a company only manages and sees its own guides.
    /// </summary>
    public interface ITouristGuideService
    {
        /// <summary>Creates a guide and links it to the company of <paramref name="ownerUserId"/>.</summary>
        Task<TouristGuideResponse> CreateAsync(int ownerUserId, TouristGuideCreateRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates a guide; only a company the guide belongs to may do so.</summary>
        Task<TouristGuideResponse> UpdateAsync(int id, int ownerUserId, TouristGuideUpdateRequest request, CancellationToken cancellationToken = default);

        /// <summary>Unlinks a guide from the company (and deletes it if no links remain).</summary>
        Task<bool> DeleteAsync(int id, int ownerUserId, CancellationToken cancellationToken = default);

        /// <summary>Gets a single guide owned by the company of <paramref name="ownerUserId"/>.</summary>
        Task<TouristGuideResponse> GetAsync(int id, int ownerUserId, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<TouristGuideResponseSummary>> GetMineAsync(int page,int pageSize,CancellationToken cancellationToken);
    }
}
