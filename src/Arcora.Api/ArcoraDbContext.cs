// ===================================THIS FILE WAS AUTO GENERATED===================================

using Arcora.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api
{
    public partial class ArcoraDbContext : IdentityDbContext<User, Role, long>
    {
         private readonly IConfiguration _configuration;
        public ArcoraDbContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration) : base(options)
        {
             this._configuration = configuration;
        }

        #region ========================================== Database Entities ==========================================
          public virtual DbSet<Preference> Preferences { get; set; }
          public virtual DbSet<AccountSettings> AccountSettings { get; set; }
          public virtual DbSet<Tenant> Tenants { get; set; }
          public virtual DbSet<Property> Properties { get; set; }
          public virtual DbSet<Lease> Leases { get; set; }
          public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
          public virtual DbSet<Address> Addresses { get; set; }
          public virtual DbSet<RentalUnit> RentalUnits { get; set; }
          public virtual DbSet<Organization> Organizations { get; set; }
          public virtual DbSet<OrganizationMember> OrganizationMembers { get; set; }
          public virtual DbSet<CohostInvitation> CohostInvitations { get; set; }
          public virtual DbSet<Listing> Listings { get; set; }
          public virtual DbSet<ListingTermPrice> ListingTermPrices { get; set; }
          public virtual DbSet<ListingAccessInstruction> ListingAccessInstructions { get; set; }
          public virtual DbSet<ListingPhoto> ListingPhotos { get; set; }
          public virtual DbSet<TenantInvitation> TenantInvitations { get; set; }
          public virtual DbSet<TenantInvitationEmail> TenantInvitationEmails { get; set; }
          public virtual DbSet<TenantEmployment> TenantEmployments { get; set; }
          public virtual DbSet<TenantGuarantor> TenantGuarantors { get; set; }
          public virtual DbSet<TenantEmergencyContact> TenantEmergencyContacts { get; set; }
          public virtual DbSet<TenantScreeningCheck> TenantScreeningChecks { get; set; }
          public virtual DbSet<ApplicationOccupant> ApplicationOccupants { get; set; }
          public virtual DbSet<TenancyType> TenancyTypes { get; set; }
          public virtual DbSet<RentalApplication> RentalApplications { get; set; }
          public virtual DbSet<ReservationHold> ReservationHolds { get; set; }
          public virtual DbSet<SecurityDeposit> SecurityDeposits { get; set; }
          public virtual DbSet<SecurityDepositTransaction> SecurityDepositTransactions { get; set; }
          public virtual DbSet<LeaseOccupants> LeaseOccupants { get; set; }
          public virtual DbSet<LeaseRenewals> LeaseRenewals { get; set; }
          public virtual DbSet<LeaseDocuments> LeaseDocuments { get; set; }
          public virtual DbSet<LeaseSignatory> LeaseSignatories { get; set; }
          public virtual DbSet<LeaseRecurringCharges> LeaseRecurringCharges { get; set; }
          public virtual DbSet<ViewingAppointments> ViewingAppointments { get; set; }
          public virtual DbSet<Fee> Fees { get; set; }
          public virtual DbSet<FeeType> FeeTypes { get; set; }
          public virtual DbSet<InvoiceMaster> InvoiceMasters { get; set; }
          public virtual DbSet<TaxRate> TaxRates { get; set; }
          public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
          public virtual DbSet<AutopayMandate> AutopayMandates { get; set; }
          public virtual DbSet<AutopayConsentAudit> AutopayConsentAudits { get; set; }
          public virtual DbSet<OrganizationStatement> OrganizationStatements { get; set; }
          public virtual DbSet<PaymentIntent> PaymentIntents { get; set; }
          public virtual DbSet<PaymentAttempt> PaymentAttempts { get; set; }
          public virtual DbSet<Payment> Payments { get; set; }
          public virtual DbSet<RentPaymentAccounting> RentPaymentAccountings { get; set; }
          public virtual DbSet<RentDisbursement> RentDisbursements { get; set; }
          public virtual DbSet<OperationsIncident> OperationsIncidents { get; set; }
          public virtual DbSet<Chargeback> ChargeBacks { get; set; }
          public virtual DbSet<PaymentProviderEvent> PaymentProviderEvents { get; set; }
          public virtual DbSet<Refund> Refunds { get; set; }
          public virtual DbSet<OrgPayoutAccount> OrgPayoutAccounts { get; set; }
          public virtual DbSet<ReceiptMaster> ReceiptMasters { get; set; }
          public virtual DbSet<LedgerTransaction> LedgerTransactions { get; set; }
          public virtual DbSet<LedgerEntry> LedgerEntries { get; set; }
          public virtual DbSet<PaymentAllocation> PaymentAllocations { get; set; }
          public virtual DbSet<Payout> Payouts { get; set; }
          public virtual DbSet<LedgerAccount> LedgerAccounts { get; set; }
          public virtual DbSet<PayoutItem> PayoutItems { get; set; }
          public virtual DbSet<PaymentReminder> PaymentReminders { get; set; }
          public virtual DbSet<Contractor> Contractors { get; set; }
          public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
          public virtual DbSet<Category> Categories { get; set; }
          public virtual DbSet<WorkOrder> WorkOrders { get; set; }
          public virtual DbSet<Inspection> Inspections { get; set; }
          public virtual DbSet<InspectionItem> InspectionItems { get; set; }
          public virtual DbSet<Attachment> Attachments { get; set; }
          public virtual DbSet<Conversation> Conversations { get; set; }
          public virtual DbSet<ConversationParticipant> ConversationParticipants { get; set; }
          public virtual DbSet<ConversationMessage> ConversationMessages { get; set; }
          public virtual DbSet<ConversationMessageReaction> ConversationMessageReactions { get; set; }
          public virtual DbSet<ConversationNote> ConversationNotes { get; set; }
          public virtual DbSet<Rating> Ratings { get; set; }
          public virtual DbSet<FraudCase> FraudCases { get; set; }
          public virtual DbSet<IdentityVerification> IdentityVerifications { get; set; }
          public virtual DbSet<Dispute> Disputes { get; set; }
          public virtual DbSet<CreditReportingEnrollment> CreditReportingEnrollments { get; set; }
          public virtual DbSet<CreditReportingConsentAudit> CreditReportingConsentAudits { get; set; }
          public virtual DbSet<CreditReporting> CreditReportings { get; set; }
          public virtual DbSet<Notification> Notifications { get; set; }
          public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
          public virtual DbSet<OrgSubscription> OrgSubscriptions { get; set; }
          public virtual DbSet<AuditLog> AuditLogs { get; set; }
          public virtual DbSet<LeaseDocExtractedTerm> LeaseDocExtractedTerms { get; set; }
          public virtual DbSet<LeaseContractTemplate> LeaseContractTemplates { get; set; }
          public virtual DbSet<LeaseContractTemplateVersion> LeaseContractTemplateVersions { get; set; }
          public virtual DbSet<AmenityCatalog> AmenityCatalogs { get; set; }
          public virtual DbSet<ListingAmenity> ListingAmenities { get; set; }
          public virtual DbSet<ListingRule> ListingRules { get; set; }
          public virtual DbSet<ListingPolicy> ListingPolicies { get; set; }
          public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }
          public virtual DbSet<ListingType> ListingTypes { get; set; }
          public virtual DbSet<UnitType> UnitTypes { get; set; }
          public virtual DbSet<PostalLookupSuggestion> PostalLookupSuggestions { get; set; }
          public virtual DbSet<InspectionReview> InspectionReviews { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this._configuration.GetConnectionString("DefaultAppConnection"));
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var invitation = builder.Entity<TenantInvitation>();
            invitation.Property(i => i.MonthlyRentAmount).HasPrecision(18, 2);
            invitation.Property(i => i.SecurityDepositAmount).HasPrecision(18, 2);
            foreach (var name in new[]
            {
                nameof(TenantInvitation.MonthlyRentAmount), nameof(TenantInvitation.SecurityDepositAmount),
                nameof(TenantInvitation.Currency), nameof(TenantInvitation.StartDate), nameof(TenantInvitation.EndDate),
                nameof(TenantInvitation.LeaseTermMonths), nameof(TenantInvitation.ReservationHoldID)
            })
                invitation.Property(name).Metadata.SetAfterSaveBehavior(Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Throw);
            builder.Entity<User>().Property(user => user.PhoneNumber).HasMaxLength(16);
            builder.Entity<User>().Property(user => user.SmsCode).HasMaxLength(10);
            builder.Entity<User>().HasIndex(user => user.PhoneNumber)
                .IsUnique().HasFilter("[PhoneNumber] IS NOT NULL");
            builder.Entity<TenantInvitationEmail>()
                .HasOne(email => email.TenantInvitation)
                .WithOne()
                .HasForeignKey<TenantInvitationEmail>(email => email.TenantInvitationID);
            builder.Entity<TenantInvitationEmail>()
                .HasIndex(email => new { email.Status, email.NextAttemptAt });
            builder.Entity<AccountSettings>()
                .HasOne(settings => settings.User)
                .WithOne()
                .HasForeignKey<AccountSettings>(settings => settings.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Use the DbSet property names (pluralized) as table names, overriding any
            // singular [Table(...)] attributes on the entities so the schema matches the context.
            // Only the DbSets declared on ArcoraDbContext are affected, leaving the inherited
            // ASP.NET Identity tables (AspNetUsers, AspNetRoles, ...) untouched.
            foreach (var property in typeof(ArcoraDbContext).GetProperties(
                         System.Reflection.BindingFlags.Public |
                         System.Reflection.BindingFlags.Instance |
                         System.Reflection.BindingFlags.DeclaredOnly))
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    var entityClrType = property.PropertyType.GetGenericArguments()[0];
                    var entityType = builder.Model.FindEntityType(entityClrType);
                    entityType?.SetTableName(property.Name);
                }
            }

            // Disable cascade delete on all foreign keys to avoid SQL Server
            // "multiple cascade paths" errors. Deletions are handled explicitly.
            foreach (var relationship in builder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }

            builder.Entity<AutopayMandate>()
                .HasIndex(x => x.TenantID)
                .IsUnique()
                .HasFilter("[Status] = 'ACTIVE'");

            builder.Entity<IdentityVerification>()
                .HasIndex(x => new { x.UserID, x.ProviderName, x.VerificationType })
                .IsUnique()
                .HasFilter("[ProviderName] IS NOT NULL AND [VerificationType] IS NOT NULL AND [Status] IN ('NOT_STARTED','PENDING','SUBMITTED','UNDER_REVIEW')");

            builder.Entity<Inspection>()
                .HasIndex(x => new { x.LeaseID, x.InspectionType })
                .IsUnique()
                .HasFilter("[LeaseID] IS NOT NULL AND [InspectionType] = 'MOVE_IN'");
            builder.Entity<Inspection>()
                .HasIndex(x => x.LeaseID)
                .IsUnique()
                .HasFilter("[LeaseID] IS NOT NULL AND [InspectionType] = 'TENANT_MOVE_OUT'")
                .HasDatabaseName("IX_Inspections_LeaseID_TenantMoveOut");

            builder.Entity<PaymentIntent>()
                .HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            var rentAccounting = builder.Entity<RentPaymentAccounting>();
            builder.Entity<OperationsIncident>().HasIndex(x => new { x.InstanceId, x.CheckId })
                .IsUnique().HasFilter("[ResolvedAt] IS NULL");
            builder.Entity<RentDisbursement>().Property(x => x.Amount).HasPrecision(18, 2);
            builder.Entity<RentDisbursement>().Property(x => x.CollectedAmount).HasPrecision(18, 2);
            builder.Entity<RentDisbursement>().HasOne<Payment>().WithOne()
                .HasForeignKey<RentDisbursement>(x => x.PaymentID).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<RentDisbursement>().HasOne<Payout>().WithOne()
                .HasForeignKey<RentDisbursement>(x => x.PayoutID).OnDelete(DeleteBehavior.Restrict);
            rentAccounting.HasOne(x => x.PaymentIntent).WithOne()
                .HasForeignKey<RentPaymentAccounting>(x => x.PaymentIntentID).OnDelete(DeleteBehavior.Restrict);
            rentAccounting.HasOne(x => x.Payment).WithMany()
                .HasForeignKey(x => x.PaymentID).OnDelete(DeleteBehavior.Restrict);
            rentAccounting.HasIndex(x => x.PaymentID).IsUnique().HasFilter("[PaymentID] IS NOT NULL");
            rentAccounting.HasIndex(x => x.LeaseID).IsUnique()
                .HasFilter("[SettledAt] IS NOT NULL AND [HostPlacementFee] > 0")
                .HasDatabaseName("IX_RentPaymentAccountings_PlacementOnce");
            rentAccounting.HasIndex(x => new { x.RentalUnitID, x.RentPeriod }).IsUnique()
                .HasFilter("[SettledAt] IS NOT NULL AND [HostMonthlyFee] > 0")
                .HasDatabaseName("IX_RentPaymentAccountings_MonthlyOnce");
            foreach (var property in rentAccounting.Metadata.GetProperties()
                .Where(x => x.ClrType == typeof(decimal)))
                rentAccounting.Property<decimal>(property.Name).HasPrecision(18, 2);

            builder.Entity<PaymentProviderEvent>()
                .HasIndex(x => new { x.ProviderName, x.ProviderEventID })
                .IsUnique()
                .HasFilter("[ProviderName] = 'INTERNAL_SECURITY_DEPOSIT'")
                .HasDatabaseName("IX_PaymentProviderEvents_InternalSecurityDeposit_Idempotency");

            builder.Entity<Attachment>()
                .HasIndex(x => new { x.EntityType, x.EntityID, x.ClientUploadID })
                .IsUnique()
                .HasFilter("[ClientUploadID] IS NOT NULL");

            builder.Entity<Conversation>()
                .HasOne(x => x.Listing).WithMany()
                .HasForeignKey(x => x.ListingID).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Conversation>()
                .HasIndex(x => new { x.TenantID, x.OrganizationID, x.ListingID })
                .IsUnique()
                .HasFilter("[ConversationType] = 'DIRECT' AND [TenantID] IS NOT NULL AND [OrganizationID] IS NOT NULL AND [ListingID] IS NOT NULL")
                .HasDatabaseName("IX_Conversations_DirectTenantOrganizationListing");

            builder.Entity<InspectionReview>()
                .HasIndex(x => new { x.InspectionID, x.ReviewedAt });
            var participant = builder.Entity<ConversationParticipant>();
            participant.HasIndex(x => new { x.ConversationID, x.UserID })
                .IsUnique().HasFilter("[UserID] IS NOT NULL");
            participant.HasIndex(x => new { x.ConversationID, x.TenantID })
                .IsUnique().HasFilter("[TenantID] IS NOT NULL");
            participant.HasIndex(x => new { x.ConversationID, x.OrganizationMemberID })
                .IsUnique().HasFilter("[OrganizationMemberID] IS NOT NULL");
            builder.Entity<ConversationNote>().HasIndex(x => x.ConversationID).IsUnique();
            var reaction = builder.Entity<ConversationMessageReaction>();
            reaction.HasIndex(x => new { x.ConversationMessageID, x.ReactorTenantID })
                .IsUnique().HasFilter("[ReactorTenantID] IS NOT NULL");
            reaction.HasIndex(x => new { x.ConversationMessageID, x.ReactorOrganizationMemberID })
                .IsUnique().HasFilter("[ReactorOrganizationMemberID] IS NOT NULL");
            reaction.ToTable(table => table.HasCheckConstraint("CK_ConversationMessageReactions_SingleReactor",
                "([ReactorTenantID] IS NULL AND [ReactorOrganizationMemberID] IS NOT NULL) OR ([ReactorTenantID] IS NOT NULL AND [ReactorOrganizationMemberID] IS NULL)"));
            // You can seed the tables in your database, after this commented line, 
        }
    }

////Install-Package Microsoft.EntityFrameworkCore.Tools
////Add-Migration InitialCreate
////Update-Database

////OR

////dotnet new tool-manifest
////dotnone tool install --global dotnet-ef --source https://api.nuget.org/v3/index.json
////dotnet add package Microsoft.EntityFrameworkCore.Design --source https://api.nuget.org/v3/index.json
////dotnet ef migrations add InitialCreate
////ef migrations remove
////dotnet-ef database update
}
