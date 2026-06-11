namespace Application.DTOs.City.Response
{
    public class CityResponse
    {
        public int Id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public int HotelCount { get; set; }
        public int AttractionCount { get; set; }
    }
}