namespace Application.DTOs.Favorite.Response
{
    public class FavoriteResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyDescription { get; set; }
        public string? CompanyLogo { get; set; }
        public int NumberOfTourists { get; set; }
        public int NumberOfPackages { get; set; }
        public double Rate { get; set; }
    }
}
