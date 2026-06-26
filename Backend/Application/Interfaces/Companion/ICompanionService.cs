using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;

namespace Application.Interfaces.Companion
{
    public interface ICompanionService
    {
        Task<CompanionResponse> CreateAsync(int userId, CompanionCreateRequest request, CancellationToken cancellationToken);
        Task<CompanionResponse> GetAsync(int id, int userId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CompanionResponse>> GetAllAsync(int userId, CancellationToken cancellationToken);
        Task<CompanionResponse> UpdateAsync(int id, int userId, CompanionUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken);
    }
}
