using Domain.Enums;

namespace Domain.Entities
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual SupportReply? SupportReply { get; set; }
    }
}
