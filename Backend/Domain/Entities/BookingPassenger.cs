using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class BookingPassenger
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = null!;
        public int Age { get; set; }
        public IdentityType? IdentityType { get; set; }
        public string? IdentityDocumentPath { get; set; } = null!;
        public int NatinalityCountryID { get; set; }
        public int BookingID { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public virtual Country Country { get; set; } = null!;
    }
}
