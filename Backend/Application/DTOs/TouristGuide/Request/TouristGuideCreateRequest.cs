using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TouristGuide.Request
{
    /// <summary>
    /// Create request for a tour guide ("إضافة مرشد" form). Sent as
    /// multipart/form-data because it carries the profile/ID/passport images.
    /// The guide is linked to the signed-in company; the company is resolved
    /// from the JWT, never from the client.
    /// </summary>
    public class TouristGuideCreateRequest
    {
        /// <summary>الاسم الأول.</summary>
        public string Firstname { get; set; } = null!;

        /// <summary>الاسم الأخير.</summary>
        public string Lastname { get; set; } = null!;

        /// <summary>رقم الهاتف.</summary>
        public string Phone { get; set; } = null!;

        /// <summary>البريد الإلكتروني.</summary>
        public string Email { get; set; } = null!;

        /// <summary>الجنسية (معرّف الدولة).</summary>
        public int NationalityCountryId { get; set; }

        /// <summary>الجنس: true = ذكر, false = أنثى.</summary>
        public bool Gender { get; set; }

        /// <summary>تاريخ الميلاد.</summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>سنوات الخبرة.</summary>
        public int YearsOfExperiance { get; set; }

        /// <summary>وصف عن خبرته (نبذة).</summary>
        public string Bio { get; set; } = null!;

        /// <summary>مكان الإقامة (نص حر).</summary>
        public string PlaceOfResidence { get; set; } = null!;

        /// <summary>الموقع الحالي (نص حر). اختياري.</summary>
        public string? CurrentLocation { get; set; }

        /// <summary>الرقم الوطني.</summary>
        public string NationalNumber { get; set; } = null!;

        /// <summary>رقم جواز السفر. اختياري.</summary>
        public string? PassportNumber { get; set; }

        /// <summary>اللغات. اختياري.</summary>
        public string? Languages { get; set; }

        /// <summary>صورة شخصية للمرشد. اختياري.</summary>
        public IFormFile? ProfileImage { get; set; }

        /// <summary>صورة الهوية الشخصية. اختياري.</summary>
        public IFormFile? IdCardImage { get; set; }

        /// <summary>صورة جواز السفر. اختياري.</summary>
        public IFormFile? PassportImage { get; set; }
    }
}
