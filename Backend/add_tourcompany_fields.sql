-- ============================================================
-- Migration: TourCompany "Company Information" fields
--   Adds the extra company-profile fields shown on the UI mockup
--   (location, phone, email, founding date, tourism license number
--    and image, bank account, and a long "About" description).
--
-- Safe to run multiple times: each column is added only if missing.
-- ============================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.TourCompanies', 'Location') IS NULL
    ALTER TABLE dbo.TourCompanies ADD Location NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'PhoneNumber') IS NULL
    ALTER TABLE dbo.TourCompanies ADD PhoneNumber NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'Email') IS NULL
    ALTER TABLE dbo.TourCompanies ADD Email NVARCHAR(75) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'FoundingDate') IS NULL
    ALTER TABLE dbo.TourCompanies ADD FoundingDate DATE NULL;

IF COL_LENGTH('dbo.TourCompanies', 'TourismLicenseNumber') IS NULL
    ALTER TABLE dbo.TourCompanies ADD TourismLicenseNumber NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'TourismLicenseImage') IS NULL
    ALTER TABLE dbo.TourCompanies ADD TourismLicenseImage NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'BankAccount') IS NULL
    ALTER TABLE dbo.TourCompanies ADD BankAccount NVARCHAR(100) NULL;

IF COL_LENGTH('dbo.TourCompanies', 'About') IS NULL
    ALTER TABLE dbo.TourCompanies ADD About NVARCHAR(2000) NULL;

COMMIT TRANSACTION;
GO

PRINT 'Migration completed: TourCompany company-information fields applied.';
GO
