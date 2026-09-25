/*
    Renames Listings.BaseMonthlyRentAmountEnd to BaseMonthlyRentAmountMax.
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

IF COL_LENGTH(@TableName, 'BaseMonthlyRentAmountEnd') IS NOT NULL
   AND COL_LENGTH(@TableName, 'BaseMonthlyRentAmountMax') IS NULL
BEGIN
    SET @sql = N'EXEC sp_rename N''' + @TableName + N'.BaseMonthlyRentAmountEnd'', N''BaseMonthlyRentAmountMax'', N''COLUMN'';';
    EXEC sp_executesql @sql;
END;

PRINT 'BaseMonthlyRentAmountMax column name ensured.';
