using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public string? PlaceOfResidence { get; set; }

        public string? CurrentLocation { get; set; }

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
