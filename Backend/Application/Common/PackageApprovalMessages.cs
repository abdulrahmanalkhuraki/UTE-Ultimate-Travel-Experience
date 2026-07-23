using Domain.Enums;

namespace Application.Common
{
    /// <summary>
    /// Ready-to-display Arabic messages sent to a company when an admin moderates
    /// one of its packages. Centralized so notifications stay consistent.
    /// </summary>
    public static class PackageApprovalMessages
    {
        public const string Accepted =
            "Good news! Your tour package has been successfully approved. You can view and manage all its details in the 'My Programs' tab.";

        public const string Rejected =
            "Thank you for your submission. Unfortunately, your tour package was not approved after review. You can check the reasons and view more details in the 'My Programs' tab.";

        public static string For(TourPackageStatus status) => status switch
        {
            TourPackageStatus.Active => Accepted,
            TourPackageStatus.Rejected => Rejected,
            _ => string.Empty
        };
    }
}
