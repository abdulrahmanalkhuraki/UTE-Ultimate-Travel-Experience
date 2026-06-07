using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class Companion : BaseEntity
    {
        public int UserId { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int NationalityCountryId { get; set; }
        public int ResidentialCountryId { get; set; }
        public bool Gender {  get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string IdCard { get; set; } = null!;
        public string PassportScan { get; set; } = null!;
        public CompanionRelationship Relationship { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Country NationalityCountry { get; set; } = null!;
        public virtual Country ResidentialCountry { get; set; } = null!;
        public virtual ICollection<CompanionBooking> CompanionBookings { get; set; } = new List<CompanionBooking>();
    }
}
