namespace Domain.Enums
{
    /// <summary>
    /// Lifecycle state of a tour program. A program starts <see cref="Active"/> and
    /// stays active until the company cancels it (<see cref="Cancelled"/>). Whether an
    /// active program is "current" or "past" is derived from its dates, not stored here.
    /// </summary>
    public enum TourPackageStatus
    {
        Active = 0,
        Cancelled = 1
    }
}
