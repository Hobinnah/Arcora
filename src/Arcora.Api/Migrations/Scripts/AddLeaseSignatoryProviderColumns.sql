/* =============================================================================
   Adds missing signing provider columns to LeaseSignatories.
   Fixes runtime SQL errors like:
     - Invalid column name 'ProviderDocumentID'
     - Invalid column name 'ProviderDocumentUrl'
     - Invalid column name 'ProviderRequestID'
     - Invalid column name 'SignatureImageUrl'

   Idempotent and safe to run multiple times.
   ============================================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.LeaseSignatories', N'U') IS NULL
    BEGIN
        THROW 50000, 'Table dbo.LeaseSignatories was not found.', 1;
    END;

    IF COL_LENGTH('dbo.LeaseSignatories', 'ProviderRequestID') IS NULL
    BEGIN
        ALTER TABLE dbo.LeaseSignatories ADD ProviderRequestID NVARCHAR(255) NULL;
    END;

    IF COL_LENGTH('dbo.LeaseSignatories', 'ProviderDocumentID') IS NULL
    BEGIN
        ALTER TABLE dbo.LeaseSignatories ADD ProviderDocumentID NVARCHAR(255) NULL;
    END;

    IF COL_LENGTH('dbo.LeaseSignatories', 'ProviderDocumentUrl') IS NULL
    BEGIN
        ALTER TABLE dbo.LeaseSignatories ADD ProviderDocumentUrl NVARCHAR(1000) NULL;
    END;

    IF COL_LENGTH('dbo.LeaseSignatories', 'SignatureImageUrl') IS NULL
    BEGIN
        ALTER TABLE dbo.LeaseSignatories ADD SignatureImageUrl NVARCHAR(1000) NULL;
    END;

    COMMIT TRANSACTION;

    PRINT 'LeaseSignatories provider columns are present.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
