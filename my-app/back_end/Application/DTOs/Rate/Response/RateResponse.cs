using Application.DTOs.TourPackage.Response;
using Application.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Rate.Response
{
    public class RateResponse
    {
        public int Id { get; set; }
        public int RateValue { get; set; }
        public UserResponse User { get; set; } = null!;
        public TourPackageResponse TourPackage { get; set; } = null!;
    }
}
