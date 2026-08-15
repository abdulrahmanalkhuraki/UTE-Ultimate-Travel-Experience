using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Admin.Response;
using Application.Exceptions;
using Application.Interfaces.Admin;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Application.Services
{
    public class AdminService : IAdminService
    {
        private const string CommissionRateKey = "CommissionRate";
        private const decimal DefaultCommissionRate = 0.03m;
        private const int GrowthMonths = 12;
        private const string ObjectName = "Admin Dashboard";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AdminService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving admin dashboard statistics");

            try
            {
                var commissionRate = GetCommissionRate();

                var activeTourists = await _unitOfWork.Users
                    .Query()
                    .AsNoTracking()
                    .CountAsync(u => !u.IsDeleted && u.Role.RoleName == "Tourist", cancellationToken);

                var activeCompanies = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .CountAsync(c => c.User != null && !c.User.IsDeleted && c.User.Role.RoleName == "TourCompany", cancellationToken);

                var packageCounts = await _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .GroupBy(p => 1)
                    .Select(g => new
                    {
                        Active = g.Count(p => p.Status == TourPackageStatus.Active),
                        Completed = g.Count(p => p.Status == TourPackageStatus.Completed)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var completedRevenue = await _unitOfWork.Bookings
                    .Query()
                    .AsNoTracking()
                    .Where(b => b.Status == BookingStatus.Completed
                        && b.TourPackage.Status == TourPackageStatus.Completed
                        && !b.TourPackage.IsDeleted)
                    .SumAsync(b => b.TotalCost, cancellationToken) ?? 0m;

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var touristCounts = await GetTouristGrowthCountsAsync(windowStart, windowEnd, cancellationToken);

                var packageCountsMonthly = await GetTourPackageGrowthCountsAsync(windowStart, windowEnd, cancellationToken);

                var response = new AdminDashboardResponse
                {
                    ActiveTourists = activeTourists,
                    ActiveCompanies = activeCompanies,
                    TourPackages = new TourPackageCounts
                    {
                        Active = packageCounts?.Active ?? 0,
                        Completed = packageCounts?.Completed ?? 0
                    },
                    TotalRevenue = completedRevenue * commissionRate,
                    CommissionRate = commissionRate,
                    TouristGrowth = BuildGrowthSeries(series, touristCounts),
                    TourPackageGrowth = BuildGrowthSeries(series, packageCountsMonthly)
                };

                _logger.LogInformation("Successfully retrieved admin dashboard statistics");
                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<AdminTouristsDashboardResponse> GetTouristsDashboardAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving admin tourist dashboard statistics");

            try
            {
                var touristCounts = await _unitOfWork.Users
                    .Query()
                    .AsNoTracking()
                    .Where(u => u.Role.RoleName == "Tourist")
                    .GroupBy(u => 1)
                    .Select(g => new
                    {
                        Active = g.Count(u => !u.IsDeleted),
                        Deleted = g.Count(u => u.IsDeleted)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var growth = await GetTouristGrowthCountsAsync(windowStart, windowEnd, cancellationToken);

                var active = touristCounts?.Active ?? 0;
                var deleted = touristCounts?.Deleted ?? 0;

                var response = new AdminTouristsDashboardResponse
                {
                    ActiveTourists = active,
                    DeletedTourists = deleted,
                    TotalTourists = active + deleted,
                    TouristGrowth = BuildGrowthSeries(series, growth)
                };

                _logger.LogInformation("Successfully retrieved admin tourist dashboard statistics");
                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<AdminTourPackagesDashboardResponse> GetTourPackagesDashboardAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving admin tour package dashboard statistics");

            try
            {
                var packageCounts = await _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => !p.IsDeleted)
                    .GroupBy(p => 1)
                    .Select(g => new
                    {
                        Total = g.Count(),
                        Rejected = g.Count(p => p.Status == TourPackageStatus.Rejected),
                        Pending = g.Count(p => p.Status == TourPackageStatus.Pending)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var growth = await GetTourPackageGrowthCountsAsync(windowStart, windowEnd, cancellationToken);

                var response = new AdminTourPackagesDashboardResponse
                {
                    TotalTourPackages = packageCounts?.Total ?? 0,
                    RejectedTourPackages = packageCounts?.Rejected ?? 0,
                    PendingTourPackages = packageCounts?.Pending ?? 0,
                    TourPackageGrowth = BuildGrowthSeries(series, growth)
                };

                _logger.LogInformation("Successfully retrieved admin tour package dashboard statistics");
                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        private decimal GetCommissionRate()
        {
            if (decimal.TryParse(
                _configuration[CommissionRateKey],
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var rate) && rate > 0 && rate < 1)
            {
                return rate;
            }

            return DefaultCommissionRate;
        }

        /// <summary>Builds a 12-month series (oldest to newest) anchored on the current month.</summary>
        private static List<(int Year, int Month)> BuildMonthSeries(DateTime now, int months)
        {
            var anchor = new DateTime(now.Year, now.Month, 1);
            var series = new List<(int Year, int Month)>(months);
            for (var i = months - 1; i >= 0; i--)
            {
                var date = anchor.AddMonths(-i);
                series.Add((date.Year, date.Month));
            }
            return series;
        }

        /// <summary>Merges the raw monthly counts into the full series, zero-filling missing months.</summary>
        private static List<MonthlyGrowth> BuildGrowthSeries(
            IReadOnlyList<(int Year, int Month)> series,
            IEnumerable<(int Year, int Month, int Count)> counts)
        {
            var lookup = counts.ToDictionary(c => (c.Year, c.Month), c => c.Count);

            return series
                .Select(s => new MonthlyGrowth
                {
                    Month = $"{s.Year:D4}-{s.Month:D2}",
                    Count = lookup.GetValueOrDefault((s.Year, s.Month), 0)
                })
                .ToList();
        }

        /// <summary>Groups tourists by registration month within the given window (includes active and deleted accounts).</summary>
        private async Task<List<(int Year, int Month, int Count)>> GetTouristGrowthCountsAsync(
            DateTime windowStart,
            DateTime windowEnd,
            CancellationToken cancellationToken)
        {
            var raw = await _unitOfWork.Users
                .Query()
                .AsNoTracking()
                .Where(u => u.Role.RoleName == "Tourist"
                    && u.CreatedAtUtc >= windowStart
                    && u.CreatedAtUtc < windowEnd)
                .GroupBy(u => new { u.CreatedAtUtc.Year, u.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, c.Count)).ToList();
        }

        /// <summary>Groups all non-deleted tour packages by creation month within the given window.</summary>
        private async Task<List<(int Year, int Month, int Count)>> GetTourPackageGrowthCountsAsync(
            DateTime windowStart,
            DateTime windowEnd,
            CancellationToken cancellationToken)
        {
            var raw = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => !p.IsDeleted
                    && p.CreatedAtUtc >= windowStart
                    && p.CreatedAtUtc < windowEnd)
                .GroupBy(p => new { p.CreatedAtUtc.Year, p.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, c.Count)).ToList();
        }
    }
}
