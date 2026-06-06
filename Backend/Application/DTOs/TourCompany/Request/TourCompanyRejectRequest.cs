using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.TourCompany.Request
{
    /// <summary>Body for the admin reject action: the reason shown to the company owner.</summary>
    public class TourCompanyRejectRequest
    {
        [Required(ErrorMessage = "Rejection reason is required.")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Rejection reason must be between 3 and 1000 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
