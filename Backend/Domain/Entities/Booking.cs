using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Booking
{
    public DateTime BookingDate { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int NumberOfPeople { get; set; }

    public int Status { get; set; }

    public int? HotelId { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public int? FlightId { get; set; }

    public int? PackageId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int BookingId { get; set; }

    public virtual Flight? Flight { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual TourPackage? Package { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
