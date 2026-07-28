using Microsoft.Extensions.Logging;

namespace Application.Common.Logging
{
    public static partial class Log
    {
        [LoggerMessage(
            EventId = 1001,
            Level = LogLevel.Information,
            Message = "Attempting to {OperationName} {OperationObject} {ObjectId} by User {UserId}")]
        public static partial void StartOperation(
            this ILogger logger, string operationName, string operationObject, int objectId, int userId);

        [LoggerMessage(
            EventId = 1002,
            Level = LogLevel.Information,
            Message = "Attempting to {OperationName} {OperationObject} by User {UserId}")]
        public static partial void StartOperation(
            this ILogger logger, string operationName, string operationObject, int userId);

        [LoggerMessage(
            EventId = 1003,
            Level = LogLevel.Information,
            Message = "User {UserId} successfully {OperationName} {OperationObject} {ObjectId}.")]
        public static partial void SuccessfulOperation(
            this ILogger logger, int userId, string operationName, string operationObject, int objectId);

        [LoggerMessage(
            EventId = 1004,
            Level = LogLevel.Warning,
            Message = "{OperationObject} with Id {ObjectId} not found")]
        public static partial void EntityNotFound(
            this ILogger logger, string operationObject, int objectId);
        [LoggerMessage(
            EventId = 1005,
            Level = LogLevel.Warning,
            Message = "{OperationObject} {OperationName} validation failed for {ObjectId}: {Errors}")]
        public static partial void ValidationFailed(
            this ILogger logger, string operationName, string operationObject, int objectId, string errors);

        [LoggerMessage(
            EventId = 1006,
            Level = LogLevel.Warning,
            Message = "{OperationObject} {OperationName} validation failed: {Errors}")]
        public static partial void ValidationFailed(
            this ILogger logger, string operationName, string operationObject, string errors);

        [LoggerMessage(
            EventId = 1007,
            Level = LogLevel.Warning,
            Message = "User {UserId} is forbidden to {OperationName} {OperationObject} {ObjectId}")]
        public static partial void ForbiddenAction(
            this ILogger logger, int userId, string operationName, string operationObject, int objectId);

        [LoggerMessage(
            EventId = 1008,
            Level = LogLevel.Error,
            Message = "Unexpected error while {OperationName} {OperationObject}")]
        public static partial void ServerError(
            this ILogger logger, string operationName, string operationObject, Exception exception);

        [LoggerMessage(
            EventId = 1009,
            Level = LogLevel.Warning,
            Message = "User {UserId} authentication failed: {Detail}")]
        public static partial void AuthFailed(
            this ILogger logger, int userId, string detail);

        [LoggerMessage(
            EventId = 1010,
            Level = LogLevel.Warning,
            Message = "Authentication failed: {Detail}")]
        public static partial void AuthFailed(
            this ILogger logger, string detail);

        [LoggerMessage(
            EventId = 1011,
            Level = LogLevel.Warning,
            Message = "{OperationObject} business rule violated: {Rule}")]
        public static partial void BusinessRuleViolated(
            this ILogger logger, string operationObject, string rule);

        [LoggerMessage(
            EventId = 1012,
            Level = LogLevel.Warning,
            Message = "{OperationObject} conflict: {Detail}")]
        public static partial void ConflictDetected(
            this ILogger logger, string operationObject, string detail);

        [LoggerMessage(
            EventId = 1013,
            Level = LogLevel.Error,
            Message = "{OperationObject} concurrency conflict")]
        public static partial void ConcurrencyConflict(
            this ILogger logger, string operationObject);

        [LoggerMessage(
            EventId = 1014,
            Level = LogLevel.Warning,
            Message = "User {UserId} is forbidden to {OperationName} {OperationObject}")]
        public static partial void ForbiddenAction(
            this ILogger logger, int userId, string operationName, string operationObject);

        [LoggerMessage(
            EventId = 1015,
            Level = LogLevel.Information,
            Message = "Successfully {OperationName} {OperationObject}")]
        public static partial void SuccessfulOperation(
            this ILogger logger, string operationName, string operationObject);

    }
}
