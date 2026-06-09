using Domain.Enums;

namespace Application.Common
{
    /// <summary>
    /// Ready-to-display Arabic messages sent to a company when an admin moderates
    /// one of its programs. Centralized so notifications stay consistent.
    /// </summary>
    public static class ProgramApprovalMessages
    {
        public const string Accepted =
            "تم قبول برنامجك من قبل مدير التطبيق وأصبح متاحاً للنشر. يمكنك متابعة تفاصيله من تبويبة برامجي.";

        public const string Rejected =
            "تم رفض برنامجك من قبل مدير التطبيق.";

        public static string For(ProgramApprovalStatus status) => status switch
        {
            ProgramApprovalStatus.Accepted => Accepted,
            ProgramApprovalStatus.Rejected => Rejected,
            _ => string.Empty
        };
    }
}
