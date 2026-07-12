namespace Domain.Enums
{
    /// <summary>
    /// Kind of notification, stored as an int on <see cref="Domain.Entities.Notification.Type"/>.
    /// </summary>
    public enum NotificationType
    {
        General,
        CompanyApproved,
        CompanyRejected,
        PackageAccepted,
        PackageRejected,
        NewBooking,
        BookingApproved, // booking approved by company
        BookingRejected, // booking rejected by company
        BookingConfirmed, // by tourist
        BookingDeclined, // by tourist
        NewPackage,
        BookingStartingSoon,
        PriceDrop,
        RegistrationDeadlineReminder,
    }
}
