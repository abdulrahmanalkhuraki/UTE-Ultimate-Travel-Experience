using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Booking.Request
{
    public sealed record BookingApproveRequest
    (
        decimal? NewCalculatedCost
    );
}
