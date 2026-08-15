using Application.DTOs.Admin.Response;

namespace Application.Interfaces.Admin
{
    public interface IAdminService
    {
        /// <summary>Retrieves aggregated statistics for the admin dashboard.</summary>
        Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default);
    }
}
