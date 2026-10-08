using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    [DbContext(typeof(ArcoraDbContext))]
    [Migration("20261008000000_AddCanonicalUserPhoneUniqueIndex")]
    public partial class AddCanonicalUserPhoneUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $"""
                IF OBJECT_ID(N'[dbo].[{Arcora.Api.Accounts.AccountPhoneNormalizationPreflight.SnapshotTableName}]', N'U') IS NULL
                    THROW 51000, 'Run the explicit account-phone normalization preflight against this database, then retry the migration.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM [dbo].[AspNetUsers] AS [users]
                    FULL OUTER JOIN [dbo].[{Arcora.Api.Accounts.AccountPhoneNormalizationPreflight.SnapshotTableName}] AS [preflight]
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

                DROP TABLE [dbo].[{Arcora.Api.Accounts.AccountPhoneNormalizationPreflight.SnapshotTableName}];
                """);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true);
        }
    }
}
