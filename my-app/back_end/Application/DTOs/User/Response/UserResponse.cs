namespace Application.DTOs.User.Response
{
    public class UserResponse
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Image { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        public LocationResponse? CurrentLocation { get; set; }

        public int? NationalityCountryId { get; set; }

        public string? NationalityCountryName { get; set; }

        public int? ResidentialCityId { get; set; }

        public string? ResidentialCityName { get; set; }

        public string? NationalNumber { get; set; }

        public string? NationalIdImage { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportImage { get; set; }

        public string? BankAccount { get; set; }

        public int? RoleId { get; set; }

        public string? RoleName { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsProfileCompleted { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
