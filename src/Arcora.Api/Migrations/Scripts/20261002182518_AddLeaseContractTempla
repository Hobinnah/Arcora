IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AmenityCatalog] (
    [AmenityID] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Category] nvarchar(100) NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_AmenityCatalog] PRIMARY KEY ([AmenityID])
);
GO

CREATE TABLE [AspNetRoles] (
    [Id] bigint NOT NULL IDENTITY,
    [Description] nvarchar(max) NULL,
    [Enabled] bit NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] bigint NOT NULL IDENTITY,
    [DisplayName] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [ClientName] nvarchar(max) NULL,
    [BranchName] nvarchar(max) NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Attachment] (
    [AttachmentID] uniqueidentifier NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [EntityID] nvarchar(100) NOT NULL,
    [FileName] nvarchar(100) NOT NULL,
    [MimeType] nvarchar(100) NULL,
    [FileSizeBytes] bigint NULL,
    [StorageProvider] nvarchar(100) NULL,
    [StorageReference] nvarchar(500) NOT NULL,
    [AttachmentType] nvarchar(50) NULL,
    [Description] nvarchar(255) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Attachment] PRIMARY KEY ([AttachmentID])
);
GO

CREATE TABLE [Category] (
    [CategoryID] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Maintenance] bit NULL,
    [Contractor] bit NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([CategoryID])
);
GO

CREATE TABLE [FeeType] (
    [FeeTypeID] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [IsPlatformFee] bit NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NULL,
    CONSTRAINT [PK_FeeType] PRIMARY KEY ([FeeTypeID])
);
GO

CREATE TABLE [LeaseDocExtractedTerm] (
    [LeaseDocExtractedTermID] uniqueidentifier NOT NULL,
    [LeaseDocumentID] uniqueidentifier NOT NULL,
    [ExtractedStartDate] datetime2 NULL,
    [ExtractedEndDate] datetime2 NULL,
    [ExtractedRentAmount] decimal(18,2) NULL,
    [ExtractedDepositAmount] decimal(18,2) NULL,
    [ExtractedRenewalDate] datetime2 NULL,
    [ExtractionConfidence] decimal(18,2) NULL,
    [ExtractionStatus] nvarchar(50) NOT NULL,
    [RawExtractionJson] nvarchar(256) NULL,
    [ExtractedDate] datetime2 NULL,
    [ReviewedAt] datetime2 NULL,
    [ReviewedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseDocExtractedTerm] PRIMARY KEY ([LeaseDocExtractedTermID])
);
GO

CREATE TABLE [ListingType] (
    [ListingTypeID] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_ListingType] PRIMARY KEY ([ListingTypeID])
);
GO

CREATE TABLE [Organization] (
    [OrganizationID] uniqueidentifier NOT NULL,
    [LegalName] nvarchar(100) NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    [BusinessNumber] nvarchar(100) NULL,
    [CountryCode] nvarchar(2) NOT NULL,
    [ProvinceCode] nvarchar(10) NULL,
    [IsPersonal] bit NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [DefaultCurrency] nvarchar(3) NOT NULL,
    [TimeZone] nvarchar(100) NOT NULL,
    [InvoicePrefix] nvarchar(50) NULL,
    [ReceiptPrefix] nvarchar(50) NULL,
    [LateFeeEnabled] bit NOT NULL,
    [AutoInvoiceGeneration] bit NOT NULL,
    [AutoPaymentRetry] bit NOT NULL,
    [PaymentProvider] nvarchar(100) NULL,
    [BrandLogoUrl] nvarchar(500) NULL,
    [RequireBackgroundCheck] bit NOT NULL,
    [RankingScore] decimal(18,2) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY ([OrganizationID])
);
GO

CREATE TABLE [PaymentProviderEvent] (
    [PaymentProviderEventID] uniqueidentifier NOT NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderEventID] nvarchar(255) NOT NULL,
    [EventType] nvarchar(100) NOT NULL,
    [ProcessingStatus] nvarchar(50) NOT NULL,
    [Payload] nvarchar(256) NOT NULL,
    [ReceivedAt] datetime2 NOT NULL,
    [ProcessedAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [RetryCount] int NOT NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_PaymentProviderEvent] PRIMARY KEY ([PaymentProviderEventID])
);
GO

CREATE TABLE [Preference] (
    [PreferenceID] int NOT NULL IDENTITY,
    [CompanyName] nvarchar(256) NULL,
    [TaxRate] decimal(18,2) NULL,
    [Address] nvarchar(500) NULL,
    [CompanyEmail] nvarchar(256) NULL,
    [CompanyPhone] nvarchar(50) NULL,
    [CompanyWebsite] nvarchar(256) NULL,
    [CompanyLogo] nvarchar(500) NULL,
    [TaxIdentificationNumber] nvarchar(100) NULL,
    [RegistrationNumber] nvarchar(100) NULL,
    [Country] nvarchar(100) NULL,
    [State] nvarchar(100) NULL,
    [City] nvarchar(100) NULL,
    [PostalCode] nvarchar(20) NULL,
    [DefaultCurrency] nvarchar(3) NULL,
    [RequirePaymentBeforeCaseSubmission] bit NULL,
    [AutoAssignAgentID] int NULL,
    [MaxDraftExpiryDays] int NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_Preference] PRIMARY KEY ([PreferenceID])
);
GO

CREATE TABLE [SubscriptionPlan] (
    [SubscriptionPlanID] uniqueidentifier NOT NULL,
    [Code] nvarchar(100) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [MonthlyPrice] decimal(18,2) NOT NULL,
    [AnnualPrice] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [MaxProperties] int NULL,
    [MaxRentalUnits] int NULL,
    [MaxActiveListings] int NULL,
    [MaxOrganizationMembers] int NULL,
    [Features] nvarchar(256) NULL,
    [ProviderProductID] nvarchar(255) NULL,
    [ProviderMonthlyPriceID] nvarchar(255) NULL,
    [ProviderAnnualPriceID] nvarchar(255) NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_SubscriptionPlan] PRIMARY KEY ([SubscriptionPlanID])
);
GO

CREATE TABLE [TaxRate] (
    [TaxID] int NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [CountryCode] nvarchar(2) NOT NULL,
    [ProvinceCode] nvarchar(10) NULL,
    [Rate] decimal(18,2) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TaxRate] PRIMARY KEY ([TaxID])
);
GO

CREATE TABLE [TenancyType] (
    [TenancyTypeID] int NOT NULL IDENTITY,
    [Code] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [MinimumMonths] smallint NULL,
    [MaximumMonths] smallint NULL,
    [IsPeriodic] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TenancyType] PRIMARY KEY ([TenancyTypeID])
);
GO

