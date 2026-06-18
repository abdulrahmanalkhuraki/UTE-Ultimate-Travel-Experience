using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public Domain.Enums.TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual SupportReply? SupportReply { get; set; }
    }
}
