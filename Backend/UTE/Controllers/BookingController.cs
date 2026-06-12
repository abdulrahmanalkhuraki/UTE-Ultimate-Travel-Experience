using Application.DTOs.Booking.Request;
using Application.DTOs.Booking.Response;
using Application.DTOs.Hotel.Response;
using Application.Exceptions;
using Application.Interfaces.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("{id:int:min(1)}")]
        [Authorize]
        public async Task<ActionResult<BookingResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var booking = await _bookingService.GetAsync(id, cancellationToken);
            return Ok(booking);
        }

        /// <summary>
        /// Retrieves all Bookings
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all bookings</returns>
        /// <response code="200">Returns the list of bookings</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<BookingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
            return Ok(bookings);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateValidationProblemDetails());

            var createdBooking = await _bookingService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdBooking.Id, version = "1" },
                createdBooking);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<BookingResponse>> Update(int id,
            [FromBody] BookingUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateValidationProblemDetails());

            var updatedBooking = await _bookingService.UpdateAsync(id, request, cancellationToken);
            return Ok(updatedBooking);
        }


        [HttpDelete("Cancel/{id}")]
        [Authorize]
        public async Task<ActionResult> Cancel(int id, CancellationToken cancellationToken = default)
        {
            await _bookingService.CancelAsync(id, cancellationToken);
            return NoContent();
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