CREATE TABLE [UnitType] (
    [UnitTypeID] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_UnitType] PRIMARY KEY ([UnitTypeID])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] bigint NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id])
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] bigint NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] bigint NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] bigint NOT NULL,
    [RoleId] bigint NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] bigint NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [IdentityVerification] (
    [IdentityVerificationID] uniqueidentifier NOT NULL,
    [UserID] bigint NOT NULL,
    [VerificationType] nvarchar(50) NOT NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [Status] nvarchar(50) NOT NULL,
    [RequestedAt] datetime2 NULL,
    [VerifiedAt] datetime2 NULL,
    [ExpiresAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_IdentityVerification] PRIMARY KEY ([IdentityVerificationID]),
    CONSTRAINT [FK_IdentityVerification_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Tenant] (
    [TenantID] uniqueidentifier NOT NULL,
    [Code] nvarchar(100) NOT NULL,
    [UserID] bigint NOT NULL,
    [Description] nvarchar(256) NOT NULL,
    [PhoneNumber] nvarchar(100) NULL,
    [PhotoUrl] nvarchar(256) NULL,
    [DateOfBirth] datetime2 NULL,
    [ProfileStatus] nvarchar(100) NULL,
    [IsActive] bit NULL,
    [IsPADRegistered] bit NULL,
    [IsCardRegistered] bit NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Tenant] PRIMARY KEY ([TenantID]),
    CONSTRAINT [FK_Tenant_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Address] (
    [AddressID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NULL,
    [AddressType] nvarchar(50) NULL,
    [Line1] nvarchar(255) NOT NULL,
    [Line2] nvarchar(255) NULL,
    [City] nvarchar(120) NOT NULL,
    [ProvinceCode] nvarchar(10) NULL,
    [PostalCode] nvarchar(20) NULL,
    [CountryCode] nvarchar(2) NOT NULL,
    [Latitude] decimal(18,2) NULL,
    [Longitude] decimal(18,2) NULL,
    [PlaceProvider] nvarchar(50) NULL,
    [PlaceProviderReferenceID] nvarchar(255) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY ([AddressID]),
    CONSTRAINT [FK_Address_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [Contractor] (
    [ContractorID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [CompanyName] nvarchar(100) NULL,
    [ContactName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [CategoryID] int NULL,
    [Status] nvarchar(50) NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Contractor] PRIMARY KEY ([ContractorID]),
    CONSTRAINT [FK_Contractor_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [Fee] (
    [FeeID] uniqueidentifier NOT NULL,
    [FeeTypeID] int NOT NULL,
    [OrganizationID] uniqueidentifier NULL,
    [Code] nvarchar(100) NULL,
    [Name] nvarchar(100) NOT NULL,
    [CalculationType] nvarchar(50) NOT NULL,
    [FixedAmount] decimal(18,2) NULL,
    [PercentageRate] decimal(18,2) NULL,
    [MinimumFeeAmount] decimal(18,2) NULL,
    [MaximumFeeAmount] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [IsTaxable] bit NOT NULL,
    [EffectiveFrom] datetime2 NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Fee] PRIMARY KEY ([FeeID]),
    CONSTRAINT [FK_Fee_FeeType_FeeTypeID] FOREIGN KEY ([FeeTypeID]) REFERENCES [FeeType] ([FeeTypeID]),
    CONSTRAINT [FK_Fee_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [LedgerAccount] (
    [LedgerAccountID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [AccountCode] nvarchar(100) NOT NULL,
    [AccountType] nvarchar(100) NOT NULL,
    [AccountCategory] nvarchar(100) NOT NULL,
    [Currency] nvarchar(100) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [IsSystemAccount] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_LedgerAccount] PRIMARY KEY ([LedgerAccountID]),
    CONSTRAINT [FK_LedgerAccount_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [OrganizationMember] (
    [OrganizationMemberID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [UserID] bigint NOT NULL,
    [RoleName] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [IsPrimaryOwner] bit NOT NULL,
    [InvitedAt] datetime2 NULL,
    [AcceptedAt] datetime2 NULL,
    [DeactivatedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_OrganizationMember] PRIMARY KEY ([OrganizationMemberID]),
    CONSTRAINT [FK_OrganizationMember_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_OrganizationMember_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [OrganizationStatement] (
    [OrganizationStatementID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [Period] int NULL,
    [Income] decimal(18,2) NULL,
    [Expenses] decimal(18,2) NULL,
    [NetAmount] decimal(18,2) NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_OrganizationStatement] PRIMARY KEY ([OrganizationStatementID]),
    CONSTRAINT [FK_OrganizationStatement_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [OrgPayoutAccount] (
    [OrgPayoutAccountID] bigint NOT NULL IDENTITY,
    [OrganizationID] uniqueidentifier NOT NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderAccountID] nvarchar(100) NOT NULL,
    [AccountType] nvarchar(100) NULL,
    [BankName] nvarchar(100) NULL,
    [AccountLast4] nvarchar(4) NULL,
    [Currency] nvarchar(10) NOT NULL,
    [VerificationStatus] nvarchar(50) NOT NULL,
    [VerifiedAt] datetime2 NULL,
    [IsDefault] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_OrgPayoutAccount] PRIMARY KEY ([OrgPayoutAccountID]),
    CONSTRAINT [FK_OrgPayoutAccount_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [OrgSubscription] (
    [OrgSubscriptionID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [SubscriptionPlanID] uniqueidentifier NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [BillingFrequency] nvarchar(50) NOT NULL,
    [StartedAt] datetime2 NOT NULL,
    [TrialEndsAt] datetime2 NULL,
    [CurrentPeriodStart] datetime2 NULL,
    [CurrentPeriodEnd] datetime2 NULL,
    [CancelAtPeriodEnd] bit NOT NULL,
    [CancelledAt] datetime2 NULL,
    [EndedAt] datetime2 NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderCustomerID] nvarchar(255) NULL,
    [ProviderSubscriptionID] nvarchar(255) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_OrgSubscription] PRIMARY KEY ([OrgSubscriptionID]),
    CONSTRAINT [FK_OrgSubscription_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_OrgSubscription_SubscriptionPlan_SubscriptionPlanID] FOREIGN KEY ([SubscriptionPlanID]) REFERENCES [SubscriptionPlan] ([SubscriptionPlanID])
);
GO

CREATE TABLE [ApplicationOccupant] (
    [ApplicationOccupantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NULL,
    [UserID] bigint NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [OccupantType] nvarchar(50) NOT NULL,
    [IsPrimaryApplicant] bit NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ApplicationOccupant] PRIMARY KEY ([ApplicationOccupantID]),
    CONSTRAINT [FK_ApplicationOccupant_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ApplicationOccupant_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [PaymentMethod] (
    [PaymentMethodID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [PaymentMethodType] nvarchar(50) NOT NULL,
    [DisplayName] nvarchar(100) NULL,
    [AccountLast4] nvarchar(20) NULL,
    [CardBrand] nvarchar(50) NULL,
    [BankName] nvarchar(100) NULL,
    [ExpiryMonth] smallint NULL,
    [ExpiryYear] smallint NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderCustomerID] nvarchar(255) NOT NULL,
    [ProviderPaymentMethodID] nvarchar(255) NOT NULL,
    [VerificationStatus] nvarchar(50) NOT NULL,
    [VerifiedAt] datetime2 NULL,
    [IsDefault] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_PaymentMethod] PRIMARY KEY ([PaymentMethodID]),
    CONSTRAINT [FK_PaymentMethod_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [TenantEmergencyContact] (
    [TenantEmergencyContactID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(100) NULL,
    [PhoneNumber] nvarchar(50) NOT NULL,
    [Email] nvarchar(255) NULL,
    [IsPrimary] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TenantEmergencyContact] PRIMARY KEY ([TenantEmergencyContactID]),
    CONSTRAINT [FK_TenantEmergencyContact_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [TenantEmployment] (
    [TenantEmploymentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [EmployerName] nvarchar(100) NULL,
    [JobTitle] nvarchar(150) NULL,
    [EmploymentType] nvarchar(50) NULL,
    [EmployerEmail] nvarchar(255) NULL,
    [EmployerPhoneNumber] nvarchar(50) NULL,
    [AnnualIncome] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [StartedAt] datetime2 NULL,
    [EndedAt] datetime2 NULL,
    [IsCurrent] bit NOT NULL,
    [VerificationStatus] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TenantEmployment] PRIMARY KEY ([TenantEmploymentID]),
    CONSTRAINT [FK_TenantEmployment_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [TenantGuarantor] (
    [TenantGuarantorID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [UserID] bigint NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [Relationship] nvarchar(100) NULL,
    [AnnualIncome] decimal(18,2) NULL,
    [Status] nvarchar(50) NOT NULL,
    [InvitedAt] datetime2 NULL,
    [AcceptedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TenantGuarantor] PRIMARY KEY ([TenantGuarantorID]),
    CONSTRAINT [FK_TenantGuarantor_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_TenantGuarantor_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [TenantScreeningCheck] (
    [TenantScreeningCheckID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [CheckType] nvarchar(50) NOT NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [Status] nvarchar(50) NOT NULL,
    [ConsentCapturedAt] datetime2 NULL,
    [Score] decimal(18,2) NULL,
    [ResultSummary] nvarchar(256) NULL,
    [ReportReference] nvarchar(500) NULL,
    [RequestedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    [ExpiresAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_TenantScreeningCheck] PRIMARY KEY ([TenantScreeningCheckID]),
    CONSTRAINT [FK_TenantScreeningCheck_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Property] (
    [PropertyID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [AddressID] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NULL,
    [PropertyType] nvarchar(50) NOT NULL,
    [YearBuilt] int NULL,
    [TimeZone] nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Status] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Property] PRIMARY KEY ([PropertyID]),
    CONSTRAINT [FK_Property_Address_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [Address] ([AddressID]),
    CONSTRAINT [FK_Property_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [AuditLog] (
    [AuditLogID] uniqueidentifier NOT NULL,
    [ActorUserID] bigint NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationMemberID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [ActorType] nvarchar(50) NULL,
    [Action] nvarchar(255) NOT NULL,
    [EntityType] nvarchar(100) NOT NULL,
    [EntityID] nvarchar(100) NOT NULL,
    [OldValues] nvarchar(256) NULL,
    [NewValues] nvarchar(256) NULL,
    [IpAddress] nvarchar(100) NULL,
    [UserAgent] nvarchar(500) NULL,
    [CorrelationID] nvarchar(100) NULL,
    [Note] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_AuditLog] PRIMARY KEY ([AuditLogID]),
    CONSTRAINT [FK_AuditLog_AspNetUsers_ActorUserID] FOREIGN KEY ([ActorUserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_AuditLog_OrganizationMember_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_AuditLog_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_AuditLog_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [LeaseSignatory] (
    [LeaseSignatoryID] uniqueidentifier NOT NULL,
    [LeaseDocumentID] uniqueidentifier NOT NULL,
    [UserID] bigint NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationMemberID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [SignatoryRole] nvarchar(50) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Email] nvarchar(255) NULL,
    [Status] nvarchar(50) NOT NULL,
    [SignatureOrder] int NULL,
    [ProviderSignerID] nvarchar(255) NULL,
    [ViewedAt] datetime2 NULL,
    [SignedAt] datetime2 NULL,
    [DeclinedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseSignatory] PRIMARY KEY ([LeaseSignatoryID]),
    CONSTRAINT [FK_LeaseSignatory_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_LeaseSignatory_OrganizationMember_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_LeaseSignatory_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_LeaseSignatory_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Notification] (
    [NotificationID] uniqueidentifier NOT NULL,
    [RecipientUserID] bigint NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [OrganizationMemberID] uniqueidentifier NULL,
    [RecipientEmail] nvarchar(255) NULL,
    [RecipientPhoneNumber] nvarchar(50) NULL,
    [Channel] nvarchar(50) NOT NULL,
    [TemplateCode] nvarchar(100) NULL,
    [Subject] nvarchar(255) NULL,
    [Body] nvarchar(256) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ScheduledAt] datetime2 NOT NULL,
    [SentAt] datetime2 NULL,
    [DeliveredAt] datetime2 NULL,
    [ReadAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [ProviderMessageID] nvarchar(255) NULL,
    [Metadata] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY ([NotificationID]),
    CONSTRAINT [FK_Notification_AspNetUsers_RecipientUserID] FOREIGN KEY ([RecipientUserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Notification_OrganizationMember_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_Notification_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_Notification_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Payout] (
    [PayoutID] bigint NOT NULL IDENTITY,
    [OrganizationID] uniqueidentifier NOT NULL,
    [OrgPayoutAccountID] bigint NOT NULL,
    [From] datetime2 NULL,
    [To] datetime2 NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(100) NOT NULL,
    [Status] nvarchar(100) NOT NULL,
    [ScheduledAt] datetime2 NULL,
    [RequestedAt] datetime2 NOT NULL,
    [ProcessedAt] datetime2 NULL,
    [PaidAt] datetime2 NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderPayoutID] nvarchar(256) NULL,
    [FailureReason] nvarchar(256) NULL,
    [CapturedBy] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_Payout] PRIMARY KEY ([PayoutID]),
    CONSTRAINT [FK_Payout_OrgPayoutAccount_OrgPayoutAccountID] FOREIGN KEY ([OrgPayoutAccountID]) REFERENCES [OrgPayoutAccount] ([OrgPayoutAccountID]),
    CONSTRAINT [FK_Payout_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID])
);
GO

CREATE TABLE [RentalUnit] (
    [RentalUnitID] uniqueidentifier NOT NULL,
    [PropertyID] uniqueidentifier NOT NULL,
    [UnitTypeID] uniqueidentifier NULL,
    [UnitNumber] nvarchar(50) NULL,
    [FloorNumber] nvarchar(20) NULL,
    [Bedrooms] decimal(18,2) NULL,
    [Bathrooms] decimal(18,2) NULL,
    [SquareFeet] int NULL,
    [MaximumOccupants] int NULL,
    [Notes] nvarchar(1000) NULL,
    [Status] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_RentalUnit] PRIMARY KEY ([RentalUnitID]),
    CONSTRAINT [FK_RentalUnit_Property_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Property] ([PropertyID]),
    CONSTRAINT [FK_RentalUnit_UnitType_UnitTypeID] FOREIGN KEY ([UnitTypeID]) REFERENCES [UnitType] ([UnitTypeID])
);
GO

CREATE TABLE [Listing] (
    [ListingID] uniqueidentifier NOT NULL,
    [RentalUnitID] uniqueidentifier NOT NULL,
    [ListingTypeID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [CheckInDoorCode] nvarchar(10) NULL,
    [IsFurnished] bit NOT NULL,
    [Bedrooms] decimal(18,2) NULL,
    [Bathrooms] decimal(18,2) NULL,
    [SquareFeet] decimal(18,2) NULL,
    [BaseMonthlyRentAmount] decimal(18,2) NOT NULL,
    [SecurityDepositAmount] decimal(18,2) NOT NULL,
    [YearBuilt] int NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [PublishedAt] datetime2 NULL,
    [UnpublishedAt] datetime2 NULL,
    [Currency] nvarchar(3) NOT NULL,
    [AvailableFrom] datetime2 NULL,
    [AvailableTo] datetime2 NULL,
    [MinimumLeaseMonths] smallint NOT NULL,
    [MaximumLeaseMonths] smallint NOT NULL,
    [ApplicationDeadline] datetime2 NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [WIFINetwork] nvarchar(50) NULL,
    [WIFIPassword] nvarchar(50) NULL,
    [AcceptingApplications] bit NOT NULL,
    [Tag] nvarchar(50) NULL,
    [CapturedBy] nvarchar(50) NULL,
    [CapturedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Listing] PRIMARY KEY ([ListingID]),
    CONSTRAINT [FK_Listing_ListingType_ListingTypeID] FOREIGN KEY ([ListingTypeID]) REFERENCES [ListingType] ([ListingTypeID]),
    CONSTRAINT [FK_Listing_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_Listing_RentalUnit_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnit] ([RentalUnitID])
);
GO

CREATE TABLE [ListingAmenity] (
    [ListingAmenityID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [AmenityID] uniqueidentifier NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_ListingAmenity] PRIMARY KEY ([ListingAmenityID]),
    CONSTRAINT [FK_ListingAmenity_AmenityCatalog_AmenityID] FOREIGN KEY ([AmenityID]) REFERENCES [AmenityCatalog] ([AmenityID]),
    CONSTRAINT [FK_ListingAmenity_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [ListingPhoto] (
    [ListingPhotoID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [Url] nvarchar(500) NOT NULL,
    [Location] nvarchar(50) NULL,
    [Caption] nvarchar(255) NULL,
    [AltText] nvarchar(255) NULL,
    [DisplayOrder] int NOT NULL,
    [IsCoverPhoto] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ListingPhoto] PRIMARY KEY ([ListingPhotoID]),
    CONSTRAINT [FK_ListingPhoto_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [ListingPolicy] (
    [ListingPolicyID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [AllowsPets] bit NULL,
    [AllowsSmoking] bit NULL,
    [AllowsChildren] bit NULL,
    [MaximumOccupants] int NOT NULL,
    [Furnished] bit NULL,
    [ParkingIncluded] bit NULL,
    [UtilitiesIncluded] bit NULL,
    [MinimumCreditScore] int NULL,
    [RequiresBackgroundCheck] bit NULL,
    [ApplicationInstructions] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_ListingPolicy] PRIMARY KEY ([ListingPolicyID]),
    CONSTRAINT [FK_ListingPolicy_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [ListingRule] (
    [ListingRuleID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [RuleType] nvarchar(100) NOT NULL,
    [RuleTitle] nvarchar(200) NOT NULL,
    [RuleDescription] nvarchar(1000) NULL,
    [IsAllowed] bit NULL,
    [EffectiveFrom] datetime2 NULL,
    [EffectiveTo] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ListingRule] PRIMARY KEY ([ListingRuleID]),
    CONSTRAINT [FK_ListingRule_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [ListingTermPrice] (
    [ListingTermPriceID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [LeaseTermMonths] smallint NOT NULL,
    [MonthlyRentAmount] decimal(18,2) NOT NULL,
    [SecurityDepositAmount] decimal(18,2) NULL,
    [EffectiveFrom] datetime2 NULL,
    [EffectiveTo] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ListingTermPrice] PRIMARY KEY ([ListingTermPriceID]),
    CONSTRAINT [FK_ListingTermPrice_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [RentalApplication] (
    [RentalApplicationID] uniqueidentifier NOT NULL,
    [ApplicationCode] nvarchar(50) NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [DesiredMoveInDate] datetime2 NOT NULL,
    [DesiredMoveOutDate] datetime2 NULL,
    [RequestedLeaseTermMonths] smallint NOT NULL,
    [AdultOccupantCount] int NOT NULL,
    [ChildOccupantCount] int NOT NULL,
    [PetCount] int NOT NULL,
    [ProposedMonthlyRentAmount] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ScreeningStatus] nvarchar(50) NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [SubmittedAt] datetime2 NULL,
    [ReviewedAt] datetime2 NULL,
    [ReviewedByOrganizationMemberID] uniqueidentifier NULL,
    [ApprovedAt] datetime2 NULL,
    [DeclinedAt] datetime2 NULL,
    [DeclineReason] nvarchar(256) NULL,
    [ExpiresAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_RentalApplication] PRIMARY KEY ([RentalApplicationID]),
    CONSTRAINT [FK_RentalApplication_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_RentalApplication_OrganizationMember_ReviewedByOrganizationMemberID] FOREIGN KEY ([ReviewedByOrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_RentalApplication_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_RentalApplication_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [ViewingAppointments] (
    [ViewingAppointmentID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [RequestedByUserID] bigint NOT NULL,
    [TenantID] uniqueidentifier NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [AssignedOrganizationMemberID] uniqueidentifier NULL,
    [ScheduledFor] datetime2 NOT NULL,
    [DurationMinutes] int NOT NULL,
    [TimeZone] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ViewingType] nvarchar(50) NOT NULL,
    [MeetingUrl] nvarchar(500) NULL,
    [Notes] nvarchar(1000) NULL,
    [CancelledAt] datetime2 NULL,
    [CancellationReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ViewingAppointments] PRIMARY KEY ([ViewingAppointmentID]),
    CONSTRAINT [FK_ViewingAppointments_AspNetUsers_RequestedByUserID] FOREIGN KEY ([RequestedByUserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ViewingAppointments_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_ViewingAppointments_OrganizationMember_AssignedOrganizationMemberID] FOREIGN KEY ([AssignedOrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_ViewingAppointments_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Lease] (
    [LeaseID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [RentalUnitID] uniqueidentifier NOT NULL,
    [TenancyTypeID] int NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [LeaseCode] nvarchar(50) NOT NULL,
    [LeaseNumber] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [LeaseTermMonths] smallint NULL,
    [BaseRentAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [GracePeriodDays] smallint NOT NULL,
    [LateFeeFixedAmount] decimal(18,2) NOT NULL,
    [LateFeePercentage] decimal(18,2) NOT NULL,
    [AutoRenew] bit NOT NULL,
    [RenewalNoticeDays] int NULL,
    [SignedAt] datetime2 NULL,
    [ActivatedAt] datetime2 NULL,
    [ActualMoveInAt] datetime2 NULL,
    [ActualMoveOutAt] datetime2 NULL,
    [TerminatedAt] datetime2 NULL,
    [TerminationReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Lease] PRIMARY KEY ([LeaseID]),
    CONSTRAINT [FK_Lease_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_Lease_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_Lease_RentalApplication_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplication] ([RentalApplicationID]),
    CONSTRAINT [FK_Lease_RentalUnit_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnit] ([RentalUnitID]),
    CONSTRAINT [FK_Lease_TenancyType_TenancyTypeID] FOREIGN KEY ([TenancyTypeID]) REFERENCES [TenancyType] ([TenancyTypeID]),
    CONSTRAINT [FK_Lease_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [ReservationHold] (
    [ReservationHoldID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [TenantID] uniqueidentifier NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [HoldReason] nvarchar(100) NULL,
    [Status] nvarchar(50) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [ReleasedAt] datetime2 NULL,
    [ConvertedToLeaseAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ReservationHold] PRIMARY KEY ([ReservationHoldID]),
    CONSTRAINT [FK_ReservationHold_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_ReservationHold_RentalApplication_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplication] ([RentalApplicationID]),
    CONSTRAINT [FK_ReservationHold_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [AutopayMandate] (
    [AutopayMandateID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [PaymentMethodID] uniqueidentifier NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [MandateType] nvarchar(50) NOT NULL,
    [PaymentRail] nvarchar(50) NOT NULL,
    [MaximumAmountPerDebit] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Frequency] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderMandateID] nvarchar(255) NULL,
    [ConsentVersion] nvarchar(50) NOT NULL,
    [ConsentTextHash] nvarchar(255) NULL,
    [ConsentIpAddress] nvarchar(100) NULL,
    [ConsentedAt] datetime2 NULL,
    [ActivatedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [CancellationReason] nvarchar(256) NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_AutopayMandate] PRIMARY KEY ([AutopayMandateID]),
    CONSTRAINT [FK_AutopayMandate_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_AutopayMandate_PaymentMethod_PaymentMethodID] FOREIGN KEY ([PaymentMethodID]) REFERENCES [PaymentMethod] ([PaymentMethodID]),
    CONSTRAINT [FK_AutopayMandate_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [CreditReportingEnrollment] (
    [CreditReportingEnrollmentID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ConsentedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_CreditReportingEnrollment] PRIMARY KEY ([CreditReportingEnrollmentID]),
    CONSTRAINT [FK_CreditReportingEnrollment_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_CreditReportingEnrollment_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Inspection] (
    [InspectionID] uniqueidentifier NOT NULL,
    [PropertyID] uniqueidentifier NULL,
    [RentalUnitID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [InspectionType] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ScheduledFor] datetime2 NULL,
    [StartedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [OverallCondition] nvarchar(50) NULL,
    [Notes] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Inspection] PRIMARY KEY ([InspectionID]),
    CONSTRAINT [FK_Inspection_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_Inspection_Property_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Property] ([PropertyID]),
    CONSTRAINT [FK_Inspection_RentalUnit_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnit] ([RentalUnitID])
);
GO

CREATE TABLE [InvoiceMaster] (
    [InvoiceMasterID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [InvoiceNumber] nvarchar(100) NOT NULL,
    [BillingPeriodStart] datetime2 NOT NULL,
    [BillingPeriodEnd] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [SubtotalAmount] decimal(18,2) NOT NULL,
    [TaxAmount] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [LateFeeAmount] decimal(18,2) NOT NULL,
    [AdjustmentAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [AmountPaid] decimal(18,2) NOT NULL,
    [BalanceDue] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [IssuedAt] datetime2 NULL,
    [PaidAt] datetime2 NULL,
    [VoidedAt] datetime2 NULL,
    [VoidReason] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_InvoiceMaster] PRIMARY KEY ([InvoiceMasterID]),
    CONSTRAINT [FK_InvoiceMaster_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_InvoiceMaster_Lease_LeaseRenewalID] FOREIGN KEY ([LeaseRenewalID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_InvoiceMaster_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_InvoiceMaster_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [LeaseRecurringCharges] (
    [LeaseRecurringChargeID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [FeeID] uniqueidentifier NULL,
    [ChargeCode] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Frequency] nvarchar(50) NOT NULL,
    [BillingDayOfMonth] smallint NULL,
    [FirstDueDate] datetime2 NOT NULL,
    [LastDueDate] datetime2 NULL,
    [ProrationRule] nvarchar(50) NULL,
    [AutoGenerateInvoice] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseRecurringCharges] PRIMARY KEY ([LeaseRecurringChargeID]),
    CONSTRAINT [FK_LeaseRecurringCharges_Fee_FeeID] FOREIGN KEY ([FeeID]) REFERENCES [Fee] ([FeeID]),
    CONSTRAINT [FK_LeaseRecurringCharges_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID])
);
GO

CREATE TABLE [LeaseRenewals] (
    [LeaseRenewalID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [OfferedAt] datetime2 NULL,
    [OfferExpiresAt] datetime2 NULL,
    [AcceptedAt] datetime2 NULL,
    [DeclinedAt] datetime2 NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [LeaseTermMonths] smallint NULL,
    [RentAmount] decimal(18,2) NOT NULL,
    [SecurityDepositAdjustmentAmount] decimal(18,2) NULL,
    [Notes] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseRenewals] PRIMARY KEY ([LeaseRenewalID]),
    CONSTRAINT [FK_LeaseRenewals_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID])
);
GO

CREATE TABLE [ListingAccessInstruction] (
    [ListingAccessInstructionID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NULL,
    [InstructionType] nvarchar(50) NOT NULL,
    [Instructions] nvarchar(256) NULL,
    [SecretReference] nvarchar(500) NULL,
    [AvailableFrom] datetime2 NULL,
    [AvailableUntil] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_ListingAccessInstruction] PRIMARY KEY ([ListingAccessInstructionID]),
    CONSTRAINT [FK_ListingAccessInstruction_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_ListingAccessInstruction_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID])
);
GO

CREATE TABLE [MaintenanceRequest] (
    [MaintenanceRequestID] uniqueidentifier NOT NULL,
    [PropertyID] uniqueidentifier NULL,
    [RentalUnitID] uniqueidentifier NULL,
    [ListingID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [SubmittedByTenantID] uniqueidentifier NULL,
    [CategoryID] int NOT NULL,
    [Priority] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [PermissionToEnter] bit NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [AcknowledgedAt] datetime2 NULL,
    [ScheduledAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_MaintenanceRequest] PRIMARY KEY ([MaintenanceRequestID]),
    CONSTRAINT [FK_MaintenanceRequest_Category_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [Category] ([CategoryID]),
    CONSTRAINT [FK_MaintenanceRequest_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_MaintenanceRequest_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_MaintenanceRequest_Property_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Property] ([PropertyID]),
    CONSTRAINT [FK_MaintenanceRequest_RentalUnit_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnit] ([RentalUnitID]),
    CONSTRAINT [FK_MaintenanceRequest_Tenant_SubmittedByTenantID] FOREIGN KEY ([SubmittedByTenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Rating] (
    [RatingID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [ReviewerUserID] bigint NOT NULL,
    [SubjectType] nvarchar(50) NOT NULL,
    [SubjectReferenceID] nvarchar(100) NOT NULL,
    [OverallRating] smallint NOT NULL,
    [PaymentRating] smallint NULL,
    [CommunicationRating] smallint NULL,
    [PropertyCareRating] smallint NULL,
    [ResponsivenessRating] smallint NULL,
    [AccuracyRating] smallint NULL,
    [CleanlinessRating] smallint NULL,
    [ReviewBody] nvarchar(256) NULL,
    [IsPublic] bit NOT NULL,
    [PublishedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    [UpdatedDate] datetime2 NULL,
    CONSTRAINT [PK_Rating] PRIMARY KEY ([RatingID]),
    CONSTRAINT [FK_Rating_AspNetUsers_ReviewerUserID] FOREIGN KEY ([ReviewerUserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Rating_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID])
);
GO

CREATE TABLE [SecurityDeposit] (
    [SecurityDepositID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [RequiredAmount] decimal(18,2) NOT NULL,
    [ReceivedAmount] decimal(18,2) NOT NULL,
    [AppliedAmount] decimal(18,2) NOT NULL,
    [ReturnedAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [DueDate] datetime2 NULL,
    [FullyFundedAt] datetime2 NULL,
    [HeldAt] datetime2 NULL,
    [ClosedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_SecurityDeposit] PRIMARY KEY ([SecurityDepositID]),
    CONSTRAINT [FK_SecurityDeposit_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_SecurityDeposit_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_SecurityDeposit_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [TenantInvitation] (
    [TenantInvitationID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [InvitationPurpose] nvarchar(50) NOT NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [TokenHash] nvarchar(500) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [AcceptedAt] datetime2 NULL,
    [RevokedAt] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_TenantInvitation] PRIMARY KEY ([TenantInvitationID]),
    CONSTRAINT [FK_TenantInvitation_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_TenantInvitation_RentalApplication_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplication] ([RentalApplicationID])
);
GO

CREATE TABLE [AutopayConsentAudit] (
    [AutopayConsentAuditID] uniqueidentifier NOT NULL,
    [AutopayMandateID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [ConsentVersion] nvarchar(50) NULL,
    [ConsentTextHash] nvarchar(255) NULL,
    [IpAddress] nvarchar(100) NULL,
    [UserAgent] nvarchar(500) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [ActionAt] datetime2 NOT NULL,
    [Metadata] nvarchar(256) NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_AutopayConsentAudit] PRIMARY KEY ([AutopayConsentAuditID]),
    CONSTRAINT [FK_AutopayConsentAudit_AutopayMandate_AutopayMandateID] FOREIGN KEY ([AutopayMandateID]) REFERENCES [AutopayMandate] ([AutopayMandateID]),
    CONSTRAINT [FK_AutopayConsentAudit_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [CreditReportingConsentAudit] (
    [ConsentAuditID] uniqueidentifier NOT NULL,
    [CreditReportingEnrollmentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [ConsentVersion] nvarchar(50) NULL,
    [ConsentTextHash] nvarchar(255) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [ActionAt] datetime2 NOT NULL,
    [Metadata] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_CreditReportingConsentAudit] PRIMARY KEY ([ConsentAuditID]),
    CONSTRAINT [FK_CreditReportingConsentAudit_CreditReportingEnrollment_CreditReportingEnrollmentID] FOREIGN KEY ([CreditReportingEnrollmentID]) REFERENCES [CreditReportingEnrollment] ([CreditReportingEnrollmentID]),
    CONSTRAINT [FK_CreditReportingConsentAudit_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [InspectionItem] (
    [InspectionItemID] uniqueidentifier NOT NULL,
    [InspectionID] uniqueidentifier NOT NULL,
    [Area] nvarchar(100) NOT NULL,
    [ItemName] nvarchar(100) NOT NULL,
    [Condition] nvarchar(50) NULL,
    [Notes] nvarchar(1000) NULL,
    [RequiresRepair] bit NOT NULL,
    [EstimatedRepairCost] decimal(18,2) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_InspectionItem] PRIMARY KEY ([InspectionItemID]),
    CONSTRAINT [FK_InspectionItem_Inspection_InspectionID] FOREIGN KEY ([InspectionID]) REFERENCES [Inspection] ([InspectionID])
);
GO

CREATE TABLE [InvoiceDetail] (
    [InvoiceDetailID] uniqueidentifier NOT NULL,
    [InvoiceMasterID] uniqueidentifier NOT NULL,
    [LeaseRecurringChargeID] uniqueidentifier NULL,
    [FeeID] uniqueidentifier NULL,
    [LineType] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [ServicePeriodStart] datetime2 NULL,
    [ServicePeriodEnd] datetime2 NULL,
    [Quantity] decimal(18,2) NOT NULL,
    [UnitAmount] decimal(18,2) NOT NULL,
    [LineAmount] decimal(18,2) NOT NULL,
    [TaxRate] decimal(18,2) NULL,
    [TaxAmount] decimal(18,2) NOT NULL,
    [TotalLineAmount] decimal(18,2) NOT NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_InvoiceDetail] PRIMARY KEY ([InvoiceDetailID]),
    CONSTRAINT [FK_InvoiceDetail_Fee_FeeID] FOREIGN KEY ([FeeID]) REFERENCES [Fee] ([FeeID]),
    CONSTRAINT [FK_InvoiceDetail_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID])
);
GO

CREATE TABLE [PaymentIntent] (
    [PaymentIntentID] uniqueidentifier NOT NULL,
    [InvoiceMasterID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [AutopayMandateID] uniqueidentifier NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [PaymentMethodID] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [CollectionMethod] nvarchar(50) NOT NULL,
    [ProviderName] nvarchar(100) NOT NULL,
    [ProviderPaymentIntentID] nvarchar(255) NULL,
    [IdempotencyKey] nvarchar(255) NOT NULL,
    [ScheduledChargeAt] datetime2 NOT NULL,
    [StartedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_PaymentIntent] PRIMARY KEY ([PaymentIntentID]),
    CONSTRAINT [FK_PaymentIntent_AutopayMandate_AutopayMandateID] FOREIGN KEY ([AutopayMandateID]) REFERENCES [AutopayMandate] ([AutopayMandateID]),
    CONSTRAINT [FK_PaymentIntent_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_PaymentIntent_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_PaymentIntent_PaymentMethod_PaymentMethodID] FOREIGN KEY ([PaymentMethodID]) REFERENCES [PaymentMethod] ([PaymentMethodID]),
    CONSTRAINT [FK_PaymentIntent_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [PaymentReminder] (
    [PaymentReminderID] uniqueidentifier NOT NULL,
    [InvoiceMasterID] uniqueidentifier NOT NULL,
    [Channel] nvarchar(50) NOT NULL,
    [ScheduledAt] datetime2 NOT NULL,
    [SentAt] datetime2 NULL,
    [Status] nvarchar(50) NOT NULL,
    [MessageSubject] nvarchar(255) NULL,
    [MessageBody] nvarchar(256) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_PaymentReminder] PRIMARY KEY ([PaymentReminderID]),
    CONSTRAINT [FK_PaymentReminder_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID])
);
GO

CREATE TABLE [LeaseDocuments] (
    [LeaseDocumentID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [DocumentType] nvarchar(50) NOT NULL,
    [DocumentStatus] nvarchar(50) NOT NULL,
    [OriginalFilename] nvarchar(100) NOT NULL,
    [StorageProvider] nvarchar(100) NOT NULL,
    [StorageContainer] nvarchar(255) NULL,
    [StorageReference] nvarchar(500) NOT NULL,
    [FileHash] nvarchar(255) NULL,
    [IsPrimary] bit NOT NULL,
    [GeneratedAt] datetime2 NULL,
    [SentForSignatureAt] datetime2 NULL,
    [FullySignedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseDocuments] PRIMARY KEY ([LeaseDocumentID]),
    CONSTRAINT [FK_LeaseDocuments_LeaseRenewals_LeaseRenewalID] FOREIGN KEY ([LeaseRenewalID]) REFERENCES [LeaseRenewals] ([LeaseRenewalID]),
    CONSTRAINT [FK_LeaseDocuments_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID])
);
GO

CREATE TABLE [LeaseOccupants] (
    [LeaseOccupantID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NOT NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [TenantID] uniqueidentifier NULL,
    [UserID] bigint NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Email] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [OccupantType] nvarchar(50) NOT NULL,
    [IsPrimaryTenant] bit NOT NULL,
    [IsFinanciallyResponsible] bit NOT NULL,
    [JoinedAt] datetime2 NULL,
    [RemovedAt] datetime2 NULL,
    [Status] nvarchar(50) NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseOccupants] PRIMARY KEY ([LeaseOccupantID]),
    CONSTRAINT [FK_LeaseOccupants_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_LeaseOccupants_LeaseRenewals_LeaseRenewalID] FOREIGN KEY ([LeaseRenewalID]) REFERENCES [LeaseRenewals] ([LeaseRenewalID]),
    CONSTRAINT [FK_LeaseOccupants_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_LeaseOccupants_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [CalendarEvent] (
    [CalendarEventID] uniqueidentifier NOT NULL,
    [ListingID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NULL,
    [RentalApplicationID] uniqueidentifier NULL,
    [ReservationHoldID] uniqueidentifier NULL,
    [MaintenanceRequestID] uniqueidentifier NULL,
    [EventType] nvarchar(50) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [StartAt] datetime2 NOT NULL,
    [EndAt] datetime2 NOT NULL,
    [IsAllDay] bit NOT NULL,
    [Title] nvarchar(255) NULL,
    [OccupantName] nvarchar(100) NULL,
    [OccupantCount] int NULL,
    [SourceSystem] nvarchar(100) NULL,
    [SourceReferenceID] nvarchar(255) NULL,
    [ExternalCalendarID] nvarchar(255) NULL,
    [BlocksAvailability] bit NOT NULL,
    [Notes] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_CalendarEvent] PRIMARY KEY ([CalendarEventID]),
    CONSTRAINT [FK_CalendarEvent_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_CalendarEvent_Listing_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listing] ([ListingID]),
    CONSTRAINT [FK_CalendarEvent_MaintenanceRequest_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequest] ([MaintenanceRequestID]),
    CONSTRAINT [FK_CalendarEvent_RentalApplication_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplication] ([RentalApplicationID]),
    CONSTRAINT [FK_CalendarEvent_ReservationHold_ReservationHoldID] FOREIGN KEY ([ReservationHoldID]) REFERENCES [ReservationHold] ([ReservationHoldID])
);
GO

CREATE TABLE [WorkOrder] (
    [WorkOrderID] uniqueidentifier NOT NULL,
    [MaintenanceRequestID] uniqueidentifier NOT NULL,
    [ContractorID] uniqueidentifier NULL,
    [AssignedOrganizationMemberID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [Status] nvarchar(50) NOT NULL,
    [EstimatedCost] decimal(18,2) NULL,
    [FinalCost] decimal(18,2) NULL,
    [Currency] nvarchar(3) NOT NULL,
    [ScheduledAt] datetime2 NULL,
    [StartedAt] datetime2 NULL,
    [CompletedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_WorkOrder] PRIMARY KEY ([WorkOrderID]),
    CONSTRAINT [FK_WorkOrder_Contractor_ContractorID] FOREIGN KEY ([ContractorID]) REFERENCES [Contractor] ([ContractorID]),
    CONSTRAINT [FK_WorkOrder_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_WorkOrder_MaintenanceRequest_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequest] ([MaintenanceRequestID]),
    CONSTRAINT [FK_WorkOrder_OrganizationMember_AssignedOrganizationMemberID] FOREIGN KEY ([AssignedOrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID])
);
GO

CREATE TABLE [Payment] (
    [PaymentID] uniqueidentifier NOT NULL,
    [PaymentIntentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [ProviderChargeID] nvarchar(255) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [GrossAmount] decimal(18,2) NOT NULL,
    [PlatformFeeAmount] decimal(18,2) NOT NULL,
    [ProcessorFeeAmount] decimal(18,2) NOT NULL,
    [RefundedAmount] decimal(18,2) NOT NULL,
    [NetAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [PaidAt] datetime2 NULL,
    [SettledAt] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Payment] PRIMARY KEY ([PaymentID]),
    CONSTRAINT [FK_Payment_PaymentIntent_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntent] ([PaymentIntentID]),
    CONSTRAINT [FK_Payment_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [PaymentAttempt] (
    [PaymentAttemptID] uniqueidentifier NOT NULL,
    [PaymentIntentID] uniqueidentifier NOT NULL,
    [AttemptNumber] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [ProviderAttemptID] nvarchar(255) NULL,
    [FailureCode] nvarchar(100) NULL,
    [FailureMessage] nvarchar(256) NULL,
    [AttemptedAt] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    [NextRetryAt] datetime2 NULL,
    [ProviderResponse] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_PaymentAttempt] PRIMARY KEY ([PaymentAttemptID]),
    CONSTRAINT [FK_PaymentAttempt_PaymentIntent_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntent] ([PaymentIntentID])
);
GO

CREATE TABLE [Chargeback] (
    [ChargebackID] uniqueidentifier NOT NULL,
    [PaymentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [ProviderDisputeID] nvarchar(255) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [ReasonCode] nvarchar(100) NULL,
    [Status] nvarchar(50) NOT NULL,
    [OpenedAt] datetime2 NOT NULL,
    [EvidenceDueAt] datetime2 NULL,
    [EvidenceSubmittedAt] datetime2 NULL,
    [ResolvedAt] datetime2 NULL,
    [Outcome] nvarchar(50) NULL,
    [ProviderResponse] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Chargeback] PRIMARY KEY ([ChargebackID]),
    CONSTRAINT [FK_Chargeback_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_Chargeback_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_Chargeback_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [CreditReporting] (
    [CreditReportingID] uniqueidentifier NOT NULL,
    [CreditReportingEnrollmentID] uniqueidentifier NOT NULL,
    [InvoiceMasterID] uniqueidentifier NULL,
    [PaymentID] uniqueidentifier NULL,
    [ReportedAmount] decimal(18,2) NOT NULL,
    [WasPaidOnTime] bit NULL,
    [ReportingPeriodStart] datetime2 NULL,
    [ReportingPeriodEnd] datetime2 NULL,
    [ProviderStatus] nvarchar(100) NULL,
    [ProviderReferenceID] nvarchar(255) NULL,
    [ProviderResponse] nvarchar(256) NULL,
    [ReportedAt] datetime2 NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_CreditReporting] PRIMARY KEY ([CreditReportingID]),
    CONSTRAINT [FK_CreditReporting_CreditReportingEnrollment_CreditReportingEnrollmentID] FOREIGN KEY ([CreditReportingEnrollmentID]) REFERENCES [CreditReportingEnrollment] ([CreditReportingEnrollmentID]),
    CONSTRAINT [FK_CreditReporting_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_CreditReporting_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID])
);
GO

CREATE TABLE [PaymentAllocation] (
    [PaymentAllocationID] bigint NOT NULL IDENTITY,
    [PaymentID] uniqueidentifier NOT NULL,
    [InvoiceMasterID] uniqueidentifier NOT NULL,
    [InvoiceDetailID] uniqueidentifier NULL,
    [AllocatedAmount] decimal(18,2) NOT NULL,
    [AllocationType] nvarchar(100) NOT NULL,
    [AllocatedAt] datetime2 NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_PaymentAllocation] PRIMARY KEY ([PaymentAllocationID]),
    CONSTRAINT [FK_PaymentAllocation_InvoiceDetail_InvoiceDetailID] FOREIGN KEY ([InvoiceDetailID]) REFERENCES [InvoiceDetail] ([InvoiceDetailID]),
    CONSTRAINT [FK_PaymentAllocation_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_PaymentAllocation_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID])
);
GO

CREATE TABLE [PayoutItem] (
    [PayoutItemID] bigint NOT NULL IDENTITY,
    [PayoutID] bigint NOT NULL,
    [PaymentID] uniqueidentifier NOT NULL,
    [GrossAmount] decimal(18,2) NOT NULL,
    [DeductionAmount] decimal(18,2) NOT NULL,
    [NetPayoutAmount] decimal(18,2) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_PayoutItem] PRIMARY KEY ([PayoutItemID]),
    CONSTRAINT [FK_PayoutItem_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_PayoutItem_Payout_PayoutID] FOREIGN KEY ([PayoutID]) REFERENCES [Payout] ([PayoutID])
);
GO

CREATE TABLE [ReceiptMaster] (
    [ReceiptMasterID] bigint NOT NULL IDENTITY,
    [PaymentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [ReceiptNumber] nvarchar(100) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(10) NOT NULL,
    [IssuedAt] datetime2 NOT NULL,
    [VoidedAt] datetime2 NULL,
    [VoidReason] nvarchar(256) NULL,
    [CapturedBy] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_ReceiptMaster] PRIMARY KEY ([ReceiptMasterID]),
    CONSTRAINT [FK_ReceiptMaster_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_ReceiptMaster_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Refund] (
    [RefundID] uniqueidentifier NOT NULL,
    [PaymentID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(10) NOT NULL,
    [Reason] nvarchar(256) NULL,
    [Status] nvarchar(50) NOT NULL,
    [ProviderName] nvarchar(100) NULL,
    [ProviderRefundID] nvarchar(100) NULL,
    [RequestedAt] datetime2 NOT NULL,
    [ProcessedAt] datetime2 NULL,
    [FailedAt] datetime2 NULL,
    [FailureReason] nvarchar(256) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_Refund] PRIMARY KEY ([RefundID]),
    CONSTRAINT [FK_Refund_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_Refund_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [Dispute] (
    [DisputeID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [InvoiceMasterID] uniqueidentifier NULL,
    [PaymentID] uniqueidentifier NULL,
    [ChargebackID] uniqueidentifier NULL,
    [MaintenanceRequestID] uniqueidentifier NULL,
    [Status] nvarchar(50) NOT NULL,
    [Title] nvarchar(255) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [ResolutionNotes] nvarchar(1000) NULL,
    [OpenedAt] datetime2 NOT NULL,
    [ResolvedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Dispute] PRIMARY KEY ([DisputeID]),
    CONSTRAINT [FK_Dispute_Chargeback_ChargebackID] FOREIGN KEY ([ChargebackID]) REFERENCES [Chargeback] ([ChargebackID]),
    CONSTRAINT [FK_Dispute_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_Dispute_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_Dispute_MaintenanceRequest_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequest] ([MaintenanceRequestID]),
    CONSTRAINT [FK_Dispute_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_Dispute_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_Dispute_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [FraudCase] (
    [FraudCaseID] uniqueidentifier NOT NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationID] uniqueidentifier NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [PaymentIntentID] uniqueidentifier NULL,
    [PaymentID] uniqueidentifier NULL,
    [ChargebackID] uniqueidentifier NULL,
    [Status] nvarchar(50) NOT NULL,
    [RiskScore] decimal(18,2) NULL,
    [Reason] nvarchar(256) NULL,
    [IsBlocking] bit NOT NULL,
    [OpenedAt] datetime2 NOT NULL,
    [ReviewedAt] datetime2 NULL,
    [ClosedAt] datetime2 NULL,
    [Resolution] nvarchar(100) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_FraudCase] PRIMARY KEY ([FraudCaseID]),
    CONSTRAINT [FK_FraudCase_Chargeback_ChargebackID] FOREIGN KEY ([ChargebackID]) REFERENCES [Chargeback] ([ChargebackID]),
    CONSTRAINT [FK_FraudCase_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_FraudCase_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_FraudCase_PaymentIntent_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntent] ([PaymentIntentID]),
    CONSTRAINT [FK_FraudCase_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_FraudCase_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [LedgerTransaction] (
    [LedgerTransactionID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [TransactionType] nvarchar(100) NOT NULL,
    [TransactionDate] datetime2 NOT NULL,
    [Description] nvarchar(1000) NULL,
    [PaymentID] uniqueidentifier NULL,
    [InvoiceMasterID] uniqueidentifier NULL,
    [RefundID] uniqueidentifier NULL,
    [PayoutID] bigint NULL,
    [ReferenceNumber] nvarchar(256) NULL,
    [Status] nvarchar(50) NOT NULL,
    [ReversedTransactionID] uniqueidentifier NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_LedgerTransaction] PRIMARY KEY ([LedgerTransactionID]),
    CONSTRAINT [FK_LedgerTransaction_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_LedgerTransaction_Organization_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organization] ([OrganizationID]),
    CONSTRAINT [FK_LedgerTransaction_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_LedgerTransaction_Payout_PayoutID] FOREIGN KEY ([PayoutID]) REFERENCES [Payout] ([PayoutID]),
    CONSTRAINT [FK_LedgerTransaction_Refund_RefundID] FOREIGN KEY ([RefundID]) REFERENCES [Refund] ([RefundID])
);
GO

CREATE TABLE [SecurityDepositTransaction] (
    [SecurityDepositTransactionID] uniqueidentifier NOT NULL,
    [SecurityDepositID] uniqueidentifier NOT NULL,
    [PaymentID] uniqueidentifier NULL,
    [RefundID] uniqueidentifier NULL,
    [InvoiceMasterID] uniqueidentifier NULL,
    [InvoiceDetailID] uniqueidentifier NULL,
    [TransactionType] nvarchar(50) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Description] nvarchar(255) NULL,
    [OccurredAt] datetime2 NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_SecurityDepositTransaction] PRIMARY KEY ([SecurityDepositTransactionID]),
    CONSTRAINT [FK_SecurityDepositTransaction_InvoiceDetail_InvoiceDetailID] FOREIGN KEY ([InvoiceDetailID]) REFERENCES [InvoiceDetail] ([InvoiceDetailID]),
    CONSTRAINT [FK_SecurityDepositTransaction_InvoiceMaster_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMaster] ([InvoiceMasterID]),
    CONSTRAINT [FK_SecurityDepositTransaction_Payment_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payment] ([PaymentID]),
    CONSTRAINT [FK_SecurityDepositTransaction_Refund_RefundID] FOREIGN KEY ([RefundID]) REFERENCES [Refund] ([RefundID]),
    CONSTRAINT [FK_SecurityDepositTransaction_SecurityDeposit_SecurityDepositID] FOREIGN KEY ([SecurityDepositID]) REFERENCES [SecurityDeposit] ([SecurityDepositID])
);
GO

CREATE TABLE [Conversation] (
    [ConversationID] uniqueidentifier NOT NULL,
    [ConversationType] nvarchar(50) NOT NULL,
    [LeaseID] uniqueidentifier NULL,
    [LeaseRenewalID] uniqueidentifier NULL,
    [MaintenanceRequestID] uniqueidentifier NULL,
    [DisputeID] uniqueidentifier NULL,
    [Subject] nvarchar(200) NULL,
    [Status] nvarchar(50) NOT NULL,
    [LastMessageAt] datetime2 NULL,
    [ClosedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_Conversation] PRIMARY KEY ([ConversationID]),
    CONSTRAINT [FK_Conversation_Dispute_DisputeID] FOREIGN KEY ([DisputeID]) REFERENCES [Dispute] ([DisputeID]),
    CONSTRAINT [FK_Conversation_Lease_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Lease] ([LeaseID]),
    CONSTRAINT [FK_Conversation_MaintenanceRequest_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequest] ([MaintenanceRequestID])
);
GO

CREATE TABLE [LedgerEntry] (
    [LedgerEntryID] uniqueidentifier NOT NULL,
    [LedgerTransactionID] uniqueidentifier NOT NULL,
    [LedgerAccountID] uniqueidentifier NOT NULL,
    [DebitAmount] decimal(18,2) NOT NULL,
    [CreditAmount] decimal(18,2) NOT NULL,
    [Currency] nvarchar(3) NOT NULL,
    [Description] nvarchar(255) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LedgerEntry] PRIMARY KEY ([LedgerEntryID]),
    CONSTRAINT [FK_LedgerEntry_LedgerAccount_LedgerAccountID] FOREIGN KEY ([LedgerAccountID]) REFERENCES [LedgerAccount] ([LedgerAccountID]),
    CONSTRAINT [FK_LedgerEntry_LedgerTransaction_LedgerTransactionID] FOREIGN KEY ([LedgerTransactionID]) REFERENCES [LedgerTransaction] ([LedgerTransactionID])
);
GO

CREATE TABLE [ConversationMessage] (
    [ConversationMessageID] uniqueidentifier NOT NULL,
    [ConversationID] uniqueidentifier NOT NULL,
    [SenderUserID] bigint NULL,
    [SenderTenantID] uniqueidentifier NULL,
    [SenderOrganizationMemberID] uniqueidentifier NULL,
    [Message] nvarchar(256) NOT NULL,
    [MessageType] nvarchar(50) NOT NULL,
    [ReplyToMessageID] uniqueidentifier NULL,
    [SentAt] datetime2 NOT NULL,
    [EditedAt] datetime2 NULL,
    [DeletedAt] datetime2 NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(256) NULL,
    CONSTRAINT [PK_ConversationMessage] PRIMARY KEY ([ConversationMessageID]),
    CONSTRAINT [FK_ConversationMessage_AspNetUsers_SenderUserID] FOREIGN KEY ([SenderUserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ConversationMessage_ConversationMessage_ReplyToMessageID] FOREIGN KEY ([ReplyToMessageID]) REFERENCES [ConversationMessage] ([ConversationMessageID]),
    CONSTRAINT [FK_ConversationMessage_Conversation_ConversationID] FOREIGN KEY ([ConversationID]) REFERENCES [Conversation] ([ConversationID]),
    CONSTRAINT [FK_ConversationMessage_OrganizationMember_SenderOrganizationMemberID] FOREIGN KEY ([SenderOrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_ConversationMessage_Tenant_SenderTenantID] FOREIGN KEY ([SenderTenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE TABLE [ConversationParticipant] (
    [ConversationParticipantID] uniqueidentifier NOT NULL,
    [ConversationID] uniqueidentifier NOT NULL,
    [UserID] bigint NULL,
    [TenantID] uniqueidentifier NULL,
    [OrganizationMemberID] uniqueidentifier NULL,
    [ParticipantRole] nvarchar(50) NULL,
    [JoinedAt] datetime2 NOT NULL,
    [LeftAt] datetime2 NULL,
    [LastReadAt] datetime2 NULL,
    [IsMuted] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    CONSTRAINT [PK_ConversationParticipant] PRIMARY KEY ([ConversationParticipantID]),
    CONSTRAINT [FK_ConversationParticipant_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_ConversationParticipant_Conversation_ConversationID] FOREIGN KEY ([ConversationID]) REFERENCES [Conversation] ([ConversationID]),
    CONSTRAINT [FK_ConversationParticipant_OrganizationMember_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMember] ([OrganizationMemberID]),
    CONSTRAINT [FK_ConversationParticipant_Tenant_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenant] ([TenantID])
);
GO

CREATE INDEX [IX_Address_OrganizationID] ON [Address] ([OrganizationID]);
GO

CREATE INDEX [IX_ApplicationOccupant_TenantID] ON [ApplicationOccupant] ([TenantID]);
GO

CREATE INDEX [IX_ApplicationOccupant_UserID] ON [ApplicationOccupant] ([UserID]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_AuditLog_ActorUserID] ON [AuditLog] ([ActorUserID]);
GO

CREATE INDEX [IX_AuditLog_OrganizationID] ON [AuditLog] ([OrganizationID]);
GO

CREATE INDEX [IX_AuditLog_OrganizationMemberID] ON [AuditLog] ([OrganizationMemberID]);
GO

CREATE INDEX [IX_AuditLog_TenantID] ON [AuditLog] ([TenantID]);
GO

CREATE INDEX [IX_AutopayConsentAudit_AutopayMandateID] ON [AutopayConsentAudit] ([AutopayMandateID]);
GO

CREATE INDEX [IX_AutopayConsentAudit_TenantID] ON [AutopayConsentAudit] ([TenantID]);
GO

CREATE INDEX [IX_AutopayMandate_LeaseID] ON [AutopayMandate] ([LeaseID]);
GO

CREATE INDEX [IX_AutopayMandate_PaymentMethodID] ON [AutopayMandate] ([PaymentMethodID]);
GO

CREATE INDEX [IX_AutopayMandate_TenantID] ON [AutopayMandate] ([TenantID]);
GO

CREATE INDEX [IX_CalendarEvent_LeaseID] ON [CalendarEvent] ([LeaseID]);
GO

CREATE INDEX [IX_CalendarEvent_ListingID] ON [CalendarEvent] ([ListingID]);
GO

CREATE INDEX [IX_CalendarEvent_MaintenanceRequestID] ON [CalendarEvent] ([MaintenanceRequestID]);
GO

CREATE INDEX [IX_CalendarEvent_RentalApplicationID] ON [CalendarEvent] ([RentalApplicationID]);
GO

CREATE INDEX [IX_CalendarEvent_ReservationHoldID] ON [CalendarEvent] ([ReservationHoldID]);
GO

CREATE INDEX [IX_Chargeback_OrganizationID] ON [Chargeback] ([OrganizationID]);
GO

CREATE INDEX [IX_Chargeback_PaymentID] ON [Chargeback] ([PaymentID]);
GO

CREATE INDEX [IX_Chargeback_TenantID] ON [Chargeback] ([TenantID]);
GO

CREATE INDEX [IX_Contractor_OrganizationID] ON [Contractor] ([OrganizationID]);
GO

CREATE INDEX [IX_Conversation_DisputeID] ON [Conversation] ([DisputeID]);
GO

CREATE INDEX [IX_Conversation_LeaseID] ON [Conversation] ([LeaseID]);
GO

CREATE INDEX [IX_Conversation_MaintenanceRequestID] ON [Conversation] ([MaintenanceRequestID]);
GO

CREATE INDEX [IX_ConversationMessage_ConversationID] ON [ConversationMessage] ([ConversationID]);
GO

CREATE INDEX [IX_ConversationMessage_ReplyToMessageID] ON [ConversationMessage] ([ReplyToMessageID]);
GO

CREATE INDEX [IX_ConversationMessage_SenderOrganizationMemberID] ON [ConversationMessage] ([SenderOrganizationMemberID]);
GO

CREATE INDEX [IX_ConversationMessage_SenderTenantID] ON [ConversationMessage] ([SenderTenantID]);
GO

CREATE INDEX [IX_ConversationMessage_SenderUserID] ON [ConversationMessage] ([SenderUserID]);
GO

CREATE INDEX [IX_ConversationParticipant_ConversationID] ON [ConversationParticipant] ([ConversationID]);
GO

CREATE INDEX [IX_ConversationParticipant_OrganizationMemberID] ON [ConversationParticipant] ([OrganizationMemberID]);
GO

CREATE INDEX [IX_ConversationParticipant_TenantID] ON [ConversationParticipant] ([TenantID]);
GO

CREATE INDEX [IX_ConversationParticipant_UserID] ON [ConversationParticipant] ([UserID]);
GO

CREATE INDEX [IX_CreditReporting_CreditReportingEnrollmentID] ON [CreditReporting] ([CreditReportingEnrollmentID]);
GO

CREATE INDEX [IX_CreditReporting_InvoiceMasterID] ON [CreditReporting] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_CreditReporting_PaymentID] ON [CreditReporting] ([PaymentID]);
GO

CREATE INDEX [IX_CreditReportingConsentAudit_CreditReportingEnrollmentID] ON [CreditReportingConsentAudit] ([CreditReportingEnrollmentID]);
GO

CREATE INDEX [IX_CreditReportingConsentAudit_TenantID] ON [CreditReportingConsentAudit] ([TenantID]);
GO

CREATE INDEX [IX_CreditReportingEnrollment_LeaseID] ON [CreditReportingEnrollment] ([LeaseID]);
GO

CREATE INDEX [IX_CreditReportingEnrollment_TenantID] ON [CreditReportingEnrollment] ([TenantID]);
GO

CREATE INDEX [IX_Dispute_ChargebackID] ON [Dispute] ([ChargebackID]);
GO

CREATE INDEX [IX_Dispute_InvoiceMasterID] ON [Dispute] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_Dispute_LeaseID] ON [Dispute] ([LeaseID]);
GO

CREATE INDEX [IX_Dispute_MaintenanceRequestID] ON [Dispute] ([MaintenanceRequestID]);
GO

CREATE INDEX [IX_Dispute_OrganizationID] ON [Dispute] ([OrganizationID]);
GO

CREATE INDEX [IX_Dispute_PaymentID] ON [Dispute] ([PaymentID]);
GO

CREATE INDEX [IX_Dispute_TenantID] ON [Dispute] ([TenantID]);
GO

CREATE INDEX [IX_Fee_FeeTypeID] ON [Fee] ([FeeTypeID]);
GO

CREATE INDEX [IX_Fee_OrganizationID] ON [Fee] ([OrganizationID]);
GO

CREATE INDEX [IX_FraudCase_ChargebackID] ON [FraudCase] ([ChargebackID]);
GO

CREATE INDEX [IX_FraudCase_LeaseID] ON [FraudCase] ([LeaseID]);
GO

CREATE INDEX [IX_FraudCase_OrganizationID] ON [FraudCase] ([OrganizationID]);
GO

CREATE INDEX [IX_FraudCase_PaymentID] ON [FraudCase] ([PaymentID]);
GO

CREATE INDEX [IX_FraudCase_PaymentIntentID] ON [FraudCase] ([PaymentIntentID]);
GO

CREATE INDEX [IX_FraudCase_TenantID] ON [FraudCase] ([TenantID]);
GO

CREATE INDEX [IX_IdentityVerification_UserID] ON [IdentityVerification] ([UserID]);
GO

CREATE INDEX [IX_Inspection_LeaseID] ON [Inspection] ([LeaseID]);
GO

CREATE INDEX [IX_Inspection_PropertyID] ON [Inspection] ([PropertyID]);
GO

CREATE INDEX [IX_Inspection_RentalUnitID] ON [Inspection] ([RentalUnitID]);
GO

CREATE INDEX [IX_InspectionItem_InspectionID] ON [InspectionItem] ([InspectionID]);
GO

CREATE INDEX [IX_InvoiceDetail_FeeID] ON [InvoiceDetail] ([FeeID]);
GO

CREATE INDEX [IX_InvoiceDetail_InvoiceMasterID] ON [InvoiceDetail] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_InvoiceMaster_LeaseID] ON [InvoiceMaster] ([LeaseID]);
GO

CREATE INDEX [IX_InvoiceMaster_LeaseRenewalID] ON [InvoiceMaster] ([LeaseRenewalID]);
GO

CREATE INDEX [IX_InvoiceMaster_OrganizationID] ON [InvoiceMaster] ([OrganizationID]);
GO

CREATE INDEX [IX_InvoiceMaster_TenantID] ON [InvoiceMaster] ([TenantID]);
GO

CREATE INDEX [IX_Lease_ListingID] ON [Lease] ([ListingID]);
GO

CREATE INDEX [IX_Lease_OrganizationID] ON [Lease] ([OrganizationID]);
GO

CREATE INDEX [IX_Lease_RentalApplicationID] ON [Lease] ([RentalApplicationID]);
GO

CREATE INDEX [IX_Lease_RentalUnitID] ON [Lease] ([RentalUnitID]);
GO

CREATE INDEX [IX_Lease_TenancyTypeID] ON [Lease] ([TenancyTypeID]);
GO

CREATE INDEX [IX_Lease_TenantID] ON [Lease] ([TenantID]);
GO

CREATE INDEX [IX_LeaseDocuments_LeaseID] ON [LeaseDocuments] ([LeaseID]);
GO

CREATE INDEX [IX_LeaseDocuments_LeaseRenewalID] ON [LeaseDocuments] ([LeaseRenewalID]);
GO

CREATE INDEX [IX_LeaseOccupants_LeaseID] ON [LeaseOccupants] ([LeaseID]);
GO

CREATE INDEX [IX_LeaseOccupants_LeaseRenewalID] ON [LeaseOccupants] ([LeaseRenewalID]);
GO

CREATE INDEX [IX_LeaseOccupants_TenantID] ON [LeaseOccupants] ([TenantID]);
GO

CREATE INDEX [IX_LeaseOccupants_UserID] ON [LeaseOccupants] ([UserID]);
GO

CREATE INDEX [IX_LeaseRecurringCharges_FeeID] ON [LeaseRecurringCharges] ([FeeID]);
GO

CREATE INDEX [IX_LeaseRecurringCharges_LeaseID] ON [LeaseRecurringCharges] ([LeaseID]);
GO

CREATE INDEX [IX_LeaseRenewals_LeaseID] ON [LeaseRenewals] ([LeaseID]);
GO

CREATE INDEX [IX_LeaseSignatory_OrganizationID] ON [LeaseSignatory] ([OrganizationID]);
GO

CREATE INDEX [IX_LeaseSignatory_OrganizationMemberID] ON [LeaseSignatory] ([OrganizationMemberID]);
GO

CREATE INDEX [IX_LeaseSignatory_TenantID] ON [LeaseSignatory] ([TenantID]);
GO

CREATE INDEX [IX_LeaseSignatory_UserID] ON [LeaseSignatory] ([UserID]);
GO

CREATE INDEX [IX_LedgerAccount_OrganizationID] ON [LedgerAccount] ([OrganizationID]);
GO

CREATE INDEX [IX_LedgerEntry_LedgerAccountID] ON [LedgerEntry] ([LedgerAccountID]);
GO

CREATE INDEX [IX_LedgerEntry_LedgerTransactionID] ON [LedgerEntry] ([LedgerTransactionID]);
GO

CREATE INDEX [IX_LedgerTransaction_InvoiceMasterID] ON [LedgerTransaction] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_LedgerTransaction_OrganizationID] ON [LedgerTransaction] ([OrganizationID]);
GO

CREATE INDEX [IX_LedgerTransaction_PaymentID] ON [LedgerTransaction] ([PaymentID]);
GO

CREATE INDEX [IX_LedgerTransaction_PayoutID] ON [LedgerTransaction] ([PayoutID]);
GO

CREATE INDEX [IX_LedgerTransaction_RefundID] ON [LedgerTransaction] ([RefundID]);
GO

CREATE INDEX [IX_Listing_ListingTypeID] ON [Listing] ([ListingTypeID]);
GO

CREATE INDEX [IX_Listing_OrganizationID] ON [Listing] ([OrganizationID]);
GO

CREATE INDEX [IX_Listing_RentalUnitID] ON [Listing] ([RentalUnitID]);
GO

CREATE INDEX [IX_ListingAccessInstruction_LeaseID] ON [ListingAccessInstruction] ([LeaseID]);
GO

CREATE INDEX [IX_ListingAccessInstruction_ListingID] ON [ListingAccessInstruction] ([ListingID]);
GO

CREATE INDEX [IX_ListingAmenity_AmenityID] ON [ListingAmenity] ([AmenityID]);
GO

CREATE INDEX [IX_ListingAmenity_ListingID] ON [ListingAmenity] ([ListingID]);
GO

CREATE INDEX [IX_ListingPhoto_ListingID] ON [ListingPhoto] ([ListingID]);
GO

CREATE INDEX [IX_ListingPolicy_ListingID] ON [ListingPolicy] ([ListingID]);
GO

CREATE INDEX [IX_ListingRule_ListingID] ON [ListingRule] ([ListingID]);
GO

CREATE INDEX [IX_ListingTermPrice_ListingID] ON [ListingTermPrice] ([ListingID]);
GO

CREATE INDEX [IX_MaintenanceRequest_CategoryID] ON [MaintenanceRequest] ([CategoryID]);
GO

CREATE INDEX [IX_MaintenanceRequest_LeaseID] ON [MaintenanceRequest] ([LeaseID]);
GO

CREATE INDEX [IX_MaintenanceRequest_ListingID] ON [MaintenanceRequest] ([ListingID]);
GO

CREATE INDEX [IX_MaintenanceRequest_PropertyID] ON [MaintenanceRequest] ([PropertyID]);
GO

CREATE INDEX [IX_MaintenanceRequest_RentalUnitID] ON [MaintenanceRequest] ([RentalUnitID]);
GO

CREATE INDEX [IX_MaintenanceRequest_SubmittedByTenantID] ON [MaintenanceRequest] ([SubmittedByTenantID]);
GO

CREATE INDEX [IX_Notification_OrganizationID] ON [Notification] ([OrganizationID]);
GO

CREATE INDEX [IX_Notification_OrganizationMemberID] ON [Notification] ([OrganizationMemberID]);
GO

CREATE INDEX [IX_Notification_RecipientUserID] ON [Notification] ([RecipientUserID]);
GO

CREATE INDEX [IX_Notification_TenantID] ON [Notification] ([TenantID]);
GO

CREATE INDEX [IX_OrganizationMember_OrganizationID] ON [OrganizationMember] ([OrganizationID]);
GO

CREATE INDEX [IX_OrganizationMember_UserID] ON [OrganizationMember] ([UserID]);
GO

CREATE INDEX [IX_OrganizationStatement_OrganizationID] ON [OrganizationStatement] ([OrganizationID]);
GO

CREATE INDEX [IX_OrgPayoutAccount_OrganizationID] ON [OrgPayoutAccount] ([OrganizationID]);
GO

CREATE INDEX [IX_OrgSubscription_OrganizationID] ON [OrgSubscription] ([OrganizationID]);
GO

CREATE INDEX [IX_OrgSubscription_SubscriptionPlanID] ON [OrgSubscription] ([SubscriptionPlanID]);
GO

CREATE INDEX [IX_Payment_PaymentIntentID] ON [Payment] ([PaymentIntentID]);
GO

CREATE INDEX [IX_Payment_TenantID] ON [Payment] ([TenantID]);
GO

CREATE INDEX [IX_PaymentAllocation_InvoiceDetailID] ON [PaymentAllocation] ([InvoiceDetailID]);
GO

CREATE INDEX [IX_PaymentAllocation_InvoiceMasterID] ON [PaymentAllocation] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_PaymentAllocation_PaymentID] ON [PaymentAllocation] ([PaymentID]);
GO

CREATE INDEX [IX_PaymentAttempt_PaymentIntentID] ON [PaymentAttempt] ([PaymentIntentID]);
GO

CREATE INDEX [IX_PaymentIntent_AutopayMandateID] ON [PaymentIntent] ([AutopayMandateID]);
GO

CREATE INDEX [IX_PaymentIntent_InvoiceMasterID] ON [PaymentIntent] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_PaymentIntent_LeaseID] ON [PaymentIntent] ([LeaseID]);
GO

CREATE INDEX [IX_PaymentIntent_PaymentMethodID] ON [PaymentIntent] ([PaymentMethodID]);
GO

CREATE INDEX [IX_PaymentIntent_TenantID] ON [PaymentIntent] ([TenantID]);
GO

CREATE INDEX [IX_PaymentMethod_TenantID] ON [PaymentMethod] ([TenantID]);
GO

CREATE INDEX [IX_PaymentReminder_InvoiceMasterID] ON [PaymentReminder] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_Payout_OrganizationID] ON [Payout] ([OrganizationID]);
GO

CREATE INDEX [IX_Payout_OrgPayoutAccountID] ON [Payout] ([OrgPayoutAccountID]);
GO

CREATE INDEX [IX_PayoutItem_PaymentID] ON [PayoutItem] ([PaymentID]);
GO

CREATE INDEX [IX_PayoutItem_PayoutID] ON [PayoutItem] ([PayoutID]);
GO

CREATE INDEX [IX_Property_AddressID] ON [Property] ([AddressID]);
GO

CREATE INDEX [IX_Property_OrganizationID] ON [Property] ([OrganizationID]);
GO

CREATE INDEX [IX_Rating_LeaseID] ON [Rating] ([LeaseID]);
GO

CREATE INDEX [IX_Rating_ReviewerUserID] ON [Rating] ([ReviewerUserID]);
GO

CREATE INDEX [IX_ReceiptMaster_PaymentID] ON [ReceiptMaster] ([PaymentID]);
GO

CREATE INDEX [IX_ReceiptMaster_TenantID] ON [ReceiptMaster] ([TenantID]);
GO

CREATE INDEX [IX_Refund_PaymentID] ON [Refund] ([PaymentID]);
GO

CREATE INDEX [IX_Refund_TenantID] ON [Refund] ([TenantID]);
GO

CREATE INDEX [IX_RentalApplication_ListingID] ON [RentalApplication] ([ListingID]);
GO

CREATE INDEX [IX_RentalApplication_OrganizationID] ON [RentalApplication] ([OrganizationID]);
GO

CREATE INDEX [IX_RentalApplication_ReviewedByOrganizationMemberID] ON [RentalApplication] ([ReviewedByOrganizationMemberID]);
GO

CREATE INDEX [IX_RentalApplication_TenantID] ON [RentalApplication] ([TenantID]);
GO

CREATE INDEX [IX_RentalUnit_PropertyID] ON [RentalUnit] ([PropertyID]);
GO

CREATE INDEX [IX_RentalUnit_UnitTypeID] ON [RentalUnit] ([UnitTypeID]);
GO

CREATE INDEX [IX_ReservationHold_ListingID] ON [ReservationHold] ([ListingID]);
GO

CREATE INDEX [IX_ReservationHold_RentalApplicationID] ON [ReservationHold] ([RentalApplicationID]);
GO

CREATE INDEX [IX_ReservationHold_TenantID] ON [ReservationHold] ([TenantID]);
GO

CREATE INDEX [IX_SecurityDeposit_LeaseID] ON [SecurityDeposit] ([LeaseID]);
GO

CREATE INDEX [IX_SecurityDeposit_OrganizationID] ON [SecurityDeposit] ([OrganizationID]);
GO

CREATE INDEX [IX_SecurityDeposit_TenantID] ON [SecurityDeposit] ([TenantID]);
GO

CREATE INDEX [IX_SecurityDepositTransaction_InvoiceDetailID] ON [SecurityDepositTransaction] ([InvoiceDetailID]);
GO

CREATE INDEX [IX_SecurityDepositTransaction_InvoiceMasterID] ON [SecurityDepositTransaction] ([InvoiceMasterID]);
GO

CREATE INDEX [IX_SecurityDepositTransaction_PaymentID] ON [SecurityDepositTransaction] ([PaymentID]);
GO

CREATE INDEX [IX_SecurityDepositTransaction_RefundID] ON [SecurityDepositTransaction] ([RefundID]);
GO

CREATE INDEX [IX_SecurityDepositTransaction_SecurityDepositID] ON [SecurityDepositTransaction] ([SecurityDepositID]);
GO

CREATE INDEX [IX_Tenant_UserID] ON [Tenant] ([UserID]);
GO

CREATE INDEX [IX_TenantEmergencyContact_TenantID] ON [TenantEmergencyContact] ([TenantID]);
GO

CREATE INDEX [IX_TenantEmployment_TenantID] ON [TenantEmployment] ([TenantID]);
GO

CREATE INDEX [IX_TenantGuarantor_TenantID] ON [TenantGuarantor] ([TenantID]);
GO

CREATE INDEX [IX_TenantGuarantor_UserID] ON [TenantGuarantor] ([UserID]);
GO

CREATE INDEX [IX_TenantInvitation_LeaseID] ON [TenantInvitation] ([LeaseID]);
GO

CREATE INDEX [IX_TenantInvitation_RentalApplicationID] ON [TenantInvitation] ([RentalApplicationID]);
GO

CREATE INDEX [IX_TenantScreeningCheck_TenantID] ON [TenantScreeningCheck] ([TenantID]);
GO

CREATE INDEX [IX_ViewingAppointments_AssignedOrganizationMemberID] ON [ViewingAppointments] ([AssignedOrganizationMemberID]);
GO

CREATE INDEX [IX_ViewingAppointments_ListingID] ON [ViewingAppointments] ([ListingID]);
GO

CREATE INDEX [IX_ViewingAppointments_RequestedByUserID] ON [ViewingAppointments] ([RequestedByUserID]);
GO

CREATE INDEX [IX_ViewingAppointments_TenantID] ON [ViewingAppointments] ([TenantID]);
GO

CREATE INDEX [IX_WorkOrder_AssignedOrganizationMemberID] ON [WorkOrder] ([AssignedOrganizationMemberID]);
GO

CREATE INDEX [IX_WorkOrder_ContractorID] ON [WorkOrder] ([ContractorID]);
GO

CREATE INDEX [IX_WorkOrder_LeaseID] ON [WorkOrder] ([LeaseID]);
GO

CREATE INDEX [IX_WorkOrder_MaintenanceRequestID] ON [WorkOrder] ([MaintenanceRequestID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260903010118_InitialCreate', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Address] DROP CONSTRAINT [FK_Address_Organization_OrganizationID];
GO

ALTER TABLE [ApplicationOccupant] DROP CONSTRAINT [FK_ApplicationOccupant_AspNetUsers_UserID];
GO

ALTER TABLE [ApplicationOccupant] DROP CONSTRAINT [FK_ApplicationOccupant_Tenant_TenantID];
GO

ALTER TABLE [AuditLog] DROP CONSTRAINT [FK_AuditLog_AspNetUsers_ActorUserID];
GO

ALTER TABLE [AuditLog] DROP CONSTRAINT [FK_AuditLog_OrganizationMember_OrganizationMemberID];
GO

ALTER TABLE [AuditLog] DROP CONSTRAINT [FK_AuditLog_Organization_OrganizationID];
GO

ALTER TABLE [AuditLog] DROP CONSTRAINT [FK_AuditLog_Tenant_TenantID];
GO

ALTER TABLE [AutopayConsentAudit] DROP CONSTRAINT [FK_AutopayConsentAudit_AutopayMandate_AutopayMandateID];
GO

ALTER TABLE [AutopayConsentAudit] DROP CONSTRAINT [FK_AutopayConsentAudit_Tenant_TenantID];
GO

ALTER TABLE [AutopayMandate] DROP CONSTRAINT [FK_AutopayMandate_Lease_LeaseID];
GO

ALTER TABLE [AutopayMandate] DROP CONSTRAINT [FK_AutopayMandate_PaymentMethod_PaymentMethodID];
GO

ALTER TABLE [AutopayMandate] DROP CONSTRAINT [FK_AutopayMandate_Tenant_TenantID];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [FK_CalendarEvent_Lease_LeaseID];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [FK_CalendarEvent_Listing_ListingID];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [FK_CalendarEvent_MaintenanceRequest_MaintenanceRequestID];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [FK_CalendarEvent_RentalApplication_RentalApplicationID];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [FK_CalendarEvent_ReservationHold_ReservationHoldID];
GO

ALTER TABLE [Chargeback] DROP CONSTRAINT [FK_Chargeback_Organization_OrganizationID];
GO

ALTER TABLE [Chargeback] DROP CONSTRAINT [FK_Chargeback_Payment_PaymentID];
GO

ALTER TABLE [Chargeback] DROP CONSTRAINT [FK_Chargeback_Tenant_TenantID];
GO

ALTER TABLE [Contractor] DROP CONSTRAINT [FK_Contractor_Organization_OrganizationID];
GO

ALTER TABLE [Conversation] DROP CONSTRAINT [FK_Conversation_Dispute_DisputeID];
GO

ALTER TABLE [Conversation] DROP CONSTRAINT [FK_Conversation_Lease_LeaseID];
GO

ALTER TABLE [Conversation] DROP CONSTRAINT [FK_Conversation_MaintenanceRequest_MaintenanceRequestID];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [FK_ConversationMessage_AspNetUsers_SenderUserID];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [FK_ConversationMessage_ConversationMessage_ReplyToMessageID];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [FK_ConversationMessage_Conversation_ConversationID];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [FK_ConversationMessage_OrganizationMember_SenderOrganizationMemberID];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [FK_ConversationMessage_Tenant_SenderTenantID];
GO

ALTER TABLE [ConversationParticipant] DROP CONSTRAINT [FK_ConversationParticipant_AspNetUsers_UserID];
GO

ALTER TABLE [ConversationParticipant] DROP CONSTRAINT [FK_ConversationParticipant_Conversation_ConversationID];
GO

ALTER TABLE [ConversationParticipant] DROP CONSTRAINT [FK_ConversationParticipant_OrganizationMember_OrganizationMemberID];
GO

ALTER TABLE [ConversationParticipant] DROP CONSTRAINT [FK_ConversationParticipant_Tenant_TenantID];
GO

ALTER TABLE [CreditReporting] DROP CONSTRAINT [FK_CreditReporting_CreditReportingEnrollment_CreditReportingEnrollmentID];
GO

ALTER TABLE [CreditReporting] DROP CONSTRAINT [FK_CreditReporting_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [CreditReporting] DROP CONSTRAINT [FK_CreditReporting_Payment_PaymentID];
GO

ALTER TABLE [CreditReportingConsentAudit] DROP CONSTRAINT [FK_CreditReportingConsentAudit_CreditReportingEnrollment_CreditReportingEnrollmentID];
GO

ALTER TABLE [CreditReportingConsentAudit] DROP CONSTRAINT [FK_CreditReportingConsentAudit_Tenant_TenantID];
GO

ALTER TABLE [CreditReportingEnrollment] DROP CONSTRAINT [FK_CreditReportingEnrollment_Lease_LeaseID];
GO

ALTER TABLE [CreditReportingEnrollment] DROP CONSTRAINT [FK_CreditReportingEnrollment_Tenant_TenantID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_Chargeback_ChargebackID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_Lease_LeaseID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_MaintenanceRequest_MaintenanceRequestID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_Organization_OrganizationID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_Payment_PaymentID];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [FK_Dispute_Tenant_TenantID];
GO

ALTER TABLE [Fee] DROP CONSTRAINT [FK_Fee_FeeType_FeeTypeID];
GO

ALTER TABLE [Fee] DROP CONSTRAINT [FK_Fee_Organization_OrganizationID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_Chargeback_ChargebackID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_Lease_LeaseID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_Organization_OrganizationID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_PaymentIntent_PaymentIntentID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_Payment_PaymentID];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [FK_FraudCase_Tenant_TenantID];
GO

ALTER TABLE [IdentityVerification] DROP CONSTRAINT [FK_IdentityVerification_AspNetUsers_UserID];
GO

ALTER TABLE [Inspection] DROP CONSTRAINT [FK_Inspection_Lease_LeaseID];
GO

ALTER TABLE [Inspection] DROP CONSTRAINT [FK_Inspection_Property_PropertyID];
GO

ALTER TABLE [Inspection] DROP CONSTRAINT [FK_Inspection_RentalUnit_RentalUnitID];
GO

ALTER TABLE [InspectionItem] DROP CONSTRAINT [FK_InspectionItem_Inspection_InspectionID];
GO

ALTER TABLE [InvoiceDetail] DROP CONSTRAINT [FK_InvoiceDetail_Fee_FeeID];
GO

ALTER TABLE [InvoiceDetail] DROP CONSTRAINT [FK_InvoiceDetail_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [InvoiceMaster] DROP CONSTRAINT [FK_InvoiceMaster_Lease_LeaseID];
GO

ALTER TABLE [InvoiceMaster] DROP CONSTRAINT [FK_InvoiceMaster_Lease_LeaseRenewalID];
GO

ALTER TABLE [InvoiceMaster] DROP CONSTRAINT [FK_InvoiceMaster_Organization_OrganizationID];
GO

ALTER TABLE [InvoiceMaster] DROP CONSTRAINT [FK_InvoiceMaster_Tenant_TenantID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_Listing_ListingID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_Organization_OrganizationID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_RentalApplication_RentalApplicationID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_RentalUnit_RentalUnitID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_TenancyType_TenancyTypeID];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [FK_Lease_Tenant_TenantID];
GO

ALTER TABLE [LeaseDocuments] DROP CONSTRAINT [FK_LeaseDocuments_Lease_LeaseID];
GO

ALTER TABLE [LeaseOccupants] DROP CONSTRAINT [FK_LeaseOccupants_Lease_LeaseID];
GO

ALTER TABLE [LeaseOccupants] DROP CONSTRAINT [FK_LeaseOccupants_Tenant_TenantID];
GO

ALTER TABLE [LeaseRecurringCharges] DROP CONSTRAINT [FK_LeaseRecurringCharges_Fee_FeeID];
GO

ALTER TABLE [LeaseRecurringCharges] DROP CONSTRAINT [FK_LeaseRecurringCharges_Lease_LeaseID];
GO

ALTER TABLE [LeaseRenewals] DROP CONSTRAINT [FK_LeaseRenewals_Lease_LeaseID];
GO

ALTER TABLE [LeaseSignatory] DROP CONSTRAINT [FK_LeaseSignatory_AspNetUsers_UserID];
GO

ALTER TABLE [LeaseSignatory] DROP CONSTRAINT [FK_LeaseSignatory_OrganizationMember_OrganizationMemberID];
GO

ALTER TABLE [LeaseSignatory] DROP CONSTRAINT [FK_LeaseSignatory_Organization_OrganizationID];
GO

ALTER TABLE [LeaseSignatory] DROP CONSTRAINT [FK_LeaseSignatory_Tenant_TenantID];
GO

ALTER TABLE [LedgerAccount] DROP CONSTRAINT [FK_LedgerAccount_Organization_OrganizationID];
GO

ALTER TABLE [LedgerEntry] DROP CONSTRAINT [FK_LedgerEntry_LedgerAccount_LedgerAccountID];
GO

ALTER TABLE [LedgerEntry] DROP CONSTRAINT [FK_LedgerEntry_LedgerTransaction_LedgerTransactionID];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [FK_LedgerTransaction_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [FK_LedgerTransaction_Organization_OrganizationID];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [FK_LedgerTransaction_Payment_PaymentID];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [FK_LedgerTransaction_Payout_PayoutID];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [FK_LedgerTransaction_Refund_RefundID];
GO

ALTER TABLE [Listing] DROP CONSTRAINT [FK_Listing_ListingType_ListingTypeID];
GO

ALTER TABLE [Listing] DROP CONSTRAINT [FK_Listing_Organization_OrganizationID];
GO

ALTER TABLE [Listing] DROP CONSTRAINT [FK_Listing_RentalUnit_RentalUnitID];
GO

ALTER TABLE [ListingAccessInstruction] DROP CONSTRAINT [FK_ListingAccessInstruction_Lease_LeaseID];
GO

ALTER TABLE [ListingAccessInstruction] DROP CONSTRAINT [FK_ListingAccessInstruction_Listing_ListingID];
GO

ALTER TABLE [ListingAmenity] DROP CONSTRAINT [FK_ListingAmenity_AmenityCatalog_AmenityID];
GO

ALTER TABLE [ListingAmenity] DROP CONSTRAINT [FK_ListingAmenity_Listing_ListingID];
GO

ALTER TABLE [ListingPhoto] DROP CONSTRAINT [FK_ListingPhoto_Listing_ListingID];
GO

ALTER TABLE [ListingPolicy] DROP CONSTRAINT [FK_ListingPolicy_Listing_ListingID];
GO

ALTER TABLE [ListingRule] DROP CONSTRAINT [FK_ListingRule_Listing_ListingID];
GO

ALTER TABLE [ListingTermPrice] DROP CONSTRAINT [FK_ListingTermPrice_Listing_ListingID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_Category_CategoryID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_Lease_LeaseID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_Listing_ListingID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_Property_PropertyID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_RentalUnit_RentalUnitID];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [FK_MaintenanceRequest_Tenant_SubmittedByTenantID];
GO

ALTER TABLE [Notification] DROP CONSTRAINT [FK_Notification_AspNetUsers_RecipientUserID];
GO

ALTER TABLE [Notification] DROP CONSTRAINT [FK_Notification_OrganizationMember_OrganizationMemberID];
GO

ALTER TABLE [Notification] DROP CONSTRAINT [FK_Notification_Organization_OrganizationID];
GO

ALTER TABLE [Notification] DROP CONSTRAINT [FK_Notification_Tenant_TenantID];
GO

ALTER TABLE [OrganizationMember] DROP CONSTRAINT [FK_OrganizationMember_AspNetUsers_UserID];
GO

ALTER TABLE [OrganizationMember] DROP CONSTRAINT [FK_OrganizationMember_Organization_OrganizationID];
GO

ALTER TABLE [OrganizationStatement] DROP CONSTRAINT [FK_OrganizationStatement_Organization_OrganizationID];
GO

ALTER TABLE [OrgPayoutAccount] DROP CONSTRAINT [FK_OrgPayoutAccount_Organization_OrganizationID];
GO

ALTER TABLE [OrgSubscription] DROP CONSTRAINT [FK_OrgSubscription_Organization_OrganizationID];
GO

ALTER TABLE [OrgSubscription] DROP CONSTRAINT [FK_OrgSubscription_SubscriptionPlan_SubscriptionPlanID];
GO

ALTER TABLE [Payment] DROP CONSTRAINT [FK_Payment_PaymentIntent_PaymentIntentID];
GO

ALTER TABLE [Payment] DROP CONSTRAINT [FK_Payment_Tenant_TenantID];
GO

ALTER TABLE [PaymentAllocation] DROP CONSTRAINT [FK_PaymentAllocation_InvoiceDetail_InvoiceDetailID];
GO

ALTER TABLE [PaymentAllocation] DROP CONSTRAINT [FK_PaymentAllocation_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [PaymentAllocation] DROP CONSTRAINT [FK_PaymentAllocation_Payment_PaymentID];
GO

ALTER TABLE [PaymentAttempt] DROP CONSTRAINT [FK_PaymentAttempt_PaymentIntent_PaymentIntentID];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [FK_PaymentIntent_AutopayMandate_AutopayMandateID];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [FK_PaymentIntent_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [FK_PaymentIntent_Lease_LeaseID];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [FK_PaymentIntent_PaymentMethod_PaymentMethodID];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [FK_PaymentIntent_Tenant_TenantID];
GO

ALTER TABLE [PaymentMethod] DROP CONSTRAINT [FK_PaymentMethod_Tenant_TenantID];
GO

ALTER TABLE [PaymentReminder] DROP CONSTRAINT [FK_PaymentReminder_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [Payout] DROP CONSTRAINT [FK_Payout_OrgPayoutAccount_OrgPayoutAccountID];
GO

ALTER TABLE [Payout] DROP CONSTRAINT [FK_Payout_Organization_OrganizationID];
GO

ALTER TABLE [PayoutItem] DROP CONSTRAINT [FK_PayoutItem_Payment_PaymentID];
GO

ALTER TABLE [PayoutItem] DROP CONSTRAINT [FK_PayoutItem_Payout_PayoutID];
GO

ALTER TABLE [Property] DROP CONSTRAINT [FK_Property_Address_AddressID];
GO

ALTER TABLE [Property] DROP CONSTRAINT [FK_Property_Organization_OrganizationID];
GO

ALTER TABLE [Rating] DROP CONSTRAINT [FK_Rating_AspNetUsers_ReviewerUserID];
GO

ALTER TABLE [Rating] DROP CONSTRAINT [FK_Rating_Lease_LeaseID];
GO

ALTER TABLE [ReceiptMaster] DROP CONSTRAINT [FK_ReceiptMaster_Payment_PaymentID];
GO

ALTER TABLE [ReceiptMaster] DROP CONSTRAINT [FK_ReceiptMaster_Tenant_TenantID];
GO

ALTER TABLE [Refund] DROP CONSTRAINT [FK_Refund_Payment_PaymentID];
GO

ALTER TABLE [Refund] DROP CONSTRAINT [FK_Refund_Tenant_TenantID];
GO

ALTER TABLE [RentalApplication] DROP CONSTRAINT [FK_RentalApplication_Listing_ListingID];
GO

ALTER TABLE [RentalApplication] DROP CONSTRAINT [FK_RentalApplication_OrganizationMember_ReviewedByOrganizationMemberID];
GO

ALTER TABLE [RentalApplication] DROP CONSTRAINT [FK_RentalApplication_Organization_OrganizationID];
GO

ALTER TABLE [RentalApplication] DROP CONSTRAINT [FK_RentalApplication_Tenant_TenantID];
GO

ALTER TABLE [RentalUnit] DROP CONSTRAINT [FK_RentalUnit_Property_PropertyID];
GO

ALTER TABLE [RentalUnit] DROP CONSTRAINT [FK_RentalUnit_UnitType_UnitTypeID];
GO

ALTER TABLE [ReservationHold] DROP CONSTRAINT [FK_ReservationHold_Listing_ListingID];
GO

ALTER TABLE [ReservationHold] DROP CONSTRAINT [FK_ReservationHold_RentalApplication_RentalApplicationID];
GO

ALTER TABLE [ReservationHold] DROP CONSTRAINT [FK_ReservationHold_Tenant_TenantID];
GO

ALTER TABLE [SecurityDeposit] DROP CONSTRAINT [FK_SecurityDeposit_Lease_LeaseID];
GO

ALTER TABLE [SecurityDeposit] DROP CONSTRAINT [FK_SecurityDeposit_Organization_OrganizationID];
GO

ALTER TABLE [SecurityDeposit] DROP CONSTRAINT [FK_SecurityDeposit_Tenant_TenantID];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [FK_SecurityDepositTransaction_InvoiceDetail_InvoiceDetailID];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [FK_SecurityDepositTransaction_InvoiceMaster_InvoiceMasterID];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [FK_SecurityDepositTransaction_Payment_PaymentID];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [FK_SecurityDepositTransaction_Refund_RefundID];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [FK_SecurityDepositTransaction_SecurityDeposit_SecurityDepositID];
GO

ALTER TABLE [Tenant] DROP CONSTRAINT [FK_Tenant_AspNetUsers_UserID];
GO

ALTER TABLE [TenantEmergencyContact] DROP CONSTRAINT [FK_TenantEmergencyContact_Tenant_TenantID];
GO

ALTER TABLE [TenantEmployment] DROP CONSTRAINT [FK_TenantEmployment_Tenant_TenantID];
GO

ALTER TABLE [TenantGuarantor] DROP CONSTRAINT [FK_TenantGuarantor_AspNetUsers_UserID];
GO

ALTER TABLE [TenantGuarantor] DROP CONSTRAINT [FK_TenantGuarantor_Tenant_TenantID];
GO

ALTER TABLE [TenantInvitation] DROP CONSTRAINT [FK_TenantInvitation_Lease_LeaseID];
GO

ALTER TABLE [TenantInvitation] DROP CONSTRAINT [FK_TenantInvitation_RentalApplication_RentalApplicationID];
GO

ALTER TABLE [TenantScreeningCheck] DROP CONSTRAINT [FK_TenantScreeningCheck_Tenant_TenantID];
GO

ALTER TABLE [ViewingAppointments] DROP CONSTRAINT [FK_ViewingAppointments_Listing_ListingID];
GO

ALTER TABLE [ViewingAppointments] DROP CONSTRAINT [FK_ViewingAppointments_OrganizationMember_AssignedOrganizationMemberID];
GO

ALTER TABLE [ViewingAppointments] DROP CONSTRAINT [FK_ViewingAppointments_Tenant_TenantID];
GO

ALTER TABLE [WorkOrder] DROP CONSTRAINT [FK_WorkOrder_Contractor_ContractorID];
GO

ALTER TABLE [WorkOrder] DROP CONSTRAINT [FK_WorkOrder_Lease_LeaseID];
GO

ALTER TABLE [WorkOrder] DROP CONSTRAINT [FK_WorkOrder_MaintenanceRequest_MaintenanceRequestID];
GO

ALTER TABLE [WorkOrder] DROP CONSTRAINT [FK_WorkOrder_OrganizationMember_AssignedOrganizationMemberID];
GO

ALTER TABLE [WorkOrder] DROP CONSTRAINT [PK_WorkOrder];
GO

ALTER TABLE [UnitType] DROP CONSTRAINT [PK_UnitType];
GO

ALTER TABLE [TenantScreeningCheck] DROP CONSTRAINT [PK_TenantScreeningCheck];
GO

ALTER TABLE [TenantInvitation] DROP CONSTRAINT [PK_TenantInvitation];
GO

ALTER TABLE [TenantGuarantor] DROP CONSTRAINT [PK_TenantGuarantor];
GO

ALTER TABLE [TenantEmployment] DROP CONSTRAINT [PK_TenantEmployment];
GO

ALTER TABLE [TenantEmergencyContact] DROP CONSTRAINT [PK_TenantEmergencyContact];
GO

ALTER TABLE [Tenant] DROP CONSTRAINT [PK_Tenant];
GO

ALTER TABLE [TenancyType] DROP CONSTRAINT [PK_TenancyType];
GO

ALTER TABLE [TaxRate] DROP CONSTRAINT [PK_TaxRate];
GO

ALTER TABLE [SubscriptionPlan] DROP CONSTRAINT [PK_SubscriptionPlan];
GO

ALTER TABLE [SecurityDepositTransaction] DROP CONSTRAINT [PK_SecurityDepositTransaction];
GO

ALTER TABLE [SecurityDeposit] DROP CONSTRAINT [PK_SecurityDeposit];
GO

ALTER TABLE [ReservationHold] DROP CONSTRAINT [PK_ReservationHold];
GO

ALTER TABLE [RentalUnit] DROP CONSTRAINT [PK_RentalUnit];
GO

ALTER TABLE [RentalApplication] DROP CONSTRAINT [PK_RentalApplication];
GO

ALTER TABLE [Refund] DROP CONSTRAINT [PK_Refund];
GO

ALTER TABLE [ReceiptMaster] DROP CONSTRAINT [PK_ReceiptMaster];
GO

ALTER TABLE [Rating] DROP CONSTRAINT [PK_Rating];
GO

ALTER TABLE [Property] DROP CONSTRAINT [PK_Property];
GO

ALTER TABLE [Preference] DROP CONSTRAINT [PK_Preference];
GO

ALTER TABLE [PayoutItem] DROP CONSTRAINT [PK_PayoutItem];
GO

ALTER TABLE [Payout] DROP CONSTRAINT [PK_Payout];
GO

ALTER TABLE [PaymentReminder] DROP CONSTRAINT [PK_PaymentReminder];
GO

ALTER TABLE [PaymentProviderEvent] DROP CONSTRAINT [PK_PaymentProviderEvent];
GO

ALTER TABLE [PaymentMethod] DROP CONSTRAINT [PK_PaymentMethod];
GO

ALTER TABLE [PaymentIntent] DROP CONSTRAINT [PK_PaymentIntent];
GO

ALTER TABLE [PaymentAttempt] DROP CONSTRAINT [PK_PaymentAttempt];
GO

ALTER TABLE [PaymentAllocation] DROP CONSTRAINT [PK_PaymentAllocation];
GO

ALTER TABLE [Payment] DROP CONSTRAINT [PK_Payment];
GO

ALTER TABLE [OrgSubscription] DROP CONSTRAINT [PK_OrgSubscription];
GO

ALTER TABLE [OrgPayoutAccount] DROP CONSTRAINT [PK_OrgPayoutAccount];
GO

ALTER TABLE [OrganizationStatement] DROP CONSTRAINT [PK_OrganizationStatement];
GO

ALTER TABLE [OrganizationMember] DROP CONSTRAINT [PK_OrganizationMember];
GO

ALTER TABLE [Organization] DROP CONSTRAINT [PK_Organization];
GO

ALTER TABLE [Notification] DROP CONSTRAINT [PK_Notification];
GO

ALTER TABLE [MaintenanceRequest] DROP CONSTRAINT [PK_MaintenanceRequest];
GO

ALTER TABLE [ListingType] DROP CONSTRAINT [PK_ListingType];
GO

ALTER TABLE [ListingTermPrice] DROP CONSTRAINT [PK_ListingTermPrice];
GO

ALTER TABLE [ListingRule] DROP CONSTRAINT [PK_ListingRule];
GO

ALTER TABLE [ListingPolicy] DROP CONSTRAINT [PK_ListingPolicy];
GO

ALTER TABLE [ListingPhoto] DROP CONSTRAINT [PK_ListingPhoto];
GO

ALTER TABLE [ListingAmenity] DROP CONSTRAINT [PK_ListingAmenity];
GO

ALTER TABLE [ListingAccessInstruction] DROP CONSTRAINT [PK_ListingAccessInstruction];
GO

ALTER TABLE [Listing] DROP CONSTRAINT [PK_Listing];
GO

ALTER TABLE [LedgerTransaction] DROP CONSTRAINT [PK_LedgerTransaction];
GO

ALTER TABLE [LedgerEntry] DROP CONSTRAINT [PK_LedgerEntry];
GO

ALTER TABLE [LedgerAccount] DROP CONSTRAINT [PK_LedgerAccount];
GO

ALTER TABLE [LeaseSignatory] DROP CONSTRAINT [PK_LeaseSignatory];
GO

ALTER TABLE [LeaseDocExtractedTerm] DROP CONSTRAINT [PK_LeaseDocExtractedTerm];
GO

ALTER TABLE [Lease] DROP CONSTRAINT [PK_Lease];
GO

ALTER TABLE [InvoiceMaster] DROP CONSTRAINT [PK_InvoiceMaster];
GO

ALTER TABLE [InvoiceDetail] DROP CONSTRAINT [PK_InvoiceDetail];
GO

ALTER TABLE [InspectionItem] DROP CONSTRAINT [PK_InspectionItem];
GO

ALTER TABLE [Inspection] DROP CONSTRAINT [PK_Inspection];
GO

ALTER TABLE [IdentityVerification] DROP CONSTRAINT [PK_IdentityVerification];
GO

ALTER TABLE [FraudCase] DROP CONSTRAINT [PK_FraudCase];
GO

ALTER TABLE [FeeType] DROP CONSTRAINT [PK_FeeType];
GO

ALTER TABLE [Fee] DROP CONSTRAINT [PK_Fee];
GO

ALTER TABLE [Dispute] DROP CONSTRAINT [PK_Dispute];
GO

ALTER TABLE [CreditReportingEnrollment] DROP CONSTRAINT [PK_CreditReportingEnrollment];
GO

ALTER TABLE [CreditReportingConsentAudit] DROP CONSTRAINT [PK_CreditReportingConsentAudit];
GO

ALTER TABLE [CreditReporting] DROP CONSTRAINT [PK_CreditReporting];
GO

ALTER TABLE [ConversationParticipant] DROP CONSTRAINT [PK_ConversationParticipant];
GO

ALTER TABLE [ConversationMessage] DROP CONSTRAINT [PK_ConversationMessage];
GO

ALTER TABLE [Conversation] DROP CONSTRAINT [PK_Conversation];
GO

ALTER TABLE [Contractor] DROP CONSTRAINT [PK_Contractor];
GO

ALTER TABLE [Chargeback] DROP CONSTRAINT [PK_Chargeback];
GO

ALTER TABLE [Category] DROP CONSTRAINT [PK_Category];
GO

ALTER TABLE [CalendarEvent] DROP CONSTRAINT [PK_CalendarEvent];
GO

ALTER TABLE [AutopayMandate] DROP CONSTRAINT [PK_AutopayMandate];
GO

ALTER TABLE [AutopayConsentAudit] DROP CONSTRAINT [PK_AutopayConsentAudit];
GO

ALTER TABLE [AuditLog] DROP CONSTRAINT [PK_AuditLog];
GO

ALTER TABLE [Attachment] DROP CONSTRAINT [PK_Attachment];
GO

ALTER TABLE [ApplicationOccupant] DROP CONSTRAINT [PK_ApplicationOccupant];
GO

ALTER TABLE [AmenityCatalog] DROP CONSTRAINT [PK_AmenityCatalog];
GO

ALTER TABLE [Address] DROP CONSTRAINT [PK_Address];
GO

EXEC sp_rename N'[WorkOrder]', N'WorkOrders';
GO

EXEC sp_rename N'[UnitType]', N'UnitTypes';
GO

EXEC sp_rename N'[TenantScreeningCheck]', N'TenantScreeningChecks';
GO

EXEC sp_rename N'[TenantInvitation]', N'TenantInvitations';
GO

EXEC sp_rename N'[TenantGuarantor]', N'TenantGuarantors';
GO

EXEC sp_rename N'[TenantEmployment]', N'TenantEmployments';
GO

EXEC sp_rename N'[TenantEmergencyContact]', N'TenantEmergencyContacts';
GO

EXEC sp_rename N'[Tenant]', N'Tenants';
GO

EXEC sp_rename N'[TenancyType]', N'TenancyTypes';
GO

EXEC sp_rename N'[TaxRate]', N'TaxRates';
GO

EXEC sp_rename N'[SubscriptionPlan]', N'SubscriptionPlans';
GO

EXEC sp_rename N'[SecurityDepositTransaction]', N'SecurityDepositTransactions';
GO

EXEC sp_rename N'[SecurityDeposit]', N'SecurityDeposits';
GO

EXEC sp_rename N'[ReservationHold]', N'ReservationHolds';
GO

EXEC sp_rename N'[RentalUnit]', N'RentalUnits';
GO

EXEC sp_rename N'[RentalApplication]', N'RentalApplications';
GO

EXEC sp_rename N'[Refund]', N'Refunds';
GO

EXEC sp_rename N'[ReceiptMaster]', N'ReceiptMasters';
GO

EXEC sp_rename N'[Rating]', N'Ratings';
GO

EXEC sp_rename N'[Property]', N'Properties';
GO

EXEC sp_rename N'[Preference]', N'Preferences';
GO

EXEC sp_rename N'[PayoutItem]', N'PayoutItems';
GO

EXEC sp_rename N'[Payout]', N'Payouts';
GO

EXEC sp_rename N'[PaymentReminder]', N'PaymentReminders';
GO

EXEC sp_rename N'[PaymentProviderEvent]', N'PaymentProviderEvents';
GO

EXEC sp_rename N'[PaymentMethod]', N'PaymentMethods';
GO

EXEC sp_rename N'[PaymentIntent]', N'PaymentIntents';
GO

EXEC sp_rename N'[PaymentAttempt]', N'PaymentAttempts';
GO

EXEC sp_rename N'[PaymentAllocation]', N'PaymentAllocations';
GO

EXEC sp_rename N'[Payment]', N'Payments';
GO

EXEC sp_rename N'[OrgSubscription]', N'OrgSubscriptions';
GO

EXEC sp_rename N'[OrgPayoutAccount]', N'OrgPayoutAccounts';
GO

EXEC sp_rename N'[OrganizationStatement]', N'OrganizationStatements';
GO

EXEC sp_rename N'[OrganizationMember]', N'OrganizationMembers';
GO

EXEC sp_rename N'[Organization]', N'Organizations';
GO

EXEC sp_rename N'[Notification]', N'Notifications';
GO

EXEC sp_rename N'[MaintenanceRequest]', N'MaintenanceRequests';
GO

EXEC sp_rename N'[ListingType]', N'ListingTypes';
GO

EXEC sp_rename N'[ListingTermPrice]', N'ListingTermPrices';
GO

EXEC sp_rename N'[ListingRule]', N'ListingRules';
GO

EXEC sp_rename N'[ListingPolicy]', N'ListingPolicies';
GO

EXEC sp_rename N'[ListingPhoto]', N'ListingPhotos';
GO

EXEC sp_rename N'[ListingAmenity]', N'ListingAmenities';
GO

EXEC sp_rename N'[ListingAccessInstruction]', N'ListingAccessInstructions';
GO

EXEC sp_rename N'[Listing]', N'Listings';
GO

EXEC sp_rename N'[LedgerTransaction]', N'LedgerTransactions';
GO

EXEC sp_rename N'[LedgerEntry]', N'LedgerEntries';
GO

EXEC sp_rename N'[LedgerAccount]', N'LedgerAccounts';
GO

EXEC sp_rename N'[LeaseSignatory]', N'LeaseSignatories';
GO

EXEC sp_rename N'[LeaseDocExtractedTerm]', N'LeaseDocExtractedTerms';
GO

EXEC sp_rename N'[Lease]', N'Leases';
GO

EXEC sp_rename N'[InvoiceMaster]', N'InvoiceMasters';
GO

EXEC sp_rename N'[InvoiceDetail]', N'InvoiceDetails';
GO

EXEC sp_rename N'[InspectionItem]', N'InspectionItems';
GO

EXEC sp_rename N'[Inspection]', N'Inspections';
GO

EXEC sp_rename N'[IdentityVerification]', N'IdentityVerifications';
GO

EXEC sp_rename N'[FraudCase]', N'FraudCases';
GO

EXEC sp_rename N'[FeeType]', N'FeeTypes';
GO

EXEC sp_rename N'[Fee]', N'Fees';
GO

EXEC sp_rename N'[Dispute]', N'Disputes';
GO

EXEC sp_rename N'[CreditReportingEnrollment]', N'CreditReportingEnrollments';
GO

EXEC sp_rename N'[CreditReportingConsentAudit]', N'CreditReportingConsentAudits';
GO

EXEC sp_rename N'[CreditReporting]', N'CreditReportings';
GO

EXEC sp_rename N'[ConversationParticipant]', N'ConversationParticipants';
GO

EXEC sp_rename N'[ConversationMessage]', N'ConversationMessages';
GO

EXEC sp_rename N'[Conversation]', N'Conversations';
GO

EXEC sp_rename N'[Contractor]', N'Contractors';
GO

EXEC sp_rename N'[Chargeback]', N'ChargeBacks';
GO

EXEC sp_rename N'[Category]', N'Categories';
GO

EXEC sp_rename N'[CalendarEvent]', N'CalendarEvents';
GO

EXEC sp_rename N'[AutopayMandate]', N'AutopayMandates';
GO

EXEC sp_rename N'[AutopayConsentAudit]', N'AutopayConsentAudits';
GO

EXEC sp_rename N'[AuditLog]', N'AuditLogs';
GO

EXEC sp_rename N'[Attachment]', N'Attachments';
GO

EXEC sp_rename N'[ApplicationOccupant]', N'ApplicationOccupants';
GO

EXEC sp_rename N'[AmenityCatalog]', N'AmenityCatalogs';
GO

EXEC sp_rename N'[Address]', N'Addresses';
GO

EXEC sp_rename N'[WorkOrders].[IX_WorkOrder_MaintenanceRequestID]', N'IX_WorkOrders_MaintenanceRequestID', N'INDEX';
GO

EXEC sp_rename N'[WorkOrders].[IX_WorkOrder_LeaseID]', N'IX_WorkOrders_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[WorkOrders].[IX_WorkOrder_ContractorID]', N'IX_WorkOrders_ContractorID', N'INDEX';
GO

EXEC sp_rename N'[WorkOrders].[IX_WorkOrder_AssignedOrganizationMemberID]', N'IX_WorkOrders_AssignedOrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[TenantScreeningChecks].[IX_TenantScreeningCheck_TenantID]', N'IX_TenantScreeningChecks_TenantID', N'INDEX';
GO

EXEC sp_rename N'[TenantInvitations].[IX_TenantInvitation_RentalApplicationID]', N'IX_TenantInvitations_RentalApplicationID', N'INDEX';
GO

EXEC sp_rename N'[TenantInvitations].[IX_TenantInvitation_LeaseID]', N'IX_TenantInvitations_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[TenantGuarantors].[IX_TenantGuarantor_UserID]', N'IX_TenantGuarantors_UserID', N'INDEX';
GO

EXEC sp_rename N'[TenantGuarantors].[IX_TenantGuarantor_TenantID]', N'IX_TenantGuarantors_TenantID', N'INDEX';
GO

EXEC sp_rename N'[TenantEmployments].[IX_TenantEmployment_TenantID]', N'IX_TenantEmployments_TenantID', N'INDEX';
GO

EXEC sp_rename N'[TenantEmergencyContacts].[IX_TenantEmergencyContact_TenantID]', N'IX_TenantEmergencyContacts_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Tenants].[IX_Tenant_UserID]', N'IX_Tenants_UserID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDepositTransactions].[IX_SecurityDepositTransaction_SecurityDepositID]', N'IX_SecurityDepositTransactions_SecurityDepositID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDepositTransactions].[IX_SecurityDepositTransaction_RefundID]', N'IX_SecurityDepositTransactions_RefundID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDepositTransactions].[IX_SecurityDepositTransaction_PaymentID]', N'IX_SecurityDepositTransactions_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDepositTransactions].[IX_SecurityDepositTransaction_InvoiceMasterID]', N'IX_SecurityDepositTransactions_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDepositTransactions].[IX_SecurityDepositTransaction_InvoiceDetailID]', N'IX_SecurityDepositTransactions_InvoiceDetailID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDeposits].[IX_SecurityDeposit_TenantID]', N'IX_SecurityDeposits_TenantID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDeposits].[IX_SecurityDeposit_OrganizationID]', N'IX_SecurityDeposits_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[SecurityDeposits].[IX_SecurityDeposit_LeaseID]', N'IX_SecurityDeposits_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[ReservationHolds].[IX_ReservationHold_TenantID]', N'IX_ReservationHolds_TenantID', N'INDEX';
GO

EXEC sp_rename N'[ReservationHolds].[IX_ReservationHold_RentalApplicationID]', N'IX_ReservationHolds_RentalApplicationID', N'INDEX';
GO

EXEC sp_rename N'[ReservationHolds].[IX_ReservationHold_ListingID]', N'IX_ReservationHolds_ListingID', N'INDEX';
GO

EXEC sp_rename N'[RentalUnits].[IX_RentalUnit_UnitTypeID]', N'IX_RentalUnits_UnitTypeID', N'INDEX';
GO

EXEC sp_rename N'[RentalUnits].[IX_RentalUnit_PropertyID]', N'IX_RentalUnits_PropertyID', N'INDEX';
GO

EXEC sp_rename N'[RentalApplications].[IX_RentalApplication_TenantID]', N'IX_RentalApplications_TenantID', N'INDEX';
GO

EXEC sp_rename N'[RentalApplications].[IX_RentalApplication_ReviewedByOrganizationMemberID]', N'IX_RentalApplications_ReviewedByOrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[RentalApplications].[IX_RentalApplication_OrganizationID]', N'IX_RentalApplications_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[RentalApplications].[IX_RentalApplication_ListingID]', N'IX_RentalApplications_ListingID', N'INDEX';
GO

EXEC sp_rename N'[Refunds].[IX_Refund_TenantID]', N'IX_Refunds_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Refunds].[IX_Refund_PaymentID]', N'IX_Refunds_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[ReceiptMasters].[IX_ReceiptMaster_TenantID]', N'IX_ReceiptMasters_TenantID', N'INDEX';
GO

EXEC sp_rename N'[ReceiptMasters].[IX_ReceiptMaster_PaymentID]', N'IX_ReceiptMasters_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[Ratings].[IX_Rating_ReviewerUserID]', N'IX_Ratings_ReviewerUserID', N'INDEX';
GO

EXEC sp_rename N'[Ratings].[IX_Rating_LeaseID]', N'IX_Ratings_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[Properties].[IX_Property_OrganizationID]', N'IX_Properties_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Properties].[IX_Property_AddressID]', N'IX_Properties_AddressID', N'INDEX';
GO

EXEC sp_rename N'[PayoutItems].[IX_PayoutItem_PayoutID]', N'IX_PayoutItems_PayoutID', N'INDEX';
GO

EXEC sp_rename N'[PayoutItems].[IX_PayoutItem_PaymentID]', N'IX_PayoutItems_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[Payouts].[IX_Payout_OrgPayoutAccountID]', N'IX_Payouts_OrgPayoutAccountID', N'INDEX';
GO

EXEC sp_rename N'[Payouts].[IX_Payout_OrganizationID]', N'IX_Payouts_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[PaymentReminders].[IX_PaymentReminder_InvoiceMasterID]', N'IX_PaymentReminders_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[PaymentMethods].[IX_PaymentMethod_TenantID]', N'IX_PaymentMethods_TenantID', N'INDEX';
GO

EXEC sp_rename N'[PaymentIntents].[IX_PaymentIntent_TenantID]', N'IX_PaymentIntents_TenantID', N'INDEX';
GO

EXEC sp_rename N'[PaymentIntents].[IX_PaymentIntent_PaymentMethodID]', N'IX_PaymentIntents_PaymentMethodID', N'INDEX';
GO

EXEC sp_rename N'[PaymentIntents].[IX_PaymentIntent_LeaseID]', N'IX_PaymentIntents_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[PaymentIntents].[IX_PaymentIntent_InvoiceMasterID]', N'IX_PaymentIntents_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[PaymentIntents].[IX_PaymentIntent_AutopayMandateID]', N'IX_PaymentIntents_AutopayMandateID', N'INDEX';
GO

EXEC sp_rename N'[PaymentAttempts].[IX_PaymentAttempt_PaymentIntentID]', N'IX_PaymentAttempts_PaymentIntentID', N'INDEX';
GO

EXEC sp_rename N'[PaymentAllocations].[IX_PaymentAllocation_PaymentID]', N'IX_PaymentAllocations_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[PaymentAllocations].[IX_PaymentAllocation_InvoiceMasterID]', N'IX_PaymentAllocations_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[PaymentAllocations].[IX_PaymentAllocation_InvoiceDetailID]', N'IX_PaymentAllocations_InvoiceDetailID', N'INDEX';
GO

EXEC sp_rename N'[Payments].[IX_Payment_TenantID]', N'IX_Payments_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Payments].[IX_Payment_PaymentIntentID]', N'IX_Payments_PaymentIntentID', N'INDEX';
GO

EXEC sp_rename N'[OrgSubscriptions].[IX_OrgSubscription_SubscriptionPlanID]', N'IX_OrgSubscriptions_SubscriptionPlanID', N'INDEX';
GO

EXEC sp_rename N'[OrgSubscriptions].[IX_OrgSubscription_OrganizationID]', N'IX_OrgSubscriptions_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[OrgPayoutAccounts].[IX_OrgPayoutAccount_OrganizationID]', N'IX_OrgPayoutAccounts_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[OrganizationStatements].[IX_OrganizationStatement_OrganizationID]', N'IX_OrganizationStatements_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[OrganizationMembers].[IX_OrganizationMember_UserID]', N'IX_OrganizationMembers_UserID', N'INDEX';
GO

EXEC sp_rename N'[OrganizationMembers].[IX_OrganizationMember_OrganizationID]', N'IX_OrganizationMembers_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Notifications].[IX_Notification_TenantID]', N'IX_Notifications_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Notifications].[IX_Notification_RecipientUserID]', N'IX_Notifications_RecipientUserID', N'INDEX';
GO

EXEC sp_rename N'[Notifications].[IX_Notification_OrganizationMemberID]', N'IX_Notifications_OrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[Notifications].[IX_Notification_OrganizationID]', N'IX_Notifications_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_SubmittedByTenantID]', N'IX_MaintenanceRequests_SubmittedByTenantID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_RentalUnitID]', N'IX_MaintenanceRequests_RentalUnitID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_PropertyID]', N'IX_MaintenanceRequests_PropertyID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_ListingID]', N'IX_MaintenanceRequests_ListingID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_LeaseID]', N'IX_MaintenanceRequests_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[MaintenanceRequests].[IX_MaintenanceRequest_CategoryID]', N'IX_MaintenanceRequests_CategoryID', N'INDEX';
GO

EXEC sp_rename N'[ListingTermPrices].[IX_ListingTermPrice_ListingID]', N'IX_ListingTermPrices_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingRules].[IX_ListingRule_ListingID]', N'IX_ListingRules_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingPolicies].[IX_ListingPolicy_ListingID]', N'IX_ListingPolicies_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingPhotos].[IX_ListingPhoto_ListingID]', N'IX_ListingPhotos_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingAmenities].[IX_ListingAmenity_ListingID]', N'IX_ListingAmenities_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingAmenities].[IX_ListingAmenity_AmenityID]', N'IX_ListingAmenities_AmenityID', N'INDEX';
GO

EXEC sp_rename N'[ListingAccessInstructions].[IX_ListingAccessInstruction_ListingID]', N'IX_ListingAccessInstructions_ListingID', N'INDEX';
GO

EXEC sp_rename N'[ListingAccessInstructions].[IX_ListingAccessInstruction_LeaseID]', N'IX_ListingAccessInstructions_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[Listings].[IX_Listing_RentalUnitID]', N'IX_Listings_RentalUnitID', N'INDEX';
GO

EXEC sp_rename N'[Listings].[IX_Listing_OrganizationID]', N'IX_Listings_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Listings].[IX_Listing_ListingTypeID]', N'IX_Listings_ListingTypeID', N'INDEX';
GO

EXEC sp_rename N'[LedgerTransactions].[IX_LedgerTransaction_RefundID]', N'IX_LedgerTransactions_RefundID', N'INDEX';
GO

EXEC sp_rename N'[LedgerTransactions].[IX_LedgerTransaction_PayoutID]', N'IX_LedgerTransactions_PayoutID', N'INDEX';
GO

EXEC sp_rename N'[LedgerTransactions].[IX_LedgerTransaction_PaymentID]', N'IX_LedgerTransactions_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[LedgerTransactions].[IX_LedgerTransaction_OrganizationID]', N'IX_LedgerTransactions_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[LedgerTransactions].[IX_LedgerTransaction_InvoiceMasterID]', N'IX_LedgerTransactions_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[LedgerEntries].[IX_LedgerEntry_LedgerTransactionID]', N'IX_LedgerEntries_LedgerTransactionID', N'INDEX';
GO

EXEC sp_rename N'[LedgerEntries].[IX_LedgerEntry_LedgerAccountID]', N'IX_LedgerEntries_LedgerAccountID', N'INDEX';
GO

EXEC sp_rename N'[LedgerAccounts].[IX_LedgerAccount_OrganizationID]', N'IX_LedgerAccounts_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[LeaseSignatories].[IX_LeaseSignatory_UserID]', N'IX_LeaseSignatories_UserID', N'INDEX';
GO

EXEC sp_rename N'[LeaseSignatories].[IX_LeaseSignatory_TenantID]', N'IX_LeaseSignatories_TenantID', N'INDEX';
GO

EXEC sp_rename N'[LeaseSignatories].[IX_LeaseSignatory_OrganizationMemberID]', N'IX_LeaseSignatories_OrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[LeaseSignatories].[IX_LeaseSignatory_OrganizationID]', N'IX_LeaseSignatories_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_TenantID]', N'IX_Leases_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_TenancyTypeID]', N'IX_Leases_TenancyTypeID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_RentalUnitID]', N'IX_Leases_RentalUnitID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_RentalApplicationID]', N'IX_Leases_RentalApplicationID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_OrganizationID]', N'IX_Leases_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Leases].[IX_Lease_ListingID]', N'IX_Leases_ListingID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceMasters].[IX_InvoiceMaster_TenantID]', N'IX_InvoiceMasters_TenantID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceMasters].[IX_InvoiceMaster_OrganizationID]', N'IX_InvoiceMasters_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceMasters].[IX_InvoiceMaster_LeaseRenewalID]', N'IX_InvoiceMasters_LeaseRenewalID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceMasters].[IX_InvoiceMaster_LeaseID]', N'IX_InvoiceMasters_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceDetails].[IX_InvoiceDetail_InvoiceMasterID]', N'IX_InvoiceDetails_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[InvoiceDetails].[IX_InvoiceDetail_FeeID]', N'IX_InvoiceDetails_FeeID', N'INDEX';
GO

EXEC sp_rename N'[InspectionItems].[IX_InspectionItem_InspectionID]', N'IX_InspectionItems_InspectionID', N'INDEX';
GO

EXEC sp_rename N'[Inspections].[IX_Inspection_RentalUnitID]', N'IX_Inspections_RentalUnitID', N'INDEX';
GO

EXEC sp_rename N'[Inspections].[IX_Inspection_PropertyID]', N'IX_Inspections_PropertyID', N'INDEX';
GO

EXEC sp_rename N'[Inspections].[IX_Inspection_LeaseID]', N'IX_Inspections_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[IdentityVerifications].[IX_IdentityVerification_UserID]', N'IX_IdentityVerifications_UserID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_TenantID]', N'IX_FraudCases_TenantID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_PaymentIntentID]', N'IX_FraudCases_PaymentIntentID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_PaymentID]', N'IX_FraudCases_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_OrganizationID]', N'IX_FraudCases_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_LeaseID]', N'IX_FraudCases_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[FraudCases].[IX_FraudCase_ChargebackID]', N'IX_FraudCases_ChargebackID', N'INDEX';
GO

EXEC sp_rename N'[Fees].[IX_Fee_OrganizationID]', N'IX_Fees_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Fees].[IX_Fee_FeeTypeID]', N'IX_Fees_FeeTypeID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_TenantID]', N'IX_Disputes_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_PaymentID]', N'IX_Disputes_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_OrganizationID]', N'IX_Disputes_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_MaintenanceRequestID]', N'IX_Disputes_MaintenanceRequestID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_LeaseID]', N'IX_Disputes_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_InvoiceMasterID]', N'IX_Disputes_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[Disputes].[IX_Dispute_ChargebackID]', N'IX_Disputes_ChargebackID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportingEnrollments].[IX_CreditReportingEnrollment_TenantID]', N'IX_CreditReportingEnrollments_TenantID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportingEnrollments].[IX_CreditReportingEnrollment_LeaseID]', N'IX_CreditReportingEnrollments_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportingConsentAudits].[IX_CreditReportingConsentAudit_TenantID]', N'IX_CreditReportingConsentAudits_TenantID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportingConsentAudits].[IX_CreditReportingConsentAudit_CreditReportingEnrollmentID]', N'IX_CreditReportingConsentAudits_CreditReportingEnrollmentID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportings].[IX_CreditReporting_PaymentID]', N'IX_CreditReportings_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportings].[IX_CreditReporting_InvoiceMasterID]', N'IX_CreditReportings_InvoiceMasterID', N'INDEX';
GO

EXEC sp_rename N'[CreditReportings].[IX_CreditReporting_CreditReportingEnrollmentID]', N'IX_CreditReportings_CreditReportingEnrollmentID', N'INDEX';
GO

EXEC sp_rename N'[ConversationParticipants].[IX_ConversationParticipant_UserID]', N'IX_ConversationParticipants_UserID', N'INDEX';
GO

EXEC sp_rename N'[ConversationParticipants].[IX_ConversationParticipant_TenantID]', N'IX_ConversationParticipants_TenantID', N'INDEX';
GO

EXEC sp_rename N'[ConversationParticipants].[IX_ConversationParticipant_OrganizationMemberID]', N'IX_ConversationParticipants_OrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[ConversationParticipants].[IX_ConversationParticipant_ConversationID]', N'IX_ConversationParticipants_ConversationID', N'INDEX';
GO

EXEC sp_rename N'[ConversationMessages].[IX_ConversationMessage_SenderUserID]', N'IX_ConversationMessages_SenderUserID', N'INDEX';
GO

EXEC sp_rename N'[ConversationMessages].[IX_ConversationMessage_SenderTenantID]', N'IX_ConversationMessages_SenderTenantID', N'INDEX';
GO

EXEC sp_rename N'[ConversationMessages].[IX_ConversationMessage_SenderOrganizationMemberID]', N'IX_ConversationMessages_SenderOrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[ConversationMessages].[IX_ConversationMessage_ReplyToMessageID]', N'IX_ConversationMessages_ReplyToMessageID', N'INDEX';
GO

EXEC sp_rename N'[ConversationMessages].[IX_ConversationMessage_ConversationID]', N'IX_ConversationMessages_ConversationID', N'INDEX';
GO

EXEC sp_rename N'[Conversations].[IX_Conversation_MaintenanceRequestID]', N'IX_Conversations_MaintenanceRequestID', N'INDEX';
GO

EXEC sp_rename N'[Conversations].[IX_Conversation_LeaseID]', N'IX_Conversations_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[Conversations].[IX_Conversation_DisputeID]', N'IX_Conversations_DisputeID', N'INDEX';
GO

EXEC sp_rename N'[Contractors].[IX_Contractor_OrganizationID]', N'IX_Contractors_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[ChargeBacks].[IX_Chargeback_TenantID]', N'IX_ChargeBacks_TenantID', N'INDEX';
GO

EXEC sp_rename N'[ChargeBacks].[IX_Chargeback_PaymentID]', N'IX_ChargeBacks_PaymentID', N'INDEX';
GO

EXEC sp_rename N'[ChargeBacks].[IX_Chargeback_OrganizationID]', N'IX_ChargeBacks_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[CalendarEvents].[IX_CalendarEvent_ReservationHoldID]', N'IX_CalendarEvents_ReservationHoldID', N'INDEX';
GO

EXEC sp_rename N'[CalendarEvents].[IX_CalendarEvent_RentalApplicationID]', N'IX_CalendarEvents_RentalApplicationID', N'INDEX';
GO

EXEC sp_rename N'[CalendarEvents].[IX_CalendarEvent_MaintenanceRequestID]', N'IX_CalendarEvents_MaintenanceRequestID', N'INDEX';
GO

EXEC sp_rename N'[CalendarEvents].[IX_CalendarEvent_ListingID]', N'IX_CalendarEvents_ListingID', N'INDEX';
GO

EXEC sp_rename N'[CalendarEvents].[IX_CalendarEvent_LeaseID]', N'IX_CalendarEvents_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[AutopayMandates].[IX_AutopayMandate_TenantID]', N'IX_AutopayMandates_TenantID', N'INDEX';
GO

EXEC sp_rename N'[AutopayMandates].[IX_AutopayMandate_PaymentMethodID]', N'IX_AutopayMandates_PaymentMethodID', N'INDEX';
GO

EXEC sp_rename N'[AutopayMandates].[IX_AutopayMandate_LeaseID]', N'IX_AutopayMandates_LeaseID', N'INDEX';
GO

EXEC sp_rename N'[AutopayConsentAudits].[IX_AutopayConsentAudit_TenantID]', N'IX_AutopayConsentAudits_TenantID', N'INDEX';
GO

EXEC sp_rename N'[AutopayConsentAudits].[IX_AutopayConsentAudit_AutopayMandateID]', N'IX_AutopayConsentAudits_AutopayMandateID', N'INDEX';
GO

EXEC sp_rename N'[AuditLogs].[IX_AuditLog_TenantID]', N'IX_AuditLogs_TenantID', N'INDEX';
GO

EXEC sp_rename N'[AuditLogs].[IX_AuditLog_OrganizationMemberID]', N'IX_AuditLogs_OrganizationMemberID', N'INDEX';
GO

EXEC sp_rename N'[AuditLogs].[IX_AuditLog_OrganizationID]', N'IX_AuditLogs_OrganizationID', N'INDEX';
GO

EXEC sp_rename N'[AuditLogs].[IX_AuditLog_ActorUserID]', N'IX_AuditLogs_ActorUserID', N'INDEX';
GO

EXEC sp_rename N'[ApplicationOccupants].[IX_ApplicationOccupant_UserID]', N'IX_ApplicationOccupants_UserID', N'INDEX';
GO

EXEC sp_rename N'[ApplicationOccupants].[IX_ApplicationOccupant_TenantID]', N'IX_ApplicationOccupants_TenantID', N'INDEX';
GO

EXEC sp_rename N'[Addresses].[IX_Address_OrganizationID]', N'IX_Addresses_OrganizationID', N'INDEX';
GO

ALTER TABLE [WorkOrders] ADD CONSTRAINT [PK_WorkOrders] PRIMARY KEY ([WorkOrderID]);
GO

ALTER TABLE [UnitTypes] ADD CONSTRAINT [PK_UnitTypes] PRIMARY KEY ([UnitTypeID]);
GO

ALTER TABLE [TenantScreeningChecks] ADD CONSTRAINT [PK_TenantScreeningChecks] PRIMARY KEY ([TenantScreeningCheckID]);
GO

ALTER TABLE [TenantInvitations] ADD CONSTRAINT [PK_TenantInvitations] PRIMARY KEY ([TenantInvitationID]);
GO

ALTER TABLE [TenantGuarantors] ADD CONSTRAINT [PK_TenantGuarantors] PRIMARY KEY ([TenantGuarantorID]);
GO

ALTER TABLE [TenantEmployments] ADD CONSTRAINT [PK_TenantEmployments] PRIMARY KEY ([TenantEmploymentID]);
GO

ALTER TABLE [TenantEmergencyContacts] ADD CONSTRAINT [PK_TenantEmergencyContacts] PRIMARY KEY ([TenantEmergencyContactID]);
GO

ALTER TABLE [Tenants] ADD CONSTRAINT [PK_Tenants] PRIMARY KEY ([TenantID]);
GO

ALTER TABLE [TenancyTypes] ADD CONSTRAINT [PK_TenancyTypes] PRIMARY KEY ([TenancyTypeID]);
GO

ALTER TABLE [TaxRates] ADD CONSTRAINT [PK_TaxRates] PRIMARY KEY ([TaxID]);
GO

ALTER TABLE [SubscriptionPlans] ADD CONSTRAINT [PK_SubscriptionPlans] PRIMARY KEY ([SubscriptionPlanID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [PK_SecurityDepositTransactions] PRIMARY KEY ([SecurityDepositTransactionID]);
GO

ALTER TABLE [SecurityDeposits] ADD CONSTRAINT [PK_SecurityDeposits] PRIMARY KEY ([SecurityDepositID]);
GO

ALTER TABLE [ReservationHolds] ADD CONSTRAINT [PK_ReservationHolds] PRIMARY KEY ([ReservationHoldID]);
GO

ALTER TABLE [RentalUnits] ADD CONSTRAINT [PK_RentalUnits] PRIMARY KEY ([RentalUnitID]);
GO

ALTER TABLE [RentalApplications] ADD CONSTRAINT [PK_RentalApplications] PRIMARY KEY ([RentalApplicationID]);
GO

ALTER TABLE [Refunds] ADD CONSTRAINT [PK_Refunds] PRIMARY KEY ([RefundID]);
GO

ALTER TABLE [ReceiptMasters] ADD CONSTRAINT [PK_ReceiptMasters] PRIMARY KEY ([ReceiptMasterID]);
GO

ALTER TABLE [Ratings] ADD CONSTRAINT [PK_Ratings] PRIMARY KEY ([RatingID]);
GO

ALTER TABLE [Properties] ADD CONSTRAINT [PK_Properties] PRIMARY KEY ([PropertyID]);
GO

ALTER TABLE [Preferences] ADD CONSTRAINT [PK_Preferences] PRIMARY KEY ([PreferenceID]);
GO

ALTER TABLE [PayoutItems] ADD CONSTRAINT [PK_PayoutItems] PRIMARY KEY ([PayoutItemID]);
GO

ALTER TABLE [Payouts] ADD CONSTRAINT [PK_Payouts] PRIMARY KEY ([PayoutID]);
GO

ALTER TABLE [PaymentReminders] ADD CONSTRAINT [PK_PaymentReminders] PRIMARY KEY ([PaymentReminderID]);
GO

ALTER TABLE [PaymentProviderEvents] ADD CONSTRAINT [PK_PaymentProviderEvents] PRIMARY KEY ([PaymentProviderEventID]);
GO

ALTER TABLE [PaymentMethods] ADD CONSTRAINT [PK_PaymentMethods] PRIMARY KEY ([PaymentMethodID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [PK_PaymentIntents] PRIMARY KEY ([PaymentIntentID]);
GO

ALTER TABLE [PaymentAttempts] ADD CONSTRAINT [PK_PaymentAttempts] PRIMARY KEY ([PaymentAttemptID]);
GO

ALTER TABLE [PaymentAllocations] ADD CONSTRAINT [PK_PaymentAllocations] PRIMARY KEY ([PaymentAllocationID]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentID]);
GO

ALTER TABLE [OrgSubscriptions] ADD CONSTRAINT [PK_OrgSubscriptions] PRIMARY KEY ([OrgSubscriptionID]);
GO

ALTER TABLE [OrgPayoutAccounts] ADD CONSTRAINT [PK_OrgPayoutAccounts] PRIMARY KEY ([OrgPayoutAccountID]);
GO

ALTER TABLE [OrganizationStatements] ADD CONSTRAINT [PK_OrganizationStatements] PRIMARY KEY ([OrganizationStatementID]);
GO

ALTER TABLE [OrganizationMembers] ADD CONSTRAINT [PK_OrganizationMembers] PRIMARY KEY ([OrganizationMemberID]);
GO

ALTER TABLE [Organizations] ADD CONSTRAINT [PK_Organizations] PRIMARY KEY ([OrganizationID]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [PK_MaintenanceRequests] PRIMARY KEY ([MaintenanceRequestID]);
GO

ALTER TABLE [ListingTypes] ADD CONSTRAINT [PK_ListingTypes] PRIMARY KEY ([ListingTypeID]);
GO

ALTER TABLE [ListingTermPrices] ADD CONSTRAINT [PK_ListingTermPrices] PRIMARY KEY ([ListingTermPriceID]);
GO

ALTER TABLE [ListingRules] ADD CONSTRAINT [PK_ListingRules] PRIMARY KEY ([ListingRuleID]);
GO

ALTER TABLE [ListingPolicies] ADD CONSTRAINT [PK_ListingPolicies] PRIMARY KEY ([ListingPolicyID]);
GO

ALTER TABLE [ListingPhotos] ADD CONSTRAINT [PK_ListingPhotos] PRIMARY KEY ([ListingPhotoID]);
GO

ALTER TABLE [ListingAmenities] ADD CONSTRAINT [PK_ListingAmenities] PRIMARY KEY ([ListingAmenityID]);
GO

ALTER TABLE [ListingAccessInstructions] ADD CONSTRAINT [PK_ListingAccessInstructions] PRIMARY KEY ([ListingAccessInstructionID]);
GO

ALTER TABLE [Listings] ADD CONSTRAINT [PK_Listings] PRIMARY KEY ([ListingID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [PK_LedgerTransactions] PRIMARY KEY ([LedgerTransactionID]);
GO

ALTER TABLE [LedgerEntries] ADD CONSTRAINT [PK_LedgerEntries] PRIMARY KEY ([LedgerEntryID]);
GO

ALTER TABLE [LedgerAccounts] ADD CONSTRAINT [PK_LedgerAccounts] PRIMARY KEY ([LedgerAccountID]);
GO

ALTER TABLE [LeaseSignatories] ADD CONSTRAINT [PK_LeaseSignatories] PRIMARY KEY ([LeaseSignatoryID]);
GO

ALTER TABLE [LeaseDocExtractedTerms] ADD CONSTRAINT [PK_LeaseDocExtractedTerms] PRIMARY KEY ([LeaseDocExtractedTermID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [PK_Leases] PRIMARY KEY ([LeaseID]);
GO

ALTER TABLE [InvoiceMasters] ADD CONSTRAINT [PK_InvoiceMasters] PRIMARY KEY ([InvoiceMasterID]);
GO

ALTER TABLE [InvoiceDetails] ADD CONSTRAINT [PK_InvoiceDetails] PRIMARY KEY ([InvoiceDetailID]);
GO

ALTER TABLE [InspectionItems] ADD CONSTRAINT [PK_InspectionItems] PRIMARY KEY ([InspectionItemID]);
GO

ALTER TABLE [Inspections] ADD CONSTRAINT [PK_Inspections] PRIMARY KEY ([InspectionID]);
GO

ALTER TABLE [IdentityVerifications] ADD CONSTRAINT [PK_IdentityVerifications] PRIMARY KEY ([IdentityVerificationID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [PK_FraudCases] PRIMARY KEY ([FraudCaseID]);
GO

ALTER TABLE [FeeTypes] ADD CONSTRAINT [PK_FeeTypes] PRIMARY KEY ([FeeTypeID]);
GO

ALTER TABLE [Fees] ADD CONSTRAINT [PK_Fees] PRIMARY KEY ([FeeID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [PK_Disputes] PRIMARY KEY ([DisputeID]);
GO

ALTER TABLE [CreditReportingEnrollments] ADD CONSTRAINT [PK_CreditReportingEnrollments] PRIMARY KEY ([CreditReportingEnrollmentID]);
GO

ALTER TABLE [CreditReportingConsentAudits] ADD CONSTRAINT [PK_CreditReportingConsentAudits] PRIMARY KEY ([ConsentAuditID]);
GO

ALTER TABLE [CreditReportings] ADD CONSTRAINT [PK_CreditReportings] PRIMARY KEY ([CreditReportingID]);
GO

ALTER TABLE [ConversationParticipants] ADD CONSTRAINT [PK_ConversationParticipants] PRIMARY KEY ([ConversationParticipantID]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [PK_ConversationMessages] PRIMARY KEY ([ConversationMessageID]);
GO

ALTER TABLE [Conversations] ADD CONSTRAINT [PK_Conversations] PRIMARY KEY ([ConversationID]);
GO

ALTER TABLE [Contractors] ADD CONSTRAINT [PK_Contractors] PRIMARY KEY ([ContractorID]);
GO

ALTER TABLE [ChargeBacks] ADD CONSTRAINT [PK_ChargeBacks] PRIMARY KEY ([ChargebackID]);
GO

ALTER TABLE [Categories] ADD CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [PK_CalendarEvents] PRIMARY KEY ([CalendarEventID]);
GO

ALTER TABLE [AutopayMandates] ADD CONSTRAINT [PK_AutopayMandates] PRIMARY KEY ([AutopayMandateID]);
GO

ALTER TABLE [AutopayConsentAudits] ADD CONSTRAINT [PK_AutopayConsentAudits] PRIMARY KEY ([AutopayConsentAuditID]);
GO

ALTER TABLE [AuditLogs] ADD CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([AuditLogID]);
GO

ALTER TABLE [Attachments] ADD CONSTRAINT [PK_Attachments] PRIMARY KEY ([AttachmentID]);
GO

ALTER TABLE [ApplicationOccupants] ADD CONSTRAINT [PK_ApplicationOccupants] PRIMARY KEY ([ApplicationOccupantID]);
GO

ALTER TABLE [AmenityCatalogs] ADD CONSTRAINT [PK_AmenityCatalogs] PRIMARY KEY ([AmenityID]);
GO

ALTER TABLE [Addresses] ADD CONSTRAINT [PK_Addresses] PRIMARY KEY ([AddressID]);
GO

ALTER TABLE [Addresses] ADD CONSTRAINT [FK_Addresses_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [ApplicationOccupants] ADD CONSTRAINT [FK_ApplicationOccupants_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [ApplicationOccupants] ADD CONSTRAINT [FK_ApplicationOccupants_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [AuditLogs] ADD CONSTRAINT [FK_AuditLogs_AspNetUsers_ActorUserID] FOREIGN KEY ([ActorUserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [AuditLogs] ADD CONSTRAINT [FK_AuditLogs_OrganizationMembers_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [AuditLogs] ADD CONSTRAINT [FK_AuditLogs_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [AuditLogs] ADD CONSTRAINT [FK_AuditLogs_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [AutopayConsentAudits] ADD CONSTRAINT [FK_AutopayConsentAudits_AutopayMandates_AutopayMandateID] FOREIGN KEY ([AutopayMandateID]) REFERENCES [AutopayMandates] ([AutopayMandateID]);
GO

ALTER TABLE [AutopayConsentAudits] ADD CONSTRAINT [FK_AutopayConsentAudits_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [AutopayMandates] ADD CONSTRAINT [FK_AutopayMandates_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [AutopayMandates] ADD CONSTRAINT [FK_AutopayMandates_PaymentMethods_PaymentMethodID] FOREIGN KEY ([PaymentMethodID]) REFERENCES [PaymentMethods] ([PaymentMethodID]);
GO

ALTER TABLE [AutopayMandates] ADD CONSTRAINT [FK_AutopayMandates_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [FK_CalendarEvents_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [FK_CalendarEvents_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [FK_CalendarEvents_MaintenanceRequests_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequests] ([MaintenanceRequestID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [FK_CalendarEvents_RentalApplications_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplications] ([RentalApplicationID]);
GO

ALTER TABLE [CalendarEvents] ADD CONSTRAINT [FK_CalendarEvents_ReservationHolds_ReservationHoldID] FOREIGN KEY ([ReservationHoldID]) REFERENCES [ReservationHolds] ([ReservationHoldID]);
GO

ALTER TABLE [ChargeBacks] ADD CONSTRAINT [FK_ChargeBacks_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [ChargeBacks] ADD CONSTRAINT [FK_ChargeBacks_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [ChargeBacks] ADD CONSTRAINT [FK_ChargeBacks_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [Contractors] ADD CONSTRAINT [FK_Contractors_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [FK_ConversationMessages_AspNetUsers_SenderUserID] FOREIGN KEY ([SenderUserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [FK_ConversationMessages_ConversationMessages_ReplyToMessageID] FOREIGN KEY ([ReplyToMessageID]) REFERENCES [ConversationMessages] ([ConversationMessageID]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [FK_ConversationMessages_Conversations_ConversationID] FOREIGN KEY ([ConversationID]) REFERENCES [Conversations] ([ConversationID]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [FK_ConversationMessages_OrganizationMembers_SenderOrganizationMemberID] FOREIGN KEY ([SenderOrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [ConversationMessages] ADD CONSTRAINT [FK_ConversationMessages_Tenants_SenderTenantID] FOREIGN KEY ([SenderTenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [ConversationParticipants] ADD CONSTRAINT [FK_ConversationParticipants_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [ConversationParticipants] ADD CONSTRAINT [FK_ConversationParticipants_Conversations_ConversationID] FOREIGN KEY ([ConversationID]) REFERENCES [Conversations] ([ConversationID]);
GO

ALTER TABLE [ConversationParticipants] ADD CONSTRAINT [FK_ConversationParticipants_OrganizationMembers_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [ConversationParticipants] ADD CONSTRAINT [FK_ConversationParticipants_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [Conversations] ADD CONSTRAINT [FK_Conversations_Disputes_DisputeID] FOREIGN KEY ([DisputeID]) REFERENCES [Disputes] ([DisputeID]);
GO

ALTER TABLE [Conversations] ADD CONSTRAINT [FK_Conversations_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [Conversations] ADD CONSTRAINT [FK_Conversations_MaintenanceRequests_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequests] ([MaintenanceRequestID]);
GO

ALTER TABLE [CreditReportingConsentAudits] ADD CONSTRAINT [FK_CreditReportingConsentAudits_CreditReportingEnrollments_CreditReportingEnrollmentID] FOREIGN KEY ([CreditReportingEnrollmentID]) REFERENCES [CreditReportingEnrollments] ([CreditReportingEnrollmentID]);
GO

ALTER TABLE [CreditReportingConsentAudits] ADD CONSTRAINT [FK_CreditReportingConsentAudits_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [CreditReportingEnrollments] ADD CONSTRAINT [FK_CreditReportingEnrollments_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [CreditReportingEnrollments] ADD CONSTRAINT [FK_CreditReportingEnrollments_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [CreditReportings] ADD CONSTRAINT [FK_CreditReportings_CreditReportingEnrollments_CreditReportingEnrollmentID] FOREIGN KEY ([CreditReportingEnrollmentID]) REFERENCES [CreditReportingEnrollments] ([CreditReportingEnrollmentID]);
GO

ALTER TABLE [CreditReportings] ADD CONSTRAINT [FK_CreditReportings_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [CreditReportings] ADD CONSTRAINT [FK_CreditReportings_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_ChargeBacks_ChargebackID] FOREIGN KEY ([ChargebackID]) REFERENCES [ChargeBacks] ([ChargebackID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_MaintenanceRequests_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequests] ([MaintenanceRequestID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [Disputes] ADD CONSTRAINT [FK_Disputes_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [Fees] ADD CONSTRAINT [FK_Fees_FeeTypes_FeeTypeID] FOREIGN KEY ([FeeTypeID]) REFERENCES [FeeTypes] ([FeeTypeID]);
GO

ALTER TABLE [Fees] ADD CONSTRAINT [FK_Fees_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_ChargeBacks_ChargebackID] FOREIGN KEY ([ChargebackID]) REFERENCES [ChargeBacks] ([ChargebackID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_PaymentIntents_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntents] ([PaymentIntentID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [FraudCases] ADD CONSTRAINT [FK_FraudCases_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [IdentityVerifications] ADD CONSTRAINT [FK_IdentityVerifications_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [InspectionItems] ADD CONSTRAINT [FK_InspectionItems_Inspections_InspectionID] FOREIGN KEY ([InspectionID]) REFERENCES [Inspections] ([InspectionID]);
GO

ALTER TABLE [Inspections] ADD CONSTRAINT [FK_Inspections_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [Inspections] ADD CONSTRAINT [FK_Inspections_Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Properties] ([PropertyID]);
GO

ALTER TABLE [Inspections] ADD CONSTRAINT [FK_Inspections_RentalUnits_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnits] ([RentalUnitID]);
GO

ALTER TABLE [InvoiceDetails] ADD CONSTRAINT [FK_InvoiceDetails_Fees_FeeID] FOREIGN KEY ([FeeID]) REFERENCES [Fees] ([FeeID]);
GO

ALTER TABLE [InvoiceDetails] ADD CONSTRAINT [FK_InvoiceDetails_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [InvoiceMasters] ADD CONSTRAINT [FK_InvoiceMasters_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [InvoiceMasters] ADD CONSTRAINT [FK_InvoiceMasters_Leases_LeaseRenewalID] FOREIGN KEY ([LeaseRenewalID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [InvoiceMasters] ADD CONSTRAINT [FK_InvoiceMasters_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [InvoiceMasters] ADD CONSTRAINT [FK_InvoiceMasters_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [LeaseDocuments] ADD CONSTRAINT [FK_LeaseDocuments_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [LeaseOccupants] ADD CONSTRAINT [FK_LeaseOccupants_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [LeaseOccupants] ADD CONSTRAINT [FK_LeaseOccupants_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [LeaseRecurringCharges] ADD CONSTRAINT [FK_LeaseRecurringCharges_Fees_FeeID] FOREIGN KEY ([FeeID]) REFERENCES [Fees] ([FeeID]);
GO

ALTER TABLE [LeaseRecurringCharges] ADD CONSTRAINT [FK_LeaseRecurringCharges_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [LeaseRenewals] ADD CONSTRAINT [FK_LeaseRenewals_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_RentalApplications_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplications] ([RentalApplicationID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_RentalUnits_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnits] ([RentalUnitID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_TenancyTypes_TenancyTypeID] FOREIGN KEY ([TenancyTypeID]) REFERENCES [TenancyTypes] ([TenancyTypeID]);
GO

ALTER TABLE [Leases] ADD CONSTRAINT [FK_Leases_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [LeaseSignatories] ADD CONSTRAINT [FK_LeaseSignatories_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [LeaseSignatories] ADD CONSTRAINT [FK_LeaseSignatories_OrganizationMembers_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [LeaseSignatories] ADD CONSTRAINT [FK_LeaseSignatories_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [LeaseSignatories] ADD CONSTRAINT [FK_LeaseSignatories_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [LedgerAccounts] ADD CONSTRAINT [FK_LedgerAccounts_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [LedgerEntries] ADD CONSTRAINT [FK_LedgerEntries_LedgerAccounts_LedgerAccountID] FOREIGN KEY ([LedgerAccountID]) REFERENCES [LedgerAccounts] ([LedgerAccountID]);
GO

ALTER TABLE [LedgerEntries] ADD CONSTRAINT [FK_LedgerEntries_LedgerTransactions_LedgerTransactionID] FOREIGN KEY ([LedgerTransactionID]) REFERENCES [LedgerTransactions] ([LedgerTransactionID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [FK_LedgerTransactions_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [FK_LedgerTransactions_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [FK_LedgerTransactions_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [FK_LedgerTransactions_Payouts_PayoutID] FOREIGN KEY ([PayoutID]) REFERENCES [Payouts] ([PayoutID]);
GO

ALTER TABLE [LedgerTransactions] ADD CONSTRAINT [FK_LedgerTransactions_Refunds_RefundID] FOREIGN KEY ([RefundID]) REFERENCES [Refunds] ([RefundID]);
GO

ALTER TABLE [ListingAccessInstructions] ADD CONSTRAINT [FK_ListingAccessInstructions_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [ListingAccessInstructions] ADD CONSTRAINT [FK_ListingAccessInstructions_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ListingAmenities] ADD CONSTRAINT [FK_ListingAmenities_AmenityCatalogs_AmenityID] FOREIGN KEY ([AmenityID]) REFERENCES [AmenityCatalogs] ([AmenityID]);
GO

ALTER TABLE [ListingAmenities] ADD CONSTRAINT [FK_ListingAmenities_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ListingPhotos] ADD CONSTRAINT [FK_ListingPhotos_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ListingPolicies] ADD CONSTRAINT [FK_ListingPolicies_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ListingRules] ADD CONSTRAINT [FK_ListingRules_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [Listings] ADD CONSTRAINT [FK_Listings_ListingTypes_ListingTypeID] FOREIGN KEY ([ListingTypeID]) REFERENCES [ListingTypes] ([ListingTypeID]);
GO

ALTER TABLE [Listings] ADD CONSTRAINT [FK_Listings_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Listings] ADD CONSTRAINT [FK_Listings_RentalUnits_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnits] ([RentalUnitID]);
GO

ALTER TABLE [ListingTermPrices] ADD CONSTRAINT [FK_ListingTermPrices_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_Categories_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [Categories] ([CategoryID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Properties] ([PropertyID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_RentalUnits_RentalUnitID] FOREIGN KEY ([RentalUnitID]) REFERENCES [RentalUnits] ([RentalUnitID]);
GO

ALTER TABLE [MaintenanceRequests] ADD CONSTRAINT [FK_MaintenanceRequests_Tenants_SubmittedByTenantID] FOREIGN KEY ([SubmittedByTenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_AspNetUsers_RecipientUserID] FOREIGN KEY ([RecipientUserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_OrganizationMembers_OrganizationMemberID] FOREIGN KEY ([OrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [OrganizationMembers] ADD CONSTRAINT [FK_OrganizationMembers_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [OrganizationStatements] ADD CONSTRAINT [FK_OrganizationStatements_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [OrgPayoutAccounts] ADD CONSTRAINT [FK_OrgPayoutAccounts_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [OrgSubscriptions] ADD CONSTRAINT [FK_OrgSubscriptions_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [OrgSubscriptions] ADD CONSTRAINT [FK_OrgSubscriptions_SubscriptionPlans_SubscriptionPlanID] FOREIGN KEY ([SubscriptionPlanID]) REFERENCES [SubscriptionPlans] ([SubscriptionPlanID]);
GO

ALTER TABLE [PaymentAllocations] ADD CONSTRAINT [FK_PaymentAllocations_InvoiceDetails_InvoiceDetailID] FOREIGN KEY ([InvoiceDetailID]) REFERENCES [InvoiceDetails] ([InvoiceDetailID]);
GO

ALTER TABLE [PaymentAllocations] ADD CONSTRAINT [FK_PaymentAllocations_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [PaymentAllocations] ADD CONSTRAINT [FK_PaymentAllocations_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [PaymentAttempts] ADD CONSTRAINT [FK_PaymentAttempts_PaymentIntents_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntents] ([PaymentIntentID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [FK_PaymentIntents_AutopayMandates_AutopayMandateID] FOREIGN KEY ([AutopayMandateID]) REFERENCES [AutopayMandates] ([AutopayMandateID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [FK_PaymentIntents_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [FK_PaymentIntents_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [FK_PaymentIntents_PaymentMethods_PaymentMethodID] FOREIGN KEY ([PaymentMethodID]) REFERENCES [PaymentMethods] ([PaymentMethodID]);
GO

ALTER TABLE [PaymentIntents] ADD CONSTRAINT [FK_PaymentIntents_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [PaymentMethods] ADD CONSTRAINT [FK_PaymentMethods_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [PaymentReminders] ADD CONSTRAINT [FK_PaymentReminders_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_PaymentIntents_PaymentIntentID] FOREIGN KEY ([PaymentIntentID]) REFERENCES [PaymentIntents] ([PaymentIntentID]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [PayoutItems] ADD CONSTRAINT [FK_PayoutItems_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [PayoutItems] ADD CONSTRAINT [FK_PayoutItems_Payouts_PayoutID] FOREIGN KEY ([PayoutID]) REFERENCES [Payouts] ([PayoutID]);
GO

ALTER TABLE [Payouts] ADD CONSTRAINT [FK_Payouts_OrgPayoutAccounts_OrgPayoutAccountID] FOREIGN KEY ([OrgPayoutAccountID]) REFERENCES [OrgPayoutAccounts] ([OrgPayoutAccountID]);
GO

ALTER TABLE [Payouts] ADD CONSTRAINT [FK_Payouts_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Properties] ADD CONSTRAINT [FK_Properties_Addresses_AddressID] FOREIGN KEY ([AddressID]) REFERENCES [Addresses] ([AddressID]);
GO

ALTER TABLE [Properties] ADD CONSTRAINT [FK_Properties_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [Ratings] ADD CONSTRAINT [FK_Ratings_AspNetUsers_ReviewerUserID] FOREIGN KEY ([ReviewerUserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Ratings] ADD CONSTRAINT [FK_Ratings_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [ReceiptMasters] ADD CONSTRAINT [FK_ReceiptMasters_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [ReceiptMasters] ADD CONSTRAINT [FK_ReceiptMasters_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [Refunds] ADD CONSTRAINT [FK_Refunds_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [Refunds] ADD CONSTRAINT [FK_Refunds_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [RentalApplications] ADD CONSTRAINT [FK_RentalApplications_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [RentalApplications] ADD CONSTRAINT [FK_RentalApplications_OrganizationMembers_ReviewedByOrganizationMemberID] FOREIGN KEY ([ReviewedByOrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [RentalApplications] ADD CONSTRAINT [FK_RentalApplications_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [RentalApplications] ADD CONSTRAINT [FK_RentalApplications_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [RentalUnits] ADD CONSTRAINT [FK_RentalUnits_Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [Properties] ([PropertyID]);
GO

ALTER TABLE [RentalUnits] ADD CONSTRAINT [FK_RentalUnits_UnitTypes_UnitTypeID] FOREIGN KEY ([UnitTypeID]) REFERENCES [UnitTypes] ([UnitTypeID]);
GO

ALTER TABLE [ReservationHolds] ADD CONSTRAINT [FK_ReservationHolds_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ReservationHolds] ADD CONSTRAINT [FK_ReservationHolds_RentalApplications_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplications] ([RentalApplicationID]);
GO

ALTER TABLE [ReservationHolds] ADD CONSTRAINT [FK_ReservationHolds_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [SecurityDeposits] ADD CONSTRAINT [FK_SecurityDeposits_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [SecurityDeposits] ADD CONSTRAINT [FK_SecurityDeposits_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID]);
GO

ALTER TABLE [SecurityDeposits] ADD CONSTRAINT [FK_SecurityDeposits_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [FK_SecurityDepositTransactions_InvoiceDetails_InvoiceDetailID] FOREIGN KEY ([InvoiceDetailID]) REFERENCES [InvoiceDetails] ([InvoiceDetailID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [FK_SecurityDepositTransactions_InvoiceMasters_InvoiceMasterID] FOREIGN KEY ([InvoiceMasterID]) REFERENCES [InvoiceMasters] ([InvoiceMasterID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [FK_SecurityDepositTransactions_Payments_PaymentID] FOREIGN KEY ([PaymentID]) REFERENCES [Payments] ([PaymentID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [FK_SecurityDepositTransactions_Refunds_RefundID] FOREIGN KEY ([RefundID]) REFERENCES [Refunds] ([RefundID]);
GO

ALTER TABLE [SecurityDepositTransactions] ADD CONSTRAINT [FK_SecurityDepositTransactions_SecurityDeposits_SecurityDepositID] FOREIGN KEY ([SecurityDepositID]) REFERENCES [SecurityDeposits] ([SecurityDepositID]);
GO

ALTER TABLE [TenantEmergencyContacts] ADD CONSTRAINT [FK_TenantEmergencyContacts_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [TenantEmployments] ADD CONSTRAINT [FK_TenantEmployments_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [TenantGuarantors] ADD CONSTRAINT [FK_TenantGuarantors_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [TenantGuarantors] ADD CONSTRAINT [FK_TenantGuarantors_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [TenantInvitations] ADD CONSTRAINT [FK_TenantInvitations_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [TenantInvitations] ADD CONSTRAINT [FK_TenantInvitations_RentalApplications_RentalApplicationID] FOREIGN KEY ([RentalApplicationID]) REFERENCES [RentalApplications] ([RentalApplicationID]);
GO

ALTER TABLE [Tenants] ADD CONSTRAINT [FK_Tenants_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [TenantScreeningChecks] ADD CONSTRAINT [FK_TenantScreeningChecks_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [ViewingAppointments] ADD CONSTRAINT [FK_ViewingAppointments_Listings_ListingID] FOREIGN KEY ([ListingID]) REFERENCES [Listings] ([ListingID]);
GO

ALTER TABLE [ViewingAppointments] ADD CONSTRAINT [FK_ViewingAppointments_OrganizationMembers_AssignedOrganizationMemberID] FOREIGN KEY ([AssignedOrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

ALTER TABLE [ViewingAppointments] ADD CONSTRAINT [FK_ViewingAppointments_Tenants_TenantID] FOREIGN KEY ([TenantID]) REFERENCES [Tenants] ([TenantID]);
GO

ALTER TABLE [WorkOrders] ADD CONSTRAINT [FK_WorkOrders_Contractors_ContractorID] FOREIGN KEY ([ContractorID]) REFERENCES [Contractors] ([ContractorID]);
GO

ALTER TABLE [WorkOrders] ADD CONSTRAINT [FK_WorkOrders_Leases_LeaseID] FOREIGN KEY ([LeaseID]) REFERENCES [Leases] ([LeaseID]);
GO

ALTER TABLE [WorkOrders] ADD CONSTRAINT [FK_WorkOrders_MaintenanceRequests_MaintenanceRequestID] FOREIGN KEY ([MaintenanceRequestID]) REFERENCES [MaintenanceRequests] ([MaintenanceRequestID]);
GO

ALTER TABLE [WorkOrders] ADD CONSTRAINT [FK_WorkOrders_OrganizationMembers_AssignedOrganizationMemberID] FOREIGN KEY ([AssignedOrganizationMemberID]) REFERENCES [OrganizationMembers] ([OrganizationMemberID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260903012156_PluralizeTableNames', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Listings] ADD [IsPetFriendly] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260903153851_AddIsPetFriendlyToListing', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PaymentProviderEvents]') AND [c].[name] = N'Payload');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [PaymentProviderEvents] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [PaymentProviderEvents] ALTER COLUMN [Payload] nvarchar(max) NOT NULL;
GO

ALTER TABLE [PaymentMethods] ADD [CardFunding] nvarchar(20) NULL;
GO

ALTER TABLE [PaymentMethods] ADD [MethodRole] nvarchar(50) NULL;
GO

ALTER TABLE [PaymentIntents] ADD [FailureCategory] nvarchar(50) NULL;
GO

ALTER TABLE [PaymentIntents] ADD [FailureCode] nvarchar(100) NULL;
GO

ALTER TABLE [PaymentAttempts] ADD [FailureCategory] nvarchar(50) NULL;
GO

ALTER TABLE [PaymentAttempts] ADD [MethodKind] nvarchar(50) NULL;
GO

ALTER TABLE [PaymentAttempts] ADD [PaymentMethodID] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260907185402_AddStripePaymentFields', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AutopayMandates]') AND [c].[name] = N'LeaseID');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AutopayMandates] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AutopayMandates] ALTER COLUMN [LeaseID] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260907190621_MakeAutopayMandateLeaseIdNullable', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SecurityDeposits]') AND [c].[name] = N'LeaseID');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [SecurityDeposits] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [SecurityDeposits] ALTER COLUMN [LeaseID] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260908002025_MakeSecurityDepositLeaseIdNullable', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [RentalApplications] ADD [AttestationProvidedInfoIsCorrect] bit NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260908023219_AddAttestationProvidedInfoIsCorrect', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Listings] ADD [BaseMonthlyRentAmountEnd] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260923101500_AddBaseMonthlyRentAmountEndToListing', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF OBJECT_ID(N'[dbo].[Listings]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountEnd') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Listings]', N'BaseMonthlyRentAmountMax') IS NULL
BEGIN
    EXEC sp_rename N'[dbo].[Listings].[BaseMonthlyRentAmountEnd]', N'BaseMonthlyRentAmountMax', N'COLUMN';
END
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260923103000_RenameBaseMonthlyRentAmountEndToMax', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Listings] ADD [AdvanceNotice] nvarchar(100) NULL;
GO

ALTER TABLE [Listings] ADD [AllowSameDay] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260923201352_AddAdvanceNoticeAndAllowSameDayToListing', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrganizationMembers] ADD [BirthDecade] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [HomeUniqueDescription] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [PetsDescription] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [ProfilePhotoUrl] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [SchoolDescription] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [TravelDestination] nvarchar(max) NULL;
GO

ALTER TABLE [OrganizationMembers] ADD [WorkDescription] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260924041040_AddOrganizationMemberProfileFields', N'8.0.19');
GO

COMMIT;
GO

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

BEGIN TRANSACTION;
GO

ALTER TABLE [CohostInvitations] ADD [CohostName] nvarchar(150) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260925021522_AddCohostNameToCohostInvitation', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ListingRules]') AND [c].[name] = N'RuleTitle');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ListingRules] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [ListingRules] ALTER COLUMN [RuleTitle] nvarchar(max) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260925201529_MakeListingRuleTitleNvarcharMax', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [OrgPayoutAccounts] ADD [ChargesEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [OrgPayoutAccounts] ADD [DetailsSubmitted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [OrgPayoutAccounts] ADD [LastStripeSyncAt] datetime2 NULL;
GO

ALTER TABLE [OrgPayoutAccounts] ADD [PayoutsEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [OrgPayoutAccounts] ADD [RequirementsCurrentlyDue] nvarchar(max) NULL;
GO

ALTER TABLE [OrgPayoutAccounts] ADD [RequirementsEventuallyDue] nvarchar(max) NULL;
GO

ALTER TABLE [OrgPayoutAccounts] ADD [StripeAccountID] nvarchar(100) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260926192738_AddOrgPayoutAccountStripeFields', N'8.0.19');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [LeaseContractTemplates] (
    [LeaseContractTemplateID] uniqueidentifier NOT NULL,
    [OrganizationID] uniqueidentifier NOT NULL,
    [TemplateName] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [HtmlContent] nvarchar(max) NOT NULL,
    [IsDefault] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [IsSystemGenerated] bit NOT NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    [UpdatedDate] datetime2 NULL,
    [UpdatedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseContractTemplates] PRIMARY KEY ([LeaseContractTemplateID]),
    CONSTRAINT [FK_LeaseContractTemplates_Organizations_OrganizationID] FOREIGN KEY ([OrganizationID]) REFERENCES [Organizations] ([OrganizationID])
);
GO

CREATE TABLE [LeaseContractTemplateVersions] (
    [LeaseContractTemplateVersionID] uniqueidentifier NOT NULL,
    [LeaseContractTemplateID] uniqueidentifier NOT NULL,
    [VersionNumber] int NOT NULL,
    [HtmlContent] nvarchar(max) NOT NULL,
    [ChangeSummary] nvarchar(250) NULL,
    [CapturedDate] datetime2 NULL,
    [CapturedBy] nvarchar(100) NULL,
    CONSTRAINT [PK_LeaseContractTemplateVersions] PRIMARY KEY ([LeaseContractTemplateVersionID]),
    CONSTRAINT [FK_LeaseContractTemplateVersions_LeaseContractTemplates_LeaseContractTemplateID] FOREIGN KEY ([LeaseContractTemplateID]) REFERENCES [LeaseContractTemplates] ([LeaseContractTemplateID])
);
GO

CREATE INDEX [IX_LeaseContractTemplates_OrganizationID] ON [LeaseContractTemplates] ([OrganizationID]);
GO

CREATE INDEX [IX_LeaseContractTemplateVersions_LeaseContractTemplateID] ON [LeaseContractTemplateVersions] ([LeaseContractTemplateID]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261002182518_AddLeaseContractTemplates', N'8.0.19');
GO

COMMIT;
GO

