using Application.DTOs.Notification.Response;
using Application.Interfaces.Notifications;
using Domain.Enums;
using Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Notifications
{
    /// <summary>
    /// Sends notifications to a user's devices via Firebase Cloud Messaging.
    /// Push is best-effort: if Firebase is not configured or a send fails, it logs and
    /// returns without throwing (the notification has already been persisted by the caller).
    /// </summary>
    public class FirebaseNotifier : IRealtimeNotifier
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FirebaseNotifier> _logger;

        public FirebaseNotifier(IUnitOfWork unitOfWork, ILogger<FirebaseNotifier> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendToUserAsync(int userId, NotificationResponse notification, CancellationToken cancellationToken = default)
        {
            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogWarning("Firebase is not configured; skipping push for user {UserId}", userId);
                return;
            }

            var tokens = await _unitOfWork.DeviceTokens.Query()
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .Select(t => t.Token)
                .ToListAsync(cancellationToken);

            if (tokens.Count == 0)
            {
                _logger.LogInformation("User {UserId} has no registered device tokens; nothing to push", userId);
                return;
            }

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = TitleFor((NotificationType)notification.Type),
                    Body = notification.Message
                },
                Data = new Dictionary<string, string>
                {
                    ["notificationId"] = notification.Id.ToString(),
                    ["type"] = notification.Type.ToString(),
                    ["typeName"] = notification.TypeName ?? string.Empty
                }
            };

            BatchResponse response;
            try
            {
                response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FCM send failed for user {UserId}", userId);
                return;
            }

            // Drop tokens FCM reports as permanently invalid so we stop sending to dead devices.
            var stale = new List<string>();
            for (var i = 0; i < response.Responses.Count; i++)
            {
                var r = response.Responses[i];
                if (!r.IsSuccess
                    && r.Exception is FirebaseMessagingException fme
                    && fme.MessagingErrorCode is MessagingErrorCode.Unregistered or MessagingErrorCode.InvalidArgument)
                {
                    stale.Add(tokens[i]);
                }
            }

            if (stale.Count > 0)
            {
                try
                {
                    var toRemove = await _unitOfWork.DeviceTokens.Query()
                        .Where(t => stale.Contains(t.Token))
                        .ToListAsync(cancellationToken);

                    _unitOfWork.DeviceTokens.RemoveRange(toRemove);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Removed {Count} stale device tokens for user {UserId}", toRemove.Count, userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clean up stale device tokens for user {UserId}", userId);
                }
            }

            _logger.LogInformation("FCM push to user {UserId}: {Success} succeeded, {Failure} failed",
                userId, response.SuccessCount, response.FailureCount);
        }

        private static string TitleFor(NotificationType type) => type switch
        {
            NotificationType.CompanyApproved => "تمت الموافقة على شركتك",
            NotificationType.CompanyRejected => "تحديث على طلب شركتك",
            _ => "UTE"
        };
    }
}
