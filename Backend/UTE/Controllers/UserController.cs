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
        /// Retrieves all users
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all users</returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userService.GetAllAsync(cancellationToken);
                return Ok(users);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the requested user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.GetAsync(id, cancellationToken);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for user ID: {UserId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found", id);
                return NotFound(CreateProblemDetails("User not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Completes the authenticated user's profile after OTP verification.
        /// Uploads ID and passport images. Can only be called once.
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
        [Authorize]
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

            try
            {
                var profile = await _userService.CompleteProfileAsync(userId.Value, request, cancellationToken);
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument completing profile for user {UserId}", userId);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error completing profile for user {UserId}", userId);
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex, "Forbidden while completing profile for user {UserId}", userId);
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Not found while completing profile for user {UserId}", userId);
                return NotFound(CreateProblemDetails("Not Found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict completing profile for user {UserId}", userId);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict completing profile for user {UserId}", userId);
                return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error completing profile for user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Completes the authenticated user's profile as a Tour Company owner after OTP verification.
        /// The role is fixed to "TourCompany" by the server. Uploads the profile and national ID images. Can only be called once.
        /// </summary>
        /// <param name="request">Company profile information (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The completed user profile</returns>
        /// <response code="200">Returns the completed profile</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the email is not verified</response>
        /// <response code="404">If the user or the TourCompany role is not found</response>
        /// <response code="409">If the profile has already been completed, or a unique field is taken</response>
        /// <response code="500">If there was an internal server error</response>
        //[HttpPost("complete-company-profile")]
        //[Authorize]
        //[Consumes("multipart/form-data")]
        //[ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<UserResponse>> CompleteCompanyProfile(
        //    [FromForm] CompleteCompanyProfileRequest request,
        //    CancellationToken cancellationToken = default)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(CreateValidationProblemDetails());
        //    }

        //    var userId = GetCurrentUserId();
        //    if (userId is null)
        //    {
        //        return Unauthorized(CreateProblemDetails("Unauthorized", "Invalid or missing authentication token", StatusCodes.Status401Unauthorized));
        //    }

        //    try
        //    {
        //        var profile = await _userService.CompleteCompanyProfileAsync(userId.Value, request, cancellationToken);
        //        return Ok(profile);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        _logger.LogWarning(ex, "Invalid argument completing company profile for user {UserId}", userId);
        //        return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
        //    }
        //    catch (ValidationException ex)
        //    {
        //        _logger.LogWarning(ex, "Validation error completing company profile for user {UserId}", userId);
        //        var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
        //        problem.Extensions["errors"] = ex.Errors;
        //        return BadRequest(problem);
        //    }
        //    catch (ForbiddenException ex)
        //    {
        //        _logger.LogWarning(ex, "Forbidden while completing company profile for user {UserId}", userId);
        //        return StatusCode(StatusCodes.Status403Forbidden,
        //            CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        _logger.LogWarning(ex, "Not found while completing company profile for user {UserId}", userId);
        //        return NotFound(CreateProblemDetails("Not Found", ex.Message, StatusCodes.Status404NotFound));
        //    }
        //    catch (ConflictException ex)
        //    {
        //        _logger.LogWarning(ex, "Conflict completing company profile for user {UserId}", userId);
        //        return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
        //    }
        //    catch (ConcurrencyException ex)
        //    {
        //        _logger.LogWarning(ex, "Concurrency conflict completing company profile for user {UserId}", userId);
        //        return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
        //    }
        //    catch (ServiceException ex)
        //    {
        //        _logger.LogError(ex, "Error completing company profile for user {UserId}", userId);
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            CreateProblemDetails("Internal Server Error", ex.Message));
        //    }
        //}

        /// <summary>
        /// Updates the authenticated user's own profile (partial update). Password change requires the current password.
        /// </summary>
        /// <param name="request">Fields to update. All fields are optional; only provided fields will be changed.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated user profile</returns>
        /// <response code="200">Returns the updated profile</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the current password is incorrect or the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If a conflict occurs or concurrency issue</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("Update")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserResponse>> UpdateMe(
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

            try
            {
                var updatedUser = await _userService.UpdateAsync(userId.Value, request, cancellationToken);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument updating user {UserId}", userId);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating user {UserId}", userId);
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (AuthException ex)
            {
                _logger.LogWarning(ex, "Incorrect current password while updating user {UserId}", userId);
                return Unauthorized(CreateProblemDetails("Unauthorized", ex.Message, StatusCodes.Status401Unauthorized));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found for update: {UserId}", userId);
                return NotFound(CreateProblemDetails("User not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict updating user {UserId}", userId);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating user {UserId}", userId);
                return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Deletes the authenticated user's own account after confirming their password.
        /// </summary>
        /// <param name="request">Body containing the current password</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">If the account was successfully deleted</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the password is incorrect or the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("Delete")]
        [Authorize]
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

            try
            {
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
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument deleting user {UserId}", userId);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error deleting user {UserId}", userId);
                var problem = CreateProblemDetails("Validation Error", "One or more validation errors occurred", StatusCodes.Status400BadRequest);
                problem.Extensions["errors"] = ex.Errors;
                return BadRequest(problem);
            }
            catch (AuthException ex)
            {
                _logger.LogWarning(ex, "Incorrect password for self-deletion of user {UserId}", userId);
                return Unauthorized(CreateProblemDetails("Unauthorized", ex.Message, StatusCodes.Status401Unauthorized));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Admin-only: delete any user by ID.
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="200">If the user was successfully deleted</response>
        /// <response code="401">If the caller is not authenticated</response>
        /// <response code="403">If the caller is not Admin, or the target is an Admin</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdminDeleteUser(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _userService.AdminDeleteUserAsync(id, cancellationToken);

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
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument admin-deleting user {UserId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning(ex, "Admin delete blocked for user {UserId}", id);
                return StatusCode(StatusCodes.Status403Forbidden,
                    CreateProblemDetails("Forbidden", ex.Message, StatusCodes.Status403Forbidden));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error admin-deleting user {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Searches for users based on criteria
        /// </summary>
        /// <param name="firstName">First name filter (partial match)</param>
        /// <param name="lastName">Last name filter (partial match)</param>
        /// <param name="email">Email filter (partial match)</param>
        /// <param name="roleId">Role ID filter</param>
        /// <param name="isEmailVerified">Email verification status filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of users matching the search criteria</returns>
        /// <response code="200">Returns matching users</response>
        /// <response code="400">If the search parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<UserResponse>>> Filter(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] int? roleId = null,
            [FromQuery] bool? isEmailVerified = null,
            CancellationToken cancellationToken = default)
        {
            if (roleId.HasValue && roleId.Value < 0)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Role ID cannot be negative"));
            }

            try
            {
                var users = await _userService.FilterAsync(
                    firstName,
                    lastName,
                    email,
                    roleId,
                    isEmailVerified,
                    cancellationToken);

                return Ok(users);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error searching users");
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
