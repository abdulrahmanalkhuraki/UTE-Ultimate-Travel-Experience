using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TouristGuide.Request
{
    /// <summary>
    /// Update request for a tour guide. Sent as multipart/form-data.
    /// <para>
    /// PARTIAL UPDATE (تعديل جزئي): every field is OPTIONAL. Only the fields that
    /// are actually sent are changed; anything left out keeps its current value.
    /// That is why all members are nullable — <c>null</c> means "not sent, don't
    /// touch". To keep an existing image, just leave its file field empty.
    /// </para>
    /// </summary>
    public class TouristGuideUpdateRequest
    {
        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public int? NationalityCountryId { get; set; }

        public bool? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public int? YearsOfExperiance { get; set; }

        public string? Bio { get; set; }

        public string? PlaceOfResidence { get; set; }

        public string? CurrentLocation { get; set; }

        public string? NationalNumber { get; set; }

        public string? PassportNumber { get; set; }

        public string? Languages { get; set; }

        public bool? IsAvailable { get; set; }

        /// <summary>New profile image to upload. Optional.</summary>
        public IFormFile? ProfileImage { get; set; }

        /// <summary>Existing profile image URL to keep when no new file is uploaded.</summary>
        public string? ProfileImageUrl { get; set; }

        public IFormFile? IdCardImage { get; set; }
        public string? IdCardUrl { get; set; }

        public IFormFile? PassportImage { get; set; }
        public string? PassportScanUrl { get; set; }
    }
}
