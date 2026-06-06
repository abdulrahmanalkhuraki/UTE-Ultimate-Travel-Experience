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
            "قمنا بإرسال طلبك إلى مدير التطبيق وبانتظار موافقته على طلبك. سنقوم بإعلامك مباشرة عند الاجابة ونشكرك على حسن انتظارك.";

        public const string Approved =
            "تم قبول طلبك من قبل مدير التطبيق. نتطلع لنشرك المزيد من البرامج. يمكنك رؤية المعلومات الخاصة في البرنامج من تبويبة برامجي في الشريط السفلي.";

        public const string Rejected =
            "تم رفض طلبك من قبل مدير التطبيق.";

        public static string For(TourCompanyStatus status) => status switch
        {
            TourCompanyStatus.Approved => Approved,
            TourCompanyStatus.Rejected => Rejected,
            _ => Pending
        };
    }
}
