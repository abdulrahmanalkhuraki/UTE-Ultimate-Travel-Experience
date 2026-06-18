namespace Domain.Entities
{
    public partial class Person : BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? ProfileImage { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Gender { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string? NationalNumber { get; set; }

        public string? NationalIdCard { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportScan { get; set; }

        public int ResidentialCityId { get; set; }

        public int Age => CalculateAge();

        public string Fullname => FirstName + " " + LastName;

        public virtual City ResidentialCity { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual TouristGuide TouristGuide { get; set; } = null!;

        public virtual Companion Companion { get; set; } = null!;

        private int CalculateAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
