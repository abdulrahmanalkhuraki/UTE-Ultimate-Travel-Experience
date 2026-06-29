using System;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>
    /// A single activity inside a program day (النشاط). Sent as part of a
    /// multipart/form-data request; the image is an uploaded file.
    /// Field names look like: Days[0].Activities[1].Title, Days[0].Activities[1].ProfileImage ...
    /// </summary>
    public class TourPackageActivityRequest
    {
        /// <summary>Display order within the day (1-based).</summary>
        public int OrderNumber { get; set; }

        /// <summary>Activity title (عنوان النشاط).</summary>
        public string Title { get; set; } = null!;

        /// <summary>Short description (شرح مختصر عن النشاط).</summary>
        public string? Description { get; set; }

        /// <summary>Start time (من). Send as "HH:mm".</summary>
        public TimeOnly StartTime { get; set; }

        /// <summary>End time (إلى). Send as "HH:mm".</summary>
        public TimeOnly EndTime { get; set; }

        /// <summary>New activity image to upload. Optional on update.</summary>
        public IFormFile? Image { get; set; }

        /// <summary>
        /// Existing image URL to keep when no new <see cref="Image"/> is uploaded
        /// (used on update). Ignored when <see cref="Image"/> is provided.
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}
