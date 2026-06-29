using Application.DTOs.User.Response;
using Domain.Enums;

namespace Application.DTOs.Ticket.Response
{
    public class TicketResponse
    {
        public int Id { get; set; }
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}
