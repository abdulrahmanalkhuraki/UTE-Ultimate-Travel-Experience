-- ============================================================
-- Migration: Two-step registration support
--   Step 1: Register with Email/Password/ConfirmPassword only
--   Step 2: Complete profile (full personal info) via JWT
--
-- Changes:
--   1) Relax NOT NULL on previously-required fields so they
--      can be filled in later via complete-profile endpoint
--   2) Add new profile fields shown on the UI mockup
--   3) Drop and recreate the Users.RoleId FK to allow ON DELETE SET NULL
-- ============================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ----------------------------------------------------------------
-- 1) Drop existing FK on RoleId so we can change its nullability
--    and recreate the FK with ON DELETE SET NULL semantics.
-- ----------------------------------------------------------------
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = 'FK__Users__RoleId__4F7CD00D'
)
BEGIN
    ALTER TABLE dbo.Users DROP CONSTRAINT [FK__Users__RoleId__4F7CD00D];
END;

-- ----------------------------------------------------------------
-- 2) Make previously-required columns nullable
-- ----------------------------------------------------------------
ALTER TABLE dbo.Users ALTER COLUMN FirstName     NVARCHAR(50) NULL;
ALTER TABLE dbo.Users ALTER COLUMN LastName      NVARCHAR(50) NULL;
ALTER TABLE dbo.Users ALTER COLUMN Date_Of_Birth DATE         NULL;
ALTER TABLE dbo.Users ALTER COLUMN RoleId        INT          NULL;

-- ----------------------------------------------------------------
-- 3) Add new profile fields
-- ----------------------------------------------------------------
IF COL_LENGTH('dbo.Users', 'Gender') IS NULL
    ALTER TABLE dbo.Users ADD Gender NVARCHAR(10) NULL;

IF COL_LENGTH('dbo.Users', 'PlaceOfResidence') IS NULL
    ALTER TABLE dbo.Users ADD PlaceOfResidence NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.Users', 'CurrentLocation') IS NULL
    ALTER TABLE dbo.Users ADD CurrentLocation NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.Users', 'NationalNumber') IS NULL
    ALTER TABLE dbo.Users ADD NationalNumber NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.Users', 'NationalIdImage') IS NULL
    ALTER TABLE dbo.Users ADD NationalIdImage NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.Users', 'PassportNumber') IS NULL
    ALTER TABLE dbo.Users ADD PassportNumber NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.Users', 'PassportImage') IS NULL
    ALTER TABLE dbo.Users ADD PassportImage NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.Users', 'BankAccount') IS NULL
    ALTER TABLE dbo.Users ADD BankAccount NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.Users', 'IsProfileCompleted') IS NULL
    ALTER TABLE dbo.Users ADD IsProfileCompleted BIT NOT NULL CONSTRAINT DF_Users_IsProfileCompleted DEFAULT (0);

-- ----------------------------------------------------------------
-- 4) Recreate the Users.RoleId FK (SET NULL on role deletion now
--    that the column is nullable).
-- ----------------------------------------------------------------
ALTER TABLE dbo.Users
    ADD CONSTRAINT [FK__Users__RoleId__4F7CD00D]
    FOREIGN KEY (RoleId) REFERENCES dbo.Roles (Id)
    ON DELETE SET NULL;

COMMIT TRANSACTION;
GO

PRINT 'Migration completed: two-step registration profile fields applied.';
GO
