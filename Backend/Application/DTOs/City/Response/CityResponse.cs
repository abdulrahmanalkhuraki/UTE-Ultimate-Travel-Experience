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
        public ICollection<AttractionResponse> Attractions { get; set; } = new List<AttractionResponse>();
    }
}
