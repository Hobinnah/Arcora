/*
    Inserts default ListingRules (key/value) for a single listing.
    Idempotent per (ListingID, RuleType).

    Usage:
      1) Set @ListingID
      2) Run script
*/

DECLARE @ListingID UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000'; -- TODO: set target ListingID
DECLARE @CapturedBy NVARCHAR(100) = 'Seed';
DECLARE @NowUtc DATETIME2 = SYSUTCDATETIME();

IF @ListingID = '00000000-0000-0000-0000-000000000000'
BEGIN
    RAISERROR('Please set @ListingID before running this script.', 16, 1);
    RETURN;
END;

DECLARE @ListingRulesTable sysname =
    CASE
        WHEN OBJECT_ID(N'[ListingRules]', N'U') IS NOT NULL THEN N'[ListingRules]'
        WHEN OBJECT_ID(N'[ListingRule]', N'U') IS NOT NULL THEN N'[ListingRule]'
        ELSE NULL
    END;

IF @ListingRulesTable IS NULL
BEGIN
    RAISERROR('Neither [ListingRules] nor [ListingRule] table exists in this database.', 16, 1);
    RETURN;
END;

DECLARE @sql nvarchar(max) = N'
;WITH DefaultRules AS
(
    SELECT N''PARTIES'' AS RuleType, N''Parties'' AS RuleTitle UNION ALL
    SELECT N''PETS'', N''Pets'' UNION ALL
    SELECT N''QUIET_HOURS_START'', N''Quiet Hour Start'' UNION ALL
    SELECT N''QUIET_HOURS_END'', N''Quiet Hour End'' UNION ALL
    SELECT N''SMOKING'', N''Smoking'' UNION ALL
    SELECT N''NUMBER_OF_GUESTS'', N''Number Of Guests'' UNION ALL
    SELECT N''CHECK_IN'', N''Check In'' UNION ALL
    SELECT N''CHECK_OUT'', N''Check Out'' UNION ALL
    SELECT N''ADDITIONAL'', N''Additional Rules''
)
INSERT INTO ' + @ListingRulesTable + N'
(
    [ListingRuleID],
    [ListingID],
    [RuleType],
    [RuleTitle],
    [RuleDescription],
    [IsAllowed],
    [EffectiveFrom],
    [CapturedDate],
    [CapturedBy]
)
SELECT
    NEWID(),
    @ListingID,
    dr.RuleType,
    dr.RuleTitle,
    dr.RuleTitle,
    1,
    @NowUtc,
    @NowUtc,
    @CapturedBy
FROM DefaultRules dr
WHERE NOT EXISTS
(
    SELECT 1
    FROM ' + @ListingRulesTable + N' lr
    WHERE lr.[ListingID] = @ListingID
      AND lr.[RuleType] = dr.RuleType
);';

EXEC sp_executesql
    @sql,
    N'@ListingID UNIQUEIDENTIFIER, @CapturedBy NVARCHAR(100), @NowUtc DATETIME2',
    @ListingID = @ListingID,
    @CapturedBy = @CapturedBy,
    @NowUtc = @NowUtc;

PRINT 'Default listing rules inserted/ensured for ListingID: ' + CONVERT(nvarchar(36), @ListingID);
