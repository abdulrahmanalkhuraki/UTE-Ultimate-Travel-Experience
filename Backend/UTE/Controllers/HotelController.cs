using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using Application.Exceptions;
using Application.Interfaces.Hotel;
using Microsoft.AspNetCore.Authorization;
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
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger<HotelController> _logger;

        public HotelController(IHotelService hotelService, ILogger<HotelController> logger)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all hotels
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all hotels</returns>
        /// <response code="200">Returns the list of hotels</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<HotelResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<HotelResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var hotels = await _hotelService.GetAllAsync(cancellationToken);
                return Ok(hotels);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all hotels");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a specific hotel by ID
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested hotel</returns>
        /// <response code="200">Returns the requested hotel</response>
        /// <response code="404">If the hotel is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(HotelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var hotel = await _hotelService.GetAsync(id, cancellationToken);

                if (hotel == null)
                {
                    _logger.LogWarning("Hotel with ID {HotelId} not found", id);
                    return NotFound(CreateProblemDetails(
                        "Hotel not found",
                        $"Hotel with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for hotel ID: {HotelId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving hotel {HotelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Creates a new hotel
        /// </summary>
        /// <param name="request">Hotel creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created hotel</returns>
        /// <response code="201">Returns the newly created hotel</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="409">If a conflict occurs (duplicate hotel)</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(HotelResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelResponse>> Create(
            [FromBody] HotelCreateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            try
            {
                var createdHotel = await _hotelService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdHotel.Id, version = "1" },
                    createdHotel);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating hotel {HotelName}", request.HotelName);
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict creating hotel {HotelName}", request.HotelName);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error creating hotel {HotelName}", request.HotelName);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <param name="request">Hotel update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The updated hotel</returns>
        /// <response code="200">Returns the updated hotel</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="404">If the hotel is not found</response>
        /// <response code="409">If a conflict occurs or concurrency issue</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(typeof(HotelResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<HotelResponse>> Update(
            int id,
            [FromBody] HotelUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateValidationProblemDetails());
            }

            try
            {
                var updatedHotel = await _hotelService.UpdateAsync(id, request, cancellationToken);
                return Ok(updatedHotel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument updating hotel {HotelId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating hotel {HotelId}", id);
                return BadRequest(CreateProblemDetails("Validation Error", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Hotel not found for update: {HotelId}", id);
                return NotFound(CreateProblemDetails("Hotel not found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(ex, "Conflict updating hotel {HotelId}", id);
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict updating hotel {HotelId}", id);
                return Conflict(CreateProblemDetails("Concurrency Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error updating hotel {HotelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Deletes a hotel
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <response code="204">If the hotel was successfully deleted</response>
        /// <response code="404">If the hotel is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleted = await _hotelService.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    _logger.LogWarning("Hotel not found for deletion: {HotelId}", id);
                    return NotFound(CreateProblemDetails(
                        "Hotel not found",
                        $"Hotel with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                _logger.LogInformation("Hotel {HotelId} successfully deleted", id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument deleting hotel {HotelId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error deleting hotel {HotelId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Searches for hotels based on criteria
        /// </summary>
        /// <param name="cityId">City ID filter</param>
        /// <param name="minStarRating">Minimum star rating filter</param>
        /// <param name="maxStarRating">Maximum star rating filter</param>
        /// <param name="minPrice">Minimum price per night filter</param>
        /// <param name="maxPrice">Maximum price per night filter</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of hotels matching the search criteria</returns>
        /// <response code="200">Returns matching hotels</response>
        /// <response code="400">If the search parameters are invalid</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(IReadOnlyList<HotelResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<HotelResponse>>> Filter(
            [FromQuery] int? cityId = null,
            [FromQuery] int? minStarRating = null,
            [FromQuery] int? maxStarRating = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            // Validate search parameters
            if (minStarRating.HasValue && (minStarRating < 1 || minStarRating > 5))
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Minimum star rating must be between 1 and 5"));
            }

            if (maxStarRating.HasValue && (maxStarRating < 1 || maxStarRating > 5))
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Maximum star rating must be between 1 and 5"));
            }

            if (minStarRating.HasValue && maxStarRating.HasValue && minStarRating > maxStarRating)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Minimum star rating cannot be greater than maximum star rating"));
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest(CreateProblemDetails(
                    "Invalid search parameters",
                    "Minimum price cannot be greater than maximum price"));
            }

            try
            {
                var hotels = await _hotelService.SearchAsync(
                    cityId,
                    minStarRating,
                    maxStarRating,
                    minPrice,
                    maxPrice,
                    cancellationToken);

                return Ok(hotels);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error searching hotels");
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