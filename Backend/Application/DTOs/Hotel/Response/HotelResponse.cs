using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Hotel.Response
{
    public sealed class HotelResponse
    {
        public int Id { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int StarRating { get; set; }
        public decimal PricePerNight { get; set; }
        public int CityId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
