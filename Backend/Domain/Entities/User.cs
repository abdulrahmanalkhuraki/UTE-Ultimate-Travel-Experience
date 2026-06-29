using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User : BaseEntity
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Latitude { get; set; }

    public string? BankAccount { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsDeleted { get; set; }

    public int? PersonId { get; set; }

    public virtual Person? Person { get; set; }

    public virtual TourCompany? TourCompany { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    
    public virtual ICollection<SupportReply> SupportReplies { get; set; } = new List<SupportReply>();

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Companion> Companions { get; set; } = new List<Companion>();
}
