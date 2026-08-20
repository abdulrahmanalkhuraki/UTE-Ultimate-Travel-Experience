using Application.DTOs.Pagination;
using Application.DTOs.TouristGuide.Request;
using Application.DTOs.TouristGuide.Response;

namespace Application.Interfaces.TouristGuide
{
    public interface ITouristGuideService
    {
        Task<TouristGuideResponse> CreateAsync(TouristGuideCreateRequest request, CancellationToken cancellationToken);
        Task<TouristGuideResponse> UpdateAsync(int id, TouristGuideUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationTokend);
        Task<TouristGuideResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<PaginatedResponse<TouristGuideResponseSummary>> GetMineAsync(int page,int pageSize,CancellationToken cancellationToken);
    }
}
