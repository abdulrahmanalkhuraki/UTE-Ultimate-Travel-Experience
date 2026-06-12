using Domain.Enums;

namespace Domain.Entities;

public partial class Booking : BaseEntity
{
    public DateTime BookingDate { get; set; }

    public int NumberOfAdults { get; set; }

    public int NumberOfChildren { get; set; }

    public string? RoomTypePreference { get; set; }

    public string? DietaryRequirements { get; set; }

    public string? SpecialRequests { get; set; }
    // if the company reject the tourist booking it should provide reject reason
    public string? RejectReason { get; set; }

    public int TourPackageId { get; set; }

    public BookingStatus Status { get; set; }

    public FlightCabinClass FlightCabinClass { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual TourPackage TourPackage { get; set; } = null!;

    public virtual ICollection<CompanionBooking> CompanionBookings { get; set; } = new List<CompanionBooking>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
