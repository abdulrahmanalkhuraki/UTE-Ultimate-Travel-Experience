using System.Net.Mime;
using Application.DTOs.Pagination;
using Application.DTOs.TourPackage.Response;
using Application.Exceptions;
using Application.Interfaces.TourPackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "RequireCompletedProfile")]
    [Authorize(Roles = "Tourist")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<TourPackageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedResponse<TourPackageResponse>>> GetWishlist(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {          
            return Ok(await _wishlistService.GetWishlistAsync(page,pageSize,cancellationToken));
        }

        [HttpPost("add/{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddToWishlist(int id, CancellationToken cancellationToken = default)
        {
            var success = await _wishlistService.AddToWishlistAsync(id, cancellationToken);

            if (success)
                return Ok(new { message = $"Tour package {id} has been added to your wishlist." });

            return BadRequest(CreateProblemDetails("Bad Request", "Failed to add tour package to wishlist.", StatusCodes.Status400BadRequest));
        }

        [HttpPost("remove/{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveFromWishlist(int id, CancellationToken cancellationToken = default)
        {
            var success = await _wishlistService.RemoveFromWishlistAsync(id, cancellationToken);

            if (success)
                return Ok(new { message = $"Tour package {id} has been removed from your wishlist." });

            return BadRequest(CreateProblemDetails("Bad Request", "Failed to remove tour package from wishlist.", StatusCodes.Status400BadRequest));
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
