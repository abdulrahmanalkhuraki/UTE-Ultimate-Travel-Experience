using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourCompany.Request
{
    /// <summary>
    /// Create request for a Tour Company ("Company Information" form).
    /// Sent as multipart/form-data because it carries the logo and the
    /// tourism-license image. The owner (UserId) is taken from the JWT,
    /// never from the client.
    /// </summary>
    public class TourCompanyCreateRequest
    {
        /// <summary>Commercial / trade name (الاسم التجاري).</summary>
        public string Name { get; set; } = null!;

        /// <summary>Short description shown in listings (نبذة قصيرة عن الشركة).</summary>
        public string? Description { get; set; }

        /// <summary>Company location / address (موقع الشركة).</summary>
        public string? Location { get; set; }

        /// <summary>Company contact phone (رقم الهاتف).</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Company contact email (البريد الالكتروني).</summary>
        public string? Email { get; set; }

        /// <summary>Founding date (تاريخ التأسيس). The client sends the first day of the month.</summary>
        public DateOnly? FoundingDate { get; set; }

        /// <summary>Tourism registration number (رقم السجل السياحي).</summary>
        public string? TourismLicenseNumber { get; set; }

        /// <summary>Bank account details (حسابات البنك).</summary>
        public string? BankAccount { get; set; }

        /// <summary>Long marketing description shown to tourists (من الشركة).</summary>
        public string? About { get; set; }

        /// <summary>Company logo image (اللوغو الخاص بالشركة).</summary>
        public IFormFile? Logo { get; set; }

        /// <summary>Tourism registration image (صورة السجل السياحي).</summary>
        public IFormFile? TourismLicenseImage { get; set; }
    }
}
