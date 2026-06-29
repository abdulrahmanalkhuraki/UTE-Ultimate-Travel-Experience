using Application.DTOs.SupportReply.Request;
using Application.DTOs.SupportReply.Response;
using Application.Interfaces.SupportReply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class SupportReplyController : ControllerBase
    {
        private readonly ISupportReplyService _supportReplyService;
        public SupportReplyController(ISupportReplyService supportReplyService)
        {
            _supportReplyService = supportReplyService;
        }

        /// <summary>
        /// Creates a new reply to a support ticket
        /// </summary>
        /// <param name="request">Support reply creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created support reply</returns>
        /// <response code="201">Returns the newly created support reply</response>
        /// <response code="400">If the request is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an Admin</response>
        /// <response code="404">If the referenced ticket is not found</response>
        /// <response code="422">If the ticket is already closed</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(SupportReplyResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SupportReplyResponse>> Create([FromBody] SupportReplyCreateRequest request, CancellationToken cancellationToken = default)
        {
            var created = await _supportReplyService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { ticketId = created.TicketId }, created);
        }

        /// <summary>
        /// Retrieves all replies for a specific ticket
        /// </summary>
        /// <param name="ticketId">The ticket ID to retrieve replies for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A list of replies for the specified ticket</returns>
        /// <response code="200">Returns the list of replies</response>
        /// <response code="400">If the ticket ID is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="403">If the user is not an Admin</response>
        /// <response code="404">If the ticket is not found</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("{ticketId:int}")]
        [ProducesResponseType(typeof(IReadOnlyList<SupportReplyResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IReadOnlyList<SupportReplyResponse>>> Get(int ticketId, CancellationToken cancellationToken = default)
        {
            return Ok(await _supportReplyService.GetAsync(ticketId, cancellationToken));
        }
    }
}
