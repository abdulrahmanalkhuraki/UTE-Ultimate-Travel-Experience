using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum BookingStatus
    {
        /// <summary> 
        /// Initial state after user submits booking request but before payment or confirmation 
        /// </summary>
        Pending,

        /// <summary> 
        /// Payment received or booking officially reserved with supplier (hotel/airline/tour operator) 
        /// </summary>
        Confirmed,

        /// <summary> 
        /// the service is currently being used
        /// </summary>
        In_Progress,

        /// <summary> 
        /// Service fully delivered (flight taken, hotel stay finished, tour ended).
        /// </summary>
        Completed,

        /// <summary> 
        /// Booking voided before service starts
        /// </summary>
        Cancelled,

        /// <summary> 
        /// User did not use the service without prior cancellation
        /// </summary>
        No_Show
    }
}
