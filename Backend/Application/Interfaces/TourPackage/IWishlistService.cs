using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Response;

namespace Application.Interfaces.TourPackage
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(int tourPackageId, CancellationToken cancellationToken);

        Task<bool> RemoveFromWishlistAsync(int tourPackageId, CancellationToken cancellationToken);

        Task<PaginatedResponse<TourPackageResponse>> GetWishlistAsync(int page, int pageSize, CancellationToken cancellationToken);
    }
}
