using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;

namespace Application.Interfaces.Companion
{
    public interface ICompanionService
    {
        Task<CompanionResponse> CreateAsync(CompanionCreateRequest request, CancellationToken cancellationToken);
        Task<CompanionResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<CompanionResponse>> GetAllAsync(int page,int pageSize,CancellationToken cancellationToken);
        Task<CompanionResponse> UpdateAsync(int id, CompanionUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
