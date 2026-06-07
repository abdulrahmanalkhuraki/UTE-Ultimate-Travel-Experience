namespace Application.DTOs.TourPackage
{
    /// <summary>
    /// Which slice of a company's own programs to list, matching the company
    /// dashboard tabs: الحالية / السابقة / الملغاة.
    /// </summary>
    public enum ProgramTimeline
    {
        /// <summary>Active programs that have not finished yet (الحالية): EndDate &gt;= today.</summary>
        Current,

        /// <summary>Active programs that have already finished (السابقة): EndDate &lt; today.</summary>
        Previous,

        /// <summary>Programs the company cancelled (الملغاة), regardless of dates.</summary>
        Cancelled
    }
}
