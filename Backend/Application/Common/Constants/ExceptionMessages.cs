namespace Application.Common.Constants
{
    public static class ExceptionMessages
    {
        public static string NotFound(string OperationObject, int ObjectId)
        {
            return $"{OperationObject} With Id {ObjectId} Not Found";
        }
        public static string Auth()
        {
            return "You Must Be Logged In To Perform This Action.";
        }
        public static string InvalidId(string OperationObject)
        {
            return $"Invalid {OperationObject} ID";
        }
        public static string Forbidden(string OperationName, string OperationObject)
        {
            return $"Access denied: You do not Have Permission To {OperationName} this {OperationObject}.";
        }
        public static string ServiceException(string OperationName, string OperationObject, string ExceptionMessage)
        {
            return $"Failed to {OperationName} {OperationObject}: {ExceptionMessage}";
        }
        public static string BusinessRule(string message)
        {
            return message;
        }
        public static string Conflict(string OperationObject, string detail)
        {
            return $"{OperationObject} conflict: {detail}";
        }
        public static string Concurrency(string OperationObject)
        {
            return $"{OperationObject} was modified by another user. Please reload and try again.";
        }
        public static string AuthFailure(string detail)
        {
            return $"Authentication failed: {detail}";
        }
        public static string InvalidPagination()
        {
            return "Page must be >= 1, PageSize must be between 1 and 100.";
        }
    }
}
