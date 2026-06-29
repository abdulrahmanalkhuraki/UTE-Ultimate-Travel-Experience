using Application.DTOs.City.Response;

namespace Application.Interfaces.City
{
    public interface ICityService
    {
        Task<CityResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<CityResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    }
}
