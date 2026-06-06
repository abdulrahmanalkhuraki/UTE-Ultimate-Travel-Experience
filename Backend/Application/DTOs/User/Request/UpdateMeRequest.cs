using Application.Common.Validation;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Request
{
    public class UpdateMeRequest
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string? LastName { get; set; }

        [RegularExpression(@"^[\+]?[\d\s\-\(\)]{6,20}$",
       ErrorMessage = "Phone number format is invalid.")]
        [StringLength(20)]
        public string? Phone { get; set; }


        [DateOfBirth(MinAge = 18, MaxAge = 120)]
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }


        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be either 'Male' or 'Female'.")]
        public string? Gender { get; set; }


        [StringLength(100, MinimumLength = 2, ErrorMessage = "Place of residence must be between 2 and 100 characters.")]
        public string? PlaceOfResidence { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Current location must be between 2 and 100 characters.")]
        public string? CurrentLocation { get; set; }


        [StringLength(50, MinimumLength = 4, ErrorMessage = "National number must be between 4 and 50 characters.")]
        public string? NationalNumber { get; set; }

        [StringLength(50, MinimumLength = 4, ErrorMessage = "Passport number must be between 4 and 50 characters.")]
        public string? PassportNumber { get; set; }

        [StringLength(100, MinimumLength = 4, ErrorMessage = "Bank account must be between 4 and 100 characters.")]
        public string? BankAccount { get; set; }


        public IFormFile? Image { get; set; }

        public IFormFile? NationalIdImage { get; set; }

        public IFormFile? PassportImage { get; set; }


        public string? CurrentPassword { get; set; }

        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
        public string? NewPassword { get; set; }
    }
}
