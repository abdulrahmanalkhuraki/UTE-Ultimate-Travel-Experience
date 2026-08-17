using Application.DTOs.Admin.Response;

namespace Application.Interfaces.Admin
{
    public interface IAdminService
    {
        /// <summary>Retrieves aggregated statistics for the admin dashboard.</summary>
        Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves tourist statistics for the admin dashboard.</summary>
        Task<AdminTouristsDashboardResponse> GetTouristsDashboardAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves tour package statistics for the admin dashboard.</summary>
        Task<AdminTourPackagesDashboardResponse> GetTourPackagesDashboardAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves tour company statistics for the admin dashboard.</summary>
        Task<AdminCompaniesDashboardResponse> GetCompaniesDashboardAsync(CancellationToken cancellationToken = default);

        /// <summary>Retrieves statistics for a single tour company for the admin dashboard.</summary>
        Task<AdminCompanyDashboardResponse> GetCompanyDashboardAsync(int companyId, CancellationToken cancellationToken = default);

        /// <summary>Retrieves financial (profit) statistics for the admin dashboard.</summary>
        Task<AdminFinancialDashboardResponse> GetFinancialDashboardAsync(CancellationToken cancellationToken = default);
    }
}
