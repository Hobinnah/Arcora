-- Migration script: Add AgreementUrl to Lease and provider metadata to LeaseSignatory
/* =============================================================================
   Fix script for: failing "fetch leases" and "fetch rental applications".

   IMPORTANT: Table names below use the pluralized names produced by
   ArcoraDbContext (e.g. [Leases], [LeaseSignatories], [LeaseDocuments],
   [ApplicationOccupants]), NOT the singular [Table(...)] attribute names.

   PART A - Adds the new columns (AgreementUrl + Signatory provider fields).
   PART B - Repairs existing NULL values in NOT NULL (EF [Required]) string
            columns that cause "SqlNullValueException: Data is Null" while EF
            materializes the RentalApplication include graph.
   ============================================================================= */

/* -----------------------------------------------------------------------------
   PART A: Schema additions (idempotent)
   ----------------------------------------------------------------------------- */
BEGIN TRANSACTION;

-- 1) Add AgreementUrl to Leases
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'AgreementUrl' AND Object_ID = Object_ID(N'dbo.Leases'))
BEGIN
    ALTER TABLE dbo.Leases ADD AgreementUrl NVARCHAR(1000) NULL;
END

-- 2) Add provider metadata columns to LeaseSignatories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ProviderRequestID' AND Object_ID = Object_ID(N'dbo.LeaseSignatories'))
BEGIN
    ALTER TABLE dbo.LeaseSignatories ADD ProviderRequestID NVARCHAR(255) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ProviderDocumentID' AND Object_ID = Object_ID(N'dbo.LeaseSignatories'))
BEGIN
    ALTER TABLE dbo.LeaseSignatories ADD ProviderDocumentID NVARCHAR(255) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'ProviderDocumentUrl' AND Object_ID = Object_ID(N'dbo.LeaseSignatories'))
BEGIN
    ALTER TABLE dbo.LeaseSignatories ADD ProviderDocumentUrl NVARCHAR(1000) NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'SignatureImageUrl' AND Object_ID = Object_ID(N'dbo.LeaseSignatories'))
BEGIN
    ALTER TABLE dbo.LeaseSignatories ADD SignatureImageUrl NVARCHAR(1000) NULL;
END

COMMIT TRANSACTION;
PRINT 'PART A complete: columns added (or already existed).';
GO

/* -----------------------------------------------------------------------------
   PART B: Repair NULLs in NOT NULL / [Required] string columns.

   EF Core maps [Required] string properties to NOT NULL columns and reads them
   with SqlDataReader.GetString() (no null check). If existing rows contain NULL
   in those columns, materialization throws:
       System.Data.SqlTypes.SqlNullValueException: Data is Null.
   ----------------------------------------------------------------------------- */
BEGIN TRANSACTION;

-- ApplicationOccupants (included via RentalApplication.ApplicationOccupants)
UPDATE dbo.ApplicationOccupants SET FirstName    = N''        WHERE FirstName IS NULL;
UPDATE dbo.ApplicationOccupants SET LastName     = N''        WHERE LastName IS NULL;
UPDATE dbo.ApplicationOccupants SET OccupantType = N'ADULT'   WHERE OccupantType IS NULL;
UPDATE dbo.ApplicationOccupants SET Status       = N'PENDING' WHERE Status IS NULL;

-- LeaseDocuments (included via RentalApplication.LeaseDocuments)
UPDATE dbo.LeaseDocuments SET DocumentType     = N'LEASE_AGREEMENT' WHERE DocumentType IS NULL;
UPDATE dbo.LeaseDocuments SET DocumentStatus   = N'DRAFT'           WHERE DocumentStatus IS NULL;
UPDATE dbo.LeaseDocuments SET OriginalFilename = N'unknown.pdf'     WHERE OriginalFilename IS NULL;
UPDATE dbo.LeaseDocuments SET StorageProvider  = N'AZURE_BLOB'      WHERE StorageProvider IS NULL;
UPDATE dbo.LeaseDocuments SET StorageReference = N''                WHERE StorageReference IS NULL;

COMMIT TRANSACTION;
PRINT 'PART B complete: NULLs in required columns repaired.';
GO

/* -----------------------------------------------------------------------------
   OPTIONAL DIAGNOSTIC: the COUNT(*) FROM [Leases] timing out at exactly 30s
   indicates the query is BLOCKED (most likely by a previous manual script that
   started a transaction against the wrong (singular) table names and never
   committed). If timeouts persist, run:

       SELECT session_id, blocking_session_id, wait_type, wait_time, text
       FROM sys.dm_exec_requests
       CROSS APPLY sys.dm_exec_sql_text(sql_handle)
       WHERE blocking_session_id <> 0;

       DBCC OPENTRAN;
       -- KILL <orphaned_session_id>;
   ----------------------------------------------------------------------------- */
