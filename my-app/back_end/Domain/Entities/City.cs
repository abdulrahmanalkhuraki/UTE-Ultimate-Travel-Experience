using Domain.Entities.Translations;

namespace Domain.Entities;

public partial class City
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<CityTranslation> Translations { get; set; } = new List<CityTranslation>();

    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    // resedential city
    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
