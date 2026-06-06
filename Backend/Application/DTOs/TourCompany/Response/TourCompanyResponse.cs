using System;

namespace Application.DTOs.TourCompany.Response
{
    public sealed class TourCompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Logo { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateOnly? FoundingDate { get; set; }
        public string? TourismLicenseNumber { get; set; }
        public string? TourismLicenseImage { get; set; }
        public string? BankAccount { get; set; }
        public string? About { get; set; }
        public string Status { get; set; } = string.Empty;
        /// <summary>Ready-to-display Arabic message describing the current status.</summary>
        public string StatusMessage { get; set; } = string.Empty;
        /// <summary>Admin-written rejection reason; null unless the company is rejected.</summary>
        public string? RejectionReason { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
