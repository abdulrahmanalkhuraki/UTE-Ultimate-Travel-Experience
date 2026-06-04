using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public partial class TourPackageHotel
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int HotelId { get; set; }
        public int TourPackageId { get; set; }
        public virtual TourPackage TourPackage { get; set; } = null!;
        public virtual Hotel Hotel { get; set; } = null!;
    }
}
