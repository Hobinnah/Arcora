BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PaymentProviderEvents]') AND [c].[name] = N'Payload');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [PaymentProviderEvents] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [PaymentProviderEvents] ALTER COLUMN [Payload] nvarchar(max) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentMethods] ADD [CardFunding] nvarchar(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentMethods] ADD [MethodRole] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentIntents] ADD [FailureCategory] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentIntents] ADD [FailureCode] nvarchar(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentAttempts] ADD [FailureCategory] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentAttempts] ADD [MethodKind] nvarchar(50) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    ALTER TABLE [PaymentAttempts] ADD [PaymentMethodID] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907185402_AddStripePaymentFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260907185402_AddStripePaymentFields', N'8.0.19');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907190621_MakeAutopayMandateLeaseIdNullable'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AutopayMandates]') AND [c].[name] = N'LeaseID');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AutopayMandates] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [AutopayMandates] ALTER COLUMN [LeaseID] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260907190621_MakeAutopayMandateLeaseIdNullable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260907190621_MakeAutopayMandateLeaseIdNullable', N'8.0.19');
END;
GO

COMMIT;
GO

