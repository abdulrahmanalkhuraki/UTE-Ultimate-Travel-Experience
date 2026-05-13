USE TourismTest;
GO

SET NOCOUNT ON;

-- 1) Capture all foreign keys so we can recreate them later
IF OBJECT_ID('tempdb..#FKs') IS NOT NULL DROP TABLE #FKs;

CREATE TABLE #FKs (
    Name           NVARCHAR(200),
    ParentTable    NVARCHAR(200),
    ParentCol      NVARCHAR(200),
    RefTable       NVARCHAR(200),
    RefCol         NVARCHAR(200),
    DeleteAction   TINYINT,
    UpdateAction   TINYINT
);

INSERT #FKs (Name, ParentTable, ParentCol, RefTable, RefCol, DeleteAction, UpdateAction)
SELECT
    fk.name,
    OBJECT_NAME(fk.parent_object_id),
    pc.name,
    OBJECT_NAME(fk.referenced_object_id),
    rc.name,
    fk.delete_referential_action,
    fk.update_referential_action
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns pc ON pc.object_id = fkc.parent_object_id AND pc.column_id = fkc.parent_column_id
INNER JOIN sys.columns rc ON rc.object_id = fkc.referenced_object_id AND rc.column_id = fkc.referenced_column_id;

-- 2) Drop all foreign keys
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql = @sql + 'ALTER TABLE [' + OBJECT_NAME(parent_object_id) + '] DROP CONSTRAINT [' + name + '];' + CHAR(13)
FROM sys.foreign_keys;
PRINT '---- Dropping FKs ----';
PRINT @sql;
EXEC sp_executesql @sql;

-- 3) Capture single-column PKs that are not IDENTITY (exclude sysdiagrams)
IF OBJECT_ID('tempdb..#PKs') IS NOT NULL DROP TABLE #PKs;

SELECT
    t.name      AS TableName,
    c.name      AS ColumnName,
    pk.name     AS PKName
INTO #PKs
FROM sys.tables t
INNER JOIN sys.indexes pk         ON pk.object_id = t.object_id AND pk.is_primary_key = 1
INNER JOIN sys.index_columns ic   ON ic.object_id = pk.object_id AND ic.index_id = pk.index_id
INNER JOIN sys.columns c          ON c.object_id = t.object_id AND c.column_id = ic.column_id
WHERE c.is_identity = 0 AND t.name <> 'sysdiagrams';

-- 4) For each PK column: drop PK, drop column, re-add as IDENTITY, re-add PK
DECLARE @sql2 NVARCHAR(MAX) = N'';
SELECT @sql2 = @sql2 +
       'ALTER TABLE [' + TableName + '] DROP CONSTRAINT [' + PKName + '];' + CHAR(13) +
       'ALTER TABLE [' + TableName + '] DROP COLUMN [' + ColumnName + '];' + CHAR(13) +
       'ALTER TABLE [' + TableName + '] ADD [' + ColumnName + '] INT IDENTITY(1,1) NOT NULL;' + CHAR(13) +
       'ALTER TABLE [' + TableName + '] ADD CONSTRAINT [PK_' + TableName + '] PRIMARY KEY ([' + ColumnName + ']);' + CHAR(13)
FROM #PKs;
PRINT '---- Rebuilding PKs as IDENTITY ----';
PRINT @sql2;
EXEC sp_executesql @sql2;

-- 5) Re-create foreign keys
DECLARE @sql3 NVARCHAR(MAX) = N'';
SELECT @sql3 = @sql3 +
       'ALTER TABLE [' + ParentTable + '] ADD CONSTRAINT [' + Name + '] ' +
       'FOREIGN KEY ([' + ParentCol + ']) REFERENCES [' + RefTable + '] ([' + RefCol + '])' +
       CASE DeleteAction WHEN 1 THEN ' ON DELETE CASCADE'
                         WHEN 2 THEN ' ON DELETE SET NULL'
                         WHEN 3 THEN ' ON DELETE SET DEFAULT'
                         ELSE '' END +
       CASE UpdateAction WHEN 1 THEN ' ON UPDATE CASCADE'
                         WHEN 2 THEN ' ON UPDATE SET NULL'
                         WHEN 3 THEN ' ON UPDATE SET DEFAULT'
                         ELSE '' END +
       ';' + CHAR(13)
FROM #FKs;
PRINT '---- Recreating FKs ----';
PRINT @sql3;
EXEC sp_executesql @sql3;

-- 6) Enlarge the Password column on Users to support hashed values (BCrypt ~60, Argon2 ~96, etc.)
ALTER TABLE [Users] ALTER COLUMN [Password] NVARCHAR(255) NOT NULL;

DROP TABLE #FKs;
DROP TABLE #PKs;

PRINT 'Schema fix completed.';
GO
