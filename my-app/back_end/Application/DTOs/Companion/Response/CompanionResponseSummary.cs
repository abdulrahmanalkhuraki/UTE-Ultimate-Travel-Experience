namespace Application.DTOs.Companion.Response
{
    public class CompanionResponseSummary
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string Relationship { get; set; } = null!;
        public int JoinedPackagesCount { get; set; }
        public string? ProfileImage { get; set; }
        public DateOnly RegistrationDate { get; set; }
    }
}
