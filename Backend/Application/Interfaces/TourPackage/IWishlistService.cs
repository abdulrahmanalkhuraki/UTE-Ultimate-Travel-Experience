using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs.TourPackage.Response;

namespace Application.Interfaces.TourPackage
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(int tourPackageId, CancellationToken cancellationToken = default);

        Task<bool> RemoveFromWishlistAsync(int tourPackageId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TourPackageResponse>> GetWishlistAsync(CancellationToken cancellationToken = default);
    }
}
