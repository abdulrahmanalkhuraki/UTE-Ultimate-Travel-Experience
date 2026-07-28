using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Response DTO for active (ongoing) tour packages.
    /// Optimized for company dashboard with time-sensitive information.
    /// </summary>
    public class ActiveTourPackageResponse
    {
        /// <summary>Unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Package name.</summary>
        public string PackageName { get; set; } = null!;

        /// <summary>First image URL (prioritized over videos) for thumbnail display.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Package creation timestamp.</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Current status (should be Active).</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>Number of times this package has been published.</summary>
        public int PublishCount { get; set; }

        /// <summary>
        /// Days remaining until trip starts.
        /// Clamped to minimum of 0 (never negative).
        /// Formula: (StartDate - Today).Days, Math.Max(0, result)
        /// </summary>
        public int RemainingDaysUntilStart { get; set; }

        /// <summary>
        /// Days remaining until registration closes.
        /// Clamped to minimum of 0 (never negative).
        /// Formula: (RegistrationDeadline - Today).Days, Math.Max(0, result)
        /// </summary>
        public int RemainingDaysUntilRegistration { get; set; }

        /// <summary>
        /// Average rating across all ratings.
        /// Returns 0 if no ratings exist.
        /// </summary>
        public float AverageRating { get; set; }
    }
}
