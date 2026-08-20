using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourCompany.Request
{
    public sealed record TourCompanyCreateRequest
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Location { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateOnly? FoundingDate { get; set; }

        public string? TourismLicenseNumber { get; set; }

        public string? BankAccount { get; set; }

        public string? About { get; set; }

        public IFormFile? Logo { get; set; }

        public IFormFile? TourismLicenseImage { get; set; }
    }
}
