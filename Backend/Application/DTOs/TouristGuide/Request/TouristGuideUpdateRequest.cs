using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TouristGuide.Request
{
    /// <summary>
    /// Update request for a tour guide. Sent as multipart/form-data. To keep an
    /// existing image without re-uploading it, leave the file field empty and send
    /// the current URL in the matching *Url field.
    /// </summary>
    public class TouristGuideUpdateRequest
    {
        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public bool Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int YearsOfExperiance { get; set; }

        public string Bio { get; set; } = null!;

        public string PlaceOfResidence { get; set; } = null!;

        public string? CurrentLocation { get; set; }

        public string NationalNumber { get; set; } = null!;

        public string? PassportNumber { get; set; }

        public string? Languages { get; set; }

        public bool IsAvailable { get; set; } = true;

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
