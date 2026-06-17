using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public sealed class Person : BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? ProfileImage { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int Age => CalculateAge();

        public string Gender { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string? NationalNumber { get; set; }

        public string? NationalIdCard { get; set; }

        public string? PassportNumber { get; set; }

        public string? PassportScan { get; set; }

        public string Fullname => FirstName + " " + LastName;

        private int CalculateAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
