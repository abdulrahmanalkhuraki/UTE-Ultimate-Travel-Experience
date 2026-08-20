using Domain.Entities.Translations;

namespace Domain.Entities;

public partial class Country
{
    public int Id { get; set; }

    public string CountryCode { get; set; } = null!;

    public virtual ICollection<CountryTranslation> Translations { get; set; } = new List<CountryTranslation>();

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
