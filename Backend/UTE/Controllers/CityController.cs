using Application.DTOs.City.Response;
using Application.Exceptions;
using Application.Interfaces.City;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, ILogger<CityController> logger)
        {
            _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all cities
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all cities</returns>
        /// <response code="200">Returns the list of cities</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CityResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<CityResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var cities = await _cityService.GetAllAsync(cancellationToken);
                return Ok(cities);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all cities");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a specific city by ID
        /// </summary>
        /// <param name="id">City ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested city</returns>
        /// <response code="200">Returns the requested city</response>
        /// <response code="404">If the city is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CityResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var city = await _cityService.GetAsync(id, cancellationToken);

                if (city == null)
                {
                    _logger.LogWarning("City with ID {CityId} not found", id);
                    return NotFound(CreateProblemDetails(
                        "City not found",
                        $"City with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                return Ok(city);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for city ID: {CityId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "City with ID {CityId} not found", id);
                return NotFound(CreateProblemDetails(
                    "City not found",
                    ex.Message,
                    StatusCodes.Status404NotFound));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving city {CityId}", id);
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
        #endregion
    }
}