BEGIN TRANSACTION;
GO

CREATE TABLE [CohostInvitations] (
    [CohostInvitationID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [PhoneNumber] nvarchar(50) NOT NULL,
    [CohostAccess] nvarchar(100) NOT NULL,
    [TokenHash] nvarchar(500) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [AcceptedAt] datetime2 NULL,
    [DeclinedAt] datetime2 NULL,
    [RevokedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_CohostInvitations] PRIMARY KEY ([CohostInvitationID]),
    CONSTRAINT [FK_CohostInvitations_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID])
);
GO

CREATE INDEX [IX_CohostInvitations_OrganizationID] ON [CohostInvitations] ([OrganizationID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260924220031_AddCohostInvitationPersistence', N'8.0.19');
GO

COMMIT;
GO

