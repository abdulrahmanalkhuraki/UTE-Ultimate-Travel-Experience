namespace Application.DTOs.TourPackage.Response
{
    /// <summary>
    /// Aggregate counts for a company's own programs, powering the dashboard
    /// stats card (إحصائيات البرامج). Counts are independent cuts and may overlap.
    /// </summary>
    public class CompanyProgramStatsResponse
    {
        /// <summary>Total number of programs owned by the company (عدد البرامج الكلية).</summary>
        public int Total { get; set; }

        /// <summary>Active programs that have not finished yet (البرامج الحالية).</summary>
        public int Current { get; set; }

        /// <summary>Programs an admin has accepted (البرامج المقبولة).</summary>
        public int Accepted { get; set; }

        /// <summary>Programs the company cancelled (البرامج الملغاة).</summary>
        public int Cancelled { get; set; }

        /// <summary>Programs an admin has rejected (البرامج المرفوضة).</summary>
        public int Rejected { get; set; }
    }
}
