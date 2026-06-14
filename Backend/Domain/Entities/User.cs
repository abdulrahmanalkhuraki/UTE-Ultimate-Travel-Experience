using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class User : BaseEntity
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Image { get; set; }

    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Fullname => FirstName + " " + LastName;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PlaceOfResidence { get; set; }

    public string? CurrentLocation { get; set; }

    public string? NationalNumber { get; set; }

    public string? NationalIdImage { get; set; }

    public string? PassportNumber { get; set; }

    public string? PassportImage { get; set; }

    public string? BankAccount { get; set; }

    public bool IsProfileCompleted { get; set; }

    public bool IsEmailVerified { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<TourCompany> TourCompanies { get; set; } = new List<TourCompany>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Companion> Companions { get; set; } = new List<Companion>();
}
