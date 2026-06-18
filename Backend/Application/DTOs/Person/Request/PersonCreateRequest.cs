using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Person.Request
{
    public sealed record PersonCreateRequest
    (
        string FirstName,
        string LastName,
        IFormFile? ProfileImage,
        DateOnly DateOfBirth,
        string Gender,
        string Phone,
        string? NationalNumber,
        IFormFile? NationalIdCard,
        string? PassportNumber,
        IFormFile? PassportScan,
        int ResidentialCityId
        );
}
