using System;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Lightweight result of a status action (cancel / accept / reject). Returns only
    /// what the caller needs to confirm the new state — not the whole program graph.
    /// </summary>
    public class ProgramStatusResponse
    {
        public int Id { get; set; }

        public string PackageName { get; set; } = null!;

        /// <summary>Lifecycle status (حالة البرنامج): Pending, Active, Completed, Cancelled, or Rejected.</summary>
        public TourPackageStatus Status { get; set; }

        /// <summary>Reason shown to the company when rejected (سبب الرفض). Null otherwise.</summary>
        public string? RejectionReason { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
