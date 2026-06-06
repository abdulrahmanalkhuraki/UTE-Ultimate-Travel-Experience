-- ============================================================
-- Migration: Add Purpose column to EmailVerifications
--   Distinguishes between OTPs used for email verification
--   and OTPs used for password reset.
-- ============================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.EmailVerifications', 'Purpose') IS NULL
BEGIN
    ALTER TABLE dbo.EmailVerifications
        ADD Purpose NVARCHAR(30) NOT NULL
            CONSTRAINT DF_EmailVerifications_Purpose DEFAULT N'EmailVerification';
END;

COMMIT TRANSACTION;
GO

PRINT 'Migration completed: Purpose column added to EmailVerifications.';
GO
