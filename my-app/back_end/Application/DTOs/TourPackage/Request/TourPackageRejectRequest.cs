using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.TourPackage.Request
{
    /// <summary>Body for the admin reject action: the reason shown to the owning company.</summary>
    public class TourPackageRejectRequest
    {
        [Required(ErrorMessage = "Rejection reason is required.")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Rejection reason must be between 3 and 1000 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
