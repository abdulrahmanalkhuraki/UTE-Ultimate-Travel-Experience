using Domain.Enums;

namespace Application.Common
{
    /// <summary>
    /// Ready-to-display Arabic messages shown to a company owner for each approval state.
    /// Centralized so the API response and the notifications stay in sync.
    /// </summary>
    public static class TourCompanyStatusMessages
    {
        public const string Pending =
            "Your registration request has been successfully received and is currently under review by our team. We will notify you as soon as a decision is made. Thank you for your patience and understanding.";

        public const string Approved =
            "Good news! Your registration has been successfully approved. You can now log in to your account and start managing your services and their details.";

        public const string Rejected =
            "Update on your submission: Your registration request as a tour company has been reviewed and was not approved at this time.";

        public static string For(TourCompanyStatus status) => status switch
        {
            TourCompanyStatus.Approved => Approved,
            TourCompanyStatus.Rejected => Rejected,
            _ => Pending
        };
    }
}
