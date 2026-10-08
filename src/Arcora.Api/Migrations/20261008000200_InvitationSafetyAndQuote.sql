BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000000_AddCanonicalUserPhoneUniqueIndex'
)
BEGIN
    IF OBJECT_ID(N'[dbo].[AccountPhoneNormalizationPreflight]', N'U') IS NULL
        THROW 51000, 'Run the explicit account-phone normalization preflight against this database, then retry the migration.', 1;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[AspNetUsers] AS [users]
        FULL OUTER JOIN [dbo].[AccountPhoneNormalizationPreflight] AS [preflight]
            ON [users].[Id] = [preflight].[UserId]
        WHERE [users].[Id] IS NULL
           OR [preflight].[UserId] IS NULL
           OR [preflight].[PhoneHash] <> HASHBYTES(
                'SHA2_256',
                CASE
                    WHEN [users].[PhoneNumber] IS NULL THEN 0x00
                    ELSE 0x01 + CONVERT(varbinary(max), [users].[PhoneNumber])
                END)
    )
        THROW 51001, 'Account data changed after the phone normalization preflight. Rerun the preflight and retry the migration.', 1;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[AspNetUsers]
        WHERE [PhoneNumber] IS NOT NULL
          AND (
              LEN([PhoneNumber]) < 3
              OR LEN([PhoneNumber]) > 16
              OR LEFT([PhoneNumber], 1) <> N'+'
              OR SUBSTRING([PhoneNumber], 2, 1) NOT LIKE N'[1-9]'
              OR SUBSTRING([PhoneNumber], 2, 16) LIKE N'%[^0-9]%'
          )
    )
        THROW 51002, 'Cannot create the unique phone index: preflight did not canonicalize all account phone numbers to E.164 form.', 1;

    IF EXISTS (
        SELECT [PhoneNumber]
        FROM [dbo].[AspNetUsers]
        WHERE [PhoneNumber] IS NOT NULL
        GROUP BY [PhoneNumber]
        HAVING COUNT(*) > 1
    )
        THROW 51003, 'Cannot create the unique phone index: duplicate account phone numbers remain after the preflight.', 1;

    DROP TABLE [dbo].[AccountPhoneNormalizationPreflight];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000000_AddCanonicalUserPhoneUniqueIndex'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumber');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [AspNetUsers] ALTER COLUMN [PhoneNumber] nvarchar(16) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000000_AddCanonicalUserPhoneUniqueIndex'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_PhoneNumber] ON [AspNetUsers] ([PhoneNumber]) WHERE [PhoneNumber] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000000_AddCanonicalUserPhoneUniqueIndex'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008000000_AddCanonicalUserPhoneUniqueIndex', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000100_AddTenantInvitationEmailOutbox'
)
BEGIN
    CREATE TABLE [TenantInvitationEmails] (
        [TenantInvitationID] uniqueidentifier NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [ProtectedMessage] nvarchar(max) NOT NULL,
        [Attempts] int NOT NULL,
        [NextAttemptAt] datetime2 NOT NULL,
        [LastAttemptAt] datetime2 NULL,
        [SentAt] datetime2 NULL,
        CONSTRAINT [PK_TenantInvitationEmails] PRIMARY KEY ([TenantInvitationID]),
        CONSTRAINT [FK_TenantInvitationEmails_TenantInvitations_TenantInvitationID] FOREIGN KEY ([TenantInvitationID]) REFERENCES [TenantInvitations] ([TenantInvitationID])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000100_AddTenantInvitationEmailOutbox'
)
BEGIN
    CREATE INDEX [IX_TenantInvitationEmails_Status_NextAttemptAt] ON [TenantInvitationEmails] ([Status], [NextAttemptAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000100_AddTenantInvitationEmailOutbox'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008000100_AddTenantInvitationEmailOutbox', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [MonthlyRentAmount] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [SecurityDepositAmount] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [Currency] nvarchar(3) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [StartDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [EndDate] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [LeaseTermMonths] smallint NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    ALTER TABLE [TenantInvitations] ADD [ReservationHoldID] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261008000200_AddTenantInvitationQuote'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261008000200_AddTenantInvitationQuote', N'8.0.19');
END;
GO

COMMIT;
GO

