-- Creates the DeviceTokens table that stores each user's Firebase Cloud Messaging
-- registration tokens (one row per device). Used to push real-time notifications.

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DeviceTokens')
BEGIN
    CREATE TABLE dbo.DeviceTokens
    (
        Id            INT            IDENTITY(1,1) NOT NULL,
        UserId        INT            NOT NULL,
        Token         NVARCHAR(500)  NOT NULL,
        Platform      NVARCHAR(20)   NULL,
        CreatedAtUtc  DATETIME       NOT NULL CONSTRAINT DF_DeviceTokens_CreatedAtUtc DEFAULT (GETDATE()),
        UpdatedAtUtc  DATETIME       NOT NULL CONSTRAINT DF_DeviceTokens_UpdatedAtUtc DEFAULT (GETDATE()),
        CONSTRAINT PK_DeviceTokens PRIMARY KEY (Id),
        CONSTRAINT FK_DeviceTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_DeviceTokens_Token ON dbo.DeviceTokens (Token);
END
GO

PRINT 'Migration completed: DeviceTokens table applied.';
GO
