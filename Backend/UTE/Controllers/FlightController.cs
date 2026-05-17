using Application.DTOs.Flight.Request;
using Application.DTOs.Flight.Response;
using Application.Exceptions;
using Application.Interfaces.Flight;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Application.Exceptions.ValidationException;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly ILogger<FlightController> _logger;

        public FlightController(IFlightService flightService, ILogger<FlightController> logger)
        {
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all flights
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all flights</returns>
        /// <response code="200">Returns the list of flights</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<FlightResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<FlightResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var flights = await _flightService.GetAllAsync(cancellationToken);
                return Ok(flights);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all flights");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a specific flight by ID
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested flight</returns>
        /// <response code="200">Returns the requested flight</response>
        /// <response code="404">If the flight is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(FlightResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var flight = await _flightService.GetAsync(id, cancellationToken);

                if (flight == null)
                {
                    _logger.LogWarning("Flight with ID {FlightId} not found", id);
                    return NotFound(CreateProblemDetails(
                        "Flight not found",
                        $"Flight with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                return Ok(flight);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for flight ID: {FlightId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving flight {FlightId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Creates a new flight
        /// </summary>
        /// <param name="request">Flight creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created flight</returns>
        /// <response code="201">Returns the newly created flight</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="409">If a conflict occurs (duplicate flight)</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(FlightResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightResponse>> Create(
            [FromBody] FlightCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            try
            {
                var createdFlight = await _flightService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdFlight.Id},
                    createdFlight);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating flight {FlightNumber}", request.FlightNumber);
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict creating flight {FlightNumber}", request.FlightNumber);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error creating flight {FlightNumber}", request.FlightNumber);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing flight
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <param name="request">Flight update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated flight</returns>
        /// <response code="200">Returns the updated flight</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the flight is not found</response>
        /// <response code="409">If a conflict occurs or concurrency issue</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(FlightResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FlightResponse>> Update(
            int id,
            [FromBody] FlightUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            try
            {
                var updatedFlight = await _flightService.UpdateAsync(id, request, cancellationToken);
                return Ok(updatedFlight);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument updating flight {FlightId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating flight {FlightId}", id);
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Flight not found for update: {FlightId}", id);
                return NotFound(CreateProblemDetails("Flight not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict updating flight {FlightId}", id);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating flight {FlightId}", id);
                return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error updating flight {FlightId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Deletes a flight
        /// </summary>
        /// <param name="id">Flight ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">If the flight was successfully deleted</response>
        /// <response code="404">If the flight is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _flightService.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    _logger.LogWarning("Flight not found for deletion: {FlightId}", id);
                    return NotFound(CreateProblemDetails(
                        "Flight not found",
                        $"Flight with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                _logger.LogInformation("Flight {FlightId} successfully deleted", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument deleting flight {FlightId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error deleting flight {FlightId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Searches for flights based on criteria
        /// </summary>
        /// <param name="originCityId">Origin city ID filter</param>
        /// <param name="destinationCityId">Destination city ID filter</param>
        /// <param name="departureDate">Departure date filter</param>
        /// <param name="returnDate">Return date filter (for round trips)</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="airlineId">Airline ID filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of flights matching the search criteria</returns>
        /// <response code="200">Returns matching flights</response>
        /// <response code="400">If the search parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<FlightResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<FlightResponse>>> Filter(
            [FromQuery] string? airline = null,
            [FromQuery] int? departureCityId = null,
            [FromQuery] int? arrivalCityId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            // Validate search parameters
            if (fromDate.HasValue && fromDate.Value < DateTime.Today)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "fromDate cannot be in the past"));
            }

            if (fromDate.HasValue && toDate.HasValue && toDate.Value < fromDate.Value)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "toDate cannot be earlier than fromDate"));
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Minimum price cannot be greater than maximum price"));
            }

            if (minPrice.HasValue && minPrice < 0)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Minimum price cannot be negative"));
            }

            if (maxPrice.HasValue && maxPrice < 0)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Maximum price cannot be negative"));
            }

            try
            {
                var flights = await _flightService.FilterAsync(
                    airline,
                    departureCityId,
                    arrivalCityId,
                    fromDate,
                    toDate,
                    minPrice,
                    maxPrice,
                    cancellationToken);

                return Ok(flights);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error searching flights");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
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