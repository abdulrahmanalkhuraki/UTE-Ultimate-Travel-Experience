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

    public int PackageId { get; set; }

    public BookingStatus Status { get; set; }

    public BookingType BookingType { get; set; }

    public int UserId { get; set; }

    public int PaymentId { get; set; }

    public virtual Payment Payment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<CompanionBooking> CompanionBookings { get; set; } = new List<CompanionBooking>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
