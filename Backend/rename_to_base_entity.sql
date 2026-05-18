USE TourismTest;
SET NOCOUNT ON;
GO

PRINT '== Step 1: Rename PK columns to Id ==';

EXEC sp_rename 'Activities.ActivityId',                          'Id', 'COLUMN';
EXEC sp_rename 'AttractionActivities.AttractionActivityId',      'Id', 'COLUMN';
EXEC sp_rename 'AttractionCategories.AttractionCategoryId',      'Id', 'COLUMN';
EXEC sp_rename 'Attractions.AttractionId',                       'Id', 'COLUMN';
EXEC sp_rename 'Bookings.BookingId',                             'Id', 'COLUMN';
EXEC sp_rename 'Cities.CityId',                                  'Id', 'COLUMN';
EXEC sp_rename 'Countries.CountryId',                            'Id', 'COLUMN';
EXEC sp_rename 'CustomTrips.TripId',                             'Id', 'COLUMN';
EXEC sp_rename 'Favorites.FavoriteId',                           'Id', 'COLUMN';
EXEC sp_rename 'Flights.FlightId',                               'Id', 'COLUMN';
EXEC sp_rename 'Hotels.HotelId',                                 'Id', 'COLUMN';
EXEC sp_rename 'Images.ImageId',                                 'Id', 'COLUMN';
EXEC sp_rename 'Itineraries.ItineraryId',                        'Id', 'COLUMN';
EXEC sp_rename 'ItineraryAttractions.ItineraryAttractionId',     'Id', 'COLUMN';
EXEC sp_rename 'Notifications.NotificationId',                   'Id', 'COLUMN';
EXEC sp_rename 'PackageItineraries.ItineraryId',                 'Id', 'COLUMN';
EXEC sp_rename 'PackageItineraryAttractions.PackageItineraryAttractionId', 'Id', 'COLUMN';
EXEC sp_rename 'payments.PaymentId',                             'Id', 'COLUMN';
EXEC sp_rename 'Rates.RateId',                                   'Id', 'COLUMN';
EXEC sp_rename 'Reviews.ReviewId',                               'Id', 'COLUMN';
EXEC sp_rename 'Roles.RoleId',                                   'Id', 'COLUMN';
EXEC sp_rename 'TourCompanies.CompanyId',                        'Id', 'COLUMN';
EXEC sp_rename 'TourPackages.PackageId',                         'Id', 'COLUMN';
EXEC sp_rename 'Users.UserId',                                   'Id', 'COLUMN';
EXEC sp_rename 'Wishlists.WishlistId',                           'Id', 'COLUMN';

PRINT '== Step 2: Rename CreatedAt to CreatedAtUtc and UpdatedAt to UpdatedAtUtc ==';

DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 'EXEC sp_rename ''' + TABLE_NAME + '.CreatedAt'', ''CreatedAtUtc'', ''COLUMN'';' + CHAR(13)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME = 'CreatedAt' AND TABLE_NAME <> 'sysdiagrams';
EXEC sp_executesql @sql;

SET @sql = N'';
SELECT @sql += 'EXEC sp_rename ''' + TABLE_NAME + '.UpdatedAt'', ''UpdatedAtUtc'', ''COLUMN'';' + CHAR(13)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE COLUMN_NAME = 'UpdatedAt' AND TABLE_NAME <> 'sysdiagrams';
EXEC sp_executesql @sql;

PRINT '== Step 3: Add CreatedAtUtc and UpdatedAtUtc to tables that lack them ==';

DECLARE @addSql NVARCHAR(MAX);
DECLARE @tableName NVARCHAR(200);

DECLARE c CURSOR LOCAL FAST_FORWARD FOR
    SELECT t.name
    FROM sys.tables t
    WHERE t.name <> 'sysdiagrams'
      AND NOT EXISTS (
          SELECT 1 FROM sys.columns c
          WHERE c.object_id = t.object_id AND c.name = 'CreatedAtUtc'
      );
OPEN c;
FETCH NEXT FROM c INTO @tableName;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '  Adding CreatedAtUtc to ' + @tableName;
    SET @addSql = 'ALTER TABLE [' + @tableName + '] ADD CreatedAtUtc DATETIME NOT NULL CONSTRAINT [DF_' + @tableName + '_CreatedAtUtc] DEFAULT(GETUTCDATE());';
    EXEC sp_executesql @addSql;
    FETCH NEXT FROM c INTO @tableName;
END
CLOSE c;
DEALLOCATE c;

DECLARE c2 CURSOR LOCAL FAST_FORWARD FOR
    SELECT t.name
    FROM sys.tables t
    WHERE t.name <> 'sysdiagrams'
      AND NOT EXISTS (
          SELECT 1 FROM sys.columns c
          WHERE c.object_id = t.object_id AND c.name = 'UpdatedAtUtc'
      );
OPEN c2;
FETCH NEXT FROM c2 INTO @tableName;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '  Adding UpdatedAtUtc to ' + @tableName;
    SET @addSql = 'ALTER TABLE [' + @tableName + '] ADD UpdatedAtUtc DATETIME NOT NULL CONSTRAINT [DF_' + @tableName + '_UpdatedAtUtc] DEFAULT(GETUTCDATE());';
    EXEC sp_executesql @addSql;
    FETCH NEXT FROM c2 INTO @tableName;
END
CLOSE c2;
DEALLOCATE c2;

PRINT 'Migration completed.';
GO
