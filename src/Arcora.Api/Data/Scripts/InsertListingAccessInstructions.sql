/*
    Inserts default listing access instructions for one listing.
    Idempotent per (ListingID, InstructionType).

    Usage:
      1) Set @ListingID
      2) Optionally set @LeaseID and custom instruction texts
      3) Run script
*/

DECLARE @ListingID UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000000'; -- TODO: set target ListingID
DECLARE @LeaseID UNIQUEIDENTIFIER = NULL; -- optional
DECLARE @CapturedBy NVARCHAR(100) = 'Seed';
DECLARE @NowUtc DATETIME2 = SYSUTCDATETIME();

-- Default instruction bodies (nvarchar(max) friendly)
DECLARE @HouseManual NVARCHAR(MAX) = N'Please keep noise low after 10 PM, avoid moving furniture, and follow building recycling rules.';
DECLARE @CheckoutInstruction NVARCHAR(MAX) = N'Place used towels in the laundry basket, wash dishes, lock all doors, and return keys to the lockbox.';
DECLARE @GuestRequirement NVARCHAR(MAX) = N'Only registered guests are allowed. Government-issued ID may be requested at check-in.';
DECLARE @TaxesInstruction NVARCHAR(MAX) = N'Taxes are calculated at checkout based on local regulations and booking duration.';

IF @ListingID = '00000000-0000-0000-0000-000000000000'
BEGIN
    RAISERROR('Please set @ListingID before running this script.', 16, 1);
    RETURN;
END;

DECLARE @TargetTable sysname =
    CASE
        WHEN OBJECT_ID(N'[ListingAccessInstructions]', N'U') IS NOT NULL THEN N'[ListingAccessInstructions]'
        WHEN OBJECT_ID(N'[ListingAccessInstruction]', N'U') IS NOT NULL THEN N'[ListingAccessInstruction]'
        ELSE NULL
    END;

IF @TargetTable IS NULL
BEGIN
    RAISERROR('Neither [ListingAccessInstructions] nor [ListingAccessInstruction] table exists in this database.', 16, 1);
    RETURN;
END;

DECLARE @sql NVARCHAR(MAX) = N'
;WITH SourceData AS
(
    SELECT N''HOUSE_MANUAL'' AS InstructionType, @HouseManual AS Instructions UNION ALL
    SELECT N''CHECKOUT_INSTRUCTION'', @CheckoutInstruction UNION ALL
    SELECT N''GUEST_REQUIREMENT'', @GuestRequirement UNION ALL
    SELECT N''TAXES'', @TaxesInstruction
)
INSERT INTO ' + @TargetTable + N'
(
    [ListingAccessInstructionID],
    [ListingID],
    [LeaseID],
    [InstructionType],
    [Instructions],
    [SecretReference],
    [AvailableFrom],
    [AvailableUntil],
    [IsActive],
    [CapturedDate],
    [CapturedBy]
)
SELECT
    NEWID(),
    @ListingID,
    @LeaseID,
    s.InstructionType,
    s.Instructions,
    NULL,
    @NowUtc,
    NULL,
    CAST(1 AS bit),
    @NowUtc,
    @CapturedBy
FROM SourceData s
WHERE NOT EXISTS
(
    SELECT 1
    FROM ' + @TargetTable + N' t
    WHERE t.[ListingID] = @ListingID
      AND t.[InstructionType] = s.InstructionType
);
';

EXEC sp_executesql
    @sql,
    N'@ListingID UNIQUEIDENTIFIER, @LeaseID UNIQUEIDENTIFIER, @CapturedBy NVARCHAR(100), @NowUtc DATETIME2, @HouseManual NVARCHAR(MAX), @CheckoutInstruction NVARCHAR(MAX), @GuestRequirement NVARCHAR(MAX), @TaxesInstruction NVARCHAR(MAX)',
    @ListingID = @ListingID,
    @LeaseID = @LeaseID,
    @CapturedBy = @CapturedBy,
    @NowUtc = @NowUtc,
    @HouseManual = @HouseManual,
    @CheckoutInstruction = @CheckoutInstruction,
    @GuestRequirement = @GuestRequirement,
    @TaxesInstruction = @TaxesInstruction;

PRINT 'Listing access instructions inserted/ensured for ListingID: ' + CONVERT(NVARCHAR(36), @ListingID);
