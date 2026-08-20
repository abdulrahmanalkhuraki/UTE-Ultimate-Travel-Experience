namespace Domain.Enums
{
    /// <summary>
    /// Lifecycle state of a tour program. A new program starts <see cref="Pending"/>
    /// (awaiting admin approval). Once approved it becomes <see cref="Active"/>.
    /// When its end date passes it becomes <see cref="Completed"/>. A company can
    /// also <see cref="Cancelled"/> a program, and an admin can <see cref="Rejected"/> it.
    /// </summary>
    public enum TourPackageStatus
    {
        Pending = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 3,
        Rejected = 4
    }
}
