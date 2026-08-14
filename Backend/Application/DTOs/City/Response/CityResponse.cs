using Application.DTOs.Attraction.Response;

namespace Application.DTOs.City.Response
{
    public class CityResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public virtual ICollection<AttractionBriefResponse> Attractions { get; set; } = new List<AttractionBriefResponse>();
    }
}
