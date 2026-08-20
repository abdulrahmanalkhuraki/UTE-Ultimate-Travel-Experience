using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class Companion_Booking
    {
        public int Id { get; set; }
        public int CompanionId { get; set; }
        public int BookingId { get; set; }
        public Companion Companion { get; set; } = null!;
        public Booking Booking { get; set; } = null!;
    }
}
