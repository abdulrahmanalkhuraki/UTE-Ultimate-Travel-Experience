/* =====================================================================
   TourPackage feature – schema upgrade to match the "create program" UI
   Database-first project (no EF migrations). Run once against TourismTest.
   All affected tables are empty, so the changes are non-destructive in
   practice.
   ===================================================================== */
USE TourismTest;
GO

/* ---------------------------------------------------------------------
   1) TourPackages – new columns
      البلد، صورة رئيسية، تواريخ، دليل سياحي، عملة، حالة النشر
   --------------------------------------------------------------------- */
ALTER TABLE dbo.TourPackages ADD
    CountryId            INT           NOT NULL CONSTRAINT DF_TourPackages_CountryId DEFAULT(0),
    MainImageUrl         NVARCHAR(500) NULL,
    StartDate            DATE          NOT NULL CONSTRAINT DF_TourPackages_StartDate DEFAULT('2000-01-01'),
    EndDate              DATE          NOT NULL CONSTRAINT DF_TourPackages_EndDate   DEFAULT('2000-01-01'),
    RegistrationDeadline DATE          NOT NULL CONSTRAINT DF_TourPackages_RegDeadline DEFAULT('2000-01-01'),
    TourGuide            NVARCHAR(150) NULL,
    Currency             NVARCHAR(10)  NOT NULL CONSTRAINT DF_TourPackages_Currency  DEFAULT(N'USD'),
    IsPublished          BIT           NOT NULL CONSTRAINT DF_TourPackages_IsPublished DEFAULT(0);
GO

-- Drop the helper defaults again; the app always supplies these values.
ALTER TABLE dbo.TourPackages DROP CONSTRAINT DF_TourPackages_CountryId;
ALTER TABLE dbo.TourPackages DROP CONSTRAINT DF_TourPackages_StartDate;
ALTER TABLE dbo.TourPackages DROP CONSTRAINT DF_TourPackages_EndDate;
ALTER TABLE dbo.TourPackages DROP CONSTRAINT DF_TourPackages_RegDeadline;
GO

ALTER TABLE dbo.TourPackages
    ADD CONSTRAINT FK_TourPackages_Countries
        FOREIGN KEY (CountryId) REFERENCES dbo.Countries(Id);
GO

/* ---------------------------------------------------------------------
   2) PackageCities – join table for "المناطق اللي رح تنزار"
      (many-to-many TourPackage <-> City)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.PackageCities', 'U') IS NULL
CREATE TABLE dbo.PackageCities
(
    Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PackageCities PRIMARY KEY,
    PackageId    INT      NOT NULL,
    CityId       INT      NOT NULL,
    CreatedAtUtc DATETIME NOT NULL CONSTRAINT DF_PackageCities_CreatedAtUtc DEFAULT(getdate()),
    UpdatedAtUtc DATETIME NOT NULL CONSTRAINT DF_PackageCities_UpdatedAtUtc DEFAULT(getdate()),
    CONSTRAINT FK_PackageCities_TourPackages FOREIGN KEY (PackageId)
        REFERENCES dbo.TourPackages(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PackageCities_Cities FOREIGN KEY (CityId)
        REFERENCES dbo.Cities(Id),
    CONSTRAINT UQ_PackageCities_Package_City UNIQUE (PackageId, CityId)
);
GO

/* ---------------------------------------------------------------------
   3) PackageItineraries – short description for each day
      "شرح مختصر عن هذا اليوم"
   --------------------------------------------------------------------- */
IF COL_LENGTH('dbo.PackageItineraries', 'DayDescription') IS NULL
    ALTER TABLE dbo.PackageItineraries ADD DayDescription NVARCHAR(500) NULL;
GO

/* ---------------------------------------------------------------------
   4) PackageItineraryAttractions – repurpose into a free-text ACTIVITY
      Was: link to an existing Attraction (AttractionId) + single Time.
      Now: typed title/description/image + start & end time.
   --------------------------------------------------------------------- */
-- Drop the FK to Attractions (name is auto-generated, so look it up).
DECLARE @fkName SYSNAME;
SELECT @fkName = fk.name
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE fk.parent_object_id = OBJECT_ID('dbo.PackageItineraryAttractions')
  AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'AttractionId';
IF @fkName IS NOT NULL
    EXEC('ALTER TABLE dbo.PackageItineraryAttractions DROP CONSTRAINT ' + @fkName);
GO

IF COL_LENGTH('dbo.PackageItineraryAttractions', 'AttractionId') IS NOT NULL
    ALTER TABLE dbo.PackageItineraryAttractions DROP COLUMN AttractionId;
GO
IF COL_LENGTH('dbo.PackageItineraryAttractions', 'Time') IS NOT NULL
    ALTER TABLE dbo.PackageItineraryAttractions DROP COLUMN [Time];
GO

ALTER TABLE dbo.PackageItineraryAttractions ADD
    Title       NVARCHAR(100) NOT NULL CONSTRAINT DF_PIA_Title DEFAULT(N''),
    Description NVARCHAR(500) NULL,
    ImageUrl    NVARCHAR(500) NULL,
    StartTime   TIME(0)       NOT NULL CONSTRAINT DF_PIA_StartTime DEFAULT('00:00:00'),
    EndTime     TIME(0)       NOT NULL CONSTRAINT DF_PIA_EndTime   DEFAULT('00:00:00');
GO

ALTER TABLE dbo.PackageItineraryAttractions DROP CONSTRAINT DF_PIA_Title;
ALTER TABLE dbo.PackageItineraryAttractions DROP CONSTRAINT DF_PIA_StartTime;
ALTER TABLE dbo.PackageItineraryAttractions DROP CONSTRAINT DF_PIA_EndTime;
GO

/* ---------------------------------------------------------------------
   5) Seed a few Countries & Cities so CountryId/CityId are usable.
      Idempotent: only runs when the tables are still empty.
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Countries)
BEGIN
    INSERT INTO dbo.Countries (CountryName, CountryCode, Flag, CreatedAtUtc, UpdatedAtUtc) VALUES
        (N'Jordan',               'JO', NULL, getdate(), getdate()),
        (N'Turkey',               'TR', NULL, getdate(), getdate()),
        (N'Egypt',                'EG', NULL, getdate(), getdate()),
        (N'United Arab Emirates', 'AE', NULL, getdate(), getdate());
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Cities)
BEGIN
    INSERT INTO dbo.Cities (CityName, Description, Image, CountryId, CreatedAtUtc, UpdatedAtUtc)
    SELECT v.CityName, NULL, NULL, c.Id, getdate(), getdate()
    FROM (VALUES
            (N'Amman',           N'Jordan'),
            (N'Petra',           N'Jordan'),
            (N'Aqaba',           N'Jordan'),
            (N'Istanbul',        N'Turkey'),
            (N'Antalya',         N'Turkey'),
            (N'Cairo',           N'Egypt'),
            (N'Sharm El Sheikh', N'Egypt'),
            (N'Dubai',           N'United Arab Emirates'),
            (N'Abu Dhabi',       N'United Arab Emirates')
         ) AS v(CityName, CountryName)
    JOIN dbo.Countries c ON c.CountryName = v.CountryName;
END
GO

PRINT 'TourPackage schema upgrade completed.';
GO
