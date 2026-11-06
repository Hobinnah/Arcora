/*
    Creates the ConversationNotes table: one private note per conversation, shared by the host organization.
    Mirrors migration 20261022030000_AddConversationNotes. Idempotent: safe to run repeatedly.
*/
IF OBJECT_ID(N'[ConversationNotes]', N'U') IS NULL
BEGIN
    CREATE TABLE [ConversationNotes] (
        [ConversationNoteID] UNIQUEIDENTIFIER NOT NULL,
        [ConversationID] UNIQUEIDENTIFIER NOT NULL,
        [Note] NVARCHAR(2000) NOT NULL,
        [UpdatedByOrganizationMemberID] UNIQUEIDENTIFIER NULL,
        [CreatedAt] DATETIME2 NOT NULL,
        [UpdatedAt] DATETIME2 NOT NULL,
        CONSTRAINT [PK_ConversationNotes] PRIMARY KEY ([ConversationNoteID]),
        CONSTRAINT [FK_ConversationNotes_Conversations_ConversationID]
            FOREIGN KEY ([ConversationID]) REFERENCES [Conversations] ([ConversationID]),
        CONSTRAINT [FK_ConversationNotes_OrganizationMembers_UpdatedByOrganizationMemberID]
            FOREIGN KEY ([UpdatedByOrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ConversationNotes_ConversationID')
    CREATE UNIQUE INDEX [IX_ConversationNotes_ConversationID] ON [ConversationNotes] ([ConversationID]);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ConversationNotes_UpdatedByOrganizationMemberID')
    CREATE INDEX [IX_ConversationNotes_UpdatedByOrganizationMemberID] ON [ConversationNotes] ([UpdatedByOrganizationMemberID]);
GO

PRINT 'ConversationNotes table and indexes ensured.';
