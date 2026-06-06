namespace Application.DTOs.TourPackage.Response
{
    /// <summary>A city/region visited by the program (منطقة).</summary>
    public class PackageCityResponse
    {
        public int CityId { get; set; }

        public string CityName { get; set; } = null!;
    }
}
