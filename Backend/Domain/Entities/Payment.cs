using Domain.Enums;

namespace Domain.Entities;

public partial class Payment : BaseEntity
{
    public decimal Amount { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime PaymentDate { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Booking Booking { get; set; } = null!;
}
