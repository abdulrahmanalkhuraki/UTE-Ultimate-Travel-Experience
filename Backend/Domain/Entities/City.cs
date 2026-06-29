namespace Domain.Entities;

public partial class City
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public string? Image { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;
   
    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();

    public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
}
