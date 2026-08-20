namespace Application.DTOs.City.Response
{
    public class AttractionResponse
    {
        public int Id { get; set; }
        public string EnAttractionName { get; set; } = null!;
        public string ArAttractionName { get; set; } = null!;
        public int AttractionCategoryId { get; set; }
        public string? Description { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int CityId { get; set; }
    }
}
