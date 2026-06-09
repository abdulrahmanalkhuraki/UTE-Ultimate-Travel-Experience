using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities;

public partial class TourCompany : BaseEntity
{
    public string Name { get; set; } = null!;

    /// <summary>Approval state. New companies start as <see cref="TourCompanyStatus.Pending"/>.</summary>
    public TourCompanyStatus Status { get; set; } = TourCompanyStatus.Pending;

    /// <summary>Admin-written reason, set only when the company is rejected; otherwise null.</summary>
    public string? RejectionReason { get; set; }

    public string? Description { get; set; }

    public string? Logo { get; set; }

    public string? Location { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public DateOnly? FoundingDate { get; set; }

    public string? TourismLicenseNumber { get; set; }

    public string? TourismLicenseImage { get; set; }

    public string? BankAccount { get; set; }

    public string? About { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    /// <summary>Guides that work for this company (مرشدو الشركة).</summary>
    public virtual ICollection<CompanyGuide> CompanyGuides { get; set; } = new List<CompanyGuide>();

    public virtual User User { get; set; } = null!;
}
