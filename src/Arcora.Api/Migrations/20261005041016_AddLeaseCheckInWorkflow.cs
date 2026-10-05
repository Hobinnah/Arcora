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

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalConfirmedAt",
                table: "Inspections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OccupancyReadiness",
                table: "Inspections",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionIdempotencyKey",
                table: "Inspections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionRequestHash",
                table: "Inspections",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Inspections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SubmittedByUserID",
                table: "Inspections",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Inspections",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "ChecksumSha256",
                table: "Attachments",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientUploadID",
                table: "Attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadExpiresAt",
                table: "Attachments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadFailureReason",
                table: "Attachments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadStatus",
                table: "Attachments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "ProviderDocumentID",
                table: "LeaseSignatories");

            migrationBuilder.DropColumn(
                name: "ProviderDocumentUrl",
                table: "LeaseSignatories");

            migrationBuilder.DropColumn(
                name: "ProviderRequestID",
                table: "LeaseSignatories");

            migrationBuilder.DropColumn(
                name: "SignatureImageUrl",
                table: "LeaseSignatories");

            migrationBuilder.DropColumn(
                name: "AgreementUrl",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "ArrivalConfirmedAt",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "OccupancyReadiness",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SubmissionIdempotencyKey",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SubmissionRequestHash",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserID",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ChecksumSha256",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ClientUploadID",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UploadExpiresAt",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UploadFailureReason",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UploadStatus",
                table: "Attachments");

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
