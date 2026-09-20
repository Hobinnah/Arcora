/*
    Adds listing discount rate columns.
    Idempotent: safe to run repeatedly.
    Also migrates legacy [BiyearlyDiscountRate] to [SemiAnnualDiscountRate] when present.
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

IF COL_LENGTH(@TableNameWithoutBrackets, 'QuarterlyDiscountRate') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingTable + N' ADD [QuarterlyDiscountRate] INT NOT NULL CONSTRAINT [DF_' + @TableNameWithoutBrackets + N'_QuarterlyDiscountRate] DEFAULT (0);';
    EXEC sp_executesql @sql;
END;

IF COL_LENGTH(@TableNameWithoutBrackets, 'SemiAnnualDiscountRate') IS NULL
BEGIN
    IF COL_LENGTH(@TableNameWithoutBrackets, 'BiyearlyDiscountRate') IS NOT NULL
    BEGIN
        -- Rename legacy column if it already exists from a previous script run.
        SET @sql = N'EXEC sp_rename N''' + @TableNameWithoutBrackets + N'.BiyearlyDiscountRate'', N''SemiAnnualDiscountRate'', N''COLUMN'';';
        EXEC sp_executesql @sql;
    END
    ELSE
    BEGIN
        SET @sql = N'ALTER TABLE ' + @ListingTable + N' ADD [SemiAnnualDiscountRate] INT NOT NULL CONSTRAINT [DF_' + @TableNameWithoutBrackets + N'_SemiAnnualDiscountRate] DEFAULT (0);';
        EXEC sp_executesql @sql;
    END
END;

IF COL_LENGTH(@TableNameWithoutBrackets, 'YearlyDiscountRate') IS NULL
BEGIN
    SET @sql = N'ALTER TABLE ' + @ListingTable + N' ADD [YearlyDiscountRate] INT NOT NULL CONSTRAINT [DF_' + @TableNameWithoutBrackets + N'_YearlyDiscountRate] DEFAULT (0);';
    EXEC sp_executesql @sql;
END;

PRINT 'Listing discount rate columns ensured.';
