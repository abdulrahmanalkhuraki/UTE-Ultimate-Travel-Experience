using Domain.Enums;

namespace Application.DTOs.TourPackage.Response
{
    /// <summary>A flight cabin class offered by a program (درجة طيران متاحة).</summary>
    public class TourPackageCabinClassResponse
    {
        public FlightCabinClass CabinClass { get; set; }

        public decimal Price { get; set; }

        public bool IsDefault { get; set; }

        /// <summary>Arabic label for the cabin class (الاسم بالعربي).</summary>
        public string Label => CabinClass switch
        {
            FlightCabinClass.Economy => "الدرجة الاقتصادية",
            FlightCabinClass.PremiumEconomy => "الدرجة السياحية المميزة",
            FlightCabinClass.Business => "درجة رجال الأعمال",
            FlightCabinClass.First => "الدرجة الأولى",
            _ => CabinClass.ToString()
        };
    }
}
