using Application.DTOs.TourCompany.Request;
using Application.DTOs.TourCompany.Response;

namespace Application.Interfaces.TourCompany
{
    public interface ITourCompanyService
    {
        Task<TourCompanyResponse> CreateAsync(int ownerUserId, TourCompanyCreateRequest request, CancellationToken cancellationToken = default);
        Task<TourCompanyResponse> GetAsync(int id, int? requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TourCompanyResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TourCompanyResponse>> GetPendingAsync(CancellationToken cancellationToken = default);
        Task<TourCompanyResponse> ApproveAsync(int id, CancellationToken cancellationToken = default);
        Task<TourCompanyResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default);
        Task<TourCompanyResponse> UpdateAsync(int id, int requestingUserId, bool isAdmin, TourCompanyUpdateRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, int requestingUserId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TourCompanyResponse>> FilterAsync(
            string? name = null,
            string? location = null,
            int? userId = null,
            CancellationToken cancellationToken = default);
    }
}
