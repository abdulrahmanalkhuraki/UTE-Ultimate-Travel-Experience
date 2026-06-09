using Application.DTOs.Companion.Response;
using Application.DTOs.Payment.Response;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking.Response
{
    public sealed class BookingResponse
    {
        public DateTime BookingDate { get; set; }

        public int NumberOfAdults { get; set; }

        public int NumberOfChildren { get; set; }

        public string? RoomTypePreference { get; set; }

        public string? DietaryRequirements { get; set; }

        public string? SpecialRequests { get; set; }

        public int PackageId { get; set; }

        public BookingStatus Status { get; set; }

        public FlightType FlightType { get; set; }

        public int UserId { get; set; }

        public PaymentResponse Payment { get; set; } = null!;

        public int Id { get; set; }

        public ICollection<CompanionResponse> Companions { get; set; } = new List<CompanionResponse>();
    }
}
