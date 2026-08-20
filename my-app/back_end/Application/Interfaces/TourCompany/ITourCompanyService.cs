using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;

namespace Application.Interfaces.TourCompany
{
    public interface ITourCompanyService
    {
        Task<TourCompanyResponse> CreateAsync(TourCompanyCreateRequest request, CancellationToken cancellationToken);
        Task<TourCompanyResponse> GetAsync(int id, CancellationToken cancellationToken);
        Task<TourCompanyResponse> GetMineAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<TourCompanyResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<TourCompanyDashboardResponse> MyDashboard(CancellationToken cancellationToken);
        Task<IReadOnlyList<TourCompanyResponse>> GetPendingAsync(CancellationToken cancellationToken);
        Task<TourCompanyResponse> ApproveAsync(int id, CancellationToken cancellationToken);
        Task<TourCompanyResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken);
        Task<TourCompanyResponse> UpdateAsync(int id, TourCompanyUpdateRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
        Task<IReadOnlyList<TourCompanyResponse>> FilterAsync(
            string? name = null,
            string? location = null,
            int? userId = null,
            CancellationToken cancellationToken = default);
    }
}
