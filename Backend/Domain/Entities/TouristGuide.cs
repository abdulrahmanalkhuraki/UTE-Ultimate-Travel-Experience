using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// A tour guide (مرشد سياحي). Belongs to one or more companies through
    /// <see cref="CompanyGuides"/> and may be assigned to programs through
    /// <see cref="TourPackageGuides"/>. Fields mirror the "إضافة مرشد" form.
    /// </summary>
    public partial class TouristGuide : BaseEntity
    {
        /// <summary>الاسم الأول.</summary>
        public string Firstname { get; set; } = null!;

        /// <summary>الاسم الأخير.</summary>
        public string Lastname { get; set; } = null!;

        /// <summary>رقم الهاتف.</summary>
        public string Phone { get; set; } = null!;

        /// <summary>البريد الإلكتروني.</summary>
        public string Email { get; set; } = null!;

        /// <summary>الجنسية (دولة).</summary>
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

        /// <summary>الموقع الحالي (نص حر).</summary>
        public string? CurrentLocation { get; set; }

        /// <summary>الرقم الوطني.</summary>
        public string? NationalNumber { get; set; }

        /// <summary>رقم جواز السفر.</summary>
        public string? PassportNumber { get; set; }

        /// <summary>صورة شخصية للمرشد (رابط الصورة).</summary>
        public string? ProfileImageUrl { get; set; }

        /// <summary>صورة الهوية الشخصية (رابط الصورة).</summary>
        public string? IdCard { get; set; }

        /// <summary>صورة جواز السفر (رابط الصورة).</summary>
        public string? PassportScan { get; set; }

        /// <summary>اللغات (غير مطلوب في النموذج الحالي).</summary>
        public string? Languages { get; set; }

        /// <summary>صورة رخصة الإرشاد (غير مطلوب في النموذج الحالي).</summary>
        public string? LicenseScan { get; set; }

        /// <summary>متاح للعمل.</summary>
        public bool IsAvailable { get; set; } = true;

        public virtual Country NatinalityCountry { get; set; } = null!;

        /// <summary>Companies this guide works for (الشركات).</summary>
        public virtual ICollection<CompanyGuide> CompanyGuides { get; set; } = new List<CompanyGuide>();

        /// <summary>Programs this guide is assigned to (البرامج).</summary>
        public virtual ICollection<TourPackageGuide> TourPackageGuides { get; set; } = new List<TourPackageGuide>();
    }
}
