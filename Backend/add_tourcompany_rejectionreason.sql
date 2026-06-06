-- Adds the admin-written RejectionReason column to TourCompanies.
-- Populated only when a company is rejected; NULL otherwise.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TourCompanies' AND COLUMN_NAME = 'RejectionReason')
BEGIN
    ALTER TABLE dbo.TourCompanies
        ADD [RejectionReason] NVARCHAR(1000) NULL;
END
GO

PRINT 'Migration completed: TourCompanies.RejectionReason column applied.';
GO
