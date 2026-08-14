using Microsoft.Extensions.Localization;

namespace Application.Common.Constants
{
    public static class ExceptionMessages
    {
        private static IStringLocalizer? _localizer;

        public static void Initialize(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        private static string Get(string name, params object[] arguments)
        {
            return _localizer is not null
                ? _localizer[name, arguments]
                : name;
        }

        public static string NotFound(string OperationObject, int ObjectId)
        {
            return Get("Exception_NotFound", OperationObject, ObjectId);
        }
        public static string Auth()
        {
            return Get("Exception_Auth");
        }
        public static string InvalidId(string OperationObject)
        {
            return Get("Exception_InvalidId", OperationObject);
        }
        public static string Forbidden(string OperationName, string OperationObject)
        {
            return Get("Exception_Forbidden", OperationName, OperationObject);
        }
        public static string ServiceException(string OperationName, string OperationObject, string ExceptionMessage)
        {
            return Get("Exception_ServiceException", OperationName, OperationObject, ExceptionMessage);
        }
        public static string BusinessRule(string message)
        {
            return message;
        }
        public static string Conflict(string OperationObject, string detail)
        {
            return Get("Exception_Conflict", OperationObject, detail);
        }
        public static string Concurrency(string OperationObject)
        {
            return Get("Exception_Concurrency", OperationObject);
        }
        public static string AuthFailure(string detail)
        {
            return Get("Exception_AuthFailure", detail);
        }
        public static string InvalidPagination()
        {
            return Get("Exception_InvalidPagination");
        }
    }
}
