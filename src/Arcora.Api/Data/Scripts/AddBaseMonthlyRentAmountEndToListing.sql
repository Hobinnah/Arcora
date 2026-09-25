/*
    Adds BaseMonthlyRentAmountEnd to Listings/Listing.
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

DECLARE @TableName sysname = REPLACE(REPLACE(@ListingsTable, '[', ''), ']', '');
DECLARE @sql nvarchar(max);

IF COL_LENGTH(@TableName, 'BaseMonthlyRentAmountEnd') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingsTable + N' ADD [BaseMonthlyRentAmountEnd] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_' + @TableName + N'_BaseMonthlyRentAmountEnd] DEFAULT (0);';
    EXEC sp_executesql @sql;
END;

PRINT 'BaseMonthlyRentAmountEnd ensured.';
