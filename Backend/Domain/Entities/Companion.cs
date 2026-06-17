using Domain.Enums;

namespace Domain.Entities
{
    public partial class Companion : BaseEntity
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int NationalityCountryId { get; set; }
        public int ResidentialCountryId { get; set; }
        public bool Gender {  get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int Age => CalculateAge();
        public string? NationalIdCard { get; set; } = null!;
        public string? NationalNumber { get; set; } = null!;
        public string? PassportScan { get; set; } = null!;
        public string? PassportNumber { get; set; } = null!;
        public string? ResidencyCard { get; set; } = null!;
        public int UserId { get; set; }
        public CompanionRelationship Relationship { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Country NationalityCountry { get; set; } = null!;
        public virtual Country ResidentialCountry { get; set; } = null!;
        public virtual ICollection<Companion_Booking> CompanionBookings { get; set; } = new List<Companion_Booking>();

        private int CalculateAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
