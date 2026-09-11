/*
    Idempotent seed patch for newly added lookup/reference data.

    Inserts only the rows that do not already exist, so it is safe to run against a
    database that already contains seeded data (unlike DbSeeder's AddIfEmpty logic,
    which skips a table entirely once it has any rows).

    Adds:
      - FeeType : Tenant Placement Fee, Host Placement Fee, Host Active Unit Subscription Fee
      - Fee     : fixed-amount global platform fees for the above (49.99 / 99.99 / 24.99)
      - TaxRate : all remaining Canadian provinces/territories (GST/HST/PST/RST/QST)
*/

SET NOCOUNT ON;

DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @SeedBy NVARCHAR(100) = N'SYSTEM_SEED';

/* ------------------------------------------------------------------ */
/* 1. FeeType                                                          */
/* ------------------------------------------------------------------ */
INSERT INTO [FeeTypes] ([Name], [IsPlatformFee], [CapturedDate], [CapturedBy])
SELECT v.[Name], 1, @Now, @SeedBy
FROM (VALUES
    (N'Tenant Placement Fee'),
    (N'Host Placement Fee'),
    (N'Host Active Unit Subscription Fee')
) AS v([Name])
WHERE NOT EXISTS (
    SELECT 1 FROM [FeeTypes] ft WHERE ft.[Name] = v.[Name]
);

/* ------------------------------------------------------------------ */
/* 2. Fee (fixed-amount, global platform fees)                        */
/* ------------------------------------------------------------------ */
INSERT INTO [Fees]
    ([FeeID], [FeeTypeID], [OrganizationID], [Code], [Name], [CalculationType],
     [FixedAmount], [Currency], [IsTaxable], [EffectiveFrom], [IsActive],
     [CapturedDate], [CapturedBy])
SELECT
    NEWID(), ft.[FeeTypeID], NULL, v.[Code], v.[FeeTypeName], N'FIXED',
    v.[Amount], N'CAD', 0, DATEADD(YEAR, -1, @Now), 1,
    @Now, @SeedBy
FROM (VALUES
    (N'Tenant Placement Fee',              N'PLATFORM-TENANT-PLACEMENT',      CAST(49.99 AS DECIMAL(18,2))),
    (N'Host Placement Fee',                N'PLATFORM-HOST-PLACEMENT',        CAST(99.99 AS DECIMAL(18,2))),
    (N'Host Active Unit Subscription Fee', N'PLATFORM-HOST-UNIT-SUBSCRIPTION',CAST(24.99 AS DECIMAL(18,2)))
) AS v([FeeTypeName], [Code], [Amount])
INNER JOIN [FeeTypes] ft ON ft.[Name] = v.[FeeTypeName]
WHERE NOT EXISTS (
    SELECT 1 FROM [Fees] f WHERE f.[Code] = v.[Code]
);

/* ------------------------------------------------------------------ */
/* 3. TaxRate (remaining Canadian provinces/territories)              */
/* ------------------------------------------------------------------ */
INSERT INTO [TaxRates]
    ([Code], [Name], [CountryCode], [ProvinceCode], [Rate],
     [EffectiveFrom], [IsActive], [CapturedDate], [CapturedBy])
SELECT
    v.[Code], v.[Name], N'CA', v.[ProvinceCode], v.[Rate],
    DATEADD(YEAR, -5, @Now), 1, @Now, @SeedBy
FROM (VALUES
    -- HST provinces
    (N'HST-NB', N'New Brunswick HST',              N'NB', CAST(0.15    AS DECIMAL(9,5))),
    (N'HST-NL', N'Newfoundland and Labrador HST',  N'NL', CAST(0.15    AS DECIMAL(9,5))),
    (N'HST-NS', N'Nova Scotia HST',                N'NS', CAST(0.15    AS DECIMAL(9,5))),
    (N'HST-PE', N'Prince Edward Island HST',       N'PE', CAST(0.15    AS DECIMAL(9,5))),
    -- GST-only
    (N'GST-AB', N'Alberta GST',                    N'AB', CAST(0.05    AS DECIMAL(9,5))),
    (N'GST-NT', N'Northwest Territories GST',      N'NT', CAST(0.05    AS DECIMAL(9,5))),
    (N'GST-NU', N'Nunavut GST',                    N'NU', CAST(0.05    AS DECIMAL(9,5))),
    (N'GST-YT', N'Yukon GST',                      N'YT', CAST(0.05    AS DECIMAL(9,5))),
    -- GST + provincial tax
    (N'GST-MB', N'Manitoba GST',                   N'MB', CAST(0.05    AS DECIMAL(9,5))),
    (N'RST-MB', N'Manitoba RST',                   N'MB', CAST(0.07    AS DECIMAL(9,5))),
    (N'GST-SK', N'Saskatchewan GST',               N'SK', CAST(0.05    AS DECIMAL(9,5))),
    (N'PST-SK', N'Saskatchewan PST',               N'SK', CAST(0.06    AS DECIMAL(9,5))),
    (N'GST-QC', N'Quebec GST',                     N'QC', CAST(0.05    AS DECIMAL(9,5))),
    (N'QST-QC', N'Quebec QST',                     N'QC', CAST(0.09975 AS DECIMAL(9,5)))
) AS v([Code], [Name], [ProvinceCode], [Rate])
WHERE NOT EXISTS (
    SELECT 1 FROM [TaxRates] t WHERE t.[Code] = v.[Code]
);

PRINT 'Seed patch complete: FeeType, Fee, and TaxRate updated.';
