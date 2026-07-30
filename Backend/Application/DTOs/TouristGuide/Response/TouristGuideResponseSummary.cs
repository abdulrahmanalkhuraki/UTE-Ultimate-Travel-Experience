namespace Application.DTOs.TouristGuide.Response
{
    public class TouristGuideResponseSummary
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string Phone { get; set; } = null!;
        public int YearsOfExperiance { get; set; }
        public int NumberOfPackagesGuided { get; set; }
        public string? ProfileImage { get; set; }
        public DateOnly RegistrationDate { get; set; }
    }
}
