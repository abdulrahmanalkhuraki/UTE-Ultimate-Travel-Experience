using Application.DTOs.Notification.Request;
using Application.DTOs.Notification.Response;
using Application.Exceptions;
using Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Security.Claims;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns the current user's notifications, newest first (latest 50).
        /// </summary>
        /// <param name="unreadOnly">When true, only unread notifications are returned</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<NotificationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetMine(
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                var items = await _notificationService.GetForUserAsync(userId.Value, unreadOnly, cancellationToken);
                return Ok(items);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Returns the count of the current user's unread notifications.
        /// </summary>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetUnreadCount(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                var count = await _notificationService.GetUnreadCountAsync(userId.Value, cancellationToken);
                return Ok(count);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error counting notifications for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Marks one of the current user's notifications as read.
        /// </summary>
        /// <param name="id">Notification ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost("{id:int:min(1)}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkAsRead(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                var ok = await _notificationService.MarkAsReadAsync(userId.Value, id, cancellationToken);
                if (!ok)
                    return NotFound(CreateProblemDetails("Notification not found", $"Notification {id} not found", StatusCodes.Status404NotFound));

                return NoContent();
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} read for user {UserId}", id, userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Marks all of the current user's notifications as read. Returns how many were updated.
        /// </summary>
        [HttpPost("read-all")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                var updated = await _notificationService.MarkAllAsReadAsync(userId.Value, cancellationToken);
                return Ok(updated);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error marking all notifications read for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Registers (or refreshes) the calling device's FCM token so it can receive push notifications.
        /// </summary>
        /// <param name="request">The FCM token and optional platform</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost("device-token")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterDeviceToken([FromBody] DeviceTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                await _notificationService.RegisterDeviceTokenAsync(userId.Value, request.Token, request.Platform, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error registering device token for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Removes a device's FCM token (e.g. on logout) so it stops receiving push notifications.
        /// </summary>
        /// <param name="request">The FCM token to remove</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpDelete("device-token")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveDeviceToken([FromBody] DeviceTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));

            try
            {
                var removed = await _notificationService.RemoveDeviceTokenAsync(userId.Value, request.Token, cancellationToken);
                if (!removed)
                    return NotFound(CreateProblemDetails("Token not found", "Device token not found", StatusCodes.Status404NotFound));

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error removing device token for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        #region Private Helper Methods

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) && id > 0 ? id : null;
        }

        private ProblemDetails CreateProblemDetails(string title, string detail, int statusCode = StatusCodes.Status500InternalServerError)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = HttpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }

        #endregion
    }
}
