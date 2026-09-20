/*
    Adds cancellation policy columns to Listings/Listing and expands ListingAccessInstructions.Instructions to nvarchar(max).
    Idempotent: safe to run repeatedly.
*/

DECLARE @ListingsTable sysname =
    CASE
        WHEN OBJECT_ID(N'[Listings]', N'U') IS NOT NULL THEN N'[Listings]'
        WHEN OBJECT_ID(N'[Listing]', N'U') IS NOT NULL THEN N'[Listing]'
        ELSE NULL
    END;

IF @ListingsTable IS NULL
BEGIN
    RAISERROR('Neither [Listings] nor [Listing] table exists in this database.', 16, 1);
    RETURN;
END;

DECLARE @ListingsName sysname = REPLACE(REPLACE(@ListingsTable, '[', ''), ']', '');
DECLARE @sql nvarchar(max);

IF COL_LENGTH(@ListingsName, 'ShortTermCancellationPolicy') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingsTable + N' ADD [ShortTermCancellationPolicy] NVARCHAR(50) NULL;';
    EXEC sp_executesql @sql;
END;

IF COL_LENGTH(@ListingsName, 'LongTermCancellationPolicy') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingsTable + N' ADD [LongTermCancellationPolicy] NVARCHAR(50) NULL;';
    EXEC sp_executesql @sql;
END;

DECLARE @AccessTable sysname =
    CASE
        WHEN OBJECT_ID(N'[ListingAccessInstructions]', N'U') IS NOT NULL THEN N'[ListingAccessInstructions]'
        WHEN OBJECT_ID(N'[ListingAccessInstruction]', N'U') IS NOT NULL THEN N'[ListingAccessInstruction]'
        ELSE NULL
    END;

IF @AccessTable IS NULL
BEGIN
    RAISERROR('Neither [ListingAccessInstructions] nor [ListingAccessInstruction] table exists in this database.', 16, 1);
    RETURN;
END;

DECLARE @AccessName sysname = REPLACE(REPLACE(@AccessTable, '[', ''), ']', '');

IF COL_LENGTH(@AccessName, 'Instructions') IS NOT NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @AccessTable + N' ALTER COLUMN [Instructions] NVARCHAR(MAX) NULL;';
    EXEC sp_executesql @sql;
END;

PRINT 'Listing cancellation policies and access instruction length ensured.';
