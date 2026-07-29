using System;
using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{

    public class ProgramStatusResponse
    {
        public int Id { get; set; }

        public string PackageName { get; set; } = null!;

        public TourPackageStatus Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
