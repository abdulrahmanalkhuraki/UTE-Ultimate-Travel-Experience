namespace Domain.Enums
{
    /// <summary>
    /// Admin moderation state of a tour program, separate from its lifecycle
    /// <see cref="TourPackageStatus"/>. A new program starts <see cref="Pending"/>
    /// and an admin then <see cref="Accepted"/> (المقبولة) or <see cref="Rejected"/> (المرفوضة) it.
    /// </summary>
    public enum PackageApprovalStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}
