using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notification.Request
{
    public class DeviceTokenRequest
    {
        [Required(ErrorMessage = "Device token is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Device token length is invalid.")]
        public string Token { get; set; } = string.Empty;

        [RegularExpression("^(android|ios|web)$", ErrorMessage = "Platform must be 'android', 'ios', or 'web'.")]
        public string? Platform { get; set; }
    }
}
