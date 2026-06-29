namespace Application.DTOs.Favorite.Response
{
    public class FavoriteResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? CompanyLogo { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
