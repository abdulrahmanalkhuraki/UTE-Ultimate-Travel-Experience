using Application.DTOs.Notification.Response;

namespace Application.Interfaces.Notifications
{
    /// <summary>
    /// Abstraction over the real-time transport (SignalR). Implemented in the web layer
    /// so the Application layer stays free of any SignalR dependency.
    /// </summary>
    public interface IRealtimeNotifier
    {
        /// <summary>Pushes a notification to all live connections of a single user.</summary>
        Task SendToUserAsync(int userId, NotificationResponse notification, CancellationToken cancellationToken = default);
    }
}
