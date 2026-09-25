BEGIN TRANSACTION;
GO

ALTER TABLE [CohostInvitations] ADD [CohostName] nvarchar(150) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260925021522_AddCohostNameToCohostInvitation', N'8.0.19');
GO

COMMIT;
GO

