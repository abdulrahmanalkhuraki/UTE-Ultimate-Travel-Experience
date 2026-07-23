using System.Net.Mime;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.TourPackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishlistService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(IReadOnlyList<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IReadOnlyList<TourPackageResponse>>> GetWishlist(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(await _wishlistService.GetWishlistAsync(cancellationToken));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error retrieving wishlist");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpPost("add/{id:int:min(1)}")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddToWishlist(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var success = await _wishlistService.AddToWishlistAsync(id, cancellationToken);

                if (success)
                    return Ok(new { message = $"Tour package {id} has been added to your wishlist." });

                return BadRequest(CreateProblemDetails("Bad Request", "Failed to add tour package to wishlist.", StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Not Found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ConflictException ex)
            {
                return Conflict(CreateProblemDetails("Conflict", ex.Message, StatusCodes.Status409Conflict));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid Request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error adding tour package {PackageId} to wishlist", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        [HttpPost("remove/{id:int:min(1)}")]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveFromWishlist(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var success = await _wishlistService.RemoveFromWishlistAsync(id, cancellationToken);

                if (success)
                    return Ok(new { message = $"Tour package {id} has been removed from your wishlist." });

                return BadRequest(CreateProblemDetails("Bad Request", "Failed to remove tour package from wishlist.", StatusCodes.Status400BadRequest));
            }
            catch (NotFoundException ex)
            {
                return NotFound(CreateProblemDetails("Not Found", ex.Message, StatusCodes.Status404NotFound));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(CreateProblemDetails("Invalid Request", ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Error removing tour package {PackageId} from wishlist", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    CreateProblemDetails("Internal Server Error", ex.Message));
            }
        }

        #region Helpers

        private ProblemDetails CreateProblemDetails(string title, string detail, int statusCode = StatusCodes.Status500InternalServerError)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = statusCode,
                Instance = HttpContext?.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }

        #endregion
    }
}
