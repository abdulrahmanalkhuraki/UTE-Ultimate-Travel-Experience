using Application.DTOs.Notification.Response;
using Domain.Enums;

namespace Application.Interfaces.Notifications
{
    public interface INotificationService
    {
        /// <summary>Persists a notification for a user and pushes it in real time.</summary>
        Task<NotificationResponse> NotifyAsync(int userId, string message, NotificationType type, CancellationToken cancellationToken = default);

        /// <summary>Returns a user's notifications, newest first.</summary>
        Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(int userId, bool unreadOnly = false, CancellationToken cancellationToken = default);

        /// <summary>Counts a user's unread notifications.</summary>
        Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>Marks one of the user's own notifications as read. Returns false if not found / not theirs.</summary>
        Task<bool> MarkAsReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default);

        /// <summary>Marks all of the user's notifications as read. Returns how many were updated.</summary>
        Task<int> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>Registers (or refreshes) an FCM device token for the user. Idempotent.</summary>
        Task RegisterDeviceTokenAsync(int userId, string token, string? platform, CancellationToken cancellationToken = default);

        /// <summary>Removes an FCM device token (e.g. on logout). Returns false if it did not exist.</summary>
        Task<bool> RemoveDeviceTokenAsync(int userId, string token, CancellationToken cancellationToken = default);
    }
}
