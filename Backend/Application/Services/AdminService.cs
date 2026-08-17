using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Admin.Response;
using Application.DTOs.TourCompany.Response;
using Application.Exceptions;
using Application.Interfaces.Admin;
using Application.Interfaces.Localization;
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
        private readonly ILocalizedMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILocalizedMapper mapper,
            ILogger<AdminService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        public async Task<AdminCompaniesDashboardResponse> GetCompaniesDashboardAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving admin company dashboard statistics");

            try
            {
                var companyCounts = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .Where(c => c.User != null && c.User.Role.RoleName == "TourCompany")
                    .GroupBy(c => 1)
                    .Select(g => new
                    {
                        Active = g.Count(c => c.Status == TourCompanyStatus.Approved && !c.User.IsDeleted),
                        Deleted = g.Count(c => c.User.IsDeleted),
                        Pending = g.Count(c => c.Status == TourCompanyStatus.Pending)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var growth = await GetTourCompanyGrowthCountsAsync(windowStart, windowEnd, cancellationToken);

                var response = new AdminCompaniesDashboardResponse
                {
                    ActiveCompanies = companyCounts?.Active ?? 0,
                    DeletedCompanies = companyCounts?.Deleted ?? 0,
                    PendingCompanies = companyCounts?.Pending ?? 0,
                    CompanyGrowth = BuildGrowthSeries(series, growth)
                };

                _logger.LogInformation("Successfully retrieved admin company dashboard statistics");
                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        /// <summary>Groups all companies by creation month within the given window.</summary>
        private async Task<List<(int Year, int Month, int Count)>> GetTourCompanyGrowthCountsAsync(
            DateTime windowStart,
            DateTime windowEnd,
            CancellationToken cancellationToken)
        {
            var raw = await _unitOfWork.TourCompanies
                .Query()
                .AsNoTracking()
                .Where(c => c.User != null && c.User.Role.RoleName == "TourCompany"
                    && c.CreatedAtUtc >= windowStart
                    && c.CreatedAtUtc < windowEnd)
                .GroupBy(c => new { c.CreatedAtUtc.Year, c.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, c.Count)).ToList();
        }

        public async Task<AdminFinancialDashboardResponse> GetFinancialDashboardAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving admin financial dashboard statistics");

            try
            {
                var commissionRate = GetCommissionRate();

                var totalProfit = (await _unitOfWork.Bookings
                    .Query()
                    .AsNoTracking()
                    .Where(b => b.Status == BookingStatus.Completed
                        && b.TourPackage.Status == TourPackageStatus.Completed
                        && !b.TourPackage.IsDeleted)
                    .SumAsync(b => b.TotalCost, cancellationToken) ?? 0m) * commissionRate;

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var monthlyProfit = await GetMonthlyProfitAsync(windowStart, windowEnd, commissionRate, cancellationToken);

                var response = new AdminFinancialDashboardResponse
                {
                    TotalProfit = totalProfit,
                    CommissionRate = commissionRate,
                    ProfitGrowth = BuildProfitSeries(series, monthlyProfit)
                };

                _logger.LogInformation("Successfully retrieved admin financial dashboard statistics");
                return response;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<AdminCompanyDashboardResponse> GetCompanyDashboardAsync(int companyId, CancellationToken cancellationToken = default)
        {
            if (companyId <= 0)
                throw new ArgumentException("Invalid tour company ID", nameof(companyId));

            _logger.LogDebug("Retrieving admin company dashboard statistics for company {CompanyId}", companyId);

            try
            {
                var company = await _unitOfWork.TourCompanies
                    .Query()
                    .AsNoTracking()
                    .Include(c => c.Translations)
                    .SingleOrDefaultAsync(c => c.Id == companyId, cancellationToken);

                if (company == null)
                {
                    _logger.LogDebug("Tour company with ID {CompanyId} not found", companyId);
                    throw new NotFoundException($"Tour company with ID {companyId} not found");
                }

                var packageCounts = await _unitOfWork.TourPackages
                    .Query()
                    .AsNoTracking()
                    .Where(p => p.CompanyId == companyId && !p.IsDeleted)
                    .GroupBy(p => 1)
                    .Select(g => new
                    {
                        Total = g.Count(),
                        Reviews = g.Sum(p => p.Reviews.Count)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                var averageRating = await GetAverageRatingAsync(companyId, cancellationToken);

                var completedRevenue = await _unitOfWork.Bookings
                    .Query()
                    .AsNoTracking()
                    .Where(b => b.TourPackage.CompanyId == companyId
                        && !b.TourPackage.IsDeleted
                        && b.Status == BookingStatus.Completed
                        && b.TourPackage.Status == TourPackageStatus.Completed)
                    .SumAsync(b => b.TotalCost, cancellationToken) ?? 0m;

                var bookingsCount = await _unitOfWork.Bookings
                    .Query()
                    .AsNoTracking()
                    .CountAsync(b => b.TourPackage.CompanyId == companyId && !b.TourPackage.IsDeleted, cancellationToken);

                var now = DateTime.UtcNow;
                var series = BuildMonthSeries(now, GrowthMonths);
                var windowStart = new DateTime(series[0].Year, series[0].Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var windowEnd = windowStart.AddMonths(GrowthMonths);

                var bookingGrowth = await GetBookingGrowthCountsAsync(companyId, windowStart, windowEnd, cancellationToken);
                var packageGrowth = await GetTourPackageGrowthCountsAsync(companyId, windowStart, windowEnd, cancellationToken);

                var response = new AdminCompanyDashboardResponse
                {
                    Company = _mapper.Map<TourCompanyResponse>(company),
                    BookingsCount = bookingsCount,
                    TotalTourPackages = packageCounts?.Total ?? 0,
                    AverageRating = averageRating,
                    ReviewsCount = packageCounts?.Reviews ?? 0,
                    TotalRevenue = completedRevenue,
                    BookingGrowth = BuildGrowthSeries(series, bookingGrowth),
                    TourPackageGrowth = BuildGrowthSeries(series, packageGrowth)
                };

                _logger.LogInformation("Successfully retrieved admin company dashboard statistics for company {CompanyId}", companyId);
                return response;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.ServerError("retrieving", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        /// <summary>Computes the average of the average rating of each rated tour package.</summary>
        private async Task<double> GetAverageRatingAsync(int companyId, CancellationToken cancellationToken)
        {
            var packageAverages = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId && !p.IsDeleted && p.Rates.Any())
                .Select(p => p.Rates.Average(r => (double)r.RateValue))
                .ToListAsync(cancellationToken);

            return packageAverages.Count > 0 ? packageAverages.Average() : 0d;
        }

        /// <summary>Groups bookings of the company's packages by creation month within the given window.</summary>
        private async Task<List<(int Year, int Month, int Count)>> GetBookingGrowthCountsAsync(
            int companyId,
            DateTime windowStart,
            DateTime windowEnd,
            CancellationToken cancellationToken)
        {
            var raw = await _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .Where(b => b.TourPackage.CompanyId == companyId
                    && !b.TourPackage.IsDeleted
                    && b.CreatedAtUtc >= windowStart
                    && b.CreatedAtUtc < windowEnd)
                .GroupBy(b => new { b.CreatedAtUtc.Year, b.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, c.Count)).ToList();
        }

        /// <summary>Groups the company's tour packages by creation month within the given window.</summary>
        private async Task<List<(int Year, int Month, int Count)>> GetTourPackageGrowthCountsAsync(
            int companyId,
            DateTime windowStart,
            DateTime windowEnd,
            CancellationToken cancellationToken)
        {
            var raw = await _unitOfWork.TourPackages
                .Query()
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId
                    && !p.IsDeleted
                    && p.CreatedAtUtc >= windowStart
                    && p.CreatedAtUtc < windowEnd)
                .GroupBy(p => new { p.CreatedAtUtc.Year, p.CreatedAtUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, c.Count)).ToList();
        }

        /// <summary>Groups tour package bookings by the package's end month within the given window and applies the commission rate.</summary>
        private async Task<List<(int Year, int Month, decimal Profit)>> GetMonthlyProfitAsync(
            DateTime windowStart,
            DateTime windowEnd,
            decimal commissionRate,
            CancellationToken cancellationToken)
        {
            var from = DateOnly.FromDateTime(windowStart);
            var to = DateOnly.FromDateTime(windowEnd);

            var raw = await _unitOfWork.Bookings
                .Query()
                .AsNoTracking()
                .Where(b => b.Status == BookingStatus.Completed
                    && b.TourPackage.Status == TourPackageStatus.Completed
                    && !b.TourPackage.IsDeleted
                    && b.TourPackage.EndDate >= from
                    && b.TourPackage.EndDate < to)
                .GroupBy(b => new { b.TourPackage.EndDate.Year, b.TourPackage.EndDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Profit = g.Sum(b => b.TotalCost) })
                .ToListAsync(cancellationToken);

            return raw.Select(c => (c.Year, c.Month, (c.Profit ?? 0m) * commissionRate)).ToList();
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

        /// <summary>Merges the raw monthly profits into the full series, zero-filling missing months.</summary>
        private static List<MonthlyProfit> BuildProfitSeries(
            IReadOnlyList<(int Year, int Month)> series,
            IEnumerable<(int Year, int Month, decimal Profit)> profits)
        {
            var lookup = profits.ToDictionary(p => (p.Year, p.Month), p => p.Profit);

            return series
                .Select(s => new MonthlyProfit
                {
                    Month = $"{s.Year:D4}-{s.Month:D2}",
                    Profit = lookup.GetValueOrDefault((s.Year, s.Month), 0m)
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
