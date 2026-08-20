using Application.Common;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Response DTO for rejected tour packages.
    /// Includes rejection reason for transparency to company owners.
    /// </summary>
    public class RejectedTourPackageResponse
    {
        /// <summary>Unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Package name.</summary>
        public string PackageName { get; set; } = null!;

        /// <summary>First image URL (prioritized over videos) for thumbnail display.</summary>
        public string? ImageUrl { get; set; }

        /// <summary>Package creation timestamp.</summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>Current status (should be Rejected).</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>Human-readable label for the current status.</summary>
        public string StatusLabel => Status.Humanize();

        /// <summary>
        /// Reason provided by admin for package rejection.
        /// Helps company understand why their submission was declined.
        /// </summary>
        public string? RejectionReason { get; set; }
    }
}
