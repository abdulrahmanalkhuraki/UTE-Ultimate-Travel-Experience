using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class TouristGuide : BaseEntity
    {
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int NationalityCountryId { get; set; }
        public bool Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Languages { get; set; } = null!;
        public string IdCard { get; set; } = null!;
        public string PassportScan { get; set; } = null!;
        public string LicenseScan { get; set; } = null!;
        public string Bio {  get; set; } = null!;
        public bool IsAvailable { get; set; } 
        public int CityId { get; set; }
        public int YearsOfExperiance { get; set; }
        public ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();
        public virtual Country NatinalityCountry { get; set; } = null!;
        public virtual City City { get; set; } = null!;
    }
}
