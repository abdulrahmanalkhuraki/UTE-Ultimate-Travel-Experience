using Domain.Entities;
using System.Text.Json.Serialization;

namespace Application.DTOs.City.Response
{
    public class CityResponse
    {
        public int Id { get; set; }
        public string EnCityName { get; set; } = null!;
        public string? ArCityName { get; set; }
        public string? Image { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
    }
}