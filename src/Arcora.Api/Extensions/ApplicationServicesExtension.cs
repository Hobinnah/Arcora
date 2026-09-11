// ===================================THIS FILE WAS AUTO GENERATED===================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Google;
using System.Threading.RateLimiting;
using Arcora.Api.Accounts;
using AutoMapper;
using Arcora.Api.Middleware;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Configurations;
using Arcora.Api.TokenServices;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Arcora.Api.DTOs.DtoProfiles;
using Arcora.Api;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Arcora.Payments.DependencyInjection;



namespace Arcora.Api.Extensions
{
    /// <summary>
    /// Extension for adding application services
    /// </summary>
    public static class ApplicationServicesExtension
    {
        /// <summary>
        /// Cache configuration key
        /// </summary>
        private const string cacheConfigurationKey = "Framework:CommonConfig:Cache";
        private const string jwtConfigurationKey = "Authentication:JwtSettings";
        private const string googleConfigurationKey = "Authentication:Google";
        private const string oidcConfigurationKey = "Authentication:OIDC";
        private const string blobStorageConfigurationKey = "Azure:BlobStorage";

        /// <summary>
        /// Add application services
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {


            #region===========================DBContext Registration===========================
             // services.AddDbContextPool<ArcoraDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultAppConnection")));
             services.AddDbContext<ArcoraDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultAppConnection")));

            #endregion

            #region===========================IOption Configurations===========================
             services.Configure<OIDCConfiguration>(configuration.GetSection(oidcConfigurationKey));
             services.Configure<GoogleConfiguration>(configuration.GetSection(googleConfigurationKey));
             services.Configure<JwtConfiguration>(configuration.GetSection(jwtConfigurationKey));
             services.Configure<CacheConfiguration>(configuration.GetSection(cacheConfigurationKey));
             services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            #endregion

            #region===========================Azure Blob Storage===========================
             services.Configure<BlobStorageConfiguration>(configuration.GetSection(blobStorageConfigurationKey));
             var blobConfig = configuration.GetSection(blobStorageConfigurationKey).Get<BlobStorageConfiguration>() ?? new BlobStorageConfiguration();
             services.AddAzureClients(builder =>
             {
                 if (!string.IsNullOrWhiteSpace(blobConfig.ConnectionString))
                 {
                     // Connection string (real account) or Azurite ("UseDevelopmentStorage=true") during development.
                     builder.AddBlobServiceClient(blobConfig.ConnectionString);
                 }
                 else if (!string.IsNullOrWhiteSpace(blobConfig.AccountName))
                 {
                     // Managed identity in production.
                     builder.AddBlobServiceClient(new Uri($"https://{blobConfig.AccountName}.blob.core.windows.net"));
                     builder.UseCredential(new DefaultAzureCredential());
                 }
             });
             services.AddTransient<IFileStorageService, AzureBlobStorageService>();

            #endregion

            #region===========================DI Registrations===========================
             services.AddTransient<IUnitTypeService, UnitTypeService>();
             services.AddTransient<IUnitTypeRepository, UnitTypeRepository>();
             services.AddTransient<IListingTypeService, ListingTypeService>();
             services.AddTransient<IListingTypeRepository, ListingTypeRepository>();
             services.AddTransient<ICalendarEventService, CalendarEventService>();
             services.AddTransient<ICalendarEventRepository, CalendarEventRepository>();
             services.AddTransient<IListingPolicyService, ListingPolicyService>();
             services.AddTransient<IListingPolicyRepository, ListingPolicyRepository>();
             services.AddTransient<IListingRuleService, ListingRuleService>();
             services.AddTransient<IListingRuleRepository, ListingRuleRepository>();
             services.AddTransient<IListingAmenityService, ListingAmenityService>();
             services.AddTransient<IListingAmenityRepository, ListingAmenityRepository>();
             services.AddTransient<IAmenityCatalogService, AmenityCatalogService>();
             services.AddTransient<IAmenityCatalogRepository, AmenityCatalogRepository>();
             services.AddTransient<ILeaseDocExtractedTermService, LeaseDocExtractedTermService>();
             services.AddTransient<ILeaseDocExtractedTermRepository, LeaseDocExtractedTermRepository>();
             services.AddTransient<IAuditLogService, AuditLogService>();
             services.AddTransient<IAuditLogRepository, AuditLogRepository>();
             services.AddTransient<IOrgSubscriptionService, OrgSubscriptionService>();
             services.AddTransient<IOrgSubscriptionRepository, OrgSubscriptionRepository>();
             services.AddTransient<ISubscriptionPlanService, SubscriptionPlanService>();
             services.AddTransient<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
             services.AddTransient<INotificationService, NotificationService>();
             services.AddTransient<INotificationRepository, NotificationRepository>();
             services.AddTransient<ICreditReportingService, CreditReportingService>();
             services.AddTransient<ICreditReportingRepository, CreditReportingRepository>();
             services.AddTransient<ICreditReportingConsentAuditService, CreditReportingConsentAuditService>();
             services.AddTransient<ICreditReportingConsentAuditRepository, CreditReportingConsentAuditRepository>();
             services.AddTransient<ICreditReportingEnrollmentService, CreditReportingEnrollmentService>();
             services.AddTransient<ICreditReportingEnrollmentRepository, CreditReportingEnrollmentRepository>();
             services.AddTransient<IDisputeService, DisputeService>();
             services.AddTransient<IDisputeRepository, DisputeRepository>();
             services.AddTransient<IIdentityVerificationService, IdentityVerificationService>();
             services.AddTransient<IIdentityVerificationRepository, IdentityVerificationRepository>();
             services.AddTransient<IFraudCaseService, FraudCaseService>();
             services.AddTransient<IFraudCaseRepository, FraudCaseRepository>();
             services.AddTransient<IRatingService, RatingService>();
             services.AddTransient<IRatingRepository, RatingRepository>();
             services.AddTransient<IConversationMessageService, ConversationMessageService>();
             services.AddTransient<IConversationMessageRepository, ConversationMessageRepository>();
             services.AddTransient<IConversationParticipantService, ConversationParticipantService>();
             services.AddTransient<IConversationParticipantRepository, ConversationParticipantRepository>();
             services.AddTransient<IConversationService, ConversationService>();
             services.AddTransient<IConversationRepository, ConversationRepository>();
             services.AddTransient<IMessagingService, MessagingService>();
             services.AddTransient<IAttachmentService, AttachmentService>();
             services.AddTransient<IAttachmentRepository, AttachmentRepository>();
             services.AddTransient<IInspectionItemService, InspectionItemService>();
             services.AddTransient<IInspectionItemRepository, InspectionItemRepository>();
             services.AddTransient<IInspectionService, InspectionService>();
             services.AddTransient<IInspectionRepository, InspectionRepository>();
             services.AddTransient<IWorkOrderService, WorkOrderService>();
             services.AddTransient<IWorkOrderRepository, WorkOrderRepository>();
             services.AddTransient<ICategoryService, CategoryService>();
             services.AddTransient<ICategoryRepository, CategoryRepository>();
             services.AddTransient<IMaintenanceRequestService, MaintenanceRequestService>();
             services.AddTransient<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
             services.AddTransient<IContractorService, ContractorService>();
             services.AddTransient<IContractorRepository, ContractorRepository>();
             services.AddTransient<IPaymentReminderService, PaymentReminderService>();
             services.AddTransient<IPaymentReminderRepository, PaymentReminderRepository>();
             services.AddTransient<IPayoutItemService, PayoutItemService>();
             services.AddTransient<IPayoutItemRepository, PayoutItemRepository>();
             services.AddTransient<ILedgerAccountService, LedgerAccountService>();
             services.AddTransient<ILedgerAccountRepository, LedgerAccountRepository>();
             services.AddTransient<IPayoutService, PayoutService>();
             services.AddTransient<IPayoutRepository, PayoutRepository>();
             services.AddTransient<IPaymentAllocationService, PaymentAllocationService>();
             services.AddTransient<IPaymentAllocationRepository, PaymentAllocationRepository>();
             services.AddTransient<ILedgerEntryService, LedgerEntryService>();
             services.AddTransient<ILedgerEntryRepository, LedgerEntryRepository>();
             services.AddTransient<ILedgerTransactionService, LedgerTransactionService>();
             services.AddTransient<ILedgerTransactionRepository, LedgerTransactionRepository>();
             services.AddTransient<IReceiptMasterService, ReceiptMasterService>();
             services.AddTransient<IReceiptMasterRepository, ReceiptMasterRepository>();
             services.AddTransient<IOrgPayoutAccountService, OrgPayoutAccountService>();
             services.AddTransient<IOrgPayoutAccountRepository, OrgPayoutAccountRepository>();
             services.AddTransient<IRefundService, RefundService>();
             services.AddTransient<IRefundRepository, RefundRepository>();
             services.AddTransient<IPaymentProviderEventService, PaymentProviderEventService>();
             services.AddTransient<IPaymentProviderEventRepository, PaymentProviderEventRepository>();
             services.AddTransient<IChargebackService, ChargebackService>();
             services.AddTransient<IChargebackRepository, ChargebackRepository>();
             services.AddTransient<IPaymentService, PaymentService>();
             services.AddTransient<IPaymentRepository, PaymentRepository>();
             services.AddTransient<IPaymentAttemptService, PaymentAttemptService>();
             services.AddTransient<IPaymentAttemptRepository, PaymentAttemptRepository>();
             services.AddTransient<IPaymentIntentService, PaymentIntentService>();
             services.AddTransient<IPaymentIntentRepository, PaymentIntentRepository>();
             services.AddTransient<IRentCollectionOrchestrator, RentCollectionOrchestrator>();
             services.AddTransient<IPaymentOnboardingService, PaymentOnboardingService>();
             services.AddTransient<IOrganizationStatementService, OrganizationStatementService>();
             services.AddTransient<IOrganizationStatementRepository, OrganizationStatementRepository>();
             services.AddTransient<IAutopayConsentAuditService, AutopayConsentAuditService>();
             services.AddTransient<IAutopayConsentAuditRepository, AutopayConsentAuditRepository>();
             services.AddTransient<IAutopayMandateService, AutopayMandateService>();
             services.AddTransient<IAutopayMandateRepository, AutopayMandateRepository>();
             services.AddTransient<IInvoiceDetailService, InvoiceDetailService>();
             services.AddTransient<IInvoiceDetailRepository, InvoiceDetailRepository>();
             services.AddTransient<ITaxRateService, TaxRateService>();
             services.AddTransient<ITaxRateRepository, TaxRateRepository>();
             services.AddTransient<IInvoiceMasterService, InvoiceMasterService>();
             services.AddTransient<IInvoiceMasterRepository, InvoiceMasterRepository>();
             services.AddTransient<IFeeTypeService, FeeTypeService>();
             services.AddTransient<IFeeTypeRepository, FeeTypeRepository>();
             services.AddTransient<IFeeService, FeeService>();
             services.AddTransient<IFeeRepository, FeeRepository>();
             services.AddTransient<IViewingAppointmentsService, ViewingAppointmentsService>();
             services.AddTransient<IViewingAppointmentsRepository, ViewingAppointmentsRepository>();
             services.AddTransient<ILeaseRecurringChargesService, LeaseRecurringChargesService>();
             services.AddTransient<ILeaseRecurringChargesRepository, LeaseRecurringChargesRepository>();
             services.AddTransient<ILeaseSignatoriesService, LeaseSignatoriesService>();
             services.AddTransient<ILeaseSignatoriesRepository, LeaseSignatoriesRepository>();
             services.AddTransient<ILeaseDocumentsService, LeaseDocumentsService>();
             services.AddTransient<ILeaseDocumentsRepository, LeaseDocumentsRepository>();
             services.AddTransient<ILeaseRenewalsService, LeaseRenewalsService>();
             services.AddTransient<ILeaseRenewalsRepository, LeaseRenewalsRepository>();
             services.AddTransient<ILeaseOccupantsService, LeaseOccupantsService>();
             services.AddTransient<ILeaseOccupantsRepository, LeaseOccupantsRepository>();
             services.AddTransient<ISecurityDepositTransactionService, SecurityDepositTransactionService>();
             services.AddTransient<ISecurityDepositTransactionRepository, SecurityDepositTransactionRepository>();
             services.AddTransient<ISecurityDepositService, SecurityDepositService>();
             services.AddTransient<ISecurityDepositRepository, SecurityDepositRepository>();
             services.AddTransient<IReservationHoldService, ReservationHoldService>();
             services.AddTransient<IReservationHoldRepository, ReservationHoldRepository>();
             services.AddTransient<IRentalApplicationService, RentalApplicationService>();
             services.AddTransient<IRentalApplicationRepository, RentalApplicationRepository>();
             services.AddTransient<ITenancyTypeService, TenancyTypeService>();
             services.AddTransient<ITenancyTypeRepository, TenancyTypeRepository>();
             services.AddTransient<IApplicationOccupantService, ApplicationOccupantService>();
             services.AddTransient<IApplicationOccupantRepository, ApplicationOccupantRepository>();
             services.AddTransient<ITenantScreeningCheckService, TenantScreeningCheckService>();
             services.AddTransient<ITenantScreeningCheckRepository, TenantScreeningCheckRepository>();
             services.AddTransient<ITenantEmergencyContactService, TenantEmergencyContactService>();
             services.AddTransient<ITenantEmergencyContactRepository, TenantEmergencyContactRepository>();
             services.AddTransient<ITenantGuarantorService, TenantGuarantorService>();
             services.AddTransient<ITenantGuarantorRepository, TenantGuarantorRepository>();
             services.AddTransient<ITenantEmploymentService, TenantEmploymentService>();
             services.AddTransient<ITenantEmploymentRepository, TenantEmploymentRepository>();
             services.AddTransient<ITenantInvitationService, TenantInvitationService>();
             services.AddTransient<ITenantInvitationRepository, TenantInvitationRepository>();
             services.AddTransient<IListingPhotoService, ListingPhotoService>();
             services.AddTransient<IListingPhotoRepository, ListingPhotoRepository>();
             services.AddTransient<IListingAccessInstructionService, ListingAccessInstructionService>();
             services.AddTransient<IListingAccessInstructionRepository, ListingAccessInstructionRepository>();
             services.AddTransient<IListingTermPriceService, ListingTermPriceService>();
             services.AddTransient<IListingTermPriceRepository, ListingTermPriceRepository>();
             services.AddTransient<IListingService, ListingService>();
             services.AddTransient<IListingRepository, ListingRepository>();
             services.AddTransient<IOrganizationMemberService, OrganizationMemberService>();
             services.AddTransient<IOrganizationMemberRepository, OrganizationMemberRepository>();
             services.AddTransient<IOrganizationService, OrganizationService>();
             services.AddTransient<IOrganizationRepository, OrganizationRepository>();
             services.AddTransient<IRentalUnitService, RentalUnitService>();
             services.AddTransient<IRentalUnitRepository, RentalUnitRepository>();
             services.AddTransient<IAddressService, AddressService>();
             services.AddTransient<IAddressRepository, AddressRepository>();
             services.AddTransient<IPaymentMethodService, PaymentMethodService>();
             services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
             services.AddTransient<ILeaseService, LeaseService>();
             services.AddTransient<ILeaseRepository, LeaseRepository>();
             services.AddTransient<IPropertyService, PropertyService>();
             services.AddTransient<IPropertyRepository, PropertyRepository>();
             services.AddTransient<ITenantService, TenantService>();
             services.AddTransient<ITenantRepository, TenantRepository>();
             services.AddScoped<IPreferenceService, PreferenceService>();
             services.AddTransient<IPreferenceRepository, PreferenceRepository>();

                  services.AddTransient<IEmailSender, EmailSender>();
                  services.AddSingleton<Arcora.Api.Email.IEmailQueue, Arcora.Api.Email.EmailQueue>();
                  services.AddHostedService<Arcora.Api.Email.EmailQueueBackgroundService>();
            #endregion

            #region===========================Auto Mapper Configurations===========================

             services.AddAutoMapper(cfg =>
             {
                 cfg.AddProfile<UnitTypeProfile>();
                 cfg.AddProfile<ListingTypeProfile>();
                 cfg.AddProfile<CalendarEventProfile>();
                 cfg.AddProfile<ListingPolicyProfile>();
                 cfg.AddProfile<ListingRuleProfile>();
                 cfg.AddProfile<ListingAmenityProfile>();
                 cfg.AddProfile<AmenityCatalogProfile>();
                 cfg.AddProfile<LeaseDocExtractedTermProfile>();
                 cfg.AddProfile<AuditLogProfile>();
                 cfg.AddProfile<OrgSubscriptionProfile>();
                 cfg.AddProfile<SubscriptionPlanProfile>();
                 cfg.AddProfile<NotificationProfile>();
                 cfg.AddProfile<CreditReportingProfile>();
                 cfg.AddProfile<CreditReportingConsentAuditProfile>();
                 cfg.AddProfile<CreditReportingEnrollmentProfile>();
                 cfg.AddProfile<DisputeProfile>();
                 cfg.AddProfile<IdentityVerificationProfile>();
                 cfg.AddProfile<FraudCaseProfile>();
                 cfg.AddProfile<RatingProfile>();
                 cfg.AddProfile<ConversationMessageProfile>();
                 cfg.AddProfile<ConversationParticipantProfile>();
                 cfg.AddProfile<ConversationProfile>();
                 cfg.AddProfile<AttachmentProfile>();
                 cfg.AddProfile<InspectionItemProfile>();
                 cfg.AddProfile<InspectionProfile>();
                 cfg.AddProfile<WorkOrderProfile>();
                 cfg.AddProfile<CategoryProfile>();
                 cfg.AddProfile<MaintenanceRequestProfile>();
                 cfg.AddProfile<ContractorProfile>();
                 cfg.AddProfile<PaymentReminderProfile>();
                 cfg.AddProfile<PayoutItemProfile>();
                 cfg.AddProfile<LedgerAccountProfile>();
                 cfg.AddProfile<PayoutProfile>();
                 cfg.AddProfile<PaymentAllocationProfile>();
                 cfg.AddProfile<LedgerEntryProfile>();
                 cfg.AddProfile<LedgerTransactionProfile>();
                 cfg.AddProfile<ReceiptMasterProfile>();
                 cfg.AddProfile<OrgPayoutAccountProfile>();
                 cfg.AddProfile<RefundProfile>();
                 cfg.AddProfile<PaymentProviderEventProfile>();
                 cfg.AddProfile<ChargebackProfile>();
                 cfg.AddProfile<PaymentProfile>();
                 cfg.AddProfile<PaymentAttemptProfile>();
                 cfg.AddProfile<PaymentIntentProfile>();
                 cfg.AddProfile<OrganizationStatementProfile>();
                 cfg.AddProfile<AutopayConsentAuditProfile>();
                 cfg.AddProfile<AutopayMandateProfile>();
                 cfg.AddProfile<InvoiceDetailProfile>();
                 cfg.AddProfile<TaxRateProfile>();
                 cfg.AddProfile<InvoiceMasterProfile>();
                 cfg.AddProfile<FeeTypeProfile>();
                 cfg.AddProfile<FeeProfile>();
                 cfg.AddProfile<ViewingAppointmentsProfile>();
                 cfg.AddProfile<LeaseRecurringChargesProfile>();
                 cfg.AddProfile<LeaseSignatoriesProfile>();
                 cfg.AddProfile<LeaseDocumentsProfile>();
                 cfg.AddProfile<LeaseRenewalsProfile>();
                 cfg.AddProfile<LeaseOccupantsProfile>();
                 cfg.AddProfile<SecurityDepositTransactionProfile>();
                 cfg.AddProfile<SecurityDepositProfile>();
                 cfg.AddProfile<ReservationHoldProfile>();
                 cfg.AddProfile<RentalApplicationProfile>();
                 cfg.AddProfile<TenancyTypeProfile>();
                 cfg.AddProfile<ApplicationOccupantProfile>();
                 cfg.AddProfile<TenantScreeningCheckProfile>();
                 cfg.AddProfile<TenantEmergencyContactProfile>();
                 cfg.AddProfile<TenantGuarantorProfile>();
                 cfg.AddProfile<TenantEmploymentProfile>();
                 cfg.AddProfile<TenantInvitationProfile>();
                 cfg.AddProfile<ListingPhotoProfile>();
                 cfg.AddProfile<ListingAccessInstructionProfile>();
                 cfg.AddProfile<ListingTermPriceProfile>();
                 cfg.AddProfile<ListingProfile>();
                 cfg.AddProfile<OrganizationMemberProfile>();
                 cfg.AddProfile<OrganizationProfile>();
                 cfg.AddProfile<RentalUnitProfile>();
                 cfg.AddProfile<AddressProfile>();
                 cfg.AddProfile<PaymentMethodProfile>();
                 cfg.AddProfile<LeaseProfile>();
                 cfg.AddProfile<PropertyProfile>();
                 cfg.AddProfile<TenantProfile>();
             });

            #endregion

            #region===========================CORS Registrations===========================
            var corsOrigins = configuration["CorsOrigins"]?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("EnableCORS", policyBuilder =>
                {
                    policyBuilder
                        .WithOrigins(corsOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            #endregion

            #region===========================Authentication Registrations===========================

             services.AddTransient<ITokenService, TokenService>();
             services.AddTransient<IAccountService, AccountService>();
             services.AddTransient<TokenManagerMiddleware>();

             services.AddIdentityCore<User>(options =>
                    {
                       options.User.RequireUniqueEmail = true;
                        options.Password.RequiredLength = 8;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.SignIn.RequireConfirmedEmail = false;
                    })
                    .AddRoles<Role>()
                   .AddEntityFrameworkStores<ArcoraDbContext>()
                    .AddSignInManager()
                    .AddDefaultTokenProviders();


            var jwtSettings = new JwtConfiguration();
            configuration.GetSection("Authentication:JwtSettings").Bind(jwtSettings);

            var oidcSettings = new OIDCConfiguration();
            configuration.GetSection("Authentication:OIDC").Bind(oidcSettings);

            var googleSettings = new GoogleConfiguration();
            configuration.GetSection("Authentication:Google").Bind(googleSettings);


            // Policy scheme chooses the right handler per request.
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "MultiAuth";
                    options.DefaultChallengeScheme = "MultiAuth";
                })
                .AddPolicyScheme("MultiAuth", "JWT or OIDC/Google", options =>
                {
                    options.ForwardDefaultSelector = ctx =>
                    {
                        var hasBearer = ctx.Request.Headers["Authorization"].ToString()
                            .StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

                        // Exception: Google callback needs to use Cookie scheme to access Identity.External
                        if (ctx.Request.Path.StartsWithSegments("/api/Account/google-callback") || ctx.Request.Path.StartsWithSegments("/api/Account/google-login"))
                        {
                            return CookieAuthenticationDefaults.AuthenticationScheme;
                        }

                        // APIs: prefer JWT (no browser redirects)
                        if (ctx.Request.Path.StartsWithSegments("/api") || hasBearer)
                            return JwtBearerDefaults.AuthenticationScheme;

                        // Browser pages: use cookies (challenge goes to OIDC/Google)
                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };

                    // If a browser hits a protected page without auth, challenge via OIDC by default.
                    options.ForwardChallenge = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = "/login";
                    o.AccessDeniedPath = "/denied";
                    o.SlidingExpiration = true;
                    o.Cookie.HttpOnly = true;
                    var isDevelopment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development";
                    o.Cookie.SecurePolicy = isDevelopment ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
                    o.Cookie.SameSite = SameSiteMode.None;

                    // Don't redirect API requests to login page - return 401 instead
                    o.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };

                    o.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                })

                // Identity.Application scheme (required by SignInManager for sign-in operations)
                .AddCookie(IdentityConstants.ApplicationScheme, o =>
                {
                    o.LoginPath = "/login";
                    o.Cookie.Name = IdentityConstants.ApplicationScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                    o.SlidingExpiration = true;

                    // Don't redirect API requests
                    o.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                })
                // External cookie scheme for Google OAuth (required by SignInManager)
                .AddCookie(IdentityConstants.ExternalScheme, o =>
                {
                    o.Cookie.Name = IdentityConstants.ExternalScheme;
                    o.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                })

                 // ---- JWT Auth ----
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, b =>
                {
                    b.RequireHttpsMetadata = true;
                    b.SaveToken = true;
                    b.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };

                    // Suppress any redirect - always return 401/403 for API requests
                    b.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse(); // Prevents default challenge behavior (no redirect)
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"statusCode\":401,\"message\":\"Unauthorized. Please login.\"}");
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"statusCode\":403,\"message\":\"Access denied. Insufficient permissions.\"}");
                        }
                    };
                });

                // Your internal OIDC (e.g., Azure AD/Okta/Auth0/your own)
                if (!string.IsNullOrEmpty(oidcSettings.Authority) && !oidcSettings.Authority.Contains("your-authority") &&
                    !string.IsNullOrEmpty(oidcSettings.ClientId) && !oidcSettings.ClientId.Contains("your-oidc-client-id"))
                {
                    services.AddAuthentication().AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
                    {
                       o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                       o.Authority = oidcSettings.Authority;
                       o.ClientId = oidcSettings.ClientId;
                       o.ClientSecret = oidcSettings.ClientSecret;
                       o.ResponseType = "code";
                       o.SaveTokens = true;
                       o.GetClaimsFromUserInfoEndpoint = true;
                       o.Scope.Add("openid");
                       o.Scope.Add("profile");
                       o.Scope.Add("email");
                   });
                }

                // ---- Google (quick) via OAuth handler ----
                if (!string.IsNullOrEmpty(googleSettings.ClientId) && !googleSettings.ClientId.Contains("your-google-client-id") &&
                    !string.IsNullOrEmpty(googleSettings.ClientSecret) && !googleSettings.ClientSecret.Contains("your-oidc-client-secret"))
                {
                    services.AddAuthentication().AddGoogle("Google", o =>
                    {
                       o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                       o.ClientId = googleSettings.ClientId!;
                       o.ClientSecret = googleSettings.ClientSecret!;
                       // Default CallbackPath is /signin-google; register it in Google Cloud Console
                       o.SaveTokens = true;
                       o.Scope.Add("email");
                       o.Scope.Add("profile");
                       // Optional: map extras
                       // o.ClaimActions.MapJsonKey("urn:google:picture", "picture");
                   });
                }


            #endregion

            #region===========================Authorization Registrations===========================
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUserRole", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("User") ||
                        context.User.IsInRole("Viewer") ||
                        context.User.IsInRole("Admin")));
            });

            #endregion

            #region===========================Other Registrations===========================
             services.AddMemoryCache();

             services.AddHttpContextAccessor();

             services.AddRateLimiter(options =>
             {
                 options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                 options.AddPolicy("AuthPolicy", context =>
                     RateLimitPartition.GetFixedWindowLimiter(
                         partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                         factory: _ => new FixedWindowRateLimiterOptions
                         {
                             PermitLimit = 10,
                             Window = TimeSpan.FromMinutes(1),
                             QueueLimit = 0
                         }));
             });
            #endregion


            services.AddArcoraPayments(configuration);

            services.Configure<RentCollectionOptions>(configuration.GetSection(RentCollectionOptions.SectionName));
            services.AddHostedService<RecurringRentCollectionService>();

            return services;
        }
    }
}