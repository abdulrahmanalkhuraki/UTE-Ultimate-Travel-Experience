using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Response DTO for cancelled tour packages.
    /// Includes cancellation timestamp for audit and display purposes.
    /// </summary>
    public class CancelledTourPackageResponse
    {
        /// <summary>Unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Package name.</summary>
        public string PackageName { get; set; } = null!;

        /// <summary>First image URL (prioritized over videos) for thumbnail display.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Package creation timestamp.</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Current status (should be Cancelled).</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>
        /// Timestamp when the package was cancelled.
        /// Nullable for packages cancelled before this field was introduced.
        /// </summary>
        public DateTime? CancelledAtUtc { get; set; }
    }
}
