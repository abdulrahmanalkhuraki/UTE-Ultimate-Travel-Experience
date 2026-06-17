using Application.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Companion.Response
{
    public class CompanionResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Firstname { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public int NationalityCountryId { get; set; }

        public string? NationalityCountryName { get; set; }

        public int ResidentialCountryId { get; set; }

        public string? ResidentialCountryName { get; set; }

        public bool Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string? IdCard { get; set; }

        public string? PassportScan { get; set; }

        public string Relationship { get; set; } = null!;

        public DateOnly RegistrationDate { get; set; }

        public int? LastTripPackageId { get; set; }

        public int JoinedPackagesCount { get; set; }

        public decimal TotalAmountSpent { get; set; }

        public virtual UserResponse User { get; set; } = null!;
    }
}
