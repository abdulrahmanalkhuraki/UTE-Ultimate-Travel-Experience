namespace Application.DTOs.TourPackage.Response
{
    /// <summary>Lightweight guide info shown on a program (المرشد المختار).</summary>
    public class TourPackageGuideResponse
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string? ProfileImageUrl { get; set; }
    }
}
