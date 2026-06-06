namespace Domain.Enums
{
    /// <summary>
    /// Approval state of a tour company. A newly created company starts as
    /// <see cref="Pending"/> and is only publicly visible once an admin sets it to
    /// <see cref="Approved"/>.
    /// </summary>
    public enum TourCompanyStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
