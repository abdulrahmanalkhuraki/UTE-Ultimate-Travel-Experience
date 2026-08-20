namespace Domain.Entities.Translations;

public partial class TourCompanyTranslation : EntityTranslation
{
    public int CompanyId { get; set; }

    public string? Description { get; set; }

    public string? About { get; set; }

    public virtual TourCompany Company { get; set; } = null!;
}
