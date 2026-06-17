using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using Application.Exceptions;
using Application.Interfaces.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a specific booking by ID
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested booking</returns>
        /// <response code="200">Returns the requested booking</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var booking = await _bookingService.GetAsync(id, cancellationToken);
            return Ok(booking);
        }

        /// <summary>
        /// Retrieves all bookings
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all bookings</returns>
        /// <response code="200">Returns the list of bookings</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an admin</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IReadOnlyList<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
            return Ok(bookings);
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <param name="request">Booking creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created booking</returns>
        /// <response code="201">Returns the newly created booking</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="404">If a referenced entity (package/companion) is not found</response>
        /// <response code="409">If the booking conflicts with an existing booking</response>
        /// <response code="422">If the user's profile is not completed</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var createdBooking = await _bookingService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdBooking.Id },
                createdBooking);
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="request">Booking update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated booking</returns>
        /// <response code="200">Returns the updated booking</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="409">If a concurrency conflict occurs</response>
        /// <response code="422">If the booking status does not allow updates</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Update(int id,
            [FromBody] BookingUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var updatedBooking = await _bookingService.UpdateAsync(id, request, cancellationToken);
            return Ok(updatedBooking);
        }

        /// <summary>
        /// Cancels a pending booking
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">If the booking was successfully cancelled</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="422">If the booking status does not allow cancellation</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("Cancel/{id:int:min(1)}")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Cancel(int id, CancellationToken cancellationToken = default)
        {
            await _bookingService.CancelAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Retrieves unapproved (pending) bookings for the current tour company
        /// </summary>
        /// <param name="packageId">Optional package ID to filter by</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of pending bookings</returns>
        /// <response code="200">Returns the list of pending bookings</response>
        /// <response code="400">If the package ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not a tour company</response>
        /// <response code="404">If the specified package is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("UnApprovedBookings")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(IReadOnlyList<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetUnapprovedBookings(
            [FromQuery] int? packageId = null,
            CancellationToken cancellationToken = default)
        {
            var pendingBookings = await _bookingService.GetUnApprovedAsync(packageId, cancellationToken);
            return Ok(pendingBookings);
        }

        /// <summary>
        /// Approves a pending booking on behalf of the tour company
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="request">Approval request with optional new calculated cost</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The approved booking</returns>
        /// <response code="200">Returns the approved booking</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user's company does not own this booking's package</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="422">If the booking status is not pending or required data is missing</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPatch("Approve/{id:int:min(1)}")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Approve(int id, [FromBody] BookingApproveRequest request,
            CancellationToken cancellationToken = default)
        {
            var approvedBooking = await _bookingService.ApproveAsync(id, request, cancellationToken);
            return Ok(approvedBooking);
        }

        /// <summary>
        /// Rejects a pending booking on behalf of the tour company
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="request">Rejection request with reason</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The rejected booking</returns>
        /// <response code="200">Returns the rejected booking</response>
        /// <response code="400">If the request is invalid or rejection reason is missing</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user's company does not own this booking's package</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="422">If the booking status is not pending</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPatch("Reject/{id:int:min(1)}")]
        [Authorize(Roles = "TourCompany")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Reject(int id, [FromBody] BookingRejectRequest request,
            CancellationToken cancellationToken = default)
        {
            var rejectedBooking = await _bookingService.RejectAsync(id, request, cancellationToken);
            return Ok(rejectedBooking);
        }

        /// <summary>
        /// Confirms a booking after the company has accepted it (tourist action)
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The confirmed booking</returns>
        /// <response code="200">Returns the confirmed booking</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the owner of this booking</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="422">If the booking status is not Accepted_By_Company</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPatch("Confirm/{id:int:min(1)}")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Confirm(int id, CancellationToken cancellationToken = default)
        {
            var confirmedBooking = await _bookingService.ConfirmAsync(id, cancellationToken);
            return Ok(confirmedBooking);
        }

        /// <summary>
        /// Declines a booking after the company has accepted it (tourist action)
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The declined booking</returns>
        /// <response code="200">Returns the declined booking</response>
        /// <response code="400">If the ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not the owner of this booking</response>
        /// <response code="404">If the booking is not found</response>
        /// <response code="422">If the booking status is not Accepted_By_Company</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPatch("Decline/{id:int:min(1)}")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookingResponse>> Decline(int id, CancellationToken cancellationToken = default)
        {
            var declinedBooking = await _bookingService.DeclineAsync(id, cancellationToken);
            return Ok(declinedBooking);
        }

        #region Private Helper Methods

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
