using Application.DTOs.TourPackage.Response;

namespace Application.DTOs.Companion.Response
{
    public class CompanionResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Fullname { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public string? NationalityCountryName { get; set; }

        public int ResidentialCityId { get; set; }

        public string? ResidentialCityName { get; set; }

        public string Gender { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public int Age { get; set; }

        public string? NationalNumber { get; set; }

        public string? NationalIdCard { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportScan { get; set; }

        public string? ResidencyCard { get; set; }

        public string? ProfileImage { get; set; }

        public string Relationship { get; set; } = null!;

        public DateOnly RegistrationDate { get; set; }

        public int JoinedPackagesCount { get; set; }

        public decimal TotalAmountSpent { get; set; }

        public TourPackageResponse? LastTourPackage { get; set; }
    }
}
