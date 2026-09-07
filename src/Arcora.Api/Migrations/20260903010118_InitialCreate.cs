using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AmenityCatalog",
                columns: table => new
                {
                    AmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityCatalog", x => x.AmenityID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    AttachmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    StorageProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StorageReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.AttachmentID);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Maintenance = table.Column<bool>(type: "bit", nullable: true),
                    Contractor = table.Column<bool>(type: "bit", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "FeeType",
                columns: table => new
                {
                    FeeTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsPlatformFee = table.Column<bool>(type: "bit", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeType", x => x.FeeTypeID);
                });

            migrationBuilder.CreateTable(
                name: "LeaseDocExtractedTerm",
                columns: table => new
                {
                    LeaseDocExtractedTermID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseDocumentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtractedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractedRentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtractedDepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtractedRenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtractionConfidence = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExtractionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RawExtractionJson = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExtractedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseDocExtractedTerm", x => x.LeaseDocExtractedTermID);
                });

            migrationBuilder.CreateTable(
                name: "ListingType",
                columns: table => new
                {
                    ListingTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingType", x => x.ListingTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusinessNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsPersonal = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InvoicePrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReceiptPrefix = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LateFeeEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AutoInvoiceGeneration = table.Column<bool>(type: "bit", nullable: false),
                    AutoPaymentRetry = table.Column<bool>(type: "bit", nullable: false),
                    PaymentProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BrandLogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequireBackgroundCheck = table.Column<bool>(type: "bit", nullable: false),
                    RankingScore = table.Column<decimal>(type: "decimal(18,2)", maxLength: 5, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrganizationID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviderEvent",
                columns: table => new
                {
                    PaymentProviderEventID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderEventID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviderEvent", x => x.PaymentProviderEventID);
                });

            migrationBuilder.CreateTable(
                name: "Preference",
                columns: table => new
                {
                    PreferenceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CompanyPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyWebsite = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CompanyLogo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxIdentificationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    RequirePaymentBeforeCaseSubmission = table.Column<bool>(type: "bit", nullable: true),
                    AutoAssignAgentID = table.Column<int>(type: "int", nullable: true),
                    MaxDraftExpiryDays = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preference", x => x.PreferenceID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    SubscriptionPlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AnnualPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    MaxProperties = table.Column<int>(type: "int", nullable: true),
                    MaxRentalUnits = table.Column<int>(type: "int", nullable: true),
                    MaxActiveListings = table.Column<int>(type: "int", nullable: true),
                    MaxOrganizationMembers = table.Column<int>(type: "int", nullable: true),
                    Features = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ProviderProductID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderMonthlyPriceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderAnnualPriceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlan", x => x.SubscriptionPlanID);
                });

            migrationBuilder.CreateTable(
                name: "TaxRate",
                columns: table => new
                {
                    TaxID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRate", x => x.TaxID);
                });

            migrationBuilder.CreateTable(
                name: "TenancyType",
                columns: table => new
                {
                    TenancyTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MinimumMonths = table.Column<short>(type: "smallint", nullable: true),
                    MaximumMonths = table.Column<short>(type: "smallint", nullable: true),
                    IsPeriodic = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenancyType", x => x.TenancyTypeID);
                });

            migrationBuilder.CreateTable(
                name: "UnitType",
                columns: table => new
                {
                    UnitTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitType", x => x.UnitTypeID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdentityVerification",
                columns: table => new
                {
                    IdentityVerificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    VerificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityVerification", x => x.IdentityVerificationID);
                    table.ForeignKey(
                        name: "FK_IdentityVerification_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserID = table.Column<long>(type: "bigint", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfileStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", maxLength: 20, nullable: true),
                    IsPADRegistered = table.Column<bool>(type: "bit", nullable: true),
                    IsCardRegistered = table.Column<bool>(type: "bit", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantID);
                    table.ForeignKey(
                        name: "FK_Tenant_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddressType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Line2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ProvinceCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlaceProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlaceProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_Address_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "Contractor",
                columns: table => new
                {
                    ContractorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contractor", x => x.ContractorID);
                    table.ForeignKey(
                        name: "FK_Contractor_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "Fee",
                columns: table => new
                {
                    FeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeID = table.Column<int>(type: "int", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CalculationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinimumFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaximumFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsTaxable = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fee", x => x.FeeID);
                    table.ForeignKey(
                        name: "FK_Fee_FeeType_FeeTypeID",
                        column: x => x.FeeTypeID,
                        principalTable: "FeeType",
                        principalColumn: "FeeTypeID");
                    table.ForeignKey(
                        name: "FK_Fee_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "LedgerAccount",
                columns: table => new
                {
                    LedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSystemAccount = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerAccount", x => x.LedgerAccountID);
                    table.ForeignKey(
                        name: "FK_LedgerAccount_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMember",
                columns: table => new
                {
                    OrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimaryOwner = table.Column<bool>(type: "bit", nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMember", x => x.OrganizationMemberID);
                    table.ForeignKey(
                        name: "FK_OrganizationMember_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationMember_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationStatement",
                columns: table => new
                {
                    OrganizationStatementID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: true),
                    Income = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Expenses = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationStatement", x => x.OrganizationStatementID);
                    table.ForeignKey(
                        name: "FK_OrganizationStatement_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "OrgPayoutAccount",
                columns: table => new
                {
                    OrgPayoutAccountID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderAccountID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountLast4 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgPayoutAccount", x => x.OrgPayoutAccountID);
                    table.ForeignKey(
                        name: "FK_OrgPayoutAccount_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "OrgSubscription",
                columns: table => new
                {
                    OrgSubscriptionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionPlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillingFrequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrialEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelAtPeriodEnd = table.Column<bool>(type: "bit", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderCustomerID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderSubscriptionID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgSubscription", x => x.OrgSubscriptionID);
                    table.ForeignKey(
                        name: "FK_OrgSubscription_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_OrgSubscription_SubscriptionPlan_SubscriptionPlanID",
                        column: x => x.SubscriptionPlanID,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "SubscriptionPlanID");
                });

            migrationBuilder.CreateTable(
                name: "ApplicationOccupant",
                columns: table => new
                {
                    ApplicationOccupantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OccupantType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimaryApplicant = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationOccupant", x => x.ApplicationOccupantID);
                    table.ForeignKey(
                        name: "FK_ApplicationOccupant_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApplicationOccupant_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    PaymentMethodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountLast4 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CardBrand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExpiryMonth = table.Column<short>(type: "smallint", nullable: true),
                    ExpiryYear = table.Column<short>(type: "smallint", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderCustomerID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProviderPaymentMethodID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.PaymentMethodID);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "TenantEmergencyContact",
                columns: table => new
                {
                    TenantEmergencyContactID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantEmergencyContact", x => x.TenantEmergencyContactID);
                    table.ForeignKey(
                        name: "FK_TenantEmergencyContact_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "TenantEmployment",
                columns: table => new
                {
                    TenantEmploymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmployerEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EmployerPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantEmployment", x => x.TenantEmploymentID);
                    table.ForeignKey(
                        name: "FK_TenantEmployment_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "TenantGuarantor",
                columns: table => new
                {
                    TenantGuarantorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Relationship = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantGuarantor", x => x.TenantGuarantorID);
                    table.ForeignKey(
                        name: "FK_TenantGuarantor_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TenantGuarantor_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "TenantScreeningCheck",
                columns: table => new
                {
                    TenantScreeningCheckID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsentCapturedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ResultSummary = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReportReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantScreeningCheck", x => x.TenantScreeningCheckID);
                    table.ForeignKey(
                        name: "FK_TenantScreeningCheck_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YearBuilt = table.Column<int>(type: "int", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_Property_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_Property_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditLogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserID = table.Column<long>(type: "bigint", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrelationID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditLogID);
                    table.ForeignKey(
                        name: "FK_AuditLog_AspNetUsers_ActorUserID",
                        column: x => x.ActorUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLog_OrganizationMember_OrganizationMemberID",
                        column: x => x.OrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_AuditLog_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_AuditLog_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseSignatory",
                columns: table => new
                {
                    LeaseSignatoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseDocumentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignatoryRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SignatureOrder = table.Column<int>(type: "int", nullable: true),
                    ProviderSignerID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseSignatory", x => x.LeaseSignatoryID);
                    table.ForeignKey(
                        name: "FK_LeaseSignatory_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaseSignatory_OrganizationMember_OrganizationMemberID",
                        column: x => x.OrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_LeaseSignatory_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_LeaseSignatory_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserID = table.Column<long>(type: "bigint", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RecipientPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TemplateCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ProviderMessageID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notification_AspNetUsers_RecipientUserID",
                        column: x => x.RecipientUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_OrganizationMember_OrganizationMemberID",
                        column: x => x.OrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_Notification_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_Notification_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Payout",
                columns: table => new
                {
                    PayoutID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrgPayoutAccountID = table.Column<long>(type: "bigint", nullable: false),
                    From = table.Column<DateTime>(type: "datetime2", nullable: true),
                    To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderPayoutID = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payout", x => x.PayoutID);
                    table.ForeignKey(
                        name: "FK_Payout_OrgPayoutAccount_OrgPayoutAccountID",
                        column: x => x.OrgPayoutAccountID,
                        principalTable: "OrgPayoutAccount",
                        principalColumn: "OrgPayoutAccountID");
                    table.ForeignKey(
                        name: "FK_Payout_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                });

            migrationBuilder.CreateTable(
                name: "RentalUnit",
                columns: table => new
                {
                    RentalUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FloorNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Bedrooms = table.Column<decimal>(type: "decimal(18,2)", maxLength: 3, nullable: true),
                    Bathrooms = table.Column<decimal>(type: "decimal(18,2)", maxLength: 3, nullable: true),
                    SquareFeet = table.Column<int>(type: "int", nullable: true),
                    MaximumOccupants = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalUnit", x => x.RentalUnitID);
                    table.ForeignKey(
                        name: "FK_RentalUnit_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID");
                    table.ForeignKey(
                        name: "FK_RentalUnit_UnitType_UnitTypeID",
                        column: x => x.UnitTypeID,
                        principalTable: "UnitType",
                        principalColumn: "UnitTypeID");
                });

            migrationBuilder.CreateTable(
                name: "Listing",
                columns: table => new
                {
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CheckInDoorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsFurnished = table.Column<bool>(type: "bit", nullable: false),
                    Bedrooms = table.Column<decimal>(type: "decimal(18,2)", maxLength: 3, nullable: true),
                    Bathrooms = table.Column<decimal>(type: "decimal(18,2)", maxLength: 3, nullable: true),
                    SquareFeet = table.Column<decimal>(type: "decimal(18,2)", maxLength: 3, nullable: true),
                    BaseMonthlyRentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecurityDepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearBuilt = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnpublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinimumLeaseMonths = table.Column<short>(type: "smallint", nullable: false),
                    MaximumLeaseMonths = table.Column<short>(type: "smallint", nullable: false),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    WIFINetwork = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WIFIPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcceptingApplications = table.Column<bool>(type: "bit", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listing", x => x.ListingID);
                    table.ForeignKey(
                        name: "FK_Listing_ListingType_ListingTypeID",
                        column: x => x.ListingTypeID,
                        principalTable: "ListingType",
                        principalColumn: "ListingTypeID");
                    table.ForeignKey(
                        name: "FK_Listing_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_Listing_RentalUnit_RentalUnitID",
                        column: x => x.RentalUnitID,
                        principalTable: "RentalUnit",
                        principalColumn: "RentalUnitID");
                });

            migrationBuilder.CreateTable(
                name: "ListingAmenity",
                columns: table => new
                {
                    ListingAmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAmenity", x => x.ListingAmenityID);
                    table.ForeignKey(
                        name: "FK_ListingAmenity_AmenityCatalog_AmenityID",
                        column: x => x.AmenityID,
                        principalTable: "AmenityCatalog",
                        principalColumn: "AmenityID");
                    table.ForeignKey(
                        name: "FK_ListingAmenity_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "ListingPhoto",
                columns: table => new
                {
                    ListingPhotoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AltText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsCoverPhoto = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingPhoto", x => x.ListingPhotoID);
                    table.ForeignKey(
                        name: "FK_ListingPhoto_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "ListingPolicy",
                columns: table => new
                {
                    ListingPolicyID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowsPets = table.Column<bool>(type: "bit", nullable: true),
                    AllowsSmoking = table.Column<bool>(type: "bit", nullable: true),
                    AllowsChildren = table.Column<bool>(type: "bit", nullable: true),
                    MaximumOccupants = table.Column<int>(type: "int", nullable: false),
                    Furnished = table.Column<bool>(type: "bit", nullable: true),
                    ParkingIncluded = table.Column<bool>(type: "bit", nullable: true),
                    UtilitiesIncluded = table.Column<bool>(type: "bit", nullable: true),
                    MinimumCreditScore = table.Column<int>(type: "int", nullable: true),
                    RequiresBackgroundCheck = table.Column<bool>(type: "bit", nullable: true),
                    ApplicationInstructions = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingPolicy", x => x.ListingPolicyID);
                    table.ForeignKey(
                        name: "FK_ListingPolicy_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "ListingRule",
                columns: table => new
                {
                    ListingRuleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RuleTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RuleDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsAllowed = table.Column<bool>(type: "bit", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingRule", x => x.ListingRuleID);
                    table.ForeignKey(
                        name: "FK_ListingRule_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "ListingTermPrice",
                columns: table => new
                {
                    ListingTermPriceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseTermMonths = table.Column<short>(type: "smallint", nullable: false),
                    MonthlyRentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecurityDepositAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingTermPrice", x => x.ListingTermPriceID);
                    table.ForeignKey(
                        name: "FK_ListingTermPrice_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "RentalApplication",
                columns: table => new
                {
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DesiredMoveInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesiredMoveOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedLeaseTermMonths = table.Column<short>(type: "smallint", nullable: false),
                    AdultOccupantCount = table.Column<int>(type: "int", nullable: false),
                    ChildOccupantCount = table.Column<int>(type: "int", nullable: false),
                    PetCount = table.Column<int>(type: "int", nullable: false),
                    ProposedMonthlyRentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScreeningStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedByOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclineReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalApplication", x => x.RentalApplicationID);
                    table.ForeignKey(
                        name: "FK_RentalApplication_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_RentalApplication_OrganizationMember_ReviewedByOrganizationMemberID",
                        column: x => x.ReviewedByOrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_RentalApplication_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_RentalApplication_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "ViewingAppointments",
                columns: table => new
                {
                    ViewingAppointmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserID = table.Column<long>(type: "bigint", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ViewingType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MeetingUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewingAppointments", x => x.ViewingAppointmentID);
                    table.ForeignKey(
                        name: "FK_ViewingAppointments_AspNetUsers_RequestedByUserID",
                        column: x => x.RequestedByUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ViewingAppointments_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_ViewingAppointments_OrganizationMember_AssignedOrganizationMemberID",
                        column: x => x.AssignedOrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_ViewingAppointments_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Lease",
                columns: table => new
                {
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenancyTypeID = table.Column<int>(type: "int", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeaseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaseTermMonths = table.Column<short>(type: "smallint", nullable: true),
                    BaseRentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    GracePeriodDays = table.Column<short>(type: "smallint", nullable: false),
                    LateFeeFixedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateFeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    RenewalNoticeDays = table.Column<int>(type: "int", nullable: true),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualMoveInAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualMoveOutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TerminationReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lease", x => x.LeaseID);
                    table.ForeignKey(
                        name: "FK_Lease_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_Lease_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_Lease_RentalApplication_RentalApplicationID",
                        column: x => x.RentalApplicationID,
                        principalTable: "RentalApplication",
                        principalColumn: "RentalApplicationID");
                    table.ForeignKey(
                        name: "FK_Lease_RentalUnit_RentalUnitID",
                        column: x => x.RentalUnitID,
                        principalTable: "RentalUnit",
                        principalColumn: "RentalUnitID");
                    table.ForeignKey(
                        name: "FK_Lease_TenancyType_TenancyTypeID",
                        column: x => x.TenancyTypeID,
                        principalTable: "TenancyType",
                        principalColumn: "TenancyTypeID");
                    table.ForeignKey(
                        name: "FK_Lease_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationHold",
                columns: table => new
                {
                    ReservationHoldID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoldReason = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedToLeaseAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationHold", x => x.ReservationHoldID);
                    table.ForeignKey(
                        name: "FK_ReservationHold_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_ReservationHold_RentalApplication_RentalApplicationID",
                        column: x => x.RentalApplicationID,
                        principalTable: "RentalApplication",
                        principalColumn: "RentalApplicationID");
                    table.ForeignKey(
                        name: "FK_ReservationHold_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "AutopayMandate",
                columns: table => new
                {
                    AutopayMandateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MandateType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentRail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaximumAmountPerDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderMandateID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ConsentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsentTextHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ConsentIpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConsentedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutopayMandate", x => x.AutopayMandateID);
                    table.ForeignKey(
                        name: "FK_AutopayMandate_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_AutopayMandate_PaymentMethod_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "PaymentMethod",
                        principalColumn: "PaymentMethodID");
                    table.ForeignKey(
                        name: "FK_AutopayMandate_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "CreditReportingEnrollment",
                columns: table => new
                {
                    CreditReportingEnrollmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsentedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditReportingEnrollment", x => x.CreditReportingEnrollmentID);
                    table.ForeignKey(
                        name: "FK_CreditReportingEnrollment_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_CreditReportingEnrollment_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Inspection",
                columns: table => new
                {
                    InspectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InspectionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OverallCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspection", x => x.InspectionID);
                    table.ForeignKey(
                        name: "FK_Inspection_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_Inspection_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID");
                    table.ForeignKey(
                        name: "FK_Inspection_RentalUnit_RentalUnitID",
                        column: x => x.RentalUnitID,
                        principalTable: "RentalUnit",
                        principalColumn: "RentalUnitID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceMaster",
                columns: table => new
                {
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BillingPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubtotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceMaster", x => x.InvoiceMasterID);
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_Lease_LeaseRenewalID",
                        column: x => x.LeaseRenewalID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseRecurringCharges",
                columns: table => new
                {
                    LeaseRecurringChargeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChargeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillingDayOfMonth = table.Column<short>(type: "smallint", nullable: true),
                    FirstDueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProrationRule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AutoGenerateInvoice = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseRecurringCharges", x => x.LeaseRecurringChargeID);
                    table.ForeignKey(
                        name: "FK_LeaseRecurringCharges_Fee_FeeID",
                        column: x => x.FeeID,
                        principalTable: "Fee",
                        principalColumn: "FeeID");
                    table.ForeignKey(
                        name: "FK_LeaseRecurringCharges_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseRenewals",
                columns: table => new
                {
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OfferedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OfferExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeclinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LeaseTermMonths = table.Column<short>(type: "smallint", nullable: true),
                    RentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecurityDepositAdjustmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseRenewals", x => x.LeaseRenewalID);
                    table.ForeignKey(
                        name: "FK_LeaseRenewals_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                });

            migrationBuilder.CreateTable(
                name: "ListingAccessInstruction",
                columns: table => new
                {
                    ListingAccessInstructionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstructionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SecretReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAccessInstruction", x => x.ListingAccessInstructionID);
                    table.ForeignKey(
                        name: "FK_ListingAccessInstruction_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_ListingAccessInstruction_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequest",
                columns: table => new
                {
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SubmittedByTenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PermissionToEnter = table.Column<bool>(type: "bit", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequest", x => x.MaintenanceRequestID);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Category_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Category",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_RentalUnit_RentalUnitID",
                        column: x => x.RentalUnitID,
                        principalTable: "RentalUnit",
                        principalColumn: "RentalUnitID");
                    table.ForeignKey(
                        name: "FK_MaintenanceRequest_Tenant_SubmittedByTenantID",
                        column: x => x.SubmittedByTenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    RatingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerUserID = table.Column<long>(type: "bigint", nullable: false),
                    SubjectType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectReferenceID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OverallRating = table.Column<short>(type: "smallint", nullable: false),
                    PaymentRating = table.Column<short>(type: "smallint", nullable: true),
                    CommunicationRating = table.Column<short>(type: "smallint", nullable: true),
                    PropertyCareRating = table.Column<short>(type: "smallint", nullable: true),
                    ResponsivenessRating = table.Column<short>(type: "smallint", nullable: true),
                    AccuracyRating = table.Column<short>(type: "smallint", nullable: true),
                    CleanlinessRating = table.Column<short>(type: "smallint", nullable: true),
                    ReviewBody = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.RatingID);
                    table.ForeignKey(
                        name: "FK_Rating_AspNetUsers_ReviewerUserID",
                        column: x => x.ReviewerUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rating_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                });

            migrationBuilder.CreateTable(
                name: "SecurityDeposit",
                columns: table => new
                {
                    SecurityDepositID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiredAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceivedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReturnedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullyFundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HeldAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDeposit", x => x.SecurityDepositID);
                    table.ForeignKey(
                        name: "FK_SecurityDeposit_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_SecurityDeposit_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_SecurityDeposit_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "TenantInvitation",
                columns: table => new
                {
                    TenantInvitationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvitationPurpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TokenHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantInvitation", x => x.TenantInvitationID);
                    table.ForeignKey(
                        name: "FK_TenantInvitation_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_TenantInvitation_RentalApplication_RentalApplicationID",
                        column: x => x.RentalApplicationID,
                        principalTable: "RentalApplication",
                        principalColumn: "RentalApplicationID");
                });

            migrationBuilder.CreateTable(
                name: "AutopayConsentAudit",
                columns: table => new
                {
                    AutopayConsentAuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutopayMandateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConsentTextHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutopayConsentAudit", x => x.AutopayConsentAuditID);
                    table.ForeignKey(
                        name: "FK_AutopayConsentAudit_AutopayMandate_AutopayMandateID",
                        column: x => x.AutopayMandateID,
                        principalTable: "AutopayMandate",
                        principalColumn: "AutopayMandateID");
                    table.ForeignKey(
                        name: "FK_AutopayConsentAudit_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "CreditReportingConsentAudit",
                columns: table => new
                {
                    ConsentAuditID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditReportingEnrollmentID = table.Column<Guid>(type: "uniqueidentifier", maxLength: 256, nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConsentVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConsentTextHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditReportingConsentAudit", x => x.ConsentAuditID);
                    table.ForeignKey(
                        name: "FK_CreditReportingConsentAudit_CreditReportingEnrollment_CreditReportingEnrollmentID",
                        column: x => x.CreditReportingEnrollmentID,
                        principalTable: "CreditReportingEnrollment",
                        principalColumn: "CreditReportingEnrollmentID");
                    table.ForeignKey(
                        name: "FK_CreditReportingConsentAudit_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "InspectionItem",
                columns: table => new
                {
                    InspectionItemID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InspectionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresRepair = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedRepairCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionItem", x => x.InspectionItemID);
                    table.ForeignKey(
                        name: "FK_InspectionItem_Inspection_InspectionID",
                        column: x => x.InspectionID,
                        principalTable: "Inspection",
                        principalColumn: "InspectionID");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetail",
                columns: table => new
                {
                    InvoiceDetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRecurringChargeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LineType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ServicePeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServicePeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalLineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetail", x => x.InvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_Fee_FeeID",
                        column: x => x.FeeID,
                        principalTable: "Fee",
                        principalColumn: "FeeID");
                    table.ForeignKey(
                        name: "FK_InvoiceDetail_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentIntent",
                columns: table => new
                {
                    PaymentIntentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AutopayMandateID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CollectionMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProviderPaymentIntentID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ScheduledChargeAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentIntent", x => x.PaymentIntentID);
                    table.ForeignKey(
                        name: "FK_PaymentIntent_AutopayMandate_AutopayMandateID",
                        column: x => x.AutopayMandateID,
                        principalTable: "AutopayMandate",
                        principalColumn: "AutopayMandateID");
                    table.ForeignKey(
                        name: "FK_PaymentIntent_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_PaymentIntent_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_PaymentIntent_PaymentMethod_PaymentMethodID",
                        column: x => x.PaymentMethodID,
                        principalTable: "PaymentMethod",
                        principalColumn: "PaymentMethodID");
                    table.ForeignKey(
                        name: "FK_PaymentIntent_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentReminder",
                columns: table => new
                {
                    PaymentReminderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MessageSubject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MessageBody = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentReminder", x => x.PaymentReminderID);
                    table.ForeignKey(
                        name: "FK_PaymentReminder_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseDocuments",
                columns: table => new
                {
                    LeaseDocumentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DocumentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OriginalFilename = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StorageProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StorageContainer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentForSignatureAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullySignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseDocuments", x => x.LeaseDocumentID);
                    table.ForeignKey(
                        name: "FK_LeaseDocuments_LeaseRenewals_LeaseRenewalID",
                        column: x => x.LeaseRenewalID,
                        principalTable: "LeaseRenewals",
                        principalColumn: "LeaseRenewalID");
                    table.ForeignKey(
                        name: "FK_LeaseDocuments_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                });

            migrationBuilder.CreateTable(
                name: "LeaseOccupants",
                columns: table => new
                {
                    LeaseOccupantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OccupantType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPrimaryTenant = table.Column<bool>(type: "bit", nullable: false),
                    IsFinanciallyResponsible = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseOccupants", x => x.LeaseOccupantID);
                    table.ForeignKey(
                        name: "FK_LeaseOccupants_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeaseOccupants_LeaseRenewals_LeaseRenewalID",
                        column: x => x.LeaseRenewalID,
                        principalTable: "LeaseRenewals",
                        principalColumn: "LeaseRenewalID");
                    table.ForeignKey(
                        name: "FK_LeaseOccupants_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_LeaseOccupants_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvent",
                columns: table => new
                {
                    CalendarEventID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RentalApplicationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReservationHoldID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OccupantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OccupantCount = table.Column<int>(type: "int", nullable: true),
                    SourceSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SourceReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExternalCalendarID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BlocksAvailability = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvent", x => x.CalendarEventID);
                    table.ForeignKey(
                        name: "FK_CalendarEvent_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_CalendarEvent_Listing_ListingID",
                        column: x => x.ListingID,
                        principalTable: "Listing",
                        principalColumn: "ListingID");
                    table.ForeignKey(
                        name: "FK_CalendarEvent_MaintenanceRequest_MaintenanceRequestID",
                        column: x => x.MaintenanceRequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "MaintenanceRequestID");
                    table.ForeignKey(
                        name: "FK_CalendarEvent_RentalApplication_RentalApplicationID",
                        column: x => x.RentalApplicationID,
                        principalTable: "RentalApplication",
                        principalColumn: "RentalApplicationID");
                    table.ForeignKey(
                        name: "FK_CalendarEvent_ReservationHold_ReservationHoldID",
                        column: x => x.ReservationHoldID,
                        principalTable: "ReservationHold",
                        principalColumn: "ReservationHoldID");
                });

            migrationBuilder.CreateTable(
                name: "WorkOrder",
                columns: table => new
                {
                    WorkOrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignedOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FinalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrder", x => x.WorkOrderID);
                    table.ForeignKey(
                        name: "FK_WorkOrder_Contractor_ContractorID",
                        column: x => x.ContractorID,
                        principalTable: "Contractor",
                        principalColumn: "ContractorID");
                    table.ForeignKey(
                        name: "FK_WorkOrder_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_WorkOrder_MaintenanceRequest_MaintenanceRequestID",
                        column: x => x.MaintenanceRequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "MaintenanceRequestID");
                    table.ForeignKey(
                        name: "FK_WorkOrder_OrganizationMember_AssignedOrganizationMemberID",
                        column: x => x.AssignedOrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentIntentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderChargeID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlatformFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProcessorFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payment_PaymentIntent_PaymentIntentID",
                        column: x => x.PaymentIntentID,
                        principalTable: "PaymentIntent",
                        principalColumn: "PaymentIntentID");
                    table.ForeignKey(
                        name: "FK_Payment_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempt",
                columns: table => new
                {
                    PaymentAttemptID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentIntentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderAttemptID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FailureCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureMessage = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AttemptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRetryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderResponse = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempt", x => x.PaymentAttemptID);
                    table.ForeignKey(
                        name: "FK_PaymentAttempt_PaymentIntent_PaymentIntentID",
                        column: x => x.PaymentIntentID,
                        principalTable: "PaymentIntent",
                        principalColumn: "PaymentIntentID");
                });

            migrationBuilder.CreateTable(
                name: "Chargeback",
                columns: table => new
                {
                    ChargebackID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderDisputeID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ReasonCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvidenceDueAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvidenceSubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProviderResponse = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chargeback", x => x.ChargebackID);
                    table.ForeignKey(
                        name: "FK_Chargeback_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_Chargeback_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_Chargeback_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "CreditReporting",
                columns: table => new
                {
                    CreditReportingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditReportingEnrollmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WasPaidOnTime = table.Column<bool>(type: "bit", nullable: true),
                    ReportingPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReportingPeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProviderStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderReferenceID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderResponse = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditReporting", x => x.CreditReportingID);
                    table.ForeignKey(
                        name: "FK_CreditReporting_CreditReportingEnrollment_CreditReportingEnrollmentID",
                        column: x => x.CreditReportingEnrollmentID,
                        principalTable: "CreditReportingEnrollment",
                        principalColumn: "CreditReportingEnrollmentID");
                    table.ForeignKey(
                        name: "FK_CreditReporting_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_CreditReporting_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentAllocation",
                columns: table => new
                {
                    PaymentAllocationID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceDetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AllocationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AllocatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAllocation", x => x.PaymentAllocationID);
                    table.ForeignKey(
                        name: "FK_PaymentAllocation_InvoiceDetail_InvoiceDetailID",
                        column: x => x.InvoiceDetailID,
                        principalTable: "InvoiceDetail",
                        principalColumn: "InvoiceDetailID");
                    table.ForeignKey(
                        name: "FK_PaymentAllocation_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_PaymentAllocation_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                });

            migrationBuilder.CreateTable(
                name: "PayoutItem",
                columns: table => new
                {
                    PayoutItemID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PayoutID = table.Column<long>(type: "bigint", nullable: false),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeductionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetPayoutAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutItem", x => x.PayoutItemID);
                    table.ForeignKey(
                        name: "FK_PayoutItem_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_PayoutItem_Payout_PayoutID",
                        column: x => x.PayoutID,
                        principalTable: "Payout",
                        principalColumn: "PayoutID");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptMaster",
                columns: table => new
                {
                    ReceiptMasterID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptMaster", x => x.ReceiptMasterID);
                    table.ForeignKey(
                        name: "FK_ReceiptMaster_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_ReceiptMaster_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Refund",
                columns: table => new
                {
                    RefundID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProviderRefundID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refund", x => x.RefundID);
                    table.ForeignKey(
                        name: "FK_Refund_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_Refund_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "Dispute",
                columns: table => new
                {
                    DisputeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChargebackID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispute", x => x.DisputeID);
                    table.ForeignKey(
                        name: "FK_Dispute_Chargeback_ChargebackID",
                        column: x => x.ChargebackID,
                        principalTable: "Chargeback",
                        principalColumn: "ChargebackID");
                    table.ForeignKey(
                        name: "FK_Dispute_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_Dispute_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_Dispute_MaintenanceRequest_MaintenanceRequestID",
                        column: x => x.MaintenanceRequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "MaintenanceRequestID");
                    table.ForeignKey(
                        name: "FK_Dispute_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_Dispute_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_Dispute_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "FraudCase",
                columns: table => new
                {
                    FraudCaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentIntentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChargebackID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RiskScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudCase", x => x.FraudCaseID);
                    table.ForeignKey(
                        name: "FK_FraudCase_Chargeback_ChargebackID",
                        column: x => x.ChargebackID,
                        principalTable: "Chargeback",
                        principalColumn: "ChargebackID");
                    table.ForeignKey(
                        name: "FK_FraudCase_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_FraudCase_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_FraudCase_PaymentIntent_PaymentIntentID",
                        column: x => x.PaymentIntentID,
                        principalTable: "PaymentIntent",
                        principalColumn: "PaymentIntentID");
                    table.ForeignKey(
                        name: "FK_FraudCase_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_FraudCase_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "LedgerTransaction",
                columns: table => new
                {
                    LedgerTransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefundID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PayoutID = table.Column<long>(type: "bigint", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReversedTransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerTransaction", x => x.LedgerTransactionID);
                    table.ForeignKey(
                        name: "FK_LedgerTransaction_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_LedgerTransaction_Organization_OrganizationID",
                        column: x => x.OrganizationID,
                        principalTable: "Organization",
                        principalColumn: "OrganizationID");
                    table.ForeignKey(
                        name: "FK_LedgerTransaction_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_LedgerTransaction_Payout_PayoutID",
                        column: x => x.PayoutID,
                        principalTable: "Payout",
                        principalColumn: "PayoutID");
                    table.ForeignKey(
                        name: "FK_LedgerTransaction_Refund_RefundID",
                        column: x => x.RefundID,
                        principalTable: "Refund",
                        principalColumn: "RefundID");
                });

            migrationBuilder.CreateTable(
                name: "SecurityDepositTransaction",
                columns: table => new
                {
                    SecurityDepositTransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityDepositID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefundID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceMasterID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceDetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDepositTransaction", x => x.SecurityDepositTransactionID);
                    table.ForeignKey(
                        name: "FK_SecurityDepositTransaction_InvoiceDetail_InvoiceDetailID",
                        column: x => x.InvoiceDetailID,
                        principalTable: "InvoiceDetail",
                        principalColumn: "InvoiceDetailID");
                    table.ForeignKey(
                        name: "FK_SecurityDepositTransaction_InvoiceMaster_InvoiceMasterID",
                        column: x => x.InvoiceMasterID,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceMasterID");
                    table.ForeignKey(
                        name: "FK_SecurityDepositTransaction_Payment_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payment",
                        principalColumn: "PaymentID");
                    table.ForeignKey(
                        name: "FK_SecurityDepositTransaction_Refund_RefundID",
                        column: x => x.RefundID,
                        principalTable: "Refund",
                        principalColumn: "RefundID");
                    table.ForeignKey(
                        name: "FK_SecurityDepositTransaction_SecurityDeposit_SecurityDepositID",
                        column: x => x.SecurityDepositID,
                        principalTable: "SecurityDeposit",
                        principalColumn: "SecurityDepositID");
                });

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LeaseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LeaseRenewalID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceRequestID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DisputeID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.ConversationID);
                    table.ForeignKey(
                        name: "FK_Conversation_Dispute_DisputeID",
                        column: x => x.DisputeID,
                        principalTable: "Dispute",
                        principalColumn: "DisputeID");
                    table.ForeignKey(
                        name: "FK_Conversation_Lease_LeaseID",
                        column: x => x.LeaseID,
                        principalTable: "Lease",
                        principalColumn: "LeaseID");
                    table.ForeignKey(
                        name: "FK_Conversation_MaintenanceRequest_MaintenanceRequestID",
                        column: x => x.MaintenanceRequestID,
                        principalTable: "MaintenanceRequest",
                        principalColumn: "MaintenanceRequestID");
                });

            migrationBuilder.CreateTable(
                name: "LedgerEntry",
                columns: table => new
                {
                    LedgerEntryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LedgerTransactionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LedgerAccountID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntry", x => x.LedgerEntryID);
                    table.ForeignKey(
                        name: "FK_LedgerEntry_LedgerAccount_LedgerAccountID",
                        column: x => x.LedgerAccountID,
                        principalTable: "LedgerAccount",
                        principalColumn: "LedgerAccountID");
                    table.ForeignKey(
                        name: "FK_LedgerEntry_LedgerTransaction_LedgerTransactionID",
                        column: x => x.LedgerTransactionID,
                        principalTable: "LedgerTransaction",
                        principalColumn: "LedgerTransactionID");
                });

            migrationBuilder.CreateTable(
                name: "ConversationMessage",
                columns: table => new
                {
                    ConversationMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserID = table.Column<long>(type: "bigint", nullable: true),
                    SenderTenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderOrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReplyToMessageID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CapturedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMessage", x => x.ConversationMessageID);
                    table.ForeignKey(
                        name: "FK_ConversationMessage_AspNetUsers_SenderUserID",
                        column: x => x.SenderUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversationMessage_ConversationMessage_ReplyToMessageID",
                        column: x => x.ReplyToMessageID,
                        principalTable: "ConversationMessage",
                        principalColumn: "ConversationMessageID");
                    table.ForeignKey(
                        name: "FK_ConversationMessage_Conversation_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversation",
                        principalColumn: "ConversationID");
                    table.ForeignKey(
                        name: "FK_ConversationMessage_OrganizationMember_SenderOrganizationMemberID",
                        column: x => x.SenderOrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_ConversationMessage_Tenant_SenderTenantID",
                        column: x => x.SenderTenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateTable(
                name: "ConversationParticipant",
                columns: table => new
                {
                    ConversationParticipantID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConversationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<long>(type: "bigint", nullable: true),
                    TenantID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationMemberID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParticipantRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeftAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsMuted = table.Column<bool>(type: "bit", nullable: false),
                    CapturedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationParticipant", x => x.ConversationParticipantID);
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Conversation_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversation",
                        principalColumn: "ConversationID");
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_OrganizationMember_OrganizationMemberID",
                        column: x => x.OrganizationMemberID,
                        principalTable: "OrganizationMember",
                        principalColumn: "OrganizationMemberID");
                    table.ForeignKey(
                        name: "FK_ConversationParticipant_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_OrganizationID",
                table: "Address",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationOccupant_TenantID",
                table: "ApplicationOccupant",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationOccupant_UserID",
                table: "ApplicationOccupant",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_ActorUserID",
                table: "AuditLog",
                column: "ActorUserID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_OrganizationID",
                table: "AuditLog",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_OrganizationMemberID",
                table: "AuditLog",
                column: "OrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_TenantID",
                table: "AuditLog",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayConsentAudit_AutopayMandateID",
                table: "AutopayConsentAudit",
                column: "AutopayMandateID");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayConsentAudit_TenantID",
                table: "AutopayConsentAudit",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayMandate_LeaseID",
                table: "AutopayMandate",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayMandate_PaymentMethodID",
                table: "AutopayMandate",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_AutopayMandate_TenantID",
                table: "AutopayMandate",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_LeaseID",
                table: "CalendarEvent",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_ListingID",
                table: "CalendarEvent",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_MaintenanceRequestID",
                table: "CalendarEvent",
                column: "MaintenanceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_RentalApplicationID",
                table: "CalendarEvent",
                column: "RentalApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvent_ReservationHoldID",
                table: "CalendarEvent",
                column: "ReservationHoldID");

            migrationBuilder.CreateIndex(
                name: "IX_Chargeback_OrganizationID",
                table: "Chargeback",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Chargeback_PaymentID",
                table: "Chargeback",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Chargeback_TenantID",
                table: "Chargeback",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Contractor_OrganizationID",
                table: "Contractor",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_DisputeID",
                table: "Conversation",
                column: "DisputeID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_LeaseID",
                table: "Conversation",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_MaintenanceRequestID",
                table: "Conversation",
                column: "MaintenanceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_ConversationID",
                table: "ConversationMessage",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_ReplyToMessageID",
                table: "ConversationMessage",
                column: "ReplyToMessageID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_SenderOrganizationMemberID",
                table: "ConversationMessage",
                column: "SenderOrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_SenderTenantID",
                table: "ConversationMessage",
                column: "SenderTenantID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_SenderUserID",
                table: "ConversationMessage",
                column: "SenderUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_ConversationID",
                table: "ConversationParticipant",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_OrganizationMemberID",
                table: "ConversationParticipant",
                column: "OrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_TenantID",
                table: "ConversationParticipant",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipant_UserID",
                table: "ConversationParticipant",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReporting_CreditReportingEnrollmentID",
                table: "CreditReporting",
                column: "CreditReportingEnrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReporting_InvoiceMasterID",
                table: "CreditReporting",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReporting_PaymentID",
                table: "CreditReporting",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReportingConsentAudit_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudit",
                column: "CreditReportingEnrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReportingConsentAudit_TenantID",
                table: "CreditReportingConsentAudit",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReportingEnrollment_LeaseID",
                table: "CreditReportingEnrollment",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_CreditReportingEnrollment_TenantID",
                table: "CreditReportingEnrollment",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_ChargebackID",
                table: "Dispute",
                column: "ChargebackID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_InvoiceMasterID",
                table: "Dispute",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_LeaseID",
                table: "Dispute",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_MaintenanceRequestID",
                table: "Dispute",
                column: "MaintenanceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_OrganizationID",
                table: "Dispute",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_PaymentID",
                table: "Dispute",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Dispute_TenantID",
                table: "Dispute",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Fee_FeeTypeID",
                table: "Fee",
                column: "FeeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Fee_OrganizationID",
                table: "Fee",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_ChargebackID",
                table: "FraudCase",
                column: "ChargebackID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_LeaseID",
                table: "FraudCase",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_OrganizationID",
                table: "FraudCase",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_PaymentID",
                table: "FraudCase",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_PaymentIntentID",
                table: "FraudCase",
                column: "PaymentIntentID");

            migrationBuilder.CreateIndex(
                name: "IX_FraudCase_TenantID",
                table: "FraudCase",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityVerification_UserID",
                table: "IdentityVerification",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_LeaseID",
                table: "Inspection",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_PropertyID",
                table: "Inspection",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_RentalUnitID",
                table: "Inspection",
                column: "RentalUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionItem_InspectionID",
                table: "InspectionItem",
                column: "InspectionID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_FeeID",
                table: "InvoiceDetail",
                column: "FeeID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_InvoiceMasterID",
                table: "InvoiceDetail",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_LeaseID",
                table: "InvoiceMaster",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_LeaseRenewalID",
                table: "InvoiceMaster",
                column: "LeaseRenewalID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_OrganizationID",
                table: "InvoiceMaster",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_TenantID",
                table: "InvoiceMaster",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_ListingID",
                table: "Lease",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_OrganizationID",
                table: "Lease",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_RentalApplicationID",
                table: "Lease",
                column: "RentalApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_RentalUnitID",
                table: "Lease",
                column: "RentalUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_TenancyTypeID",
                table: "Lease",
                column: "TenancyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Lease_TenantID",
                table: "Lease",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseDocuments_LeaseID",
                table: "LeaseDocuments",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseDocuments_LeaseRenewalID",
                table: "LeaseDocuments",
                column: "LeaseRenewalID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOccupants_LeaseID",
                table: "LeaseOccupants",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOccupants_LeaseRenewalID",
                table: "LeaseOccupants",
                column: "LeaseRenewalID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOccupants_TenantID",
                table: "LeaseOccupants",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOccupants_UserID",
                table: "LeaseOccupants",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseRecurringCharges_FeeID",
                table: "LeaseRecurringCharges",
                column: "FeeID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseRecurringCharges_LeaseID",
                table: "LeaseRecurringCharges",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseRenewals_LeaseID",
                table: "LeaseRenewals",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseSignatory_OrganizationID",
                table: "LeaseSignatory",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseSignatory_OrganizationMemberID",
                table: "LeaseSignatory",
                column: "OrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseSignatory_TenantID",
                table: "LeaseSignatory",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseSignatory_UserID",
                table: "LeaseSignatory",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_OrganizationID",
                table: "LedgerAccount",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_LedgerAccountID",
                table: "LedgerEntry",
                column: "LedgerAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntry_LedgerTransactionID",
                table: "LedgerEntry",
                column: "LedgerTransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransaction_InvoiceMasterID",
                table: "LedgerTransaction",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransaction_OrganizationID",
                table: "LedgerTransaction",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransaction_PaymentID",
                table: "LedgerTransaction",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransaction_PayoutID",
                table: "LedgerTransaction",
                column: "PayoutID");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerTransaction_RefundID",
                table: "LedgerTransaction",
                column: "RefundID");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_ListingTypeID",
                table: "Listing",
                column: "ListingTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_OrganizationID",
                table: "Listing",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_RentalUnitID",
                table: "Listing",
                column: "RentalUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAccessInstruction_LeaseID",
                table: "ListingAccessInstruction",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAccessInstruction_ListingID",
                table: "ListingAccessInstruction",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAmenity_AmenityID",
                table: "ListingAmenity",
                column: "AmenityID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAmenity_ListingID",
                table: "ListingAmenity",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingPhoto_ListingID",
                table: "ListingPhoto",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingPolicy_ListingID",
                table: "ListingPolicy",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingRule_ListingID",
                table: "ListingRule",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ListingTermPrice_ListingID",
                table: "ListingTermPrice",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_CategoryID",
                table: "MaintenanceRequest",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_LeaseID",
                table: "MaintenanceRequest",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_ListingID",
                table: "MaintenanceRequest",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_PropertyID",
                table: "MaintenanceRequest",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_RentalUnitID",
                table: "MaintenanceRequest",
                column: "RentalUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequest_SubmittedByTenantID",
                table: "MaintenanceRequest",
                column: "SubmittedByTenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrganizationID",
                table: "Notification",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_OrganizationMemberID",
                table: "Notification",
                column: "OrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_RecipientUserID",
                table: "Notification",
                column: "RecipientUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_TenantID",
                table: "Notification",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_OrganizationID",
                table: "OrganizationMember",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMember_UserID",
                table: "OrganizationMember",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationStatement_OrganizationID",
                table: "OrganizationStatement",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrgPayoutAccount_OrganizationID",
                table: "OrgPayoutAccount",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrgSubscription_OrganizationID",
                table: "OrgSubscription",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_OrgSubscription_SubscriptionPlanID",
                table: "OrgSubscription",
                column: "SubscriptionPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentIntentID",
                table: "Payment",
                column: "PaymentIntentID");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_TenantID",
                table: "Payment",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocation_InvoiceDetailID",
                table: "PaymentAllocation",
                column: "InvoiceDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocation_InvoiceMasterID",
                table: "PaymentAllocation",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocation_PaymentID",
                table: "PaymentAllocation",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempt_PaymentIntentID",
                table: "PaymentAttempt",
                column: "PaymentIntentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntent_AutopayMandateID",
                table: "PaymentIntent",
                column: "AutopayMandateID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntent_InvoiceMasterID",
                table: "PaymentIntent",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntent_LeaseID",
                table: "PaymentIntent",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntent_PaymentMethodID",
                table: "PaymentIntent",
                column: "PaymentMethodID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentIntent_TenantID",
                table: "PaymentIntent",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_TenantID",
                table: "PaymentMethod",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentReminder_InvoiceMasterID",
                table: "PaymentReminder",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_OrganizationID",
                table: "Payout",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_OrgPayoutAccountID",
                table: "Payout",
                column: "OrgPayoutAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutItem_PaymentID",
                table: "PayoutItem",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutItem_PayoutID",
                table: "PayoutItem",
                column: "PayoutID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_AddressID",
                table: "Property",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_OrganizationID",
                table: "Property",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_LeaseID",
                table: "Rating",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_ReviewerUserID",
                table: "Rating",
                column: "ReviewerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptMaster_PaymentID",
                table: "ReceiptMaster",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptMaster_TenantID",
                table: "ReceiptMaster",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_PaymentID",
                table: "Refund",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_TenantID",
                table: "Refund",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplication_ListingID",
                table: "RentalApplication",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplication_OrganizationID",
                table: "RentalApplication",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplication_ReviewedByOrganizationMemberID",
                table: "RentalApplication",
                column: "ReviewedByOrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplication_TenantID",
                table: "RentalApplication",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalUnit_PropertyID",
                table: "RentalUnit",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_RentalUnit_UnitTypeID",
                table: "RentalUnit",
                column: "UnitTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationHold_ListingID",
                table: "ReservationHold",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationHold_RentalApplicationID",
                table: "ReservationHold",
                column: "RentalApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationHold_TenantID",
                table: "ReservationHold",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposit_LeaseID",
                table: "SecurityDeposit",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposit_OrganizationID",
                table: "SecurityDeposit",
                column: "OrganizationID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposit_TenantID",
                table: "SecurityDeposit",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositTransaction_InvoiceDetailID",
                table: "SecurityDepositTransaction",
                column: "InvoiceDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositTransaction_InvoiceMasterID",
                table: "SecurityDepositTransaction",
                column: "InvoiceMasterID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositTransaction_PaymentID",
                table: "SecurityDepositTransaction",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositTransaction_RefundID",
                table: "SecurityDepositTransaction",
                column: "RefundID");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositTransaction_SecurityDepositID",
                table: "SecurityDepositTransaction",
                column: "SecurityDepositID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_UserID",
                table: "Tenant",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantEmergencyContact_TenantID",
                table: "TenantEmergencyContact",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantEmployment_TenantID",
                table: "TenantEmployment",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantGuarantor_TenantID",
                table: "TenantGuarantor",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantGuarantor_UserID",
                table: "TenantGuarantor",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvitation_LeaseID",
                table: "TenantInvitation",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantInvitation_RentalApplicationID",
                table: "TenantInvitation",
                column: "RentalApplicationID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantScreeningCheck_TenantID",
                table: "TenantScreeningCheck",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingAppointments_AssignedOrganizationMemberID",
                table: "ViewingAppointments",
                column: "AssignedOrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingAppointments_ListingID",
                table: "ViewingAppointments",
                column: "ListingID");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingAppointments_RequestedByUserID",
                table: "ViewingAppointments",
                column: "RequestedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ViewingAppointments_TenantID",
                table: "ViewingAppointments",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_AssignedOrganizationMemberID",
                table: "WorkOrder",
                column: "AssignedOrganizationMemberID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_ContractorID",
                table: "WorkOrder",
                column: "ContractorID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_LeaseID",
                table: "WorkOrder",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_MaintenanceRequestID",
                table: "WorkOrder",
                column: "MaintenanceRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationOccupant");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "AutopayConsentAudit");

            migrationBuilder.DropTable(
                name: "CalendarEvent");

            migrationBuilder.DropTable(
                name: "ConversationMessage");

            migrationBuilder.DropTable(
                name: "ConversationParticipant");

            migrationBuilder.DropTable(
                name: "CreditReporting");

            migrationBuilder.DropTable(
                name: "CreditReportingConsentAudit");

            migrationBuilder.DropTable(
                name: "FraudCase");

            migrationBuilder.DropTable(
                name: "IdentityVerification");

            migrationBuilder.DropTable(
                name: "InspectionItem");

            migrationBuilder.DropTable(
                name: "LeaseDocExtractedTerm");

            migrationBuilder.DropTable(
                name: "LeaseDocuments");

            migrationBuilder.DropTable(
                name: "LeaseOccupants");

            migrationBuilder.DropTable(
                name: "LeaseRecurringCharges");

            migrationBuilder.DropTable(
                name: "LeaseSignatory");

            migrationBuilder.DropTable(
                name: "LedgerEntry");

            migrationBuilder.DropTable(
                name: "ListingAccessInstruction");

            migrationBuilder.DropTable(
                name: "ListingAmenity");

            migrationBuilder.DropTable(
                name: "ListingPhoto");

            migrationBuilder.DropTable(
                name: "ListingPolicy");

            migrationBuilder.DropTable(
                name: "ListingRule");

            migrationBuilder.DropTable(
                name: "ListingTermPrice");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrganizationStatement");

            migrationBuilder.DropTable(
                name: "OrgSubscription");

            migrationBuilder.DropTable(
                name: "PaymentAllocation");

            migrationBuilder.DropTable(
                name: "PaymentAttempt");

            migrationBuilder.DropTable(
                name: "PaymentProviderEvent");

            migrationBuilder.DropTable(
                name: "PaymentReminder");

            migrationBuilder.DropTable(
                name: "PayoutItem");

            migrationBuilder.DropTable(
                name: "Preference");

            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "ReceiptMaster");

            migrationBuilder.DropTable(
                name: "SecurityDepositTransaction");

            migrationBuilder.DropTable(
                name: "TaxRate");

            migrationBuilder.DropTable(
                name: "TenantEmergencyContact");

            migrationBuilder.DropTable(
                name: "TenantEmployment");

            migrationBuilder.DropTable(
                name: "TenantGuarantor");

            migrationBuilder.DropTable(
                name: "TenantInvitation");

            migrationBuilder.DropTable(
                name: "TenantScreeningCheck");

            migrationBuilder.DropTable(
                name: "ViewingAppointments");

            migrationBuilder.DropTable(
                name: "WorkOrder");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ReservationHold");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "CreditReportingEnrollment");

            migrationBuilder.DropTable(
                name: "Inspection");

            migrationBuilder.DropTable(
                name: "LeaseRenewals");

            migrationBuilder.DropTable(
                name: "LedgerAccount");

            migrationBuilder.DropTable(
                name: "LedgerTransaction");

            migrationBuilder.DropTable(
                name: "AmenityCatalog");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "InvoiceDetail");

            migrationBuilder.DropTable(
                name: "SecurityDeposit");

            migrationBuilder.DropTable(
                name: "Contractor");

            migrationBuilder.DropTable(
                name: "Dispute");

            migrationBuilder.DropTable(
                name: "Payout");

            migrationBuilder.DropTable(
                name: "Refund");

            migrationBuilder.DropTable(
                name: "Fee");

            migrationBuilder.DropTable(
                name: "Chargeback");

            migrationBuilder.DropTable(
                name: "MaintenanceRequest");

            migrationBuilder.DropTable(
                name: "OrgPayoutAccount");

            migrationBuilder.DropTable(
                name: "FeeType");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "PaymentIntent");

            migrationBuilder.DropTable(
                name: "AutopayMandate");

            migrationBuilder.DropTable(
                name: "InvoiceMaster");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Lease");

            migrationBuilder.DropTable(
                name: "RentalApplication");

            migrationBuilder.DropTable(
                name: "TenancyType");

            migrationBuilder.DropTable(
                name: "Listing");

            migrationBuilder.DropTable(
                name: "OrganizationMember");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "ListingType");

            migrationBuilder.DropTable(
                name: "RentalUnit");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "UnitType");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Organization");
        }
    }
}
