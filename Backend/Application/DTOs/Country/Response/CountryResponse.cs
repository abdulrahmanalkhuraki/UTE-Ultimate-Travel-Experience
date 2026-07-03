using Application.DTOs.City.Response;


namespace Application.DTOs.Country.Response
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string EnCountryName { get; set; } = null!;
        public string ArCountryName { get; set; } = null!;
        public string CountryCode { get; set; } = null!;
        public virtual ICollection<CityResponse> Cities { get; set; } = new List<CityResponse>();
    }
}
