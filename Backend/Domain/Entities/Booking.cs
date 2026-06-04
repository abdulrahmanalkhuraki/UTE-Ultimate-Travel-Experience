using Domain.Enums;

namespace Domain.Entities;

public partial class Booking : BaseEntity
{
    public DateTime BookingDate { get; set; }

    public int NumberOfAdults { get; set; }

    public int NumberOfChildren { get; set; }

    public BookingStatus Status { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public BookingType BookingType { get; set; }

    public int? PackageBookingId { get; set; }

    public int? HotelBookingId { get; set; }

    public int? FlightBookingId { get; set; }

    public virtual PackageBooking? PackageBooking { get; set; }

    public virtual HotelBooking? HotelBooking { get; set; }

    public virtual FlightBooking? FlightBooking { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
