/*
    Adds the host<->tenant connection columns to the Conversation table and a supporting index.
    Idempotent: safe to run repeatedly.
*/
IF COL_LENGTH('Conversation', 'TenantID') IS NULL
    ALTER TABLE [Conversation] ADD [TenantID] UNIQUEIDENTIFIER NULL;

IF COL_LENGTH('Conversation', 'OrganizationID') IS NULL
    ALTER TABLE [Conversation] ADD [OrganizationID] UNIQUEIDENTIFIER NULL;
GO

-- Fast lookup for the (tenant, org) direct-thread guard and inbox filters.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Conversation_Tenant_Org')
    CREATE INDEX IX_Conversation_Tenant_Org
        ON [Conversation] ([TenantID], [OrganizationID], [ConversationType]);
GO

PRINT 'Conversation TenantID/OrganizationID columns and index ensured.';
