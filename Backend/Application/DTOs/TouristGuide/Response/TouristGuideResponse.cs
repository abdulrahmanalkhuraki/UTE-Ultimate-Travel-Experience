namespace Application.DTOs.TouristGuide.Response
{
    public class TouristGuideResponse
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public string? NationalityCountryName { get; set; }

        public string Gender { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public int YearsOfExperiance { get; set; }

        public string Bio { get; set; } = null!;

        public int ResidentialCityId { get; set; }

        public string? ResidentialCityName { get; set; }

        public string? NationalNumber { get; set; }

        public string? PassportNumber { get; set; }

        public string? Languages { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? NationalIdCard { get; set; }

        public string? PassportScan { get; set; }

        public bool IsAvailable { get; set; }

        public int? LastTourPackageId { get; set; }

        public int NumberOfPackagesGuided { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
