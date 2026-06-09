using System;

namespace Application.DTOs.TouristGuide.Response
{
    /// <summary>Full tour-guide details returned by the guide endpoints.</summary>
    public class TouristGuideResponse
    {
        public int Id { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        /// <summary>Convenience full name (الاسم الكامل).</summary>
        public string FullName => $"{Firstname} {Lastname}".Trim();

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public string? NationalityCountryName { get; set; }

        public bool Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int YearsOfExperiance { get; set; }

        public string Bio { get; set; } = null!;

        public string PlaceOfResidence { get; set; } = null!;

        public string? CurrentLocation { get; set; }

        public string? NationalNumber { get; set; }

        public string? PassportNumber { get; set; }

        public string? Languages { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? IdCard { get; set; }

        public string? PassportScan { get; set; }

        public bool IsAvailable { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
