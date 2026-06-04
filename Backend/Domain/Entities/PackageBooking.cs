using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class PackageBooking
    {
        public int Id { get; set; }
        public string? RoomTypePreference { get; set; }
        public string? DietaryRequirements {  get; set; }
        public string? SpecialRequests {  get; set; }
        public int PackageId { get; set; }
        public int BookingId { get; set; }
        public virtual TourPackage Package { get; set; } = null!;
        public virtual Booking Booking { get; set; } = null!;
    }
}
