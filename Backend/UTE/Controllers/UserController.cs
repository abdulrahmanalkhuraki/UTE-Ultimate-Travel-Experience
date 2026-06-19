using Application.DTOs.User.Request;
using Application.DTOs.User.Response;
using Application.Exceptions;
using Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Application.Exceptions.ValidationException;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all users. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns all users</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The user ID (must be >= 1)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetAsync(id, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Completes the authenticated user's profile after OTP verification.
        /// Uploads ID and passport images. Can only be called once per user.
        /// </summary>
        /// <param name="request">Profile information (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The completed user profile</returns>
        /// <response code="200">Returns the completed profile</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the email is not verified or the chosen role is not allowed</response>
        /// <response code="404">If the user or selected role is not found</response>
        /// <response code="409">If the profile has already been completed, or a unique field is taken</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost("complete-profile")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> CompleteProfile(
            [FromForm] CompleteProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            var profile = await _userService.CompleteProfileAsync(userId.Value, request, cancellationToken);
            return Ok(profile);
        }

        /// <summary>
        /// Updates the authenticated user's profile information.
        /// Supports uploading new images via multipart/form-data.
        /// </summary>
        /// <param name="request">Updated profile fields (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated user profile</returns>
        /// <response code="200">Returns the updated profile</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the caller does not have permission</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If there is a conflict (e.g. duplicate unique field)</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("Update")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> Update(
            [FromForm] UserUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            var updatedUser = await _userService.UpdateAsync(userId.Value, request, cancellationToken);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Updates the authenticated user's current location (latitude/longitude).
        /// </summary>
        /// <param name="request">Latitude and longitude coordinates</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated user profile with the new location</returns>
        /// <response code="200">Returns the updated user with location</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("location")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> UpdateLocation(
            [FromBody] UpdateLocationRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            var updatedUser = await _userService.UpdateLocationAsync(userId.Value, request, cancellationToken);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Changes the authenticated user's password after verifying their current password.
        /// </summary>
        /// <param name="request">Current and new password</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated user profile</returns>
        /// <response code="200">Returns the updated user</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the password is incorrect or the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("password")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            var updatedUser = await _userService.ChangePasswordAsync(userId.Value, request, cancellationToken);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes the authenticated user's own account after confirming their password.
        /// </summary>
        /// <param name="request">Body containing the current password for verification</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A confirmation message</returns>
        /// <response code="200">If the account was successfully deleted</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the password is incorrect or the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("Delete")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMe(
            [FromBody] DeleteAccountRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
            }

            var deleted = await _userService.DeleteMyAccountAsync(userId.Value, request, cancellationToken);

            if (!deleted)
            {
                _logger.LogWarning("User not found for self-deletion: {UserId}", userId);
                return NotFound(CreateProblemDetails(
                    "User not found",
                    "Your account could not be found",
                    StatusCodes.Status404NotFound));
            }

            _logger.LogInformation("User {UserId} successfully deleted their account", userId);
            return Ok(new
            {
                message = "Your account has been deleted successfully.",
                status_code = StatusCodes.Status200OK
            });

        }

        /// <summary>
        /// Admin-only: deletes a user by ID, with optional hard delete.
        /// </summary>
        /// <param name="id">The user ID to delete (must be >= 1)</param>
        /// <param name="IsHardDelete">If true, permanently removes the user; otherwise performs a soft delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A confirmation message</returns>
        /// <response code="200">If the user was successfully deleted</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not Admin</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id:int:min(1)}/{IsHardDelete:bool}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdminDeleteUser(int id,bool IsHardDelete = false,CancellationToken cancellationToken = default)
        {
            var deleted = await _userService.AdminDeleteUserAsync(id, cancellationToken, IsHardDelete);

            if (!deleted)
            {
                return NotFound(CreateProblemDetails(
                    "User not found",
                    $"User with ID {id} not found",
                    StatusCodes.Status404NotFound));
            }

            return Ok(new
            {
                message = $"User {id} has been deleted successfully.",
                status_code = StatusCodes.Status200OK
            });
        }

        /// <summary>
        /// Retrieves all soft-deleted users with a total count. Restricted to Admin role.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of deleted users with total count</returns>
        /// <response code="200">Returns deleted users and total count</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not an Admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("deleted")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DeletedUsersResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeletedUsersResponse>> GetDeletedUsers(CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetDeletedUsersAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Searches for users based on optional filter criteria.
        /// All parameters are optional; only provided filters are applied.
        /// </summary>
        /// <param name="firstName">First name filter (partial match)</param>
        /// <param name="lastName">Last name filter (partial match)</param>
        /// <param name="email">Email filter (partial match)</param>
        /// <param name="roleName">Role name filter (exact match)</param>
        /// <param name="isEmailVerified">Email verification status filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users matching the search criteria</returns>
        /// <response code="200">Returns matching users</response>
        /// <response code="400">If the search parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("filter")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<UserResponse>>> Filter(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? roleName = null,
            [FromQuery] bool? isEmailVerified = null,
            CancellationToken cancellationToken = default)
        {
            var users = await _userService.FilterAsync(
                firstName,
                lastName,
                email,
                roleName,
                isEmailVerified,
                cancellationToken);

            return Ok(users);
        }

        #region Private Helper Methods

        /// <summary>
        /// Extracts the current authenticated user's ID from the JWT claims.
        /// </summary>
        /// <returns>The user ID, or null if the claim is missing or invalid</returns>
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) && id > 0 ? id : null;
        }

        /// <summary>
        /// Creates a standardized <see cref="ProblemDetails"/> response.
        /// </summary>
        /// <param name="title">The problem title</param>
        /// <param name="detail">The problem detail</param>
        /// <param name="statusCode">HTTP status code (defaults to 500)</param>
        /// <returns>A configured ProblemDetails instance</returns>
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

        /// <summary>
        /// Creates a <see cref="ValidationProblemDetails"/> from the current model state errors.
        /// </summary>
        /// <returns>A configured ValidationProblemDetails instance</returns>
        private ValidationProblemDetails CreateValidationProblemDetails()
        {
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Type = "https://httpstatuses.com/400",
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Instance = HttpContext.Request.Path
            };

            return problemDetails;
        }

        #endregion
    }
}
