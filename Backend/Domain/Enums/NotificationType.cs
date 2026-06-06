namespace Domain.Enums
{
    /// <summary>
    /// Kind of notification, stored as an int on <see cref="Domain.Entities.Notification.Type"/>.
    /// </summary>
    public enum NotificationType
    {
        General = 0,
        CompanyApproved = 1,
        CompanyRejected = 2
    }
}
