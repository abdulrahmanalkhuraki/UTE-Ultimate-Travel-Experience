using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Hotel.Request
{
    public sealed record HotelUpdateRequest(
        string HotelName,
        string? Description,
        decimal Longitude,
        decimal Latitude,
        int CityId,
        int StarRating,
        decimal PricePerNight
        );
}
