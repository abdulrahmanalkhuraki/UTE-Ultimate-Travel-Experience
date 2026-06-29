using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Ticket.Request
{
    public sealed record TicketCreateRequest
    {
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IFormFile? Image { get; set; }
    }
}
