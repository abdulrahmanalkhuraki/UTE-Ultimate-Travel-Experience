-- Adds the approval Status column to TourCompanies.
-- 0 = Pending, 1 = Approved, 2 = Rejected. New rows default to Pending (0).
-- Existing rows are left as Pending so an admin reviews them explicitly.

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'TourCompanies' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE dbo.TourCompanies
        ADD [Status] INT NOT NULL CONSTRAINT DF_TourCompanies_Status DEFAULT (0);
END
GO

PRINT 'Migration completed: TourCompanies.Status column applied.';
GO
