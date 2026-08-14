namespace Application.DTOs.Attraction.Response
{
    public class AttractionBriefResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int AttractionCategoryId { get; set; }
        public int CityId { get; set; }
    }
}
