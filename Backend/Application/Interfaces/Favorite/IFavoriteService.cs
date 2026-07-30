using Application.DTOs.Favorite.Response;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Favorite
{
    public interface IFavoriteService
    {
        Task<bool> AddAsync(int companyId, CancellationToken cancellationToken);

        Task<PaginatedResponse<FavoriteResponse>> GetUserFavoritesAsync(int page, int pageSize, CancellationToken cancellationToken);
    }
}
