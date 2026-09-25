/*
    Adds listing advance notice and same-day booking columns.
    Idempotent: safe to run repeatedly.
*/

DECLARE @ListingTable sysname =
    CASE
        WHEN OBJECT_ID(N'[Listings]', N'U') IS NOT NULL THEN N'[Listings]'
        WHEN OBJECT_ID(N'[Listing]', N'U') IS NOT NULL THEN N'[Listing]'
        ELSE NULL
    END;

IF @ListingTable IS NULL
BEGIN
    RAISERROR('Neither [Listings] nor [Listing] table exists in this database.', 16, 1);
    RETURN;
END;

DECLARE @TableNameWithoutBrackets sysname = REPLACE(REPLACE(@ListingTable, '[', ''), ']', '');
DECLARE @sql nvarchar(max);

IF COL_LENGTH(@TableNameWithoutBrackets, 'AdvanceNotice') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingTable + N' ADD [AdvanceNotice] NVARCHAR(100) NULL;';
    EXEC sp_executesql @sql;
END;

IF COL_LENGTH(@TableNameWithoutBrackets, 'AllowSameDay') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingTable + N' ADD [AllowSameDay] BIT NOT NULL CONSTRAINT [DF_' + @TableNameWithoutBrackets + N'_AllowSameDay] DEFAULT (0);';
    EXEC sp_executesql @sql;
END;

PRINT 'AdvanceNotice and AllowSameDay columns ensured.';
