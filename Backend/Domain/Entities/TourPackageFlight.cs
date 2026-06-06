using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TourPackageFlight
    {
        public int Id { get; set; }
        public int TourPackageId { get; set; }
        public int FlightId { get; set; }
        public TourPackage TourPackage { get; set; } = null!;
        public Flight Flight { get; set; } = null!;
    }
}
