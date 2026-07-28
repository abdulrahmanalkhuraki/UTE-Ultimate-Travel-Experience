using Application.Common.Constants;
using Application.Common.Logging;
using Application.DTOs.Notification.Response;
using Application.Exceptions;
using Application.Interfaces.Notifications;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotificationEntity = Domain.Entities.Notification;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _realtimeNotifier;
        private readonly ILogger<NotificationService> _logger;
        private const string ObjectName = "Notification";

        public NotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IRealtimeNotifier realtimeNotifier,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _realtimeNotifier = realtimeNotifier ?? throw new ArgumentNullException(nameof(realtimeNotifier));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<NotificationResponse> NotifyAsync(int userId, string message, NotificationType type, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required", nameof(message));

            _logger.StartOperation("Send", ObjectName, 0);

            var entity = new NotificationEntity
            {
                UserId = userId,
                Message = message,
                Type = (int)type,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<NotificationResponse>(entity);

            // The notification is already saved; the real-time push is best-effort and must
            // never fail the calling operation (e.g. approving a company).
            try
            {
                await _realtimeNotifier.SendToUserAsync(userId, response, cancellationToken);
                _logger.LogInformation("Pushed notification {NotificationId} to user {UserId}", entity.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to push real-time notification {NotificationId} to user {UserId}; it was still saved",
                    entity.Id, userId);
            }

            return response;
        }

        public async Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(int userId, bool unreadOnly = false, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));

            _logger.StartOperation("Retrieve", ObjectName, 0);

            try
            {
                var query = _unitOfWork.Notifications.Query()
                    .AsNoTracking()
                    .Where(n => n.UserId == userId);

                if (unreadOnly)
                    query = query.Where(n => !n.IsRead);

                var entities = await query
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .Take(50)
                    .ToListAsync(cancellationToken);

                return _mapper.Map<IReadOnlyList<NotificationResponse>>(entities);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Retrieve", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("retrieve", ObjectName, ex.Message), ex);
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));

            _logger.StartOperation("Count Unread", ObjectName, 0);

            try
            {
                return await _unitOfWork.Notifications.Query()
                    .AsNoTracking()
                    .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Count Unread", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("count", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> MarkAsReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));
            if (notificationId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId(ObjectName), nameof(notificationId));

            _logger.StartOperation("Mark Read", ObjectName, 0);

            try
            {
                var entity = await _unitOfWork.Notifications.Query()
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

                // Hide existence of other users' notifications: not found == not theirs.
                if (entity == null)
                    return false;

                if (!entity.IsRead)
                {
                    entity.IsRead = true;
                    entity.UpdatedAtUtc = DateTime.UtcNow;
                    _unitOfWork.Notifications.Update(entity);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Mark Read", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("mark notification as read", ObjectName, ex.Message), ex);
            }
        }

        public async Task<int> MarkAllAsReadAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));

            _logger.StartOperation("Mark All Read", ObjectName, 0);

            try
            {
                var unread = await _unitOfWork.Notifications.Query()
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync(cancellationToken);

                if (unread.Count == 0)
                    return 0;

                var now = DateTime.UtcNow;
                foreach (var n in unread)
                {
                    n.IsRead = true;
                    n.UpdatedAtUtc = now;
                    _unitOfWork.Notifications.Update(n);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return unread.Count;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Mark All Read", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("mark notifications as read", ObjectName, ex.Message), ex);
            }
        }

        public async Task RegisterDeviceTokenAsync(int userId, string token, string? platform, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required", nameof(token));

            _logger.StartOperation("Register Device", ObjectName, 0);

            try
            {
                var existing = await _unitOfWork.DeviceTokens.Query()
                    .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

                if (existing == null)
                {
                    await _unitOfWork.DeviceTokens.AddAsync(new DeviceToken
                    {
                        UserId = userId,
                        Token = token,
                        Platform = platform,
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    }, cancellationToken);
                }
                else if (existing.UserId != userId || existing.Platform != platform)
                {
                    // The same physical device now belongs to a different user (re-login) or
                    // reports a new platform: re-point the token instead of duplicating it.
                    existing.UserId = userId;
                    existing.Platform = platform;
                    existing.UpdatedAtUtc = DateTime.UtcNow;
                    _unitOfWork.DeviceTokens.Update(existing);
                }
                else
                {
                    return; // Already registered to this user; nothing to do.
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Registered device token for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.ServerError("Register Device", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("register device token", ObjectName, ex.Message), ex);
            }
        }

        public async Task<bool> RemoveDeviceTokenAsync(int userId, string token, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidId("User"), nameof(userId));
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required", nameof(token));

            _logger.StartOperation("Remove Device", ObjectName, 0);

            try
            {
                var existing = await _unitOfWork.DeviceTokens.Query()
                    .FirstOrDefaultAsync(t => t.Token == token && t.UserId == userId, cancellationToken);

                if (existing == null)
                    return false;

                _unitOfWork.DeviceTokens.Remove(existing);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Removed device token for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.ServerError("Remove Device", ObjectName, ex);
                throw new ServiceException(ExceptionMessages.ServiceException("remove device token", ObjectName, ex.Message), ex);
            }
        }
    }
}
