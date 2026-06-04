using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class HotelBooking
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int HotelId { get; set; }
        public int BookingId { get; set; }
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual Booking Booking { get; set;} = null!;
    }
}
