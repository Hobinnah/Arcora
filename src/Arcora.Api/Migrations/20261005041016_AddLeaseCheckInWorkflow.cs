using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseCheckInWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inspections_LeaseID",
                table: "Inspections");

            migrationBuilder.Sql(@"
IF COL_LENGTH('LeaseSignatories', 'ProviderDocumentID') IS NULL
    ALTER TABLE [LeaseSignatories] ADD [ProviderDocumentID] nvarchar(255) NULL;
IF COL_LENGTH('LeaseSignatories', 'ProviderDocumentUrl') IS NULL
    ALTER TABLE [LeaseSignatories] ADD [ProviderDocumentUrl] nvarchar(1000) NULL;
IF COL_LENGTH('LeaseSignatories', 'ProviderRequestID') IS NULL
    ALTER TABLE [LeaseSignatories] ADD [ProviderRequestID] nvarchar(255) NULL;
IF COL_LENGTH('LeaseSignatories', 'SignatureImageUrl') IS NULL
    ALTER TABLE [LeaseSignatories] ADD [SignatureImageUrl] nvarchar(1000) NULL;
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('Leases', 'AgreementUrl') IS NULL
    ALTER TABLE [Leases] ADD [AgreementUrl] nvarchar(1000) NULL;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Inspections]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[Inspections]', N'ArrivalConfirmedAt') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [ArrivalConfirmedAt] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'OccupancyReadiness') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [OccupancyReadiness] nvarchar(50) NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmissionIdempotencyKey') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [SubmissionIdempotencyKey] nvarchar(255) NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmissionRequestHash') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [SubmissionRequestHash] nvarchar(128) NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmittedAt') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [SubmittedAt] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmittedByUserID') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [SubmittedByUserID] bigint NULL;

    IF COL_LENGTH(N'[dbo].[Inspections]', N'Version') IS NULL
        ALTER TABLE [dbo].[Inspections] ADD [Version] nvarchar(64) NULL;
END
");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Attachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Attachments]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[Attachments]', N'ChecksumSha256') IS NULL
        ALTER TABLE [dbo].[Attachments] ADD [ChecksumSha256] nvarchar(64) NULL;

    IF COL_LENGTH(N'[dbo].[Attachments]', N'ClientUploadID') IS NULL
        ALTER TABLE [dbo].[Attachments] ADD [ClientUploadID] nvarchar(100) NULL;

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadExpiresAt') IS NULL
        ALTER TABLE [dbo].[Attachments] ADD [UploadExpiresAt] datetime2 NULL;

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadFailureReason') IS NULL
        ALTER TABLE [dbo].[Attachments] ADD [UploadFailureReason] nvarchar(256) NULL;

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadStatus') IS NULL
        ALTER TABLE [dbo].[Attachments] ADD [UploadStatus] nvarchar(20) NULL;
END
");

            migrationBuilder.CreateTable(
                name: "InspectionReviews",
                columns: table => new
                {
                    InspectionReviewID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedByUserID = table.Column<long>(type: "bigint", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionReviews", x => x.InspectionReviewID);
                    table.ForeignKey(
                        name: "FK_InspectionReviews_AspNetUsers_ReviewedByUserID",
                        column: x => x.ReviewedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InspectionReviews_Inspections_InspectionID",
                        column: x => x.InspectionID,
                        principalTable: "Inspections",
                        principalColumn: "InspectionID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_LeaseID_InspectionType",
                table: "Inspections",
                columns: new[] { "LeaseID", "InspectionType" },
                unique: true,
                filter: "[LeaseID] IS NOT NULL AND [InspectionType] = 'MOVE_IN'");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_EntityType_EntityID_ClientUploadID",
                table: "Attachments",
                columns: new[] { "EntityType", "EntityID", "ClientUploadID" },
                unique: true,
                filter: "[ClientUploadID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReviews_InspectionID_ReviewedAt",
                table: "InspectionReviews",
                columns: new[] { "InspectionID", "ReviewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionReviews_ReviewedByUserID",
                table: "InspectionReviews",
                column: "ReviewedByUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionReviews");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_LeaseID_InspectionType",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_EntityType_EntityID_ClientUploadID",
                table: "Attachments");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[LeaseSignatories]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[LeaseSignatories]', N'ProviderDocumentID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatories] DROP COLUMN [ProviderDocumentID];

    IF COL_LENGTH(N'[dbo].[LeaseSignatories]', N'ProviderDocumentUrl') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatories] DROP COLUMN [ProviderDocumentUrl];

    IF COL_LENGTH(N'[dbo].[LeaseSignatories]', N'ProviderRequestID') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatories] DROP COLUMN [ProviderRequestID];

    IF COL_LENGTH(N'[dbo].[LeaseSignatories]', N'SignatureImageUrl') IS NOT NULL
        ALTER TABLE [dbo].[LeaseSignatories] DROP COLUMN [SignatureImageUrl];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Leases]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Leases]', N'AgreementUrl') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Leases] DROP COLUMN [AgreementUrl];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Inspections]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[Inspections]', N'ArrivalConfirmedAt') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [ArrivalConfirmedAt];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'OccupancyReadiness') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [OccupancyReadiness];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmissionIdempotencyKey') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [SubmissionIdempotencyKey];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmissionRequestHash') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [SubmissionRequestHash];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmittedAt') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [SubmittedAt];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'SubmittedByUserID') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [SubmittedByUserID];

    IF COL_LENGTH(N'[dbo].[Inspections]', N'Version') IS NOT NULL
        ALTER TABLE [dbo].[Inspections] DROP COLUMN [Version];
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Attachments]', N'U') IS NOT NULL
BEGIN
    IF COL_LENGTH(N'[dbo].[Attachments]', N'ChecksumSha256') IS NOT NULL
        ALTER TABLE [dbo].[Attachments] DROP COLUMN [ChecksumSha256];

    IF COL_LENGTH(N'[dbo].[Attachments]', N'ClientUploadID') IS NOT NULL
        ALTER TABLE [dbo].[Attachments] DROP COLUMN [ClientUploadID];

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadExpiresAt') IS NOT NULL
        ALTER TABLE [dbo].[Attachments] DROP COLUMN [UploadExpiresAt];

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadFailureReason') IS NOT NULL
        ALTER TABLE [dbo].[Attachments] DROP COLUMN [UploadFailureReason];

    IF COL_LENGTH(N'[dbo].[Attachments]', N'UploadStatus') IS NOT NULL
        ALTER TABLE [dbo].[Attachments] DROP COLUMN [UploadStatus];
END
");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Attachments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_LeaseID",
                table: "Inspections",
                column: "LeaseID");
        }
    }
}
