using System;

namespace Domain.Entities
{
    public partial class SupportReply
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AdminId { get; set; }
        public string ReplyContent { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User Admin { get; set; } = null!;
    }
}
