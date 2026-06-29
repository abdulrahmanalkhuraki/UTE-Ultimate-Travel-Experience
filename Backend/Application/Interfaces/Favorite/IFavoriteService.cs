using Application.DTOs.Favorite.Response;

namespace Application.Interfaces.Favorite
{
    public interface IFavoriteService
    {
        Task<FavoriteResponse> AddAsync(int companyId, CancellationToken cancellationToken);

        Task<IReadOnlyList<FavoriteResponse>> GetUserFavoritesAsync(CancellationToken cancellationToken);
    }
}
