USE TourismTest;
SET NOCOUNT ON;
GO

PRINT '== Step 1: Drop Is_Approved from Users ==';
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Users_IsApproved')
    ALTER TABLE Users DROP CONSTRAINT DF_Users_IsApproved;

IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Is_Approved')
    ALTER TABLE Users DROP COLUMN Is_Approved;

PRINT '== Step 2: Add IsEmailVerified to Users ==';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsEmailVerified')
    ALTER TABLE Users ADD IsEmailVerified BIT NOT NULL
        CONSTRAINT DF_Users_IsEmailVerified DEFAULT(0);

PRINT '== Step 3: Create EmailVerifications table ==';
IF OBJECT_ID('EmailVerifications', 'U') IS NULL
BEGIN
    CREATE TABLE EmailVerifications (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId       INT NOT NULL,
        Code         NVARCHAR(10) NOT NULL,
        ExpiresAt    DATETIME NOT NULL,
        Attempts     INT NOT NULL CONSTRAINT DF_EmailVerifications_Attempts DEFAULT(0),
        IsUsed       BIT NOT NULL CONSTRAINT DF_EmailVerifications_IsUsed DEFAULT(0),
        UsedAt       DATETIME NULL,
        CreatedAtUtc DATETIME NOT NULL CONSTRAINT DF_EmailVerifications_CreatedAtUtc DEFAULT(GETUTCDATE()),
        UpdatedAtUtc DATETIME NOT NULL CONSTRAINT DF_EmailVerifications_UpdatedAtUtc DEFAULT(GETUTCDATE()),
        CONSTRAINT FK_EmailVerifications_Users FOREIGN KEY (UserId)
            REFERENCES Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_EmailVerifications_UserId ON EmailVerifications(UserId);
    CREATE INDEX IX_EmailVerifications_UserId_IsUsed ON EmailVerifications(UserId, IsUsed);
END

PRINT 'Migration completed.';
GO
