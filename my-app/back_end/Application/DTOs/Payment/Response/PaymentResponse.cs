using Application.Common;
using Domain.Enums;

namespace Application.DTOs.Payment.Response
{
    public sealed class PaymentResponse
    {
        public decimal Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string StatusLabel => PaymentStatus.Humanize();

        public DateTime PaymentDate { get; set; }
    }
}
