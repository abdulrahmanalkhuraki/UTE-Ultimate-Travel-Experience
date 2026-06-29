namespace Domain.Entities;

/// <summary>
/// Join entity linking a <see cref="TourCompany"/> to a <see cref="TouristGuide"/>
/// that works for it (مرشدي الشركة). Many-to-many: a guide may work for several
/// companies, and a company has many guides.
/// </summary>
public partial class Company_TouristGuide : BaseEntity
{
    public int CompanyId { get; set; }

    public int TouristGuideId { get; set; }

    public virtual TourCompany Company { get; set; } = null!;

    public virtual TouristGuide TouristGuide { get; set; } = null!;
}
