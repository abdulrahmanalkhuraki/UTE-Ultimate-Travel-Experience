using Application.Common;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Response DTO for completed tour packages optimized for company dashboard.
    /// Contains earning analytics, engagement metrics, and publication history.
    /// </summary>
    public class CompletedTourPackageResponse
    {
        /// <summary>Unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Package name.</summary>
        public string PackageName { get; set; } = null!;

        /// <summary>First image URL (prioritized over videos) for thumbnail display.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Package creation timestamp.</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Current status (should be Completed).</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>Human-readable label for the current status.</summary>
        public string StatusLabel => Status.Humanize();

        /// <summary>
        /// Total net earnings after 5% platform commission deduction.
        /// Calculated from confirmed bookings: SUM(TotalCost) * 0.95
        /// </summary>
        public decimal TotalEarnedAmount { get; set; }

        /// <summary>Count of distinct tourists with confirmed bookings.</summary>
        public int NumberOfTouristsJoined { get; set; }

        /// <summary>Total count of reviews submitted by tourists.</summary>
        public int NumberOfReviews { get; set; }

        /// <summary>
        /// Average rating across all ratings.
        /// Returns 0 if no ratings exist.
        /// </summary>
        public float AverageRating { get; set; }

        /// <summary>Number of times this package has been published (republished).</summary>
        public int PublishCount { get; set; }
    }
}
