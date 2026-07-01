namespace Domain.Entities;

public partial class Country
{
    public int Id { get; set; }

    public string EnCountryName { get; set; } = null!;

    public string? ArCountryName { get; set; }

    public string CountryCode { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
