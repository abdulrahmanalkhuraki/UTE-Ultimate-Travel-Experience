using Application.DTOs.TourPackage.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TourCompany.Response
{
    public class TourCompanyDashboardResponse
    {
        public int NumberOfPackages { get; set; }
        // number of tourists that had join company Packages
        public int NumberOfTourists { get; set; }
        // number of Times that tourists had rate company packages
        public int NumberOfRates { get; set; }
        // number of Times that tourists had make review company packages
        public int NumberOfReviews { get; set; }
        // most wanted pakcages int this tour company
        public IReadOnlyList<TourPackageResponse> MostWantedPackages { get; set; } = new List<TourPackageResponse>();
    }
}
