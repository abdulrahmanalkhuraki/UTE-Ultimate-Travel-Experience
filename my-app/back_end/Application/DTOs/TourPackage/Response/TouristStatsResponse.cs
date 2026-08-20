using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TourPackage.Response
{
    public sealed record TouristStatsResponse
    {
        public int TotalUniqueTourists { get; init; }
        public IReadOnlyList<LatestBookingItem> LatestBookings { get; init; } = [];
        public IReadOnlyList<MonthlyBookingCount> MonthlyBookings { get; init; } = [];
    }

    public sealed record LatestBookingItem
    {
        public int Id { get; init; }
        public string TouristName { get; init; } = string.Empty;
        public string? TouristImage { get; init; }
        public DateTime BookingDate { get; init; }
        public string PackageName { get; init; } = string.Empty;
    }

    public sealed record MonthlyBookingCount
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public string MonthName { get; init; } = string.Empty;
        public int BookingCount { get; init; }
    }
}
