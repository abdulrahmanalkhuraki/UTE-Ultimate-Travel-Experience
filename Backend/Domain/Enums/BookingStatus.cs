
namespace Domain.Enums
{
    public enum BookingStatus
    {

        // Initial state after user submits booking request
        Pending,

        /* 
         * if there is any preferences the company should accept these preferences
         * it will also calculate the new price with preferences
         */
        Accepted_By_Company,

        // after the company calculate the new price, the tourist should accept the deal
        Accepted_By_Tourist,

        // after both company and tourist accept
        Confirmed,

        // Tour company has rejected user booking
        Rejected_By_Company,

        // Tourist has rejected new price
        Rejected_By_Tourist,

        // the service is currently being used
        In_Progress,

        // Service fully delivered (tour ended).
        Completed,

        // Booking voided before service starts
        Cancelled,

        // User did not use the service without prior cancellation
        No_Show
    }
}
