using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class FlightBooking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int BookingId { get; set; }
        public virtual Flight Flight { get; set; } = null!;
        public virtual Booking Booking { get; set; } = null!;
    }
}
