using Application.DTOs.Country.Response;
using Application.Exceptions;
using Application.Interfaces.Country;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countyService, ILogger<CountryController> logger)
        {
            _countryService = countyService ?? throw new ArgumentNullException(nameof(countyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all countries
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all countries</returns>
        /// <response code="200">Returns the list of countries</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<CountryResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var countries = await _countryService.GetAllAsync(cancellationToken);
                return Ok(countries);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving all countries");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a specific country by ID
        /// </summary>
        /// <param name="id">Country ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The requested country</returns>
        /// <response code="200">Returns the requested country</response>
        /// <response code="404">If the country is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var hotel = await _countryService.GetAsync(id, cancellationToken);

                if (hotel == null)
                {
                    _logger.LogWarning("County with ID {CountyId} not found", id);
                    return NotFound(CreateProblemDetails(
                        "County not found",
                        $"County with ID {id} not found",
                        StatusCodes.Status404NotFound));
                }

                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for county ID: {CountyId}", id);
                return BadRequest(CreateProblemDetails("Invalid request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving county {CountyId}", id);
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
