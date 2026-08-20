using Application.DTOs.Companion.Request;
using Application.DTOs.Companion.Response;
using Application.DTOs.Pagination;

namespace Application.Interfaces.Companion
{
    public interface ICompanionService
    {
        Task<CompanionResponse> CreateAsync(CompanionCreateRequest request, CancellationToken cancellationToken);
        Task<CompanionResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<PaginatedResponse<CompanionResponseSummary>> GetAllAsync(int page,int pageSize,CancellationToken cancellationToken);
        Task<PaginatedResponse<CompanionResponse>> GetByUserIdAsync(int userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<CompanionResponse> UpdateAsync(int id, CompanionUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
