using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourCompany.Request
{
    /// <summary>
    /// Partial update request for a Tour Company. Sent as multipart/form-data.
    /// Every field is optional: only the fields actually sent are changed, and an
    /// image is replaced only when a new file is uploaded.
    /// </summary>
    public class TourCompanyUpdateRequest
    {
        public string? Name { get; set; }

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
