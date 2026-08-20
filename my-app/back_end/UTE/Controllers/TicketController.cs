using Application.DTOs.Ticket.Request;
using Application.DTOs.Ticket.Response;
using Application.Interfaces.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Creates a new support ticket
        /// </summary>
        /// <param name="request">Ticket creation request (multipart/form-data)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created ticket</returns>
        /// <response code="201">Returns the newly created ticket</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user does not have permission</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [Authorize(Policy = "RequireCompletedProfile")]
        [Authorize(Roles = "Tourist,TourCompany")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TicketResponse>> Create([FromForm] TicketCreateRequest request, CancellationToken cancellationToken = default)
        {
            var created = await _ticketService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Retrieves a list of tickets, optionally filtered by user ID
        /// </summary>
        /// <param name="userId">Optional user ID to filter tickets</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of tickets matching the specified filters</returns>
        /// <response code="200">Returns the list of tickets</response>
        /// <response code="400">If the request parameters are invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IReadOnlyList<TicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<TicketResponse>>> Get([FromQuery] int? userId, CancellationToken cancellationToken = default)
        {
            return Ok(await _ticketService.GetAsync(userId, cancellationToken));
        }
    }
}
