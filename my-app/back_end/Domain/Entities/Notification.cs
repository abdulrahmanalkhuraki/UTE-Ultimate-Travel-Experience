namespace Domain.Entities;

public partial class Notification : BaseEntity
{
    public string Message { get; set; } = null!;

    public int Type { get; set; }

    public bool IsRead { get; set; }

    public int UserId { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User User { get; set; } = null!;
}
