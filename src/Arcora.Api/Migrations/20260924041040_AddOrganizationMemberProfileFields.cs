using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationMemberProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OrganizationMembers]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'BirthDecade') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [BirthDecade] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'HomeUniqueDescription') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [HomeUniqueDescription] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'PetsDescription') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [PetsDescription] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'ProfilePhotoUrl') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [ProfilePhotoUrl] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'SchoolDescription') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [SchoolDescription] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'TravelDestination') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [TravelDestination] nvarchar(max) NULL;

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'WorkDescription') IS NULL
        ALTER TABLE [dbo].[OrganizationMembers] ADD [WorkDescription] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[OrganizationMembers]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'BirthDecade') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [BirthDecade];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'HomeUniqueDescription') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [HomeUniqueDescription];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'PetsDescription') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [PetsDescription];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'ProfilePhotoUrl') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [ProfilePhotoUrl];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'SchoolDescription') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [SchoolDescription];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'TravelDestination') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [TravelDestination];

    IF COL_LENGTH(N'[dbo].[OrganizationMembers]', N'WorkDescription') IS NOT NULL
        ALTER TABLE [dbo].[OrganizationMembers] DROP COLUMN [WorkDescription];
END
");
        }
    }
}
