using System;

namespace Application.DTOs.Notification.Response
{
    public sealed class NotificationResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
