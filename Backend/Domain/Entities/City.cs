namespace Domain.Entities;

public partial class City
{
    public int Id { get; set; }

    public string EnCityName { get; set; } = null!;

    public string? ArCityName { get; set; }

    public string? Image { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;
   
    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    // resedential city
    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
