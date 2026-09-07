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
          public virtual DbSet<Tenant> Tenants { get; set; }
          public virtual DbSet<Property> Properties { get; set; }
          public virtual DbSet<Lease> Leases { get; set; }
          public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
          public virtual DbSet<Address> Addresses { get; set; }
          public virtual DbSet<RentalUnit> RentalUnits { get; set; }
          public virtual DbSet<Organization> Organizations { get; set; }
          public virtual DbSet<OrganizationMember> OrganizationMembers { get; set; }
          public virtual DbSet<Listing> Listings { get; set; }
          public virtual DbSet<ListingTermPrice> ListingTermPrices { get; set; }
          public virtual DbSet<ListingAccessInstruction> ListingAccessInstructions { get; set; }
          public virtual DbSet<ListingPhoto> ListingPhotos { get; set; }
          public virtual DbSet<TenantInvitation> TenantInvitations { get; set; }
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
          public virtual DbSet<AmenityCatalog> AmenityCatalogs { get; set; }
          public virtual DbSet<ListingAmenity> ListingAmenities { get; set; }
          public virtual DbSet<ListingRule> ListingRules { get; set; }
          public virtual DbSet<ListingPolicy> ListingPolicies { get; set; }
          public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }
          public virtual DbSet<ListingType> ListingTypes { get; set; }
          public virtual DbSet<UnitType> UnitTypes { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._configuration.GetConnectionString("DefaultAppConnection"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
            // You can seed the tables in your database, after this commented line, 
        }
    }

////Install-Package Microsoft.EntityFrameworkCore.Tools
////Add-Migration InitialCreate
////Update-Database

////OR

////dotnet new tool-manifest
////dotnet tool install --global dotnet-ef --source https://api.nuget.org/v3/index.json
////dotnet add package Microsoft.EntityFrameworkCore.Design --source https://api.nuget.org/v3/index.json
////dotnet ef migrations add InitialCreate
////ef migrations remove
////dotnet-ef database update
}
