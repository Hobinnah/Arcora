using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arcora.Api.Migrations
{
    /// <inheritdoc />
    public partial class PluralizeTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Organization_OrganizationID",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationOccupant_AspNetUsers_UserID",
                table: "ApplicationOccupant");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationOccupant_Tenant_TenantID",
                table: "ApplicationOccupant");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_AspNetUsers_ActorUserID",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_OrganizationMember_OrganizationMemberID",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_Organization_OrganizationID",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLog_Tenant_TenantID",
                table: "AuditLog");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayConsentAudit_AutopayMandate_AutopayMandateID",
                table: "AutopayConsentAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayConsentAudit_Tenant_TenantID",
                table: "AutopayConsentAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandate_Lease_LeaseID",
                table: "AutopayMandate");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandate_PaymentMethod_PaymentMethodID",
                table: "AutopayMandate");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandate_Tenant_TenantID",
                table: "AutopayMandate");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_Lease_LeaseID",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_Listing_ListingID",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_MaintenanceRequest_MaintenanceRequestID",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_RentalApplication_RentalApplicationID",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvent_ReservationHold_ReservationHoldID",
                table: "CalendarEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Chargeback_Organization_OrganizationID",
                table: "Chargeback");

            migrationBuilder.DropForeignKey(
                name: "FK_Chargeback_Payment_PaymentID",
                table: "Chargeback");

            migrationBuilder.DropForeignKey(
                name: "FK_Chargeback_Tenant_TenantID",
                table: "Chargeback");

            migrationBuilder.DropForeignKey(
                name: "FK_Contractor_Organization_OrganizationID",
                table: "Contractor");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Dispute_DisputeID",
                table: "Conversation");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_Lease_LeaseID",
                table: "Conversation");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_MaintenanceRequest_MaintenanceRequestID",
                table: "Conversation");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessage_AspNetUsers_SenderUserID",
                table: "ConversationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessage_ConversationMessage_ReplyToMessageID",
                table: "ConversationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessage_Conversation_ConversationID",
                table: "ConversationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessage_OrganizationMember_SenderOrganizationMemberID",
                table: "ConversationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessage_Tenant_SenderTenantID",
                table: "ConversationMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_AspNetUsers_UserID",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationID",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_OrganizationMember_OrganizationMemberID",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipant_Tenant_TenantID",
                table: "ConversationParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReporting_CreditReportingEnrollment_CreditReportingEnrollmentID",
                table: "CreditReporting");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReporting_InvoiceMaster_InvoiceMasterID",
                table: "CreditReporting");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReporting_Payment_PaymentID",
                table: "CreditReporting");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingConsentAudit_CreditReportingEnrollment_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingConsentAudit_Tenant_TenantID",
                table: "CreditReportingConsentAudit");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingEnrollment_Lease_LeaseID",
                table: "CreditReportingEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingEnrollment_Tenant_TenantID",
                table: "CreditReportingEnrollment");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Chargeback_ChargebackID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_InvoiceMaster_InvoiceMasterID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Lease_LeaseID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_MaintenanceRequest_MaintenanceRequestID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Organization_OrganizationID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Payment_PaymentID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Dispute_Tenant_TenantID",
                table: "Dispute");

            migrationBuilder.DropForeignKey(
                name: "FK_Fee_FeeType_FeeTypeID",
                table: "Fee");

            migrationBuilder.DropForeignKey(
                name: "FK_Fee_Organization_OrganizationID",
                table: "Fee");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_Chargeback_ChargebackID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_Lease_LeaseID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_Organization_OrganizationID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_PaymentIntent_PaymentIntentID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_Payment_PaymentID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCase_Tenant_TenantID",
                table: "FraudCase");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityVerification_AspNetUsers_UserID",
                table: "IdentityVerification");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Lease_LeaseID",
                table: "Inspection");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Property_PropertyID",
                table: "Inspection");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_RentalUnit_RentalUnitID",
                table: "Inspection");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionItem_Inspection_InspectionID",
                table: "InspectionItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetail_Fee_FeeID",
                table: "InvoiceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetail_InvoiceMaster_InvoiceMasterID",
                table: "InvoiceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMaster_Lease_LeaseID",
                table: "InvoiceMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMaster_Lease_LeaseRenewalID",
                table: "InvoiceMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMaster_Organization_OrganizationID",
                table: "InvoiceMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMaster_Tenant_TenantID",
                table: "InvoiceMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_Listing_ListingID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_Organization_OrganizationID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_RentalApplication_RentalApplicationID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_RentalUnit_RentalUnitID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_TenancyType_TenancyTypeID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_Lease_Tenant_TenantID",
                table: "Lease");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseDocuments_Lease_LeaseID",
                table: "LeaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseOccupants_Lease_LeaseID",
                table: "LeaseOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseOccupants_Tenant_TenantID",
                table: "LeaseOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRecurringCharges_Fee_FeeID",
                table: "LeaseRecurringCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRecurringCharges_Lease_LeaseID",
                table: "LeaseRecurringCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRenewals_Lease_LeaseID",
                table: "LeaseRenewals");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatory_AspNetUsers_UserID",
                table: "LeaseSignatory");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatory_OrganizationMember_OrganizationMemberID",
                table: "LeaseSignatory");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatory_Organization_OrganizationID",
                table: "LeaseSignatory");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatory_Tenant_TenantID",
                table: "LeaseSignatory");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerAccount_Organization_OrganizationID",
                table: "LedgerAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_LedgerAccount_LedgerAccountID",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntry_LedgerTransaction_LedgerTransactionID",
                table: "LedgerEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransaction_InvoiceMaster_InvoiceMasterID",
                table: "LedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransaction_Organization_OrganizationID",
                table: "LedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransaction_Payment_PaymentID",
                table: "LedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransaction_Payout_PayoutID",
                table: "LedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransaction_Refund_RefundID",
                table: "LedgerTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Listing_ListingType_ListingTypeID",
                table: "Listing");

            migrationBuilder.DropForeignKey(
                name: "FK_Listing_Organization_OrganizationID",
                table: "Listing");

            migrationBuilder.DropForeignKey(
                name: "FK_Listing_RentalUnit_RentalUnitID",
                table: "Listing");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAccessInstruction_Lease_LeaseID",
                table: "ListingAccessInstruction");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAccessInstruction_Listing_ListingID",
                table: "ListingAccessInstruction");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAmenity_AmenityCatalog_AmenityID",
                table: "ListingAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAmenity_Listing_ListingID",
                table: "ListingAmenity");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingPhoto_Listing_ListingID",
                table: "ListingPhoto");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingPolicy_Listing_ListingID",
                table: "ListingPolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingRule_Listing_ListingID",
                table: "ListingRule");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingTermPrice_Listing_ListingID",
                table: "ListingTermPrice");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_Category_CategoryID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_Lease_LeaseID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_Listing_ListingID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_Property_PropertyID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_RentalUnit_RentalUnitID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequest_Tenant_SubmittedByTenantID",
                table: "MaintenanceRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_RecipientUserID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_OrganizationMember_OrganizationMemberID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Organization_OrganizationID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Tenant_TenantID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMember_AspNetUsers_UserID",
                table: "OrganizationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMember_Organization_OrganizationID",
                table: "OrganizationMember");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStatement_Organization_OrganizationID",
                table: "OrganizationStatement");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgPayoutAccount_Organization_OrganizationID",
                table: "OrgPayoutAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgSubscription_Organization_OrganizationID",
                table: "OrgSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgSubscription_SubscriptionPlan_SubscriptionPlanID",
                table: "OrgSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentIntent_PaymentIntentID",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Tenant_TenantID",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocation_InvoiceDetail_InvoiceDetailID",
                table: "PaymentAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocation_InvoiceMaster_InvoiceMasterID",
                table: "PaymentAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocation_Payment_PaymentID",
                table: "PaymentAllocation");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAttempt_PaymentIntent_PaymentIntentID",
                table: "PaymentAttempt");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntent_AutopayMandate_AutopayMandateID",
                table: "PaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntent_InvoiceMaster_InvoiceMasterID",
                table: "PaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntent_Lease_LeaseID",
                table: "PaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntent_PaymentMethod_PaymentMethodID",
                table: "PaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntent_Tenant_TenantID",
                table: "PaymentIntent");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_Tenant_TenantID",
                table: "PaymentMethod");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReminder_InvoiceMaster_InvoiceMasterID",
                table: "PaymentReminder");

            migrationBuilder.DropForeignKey(
                name: "FK_Payout_OrgPayoutAccount_OrgPayoutAccountID",
                table: "Payout");

            migrationBuilder.DropForeignKey(
                name: "FK_Payout_Organization_OrganizationID",
                table: "Payout");

            migrationBuilder.DropForeignKey(
                name: "FK_PayoutItem_Payment_PaymentID",
                table: "PayoutItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PayoutItem_Payout_PayoutID",
                table: "PayoutItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Property_Address_AddressID",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_Property_Organization_OrganizationID",
                table: "Property");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_ReviewerUserID",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Lease_LeaseID",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptMaster_Payment_PaymentID",
                table: "ReceiptMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptMaster_Tenant_TenantID",
                table: "ReceiptMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Payment_PaymentID",
                table: "Refund");

            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Tenant_TenantID",
                table: "Refund");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplication_Listing_ListingID",
                table: "RentalApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplication_OrganizationMember_ReviewedByOrganizationMemberID",
                table: "RentalApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplication_Organization_OrganizationID",
                table: "RentalApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplication_Tenant_TenantID",
                table: "RentalApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalUnit_Property_PropertyID",
                table: "RentalUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalUnit_UnitType_UnitTypeID",
                table: "RentalUnit");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHold_Listing_ListingID",
                table: "ReservationHold");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHold_RentalApplication_RentalApplicationID",
                table: "ReservationHold");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHold_Tenant_TenantID",
                table: "ReservationHold");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposit_Lease_LeaseID",
                table: "SecurityDeposit");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposit_Organization_OrganizationID",
                table: "SecurityDeposit");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposit_Tenant_TenantID",
                table: "SecurityDeposit");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransaction_InvoiceDetail_InvoiceDetailID",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransaction_InvoiceMaster_InvoiceMasterID",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransaction_Payment_PaymentID",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransaction_Refund_RefundID",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransaction_SecurityDeposit_SecurityDepositID",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenant_AspNetUsers_UserID",
                table: "Tenant");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantEmergencyContact_Tenant_TenantID",
                table: "TenantEmergencyContact");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantEmployment_Tenant_TenantID",
                table: "TenantEmployment");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantGuarantor_AspNetUsers_UserID",
                table: "TenantGuarantor");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantGuarantor_Tenant_TenantID",
                table: "TenantGuarantor");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantInvitation_Lease_LeaseID",
                table: "TenantInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantInvitation_RentalApplication_RentalApplicationID",
                table: "TenantInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantScreeningCheck_Tenant_TenantID",
                table: "TenantScreeningCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_Listing_ListingID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_OrganizationMember_AssignedOrganizationMemberID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_Tenant_TenantID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Contractor_ContractorID",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Lease_LeaseID",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_MaintenanceRequest_MaintenanceRequestID",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_OrganizationMember_AssignedOrganizationMemberID",
                table: "WorkOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrder",
                table: "WorkOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitType",
                table: "UnitType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantScreeningCheck",
                table: "TenantScreeningCheck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantInvitation",
                table: "TenantInvitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantGuarantor",
                table: "TenantGuarantor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantEmployment",
                table: "TenantEmployment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantEmergencyContact",
                table: "TenantEmergencyContact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenant",
                table: "Tenant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenancyType",
                table: "TenancyType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxRate",
                table: "TaxRate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlan",
                table: "SubscriptionPlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityDepositTransaction",
                table: "SecurityDepositTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityDeposit",
                table: "SecurityDeposit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationHold",
                table: "ReservationHold");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentalUnit",
                table: "RentalUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentalApplication",
                table: "RentalApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refund",
                table: "Refund");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptMaster",
                table: "ReceiptMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Property",
                table: "Property");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Preference",
                table: "Preference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayoutItem",
                table: "PayoutItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payout",
                table: "Payout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentReminder",
                table: "PaymentReminder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentProviderEvent",
                table: "PaymentProviderEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentIntent",
                table: "PaymentIntent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentAttempt",
                table: "PaymentAttempt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentAllocation",
                table: "PaymentAllocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrgSubscription",
                table: "OrgSubscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrgPayoutAccount",
                table: "OrgPayoutAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationStatement",
                table: "OrganizationStatement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organization",
                table: "Organization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceRequest",
                table: "MaintenanceRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingType",
                table: "ListingType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingTermPrice",
                table: "ListingTermPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingRule",
                table: "ListingRule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingPolicy",
                table: "ListingPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingPhoto",
                table: "ListingPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingAmenity",
                table: "ListingAmenity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingAccessInstruction",
                table: "ListingAccessInstruction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Listing",
                table: "Listing");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerTransaction",
                table: "LedgerTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerEntry",
                table: "LedgerEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerAccount",
                table: "LedgerAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaseSignatory",
                table: "LeaseSignatory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaseDocExtractedTerm",
                table: "LeaseDocExtractedTerm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lease",
                table: "Lease");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceMaster",
                table: "InvoiceMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDetail",
                table: "InvoiceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionItem",
                table: "InspectionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inspection",
                table: "Inspection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityVerification",
                table: "IdentityVerification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FraudCase",
                table: "FraudCase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeeType",
                table: "FeeType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fee",
                table: "Fee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dispute",
                table: "Dispute");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReportingEnrollment",
                table: "CreditReportingEnrollment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReportingConsentAudit",
                table: "CreditReportingConsentAudit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReporting",
                table: "CreditReporting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMessage",
                table: "ConversationMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contractor",
                table: "Contractor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chargeback",
                table: "Chargeback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarEvent",
                table: "CalendarEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutopayMandate",
                table: "AutopayMandate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutopayConsentAudit",
                table: "AutopayConsentAudit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationOccupant",
                table: "ApplicationOccupant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AmenityCatalog",
                table: "AmenityCatalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "WorkOrder",
                newName: "WorkOrders");

            migrationBuilder.RenameTable(
                name: "UnitType",
                newName: "UnitTypes");

            migrationBuilder.RenameTable(
                name: "TenantScreeningCheck",
                newName: "TenantScreeningChecks");

            migrationBuilder.RenameTable(
                name: "TenantInvitation",
                newName: "TenantInvitations");

            migrationBuilder.RenameTable(
                name: "TenantGuarantor",
                newName: "TenantGuarantors");

            migrationBuilder.RenameTable(
                name: "TenantEmployment",
                newName: "TenantEmployments");

            migrationBuilder.RenameTable(
                name: "TenantEmergencyContact",
                newName: "TenantEmergencyContacts");

            migrationBuilder.RenameTable(
                name: "Tenant",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "TenancyType",
                newName: "TenancyTypes");

            migrationBuilder.RenameTable(
                name: "TaxRate",
                newName: "TaxRates");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlan",
                newName: "SubscriptionPlans");

            migrationBuilder.RenameTable(
                name: "SecurityDepositTransaction",
                newName: "SecurityDepositTransactions");

            migrationBuilder.RenameTable(
                name: "SecurityDeposit",
                newName: "SecurityDeposits");

            migrationBuilder.RenameTable(
                name: "ReservationHold",
                newName: "ReservationHolds");

            migrationBuilder.RenameTable(
                name: "RentalUnit",
                newName: "RentalUnits");

            migrationBuilder.RenameTable(
                name: "RentalApplication",
                newName: "RentalApplications");

            migrationBuilder.RenameTable(
                name: "Refund",
                newName: "Refunds");

            migrationBuilder.RenameTable(
                name: "ReceiptMaster",
                newName: "ReceiptMasters");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "Property",
                newName: "Properties");

            migrationBuilder.RenameTable(
                name: "Preference",
                newName: "Preferences");

            migrationBuilder.RenameTable(
                name: "PayoutItem",
                newName: "PayoutItems");

            migrationBuilder.RenameTable(
                name: "Payout",
                newName: "Payouts");

            migrationBuilder.RenameTable(
                name: "PaymentReminder",
                newName: "PaymentReminders");

            migrationBuilder.RenameTable(
                name: "PaymentProviderEvent",
                newName: "PaymentProviderEvents");

            migrationBuilder.RenameTable(
                name: "PaymentMethod",
                newName: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "PaymentIntent",
                newName: "PaymentIntents");

            migrationBuilder.RenameTable(
                name: "PaymentAttempt",
                newName: "PaymentAttempts");

            migrationBuilder.RenameTable(
                name: "PaymentAllocation",
                newName: "PaymentAllocations");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "OrgSubscription",
                newName: "OrgSubscriptions");

            migrationBuilder.RenameTable(
                name: "OrgPayoutAccount",
                newName: "OrgPayoutAccounts");

            migrationBuilder.RenameTable(
                name: "OrganizationStatement",
                newName: "OrganizationStatements");

            migrationBuilder.RenameTable(
                name: "OrganizationMember",
                newName: "OrganizationMembers");

            migrationBuilder.RenameTable(
                name: "Organization",
                newName: "Organizations");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "MaintenanceRequest",
                newName: "MaintenanceRequests");

            migrationBuilder.RenameTable(
                name: "ListingType",
                newName: "ListingTypes");

            migrationBuilder.RenameTable(
                name: "ListingTermPrice",
                newName: "ListingTermPrices");

            migrationBuilder.RenameTable(
                name: "ListingRule",
                newName: "ListingRules");

            migrationBuilder.RenameTable(
                name: "ListingPolicy",
                newName: "ListingPolicies");

            migrationBuilder.RenameTable(
                name: "ListingPhoto",
                newName: "ListingPhotos");

            migrationBuilder.RenameTable(
                name: "ListingAmenity",
                newName: "ListingAmenities");

            migrationBuilder.RenameTable(
                name: "ListingAccessInstruction",
                newName: "ListingAccessInstructions");

            migrationBuilder.RenameTable(
                name: "Listing",
                newName: "Listings");

            migrationBuilder.RenameTable(
                name: "LedgerTransaction",
                newName: "LedgerTransactions");

            migrationBuilder.RenameTable(
                name: "LedgerEntry",
                newName: "LedgerEntries");

            migrationBuilder.RenameTable(
                name: "LedgerAccount",
                newName: "LedgerAccounts");

            migrationBuilder.RenameTable(
                name: "LeaseSignatory",
                newName: "LeaseSignatories");

            migrationBuilder.RenameTable(
                name: "LeaseDocExtractedTerm",
                newName: "LeaseDocExtractedTerms");

            migrationBuilder.RenameTable(
                name: "Lease",
                newName: "Leases");

            migrationBuilder.RenameTable(
                name: "InvoiceMaster",
                newName: "InvoiceMasters");

            migrationBuilder.RenameTable(
                name: "InvoiceDetail",
                newName: "InvoiceDetails");

            migrationBuilder.RenameTable(
                name: "InspectionItem",
                newName: "InspectionItems");

            migrationBuilder.RenameTable(
                name: "Inspection",
                newName: "Inspections");

            migrationBuilder.RenameTable(
                name: "IdentityVerification",
                newName: "IdentityVerifications");

            migrationBuilder.RenameTable(
                name: "FraudCase",
                newName: "FraudCases");

            migrationBuilder.RenameTable(
                name: "FeeType",
                newName: "FeeTypes");

            migrationBuilder.RenameTable(
                name: "Fee",
                newName: "Fees");

            migrationBuilder.RenameTable(
                name: "Dispute",
                newName: "Disputes");

            migrationBuilder.RenameTable(
                name: "CreditReportingEnrollment",
                newName: "CreditReportingEnrollments");

            migrationBuilder.RenameTable(
                name: "CreditReportingConsentAudit",
                newName: "CreditReportingConsentAudits");

            migrationBuilder.RenameTable(
                name: "CreditReporting",
                newName: "CreditReportings");

            migrationBuilder.RenameTable(
                name: "ConversationParticipant",
                newName: "ConversationParticipants");

            migrationBuilder.RenameTable(
                name: "ConversationMessage",
                newName: "ConversationMessages");

            migrationBuilder.RenameTable(
                name: "Conversation",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "Contractor",
                newName: "Contractors");

            migrationBuilder.RenameTable(
                name: "Chargeback",
                newName: "ChargeBacks");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "CalendarEvent",
                newName: "CalendarEvents");

            migrationBuilder.RenameTable(
                name: "AutopayMandate",
                newName: "AutopayMandates");

            migrationBuilder.RenameTable(
                name: "AutopayConsentAudit",
                newName: "AutopayConsentAudits");

            migrationBuilder.RenameTable(
                name: "AuditLog",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameTable(
                name: "ApplicationOccupant",
                newName: "ApplicationOccupants");

            migrationBuilder.RenameTable(
                name: "AmenityCatalog",
                newName: "AmenityCatalogs");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrder_MaintenanceRequestID",
                table: "WorkOrders",
                newName: "IX_WorkOrders_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrder_LeaseID",
                table: "WorkOrders",
                newName: "IX_WorkOrders_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrder_ContractorID",
                table: "WorkOrders",
                newName: "IX_WorkOrders_ContractorID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrder_AssignedOrganizationMemberID",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssignedOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantScreeningCheck_TenantID",
                table: "TenantScreeningChecks",
                newName: "IX_TenantScreeningChecks_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantInvitation_RentalApplicationID",
                table: "TenantInvitations",
                newName: "IX_TenantInvitations_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantInvitation_LeaseID",
                table: "TenantInvitations",
                newName: "IX_TenantInvitations_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantGuarantor_UserID",
                table: "TenantGuarantors",
                newName: "IX_TenantGuarantors_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantGuarantor_TenantID",
                table: "TenantGuarantors",
                newName: "IX_TenantGuarantors_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantEmployment_TenantID",
                table: "TenantEmployments",
                newName: "IX_TenantEmployments_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantEmergencyContact_TenantID",
                table: "TenantEmergencyContacts",
                newName: "IX_TenantEmergencyContacts_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Tenant_UserID",
                table: "Tenants",
                newName: "IX_Tenants_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransaction_SecurityDepositID",
                table: "SecurityDepositTransactions",
                newName: "IX_SecurityDepositTransactions_SecurityDepositID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransaction_RefundID",
                table: "SecurityDepositTransactions",
                newName: "IX_SecurityDepositTransactions_RefundID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransaction_PaymentID",
                table: "SecurityDepositTransactions",
                newName: "IX_SecurityDepositTransactions_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransaction_InvoiceMasterID",
                table: "SecurityDepositTransactions",
                newName: "IX_SecurityDepositTransactions_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransaction_InvoiceDetailID",
                table: "SecurityDepositTransactions",
                newName: "IX_SecurityDepositTransactions_InvoiceDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposit_TenantID",
                table: "SecurityDeposits",
                newName: "IX_SecurityDeposits_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposit_OrganizationID",
                table: "SecurityDeposits",
                newName: "IX_SecurityDeposits_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposit_LeaseID",
                table: "SecurityDeposits",
                newName: "IX_SecurityDeposits_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHold_TenantID",
                table: "ReservationHolds",
                newName: "IX_ReservationHolds_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHold_RentalApplicationID",
                table: "ReservationHolds",
                newName: "IX_ReservationHolds_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHold_ListingID",
                table: "ReservationHolds",
                newName: "IX_ReservationHolds_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalUnit_UnitTypeID",
                table: "RentalUnits",
                newName: "IX_RentalUnits_UnitTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalUnit_PropertyID",
                table: "RentalUnits",
                newName: "IX_RentalUnits_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplication_TenantID",
                table: "RentalApplications",
                newName: "IX_RentalApplications_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplication_ReviewedByOrganizationMemberID",
                table: "RentalApplications",
                newName: "IX_RentalApplications_ReviewedByOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplication_OrganizationID",
                table: "RentalApplications",
                newName: "IX_RentalApplications_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplication_ListingID",
                table: "RentalApplications",
                newName: "IX_RentalApplications_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Refund_TenantID",
                table: "Refunds",
                newName: "IX_Refunds_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Refund_PaymentID",
                table: "Refunds",
                newName: "IX_Refunds_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptMaster_TenantID",
                table: "ReceiptMasters",
                newName: "IX_ReceiptMasters_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptMaster_PaymentID",
                table: "ReceiptMasters",
                newName: "IX_ReceiptMasters_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_ReviewerUserID",
                table: "Ratings",
                newName: "IX_Ratings_ReviewerUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_LeaseID",
                table: "Ratings",
                newName: "IX_Ratings_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Property_OrganizationID",
                table: "Properties",
                newName: "IX_Properties_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Property_AddressID",
                table: "Properties",
                newName: "IX_Properties_AddressID");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutItem_PayoutID",
                table: "PayoutItems",
                newName: "IX_PayoutItems_PayoutID");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutItem_PaymentID",
                table: "PayoutItems",
                newName: "IX_PayoutItems_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Payout_OrgPayoutAccountID",
                table: "Payouts",
                newName: "IX_Payouts_OrgPayoutAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_Payout_OrganizationID",
                table: "Payouts",
                newName: "IX_Payouts_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReminder_InvoiceMasterID",
                table: "PaymentReminders",
                newName: "IX_PaymentReminders_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethod_TenantID",
                table: "PaymentMethods",
                newName: "IX_PaymentMethods_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntent_TenantID",
                table: "PaymentIntents",
                newName: "IX_PaymentIntents_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntent_PaymentMethodID",
                table: "PaymentIntents",
                newName: "IX_PaymentIntents_PaymentMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntent_LeaseID",
                table: "PaymentIntents",
                newName: "IX_PaymentIntents_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntent_InvoiceMasterID",
                table: "PaymentIntents",
                newName: "IX_PaymentIntents_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntent_AutopayMandateID",
                table: "PaymentIntents",
                newName: "IX_PaymentIntents_AutopayMandateID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAttempt_PaymentIntentID",
                table: "PaymentAttempts",
                newName: "IX_PaymentAttempts_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocation_PaymentID",
                table: "PaymentAllocations",
                newName: "IX_PaymentAllocations_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocation_InvoiceMasterID",
                table: "PaymentAllocations",
                newName: "IX_PaymentAllocations_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocation_InvoiceDetailID",
                table: "PaymentAllocations",
                newName: "IX_PaymentAllocations_InvoiceDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_TenantID",
                table: "Payments",
                newName: "IX_Payments_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PaymentIntentID",
                table: "Payments",
                newName: "IX_Payments_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgSubscription_SubscriptionPlanID",
                table: "OrgSubscriptions",
                newName: "IX_OrgSubscriptions_SubscriptionPlanID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgSubscription_OrganizationID",
                table: "OrgSubscriptions",
                newName: "IX_OrgSubscriptions_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgPayoutAccount_OrganizationID",
                table: "OrgPayoutAccounts",
                newName: "IX_OrgPayoutAccounts_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStatement_OrganizationID",
                table: "OrganizationStatements",
                newName: "IX_OrganizationStatements_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_UserID",
                table: "OrganizationMembers",
                newName: "IX_OrganizationMembers_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMember_OrganizationID",
                table: "OrganizationMembers",
                newName: "IX_OrganizationMembers_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_TenantID",
                table: "Notifications",
                newName: "IX_Notifications_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_RecipientUserID",
                table: "Notifications",
                newName: "IX_Notifications_RecipientUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_OrganizationMemberID",
                table: "Notifications",
                newName: "IX_Notifications_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_OrganizationID",
                table: "Notifications",
                newName: "IX_Notifications_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_SubmittedByTenantID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_SubmittedByTenantID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_RentalUnitID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_PropertyID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_ListingID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_LeaseID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequest_CategoryID",
                table: "MaintenanceRequests",
                newName: "IX_MaintenanceRequests_CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingTermPrice_ListingID",
                table: "ListingTermPrices",
                newName: "IX_ListingTermPrices_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingRule_ListingID",
                table: "ListingRules",
                newName: "IX_ListingRules_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingPolicy_ListingID",
                table: "ListingPolicies",
                newName: "IX_ListingPolicies_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingPhoto_ListingID",
                table: "ListingPhotos",
                newName: "IX_ListingPhotos_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAmenity_ListingID",
                table: "ListingAmenities",
                newName: "IX_ListingAmenities_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAmenity_AmenityID",
                table: "ListingAmenities",
                newName: "IX_ListingAmenities_AmenityID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAccessInstruction_ListingID",
                table: "ListingAccessInstructions",
                newName: "IX_ListingAccessInstructions_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAccessInstruction_LeaseID",
                table: "ListingAccessInstructions",
                newName: "IX_ListingAccessInstructions_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Listing_RentalUnitID",
                table: "Listings",
                newName: "IX_Listings_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Listing_OrganizationID",
                table: "Listings",
                newName: "IX_Listings_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Listing_ListingTypeID",
                table: "Listings",
                newName: "IX_Listings_ListingTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransaction_RefundID",
                table: "LedgerTransactions",
                newName: "IX_LedgerTransactions_RefundID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransaction_PayoutID",
                table: "LedgerTransactions",
                newName: "IX_LedgerTransactions_PayoutID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransaction_PaymentID",
                table: "LedgerTransactions",
                newName: "IX_LedgerTransactions_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransaction_OrganizationID",
                table: "LedgerTransactions",
                newName: "IX_LedgerTransactions_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransaction_InvoiceMasterID",
                table: "LedgerTransactions",
                newName: "IX_LedgerTransactions_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntry_LedgerTransactionID",
                table: "LedgerEntries",
                newName: "IX_LedgerEntries_LedgerTransactionID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntry_LedgerAccountID",
                table: "LedgerEntries",
                newName: "IX_LedgerEntries_LedgerAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerAccount_OrganizationID",
                table: "LedgerAccounts",
                newName: "IX_LedgerAccounts_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatory_UserID",
                table: "LeaseSignatories",
                newName: "IX_LeaseSignatories_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatory_TenantID",
                table: "LeaseSignatories",
                newName: "IX_LeaseSignatories_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatory_OrganizationMemberID",
                table: "LeaseSignatories",
                newName: "IX_LeaseSignatories_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatory_OrganizationID",
                table: "LeaseSignatories",
                newName: "IX_LeaseSignatories_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_TenantID",
                table: "Leases",
                newName: "IX_Leases_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_TenancyTypeID",
                table: "Leases",
                newName: "IX_Leases_TenancyTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_RentalUnitID",
                table: "Leases",
                newName: "IX_Leases_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_RentalApplicationID",
                table: "Leases",
                newName: "IX_Leases_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_OrganizationID",
                table: "Leases",
                newName: "IX_Leases_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Lease_ListingID",
                table: "Leases",
                newName: "IX_Leases_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMaster_TenantID",
                table: "InvoiceMasters",
                newName: "IX_InvoiceMasters_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMaster_OrganizationID",
                table: "InvoiceMasters",
                newName: "IX_InvoiceMasters_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMaster_LeaseRenewalID",
                table: "InvoiceMasters",
                newName: "IX_InvoiceMasters_LeaseRenewalID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMaster_LeaseID",
                table: "InvoiceMasters",
                newName: "IX_InvoiceMasters_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetail_InvoiceMasterID",
                table: "InvoiceDetails",
                newName: "IX_InvoiceDetails_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetail_FeeID",
                table: "InvoiceDetails",
                newName: "IX_InvoiceDetails_FeeID");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionItem_InspectionID",
                table: "InspectionItems",
                newName: "IX_InspectionItems_InspectionID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspection_RentalUnitID",
                table: "Inspections",
                newName: "IX_Inspections_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspection_PropertyID",
                table: "Inspections",
                newName: "IX_Inspections_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspection_LeaseID",
                table: "Inspections",
                newName: "IX_Inspections_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityVerification_UserID",
                table: "IdentityVerifications",
                newName: "IX_IdentityVerifications_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_TenantID",
                table: "FraudCases",
                newName: "IX_FraudCases_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_PaymentIntentID",
                table: "FraudCases",
                newName: "IX_FraudCases_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_PaymentID",
                table: "FraudCases",
                newName: "IX_FraudCases_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_OrganizationID",
                table: "FraudCases",
                newName: "IX_FraudCases_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_LeaseID",
                table: "FraudCases",
                newName: "IX_FraudCases_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCase_ChargebackID",
                table: "FraudCases",
                newName: "IX_FraudCases_ChargebackID");

            migrationBuilder.RenameIndex(
                name: "IX_Fee_OrganizationID",
                table: "Fees",
                newName: "IX_Fees_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Fee_FeeTypeID",
                table: "Fees",
                newName: "IX_Fees_FeeTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_TenantID",
                table: "Disputes",
                newName: "IX_Disputes_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_PaymentID",
                table: "Disputes",
                newName: "IX_Disputes_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_OrganizationID",
                table: "Disputes",
                newName: "IX_Disputes_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_MaintenanceRequestID",
                table: "Disputes",
                newName: "IX_Disputes_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_LeaseID",
                table: "Disputes",
                newName: "IX_Disputes_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_InvoiceMasterID",
                table: "Disputes",
                newName: "IX_Disputes_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_Dispute_ChargebackID",
                table: "Disputes",
                newName: "IX_Disputes_ChargebackID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingEnrollment_TenantID",
                table: "CreditReportingEnrollments",
                newName: "IX_CreditReportingEnrollments_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingEnrollment_LeaseID",
                table: "CreditReportingEnrollments",
                newName: "IX_CreditReportingEnrollments_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingConsentAudit_TenantID",
                table: "CreditReportingConsentAudits",
                newName: "IX_CreditReportingConsentAudits_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingConsentAudit_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudits",
                newName: "IX_CreditReportingConsentAudits_CreditReportingEnrollmentID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReporting_PaymentID",
                table: "CreditReportings",
                newName: "IX_CreditReportings_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReporting_InvoiceMasterID",
                table: "CreditReportings",
                newName: "IX_CreditReportings_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReporting_CreditReportingEnrollmentID",
                table: "CreditReportings",
                newName: "IX_CreditReportings_CreditReportingEnrollmentID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_UserID",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_TenantID",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_OrganizationMemberID",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipant_ConversationID",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_ConversationID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessage_SenderUserID",
                table: "ConversationMessages",
                newName: "IX_ConversationMessages_SenderUserID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessage_SenderTenantID",
                table: "ConversationMessages",
                newName: "IX_ConversationMessages_SenderTenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessage_SenderOrganizationMemberID",
                table: "ConversationMessages",
                newName: "IX_ConversationMessages_SenderOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessage_ReplyToMessageID",
                table: "ConversationMessages",
                newName: "IX_ConversationMessages_ReplyToMessageID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessage_ConversationID",
                table: "ConversationMessages",
                newName: "IX_ConversationMessages_ConversationID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_MaintenanceRequestID",
                table: "Conversations",
                newName: "IX_Conversations_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_LeaseID",
                table: "Conversations",
                newName: "IX_Conversations_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_DisputeID",
                table: "Conversations",
                newName: "IX_Conversations_DisputeID");

            migrationBuilder.RenameIndex(
                name: "IX_Contractor_OrganizationID",
                table: "Contractors",
                newName: "IX_Contractors_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Chargeback_TenantID",
                table: "ChargeBacks",
                newName: "IX_ChargeBacks_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Chargeback_PaymentID",
                table: "ChargeBacks",
                newName: "IX_ChargeBacks_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Chargeback_OrganizationID",
                table: "ChargeBacks",
                newName: "IX_ChargeBacks_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvent_ReservationHoldID",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_ReservationHoldID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvent_RentalApplicationID",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvent_MaintenanceRequestID",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvent_ListingID",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvent_LeaseID",
                table: "CalendarEvents",
                newName: "IX_CalendarEvents_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandate_TenantID",
                table: "AutopayMandates",
                newName: "IX_AutopayMandates_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandate_PaymentMethodID",
                table: "AutopayMandates",
                newName: "IX_AutopayMandates_PaymentMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandate_LeaseID",
                table: "AutopayMandates",
                newName: "IX_AutopayMandates_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayConsentAudit_TenantID",
                table: "AutopayConsentAudits",
                newName: "IX_AutopayConsentAudits_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayConsentAudit_AutopayMandateID",
                table: "AutopayConsentAudits",
                newName: "IX_AutopayConsentAudits_AutopayMandateID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_TenantID",
                table: "AuditLogs",
                newName: "IX_AuditLogs_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_OrganizationMemberID",
                table: "AuditLogs",
                newName: "IX_AuditLogs_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_OrganizationID",
                table: "AuditLogs",
                newName: "IX_AuditLogs_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLog_ActorUserID",
                table: "AuditLogs",
                newName: "IX_AuditLogs_ActorUserID");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationOccupant_UserID",
                table: "ApplicationOccupants",
                newName: "IX_ApplicationOccupants_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationOccupant_TenantID",
                table: "ApplicationOccupants",
                newName: "IX_ApplicationOccupants_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Address_OrganizationID",
                table: "Addresses",
                newName: "IX_Addresses_OrganizationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrders",
                table: "WorkOrders",
                column: "WorkOrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitTypes",
                table: "UnitTypes",
                column: "UnitTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantScreeningChecks",
                table: "TenantScreeningChecks",
                column: "TenantScreeningCheckID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantInvitations",
                table: "TenantInvitations",
                column: "TenantInvitationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantGuarantors",
                table: "TenantGuarantors",
                column: "TenantGuarantorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantEmployments",
                table: "TenantEmployments",
                column: "TenantEmploymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantEmergencyContacts",
                table: "TenantEmergencyContacts",
                column: "TenantEmergencyContactID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "TenantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenancyTypes",
                table: "TenancyTypes",
                column: "TenancyTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxRates",
                table: "TaxRates",
                column: "TaxID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans",
                column: "SubscriptionPlanID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityDepositTransactions",
                table: "SecurityDepositTransactions",
                column: "SecurityDepositTransactionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityDeposits",
                table: "SecurityDeposits",
                column: "SecurityDepositID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationHolds",
                table: "ReservationHolds",
                column: "ReservationHoldID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentalUnits",
                table: "RentalUnits",
                column: "RentalUnitID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentalApplications",
                table: "RentalApplications",
                column: "RentalApplicationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds",
                column: "RefundID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptMasters",
                table: "ReceiptMasters",
                column: "ReceiptMasterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "RatingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "PropertyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Preferences",
                table: "Preferences",
                column: "PreferenceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayoutItems",
                table: "PayoutItems",
                column: "PayoutItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payouts",
                table: "Payouts",
                column: "PayoutID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentReminders",
                table: "PaymentReminders",
                column: "PaymentReminderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentProviderEvents",
                table: "PaymentProviderEvents",
                column: "PaymentProviderEventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods",
                column: "PaymentMethodID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentIntents",
                table: "PaymentIntents",
                column: "PaymentIntentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentAttempts",
                table: "PaymentAttempts",
                column: "PaymentAttemptID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentAllocations",
                table: "PaymentAllocations",
                column: "PaymentAllocationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrgSubscriptions",
                table: "OrgSubscriptions",
                column: "OrgSubscriptionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrgPayoutAccounts",
                table: "OrgPayoutAccounts",
                column: "OrgPayoutAccountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationStatements",
                table: "OrganizationStatements",
                column: "OrganizationStatementID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers",
                column: "OrganizationMemberID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations",
                column: "OrganizationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceRequests",
                table: "MaintenanceRequests",
                column: "MaintenanceRequestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingTypes",
                table: "ListingTypes",
                column: "ListingTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingTermPrices",
                table: "ListingTermPrices",
                column: "ListingTermPriceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingRules",
                table: "ListingRules",
                column: "ListingRuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingPolicies",
                table: "ListingPolicies",
                column: "ListingPolicyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingPhotos",
                table: "ListingPhotos",
                column: "ListingPhotoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingAmenities",
                table: "ListingAmenities",
                column: "ListingAmenityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingAccessInstructions",
                table: "ListingAccessInstructions",
                column: "ListingAccessInstructionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Listings",
                table: "Listings",
                column: "ListingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerTransactions",
                table: "LedgerTransactions",
                column: "LedgerTransactionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerEntries",
                table: "LedgerEntries",
                column: "LedgerEntryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerAccounts",
                table: "LedgerAccounts",
                column: "LedgerAccountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaseSignatories",
                table: "LeaseSignatories",
                column: "LeaseSignatoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaseDocExtractedTerms",
                table: "LeaseDocExtractedTerms",
                column: "LeaseDocExtractedTermID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Leases",
                table: "Leases",
                column: "LeaseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceMasters",
                table: "InvoiceMasters",
                column: "InvoiceMasterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetails",
                table: "InvoiceDetails",
                column: "InvoiceDetailID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionItems",
                table: "InspectionItems",
                column: "InspectionItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inspections",
                table: "Inspections",
                column: "InspectionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityVerifications",
                table: "IdentityVerifications",
                column: "IdentityVerificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FraudCases",
                table: "FraudCases",
                column: "FraudCaseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeeTypes",
                table: "FeeTypes",
                column: "FeeTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fees",
                table: "Fees",
                column: "FeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Disputes",
                table: "Disputes",
                column: "DisputeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReportingEnrollments",
                table: "CreditReportingEnrollments",
                column: "CreditReportingEnrollmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReportingConsentAudits",
                table: "CreditReportingConsentAudits",
                column: "ConsentAuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReportings",
                table: "CreditReportings",
                column: "CreditReportingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants",
                column: "ConversationParticipantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMessages",
                table: "ConversationMessages",
                column: "ConversationMessageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "ConversationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contractors",
                table: "Contractors",
                column: "ContractorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChargeBacks",
                table: "ChargeBacks",
                column: "ChargebackID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents",
                column: "CalendarEventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutopayMandates",
                table: "AutopayMandates",
                column: "AutopayMandateID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutopayConsentAudits",
                table: "AutopayConsentAudits",
                column: "AutopayConsentAuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "AuditLogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "AttachmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationOccupants",
                table: "ApplicationOccupants",
                column: "ApplicationOccupantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AmenityCatalogs",
                table: "AmenityCatalogs",
                column: "AmenityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Organizations_OrganizationID",
                table: "Addresses",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationOccupants_AspNetUsers_UserID",
                table: "ApplicationOccupants",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationOccupants_Tenants_TenantID",
                table: "ApplicationOccupants",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ActorUserID",
                table: "AuditLogs",
                column: "ActorUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_OrganizationMembers_OrganizationMemberID",
                table: "AuditLogs",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Organizations_OrganizationID",
                table: "AuditLogs",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Tenants_TenantID",
                table: "AuditLogs",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayConsentAudits_AutopayMandates_AutopayMandateID",
                table: "AutopayConsentAudits",
                column: "AutopayMandateID",
                principalTable: "AutopayMandates",
                principalColumn: "AutopayMandateID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayConsentAudits_Tenants_TenantID",
                table: "AutopayConsentAudits",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandates_Leases_LeaseID",
                table: "AutopayMandates",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandates_PaymentMethods_PaymentMethodID",
                table: "AutopayMandates",
                column: "PaymentMethodID",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandates_Tenants_TenantID",
                table: "AutopayMandates",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Leases_LeaseID",
                table: "CalendarEvents",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_Listings_ListingID",
                table: "CalendarEvents",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_MaintenanceRequests_MaintenanceRequestID",
                table: "CalendarEvents",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequests",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_RentalApplications_RentalApplicationID",
                table: "CalendarEvents",
                column: "RentalApplicationID",
                principalTable: "RentalApplications",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvents_ReservationHolds_ReservationHoldID",
                table: "CalendarEvents",
                column: "ReservationHoldID",
                principalTable: "ReservationHolds",
                principalColumn: "ReservationHoldID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargeBacks_Organizations_OrganizationID",
                table: "ChargeBacks",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargeBacks_Payments_PaymentID",
                table: "ChargeBacks",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChargeBacks_Tenants_TenantID",
                table: "ChargeBacks",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contractors_Organizations_OrganizationID",
                table: "Contractors",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_AspNetUsers_SenderUserID",
                table: "ConversationMessages",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_ConversationMessages_ReplyToMessageID",
                table: "ConversationMessages",
                column: "ReplyToMessageID",
                principalTable: "ConversationMessages",
                principalColumn: "ConversationMessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_Conversations_ConversationID",
                table: "ConversationMessages",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_OrganizationMembers_SenderOrganizationMemberID",
                table: "ConversationMessages",
                column: "SenderOrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessages_Tenants_SenderTenantID",
                table: "ConversationMessages",
                column: "SenderTenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_AspNetUsers_UserID",
                table: "ConversationParticipants",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationID",
                table: "ConversationParticipants",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_OrganizationMembers_OrganizationMemberID",
                table: "ConversationParticipants",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Tenants_TenantID",
                table: "ConversationParticipants",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Disputes_DisputeID",
                table: "Conversations",
                column: "DisputeID",
                principalTable: "Disputes",
                principalColumn: "DisputeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Leases_LeaseID",
                table: "Conversations",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_MaintenanceRequests_MaintenanceRequestID",
                table: "Conversations",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequests",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingConsentAudits_CreditReportingEnrollments_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudits",
                column: "CreditReportingEnrollmentID",
                principalTable: "CreditReportingEnrollments",
                principalColumn: "CreditReportingEnrollmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingConsentAudits_Tenants_TenantID",
                table: "CreditReportingConsentAudits",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingEnrollments_Leases_LeaseID",
                table: "CreditReportingEnrollments",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingEnrollments_Tenants_TenantID",
                table: "CreditReportingEnrollments",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportings_CreditReportingEnrollments_CreditReportingEnrollmentID",
                table: "CreditReportings",
                column: "CreditReportingEnrollmentID",
                principalTable: "CreditReportingEnrollments",
                principalColumn: "CreditReportingEnrollmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportings_InvoiceMasters_InvoiceMasterID",
                table: "CreditReportings",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportings_Payments_PaymentID",
                table: "CreditReportings",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_ChargeBacks_ChargebackID",
                table: "Disputes",
                column: "ChargebackID",
                principalTable: "ChargeBacks",
                principalColumn: "ChargebackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_InvoiceMasters_InvoiceMasterID",
                table: "Disputes",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Leases_LeaseID",
                table: "Disputes",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_MaintenanceRequests_MaintenanceRequestID",
                table: "Disputes",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequests",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Organizations_OrganizationID",
                table: "Disputes",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Payments_PaymentID",
                table: "Disputes",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Disputes_Tenants_TenantID",
                table: "Disputes",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_FeeTypes_FeeTypeID",
                table: "Fees",
                column: "FeeTypeID",
                principalTable: "FeeTypes",
                principalColumn: "FeeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_Organizations_OrganizationID",
                table: "Fees",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_ChargeBacks_ChargebackID",
                table: "FraudCases",
                column: "ChargebackID",
                principalTable: "ChargeBacks",
                principalColumn: "ChargebackID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_Leases_LeaseID",
                table: "FraudCases",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_Organizations_OrganizationID",
                table: "FraudCases",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_PaymentIntents_PaymentIntentID",
                table: "FraudCases",
                column: "PaymentIntentID",
                principalTable: "PaymentIntents",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_Payments_PaymentID",
                table: "FraudCases",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCases_Tenants_TenantID",
                table: "FraudCases",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityVerifications_AspNetUsers_UserID",
                table: "IdentityVerifications",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionItems_Inspections_InspectionID",
                table: "InspectionItems",
                column: "InspectionID",
                principalTable: "Inspections",
                principalColumn: "InspectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Leases_LeaseID",
                table: "Inspections",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Properties_PropertyID",
                table: "Inspections",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_RentalUnits_RentalUnitID",
                table: "Inspections",
                column: "RentalUnitID",
                principalTable: "RentalUnits",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_Fees_FeeID",
                table: "InvoiceDetails",
                column: "FeeID",
                principalTable: "Fees",
                principalColumn: "FeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetails_InvoiceMasters_InvoiceMasterID",
                table: "InvoiceDetails",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMasters_Leases_LeaseID",
                table: "InvoiceMasters",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMasters_Leases_LeaseRenewalID",
                table: "InvoiceMasters",
                column: "LeaseRenewalID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMasters_Organizations_OrganizationID",
                table: "InvoiceMasters",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMasters_Tenants_TenantID",
                table: "InvoiceMasters",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseDocuments_Leases_LeaseID",
                table: "LeaseDocuments",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseOccupants_Leases_LeaseID",
                table: "LeaseOccupants",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseOccupants_Tenants_TenantID",
                table: "LeaseOccupants",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRecurringCharges_Fees_FeeID",
                table: "LeaseRecurringCharges",
                column: "FeeID",
                principalTable: "Fees",
                principalColumn: "FeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRecurringCharges_Leases_LeaseID",
                table: "LeaseRecurringCharges",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRenewals_Leases_LeaseID",
                table: "LeaseRenewals",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Listings_ListingID",
                table: "Leases",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Organizations_OrganizationID",
                table: "Leases",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_RentalApplications_RentalApplicationID",
                table: "Leases",
                column: "RentalApplicationID",
                principalTable: "RentalApplications",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_RentalUnits_RentalUnitID",
                table: "Leases",
                column: "RentalUnitID",
                principalTable: "RentalUnits",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_TenancyTypes_TenancyTypeID",
                table: "Leases",
                column: "TenancyTypeID",
                principalTable: "TenancyTypes",
                principalColumn: "TenancyTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Tenants_TenantID",
                table: "Leases",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatories_AspNetUsers_UserID",
                table: "LeaseSignatories",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatories_OrganizationMembers_OrganizationMemberID",
                table: "LeaseSignatories",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatories_Organizations_OrganizationID",
                table: "LeaseSignatories",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatories_Tenants_TenantID",
                table: "LeaseSignatories",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerAccounts_Organizations_OrganizationID",
                table: "LedgerAccounts",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntries_LedgerAccounts_LedgerAccountID",
                table: "LedgerEntries",
                column: "LedgerAccountID",
                principalTable: "LedgerAccounts",
                principalColumn: "LedgerAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntries_LedgerTransactions_LedgerTransactionID",
                table: "LedgerEntries",
                column: "LedgerTransactionID",
                principalTable: "LedgerTransactions",
                principalColumn: "LedgerTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_InvoiceMasters_InvoiceMasterID",
                table: "LedgerTransactions",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Organizations_OrganizationID",
                table: "LedgerTransactions",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Payments_PaymentID",
                table: "LedgerTransactions",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Payouts_PayoutID",
                table: "LedgerTransactions",
                column: "PayoutID",
                principalTable: "Payouts",
                principalColumn: "PayoutID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransactions_Refunds_RefundID",
                table: "LedgerTransactions",
                column: "RefundID",
                principalTable: "Refunds",
                principalColumn: "RefundID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAccessInstructions_Leases_LeaseID",
                table: "ListingAccessInstructions",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAccessInstructions_Listings_ListingID",
                table: "ListingAccessInstructions",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAmenities_AmenityCatalogs_AmenityID",
                table: "ListingAmenities",
                column: "AmenityID",
                principalTable: "AmenityCatalogs",
                principalColumn: "AmenityID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAmenities_Listings_ListingID",
                table: "ListingAmenities",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingPhotos_Listings_ListingID",
                table: "ListingPhotos",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingPolicies_Listings_ListingID",
                table: "ListingPolicies",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingRules_Listings_ListingID",
                table: "ListingRules",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_ListingTypes_ListingTypeID",
                table: "Listings",
                column: "ListingTypeID",
                principalTable: "ListingTypes",
                principalColumn: "ListingTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_Organizations_OrganizationID",
                table: "Listings",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_RentalUnits_RentalUnitID",
                table: "Listings",
                column: "RentalUnitID",
                principalTable: "RentalUnits",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingTermPrices_Listings_ListingID",
                table: "ListingTermPrices",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Categories_CategoryID",
                table: "MaintenanceRequests",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Leases_LeaseID",
                table: "MaintenanceRequests",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Listings_ListingID",
                table: "MaintenanceRequests",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Properties_PropertyID",
                table: "MaintenanceRequests",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_RentalUnits_RentalUnitID",
                table: "MaintenanceRequests",
                column: "RentalUnitID",
                principalTable: "RentalUnits",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_Tenants_SubmittedByTenantID",
                table: "MaintenanceRequests",
                column: "SubmittedByTenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_RecipientUserID",
                table: "Notifications",
                column: "RecipientUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_OrganizationMembers_OrganizationMemberID",
                table: "Notifications",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Organizations_OrganizationID",
                table: "Notifications",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Tenants_TenantID",
                table: "Notifications",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_UserID",
                table: "OrganizationMembers",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationID",
                table: "OrganizationMembers",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStatements_Organizations_OrganizationID",
                table: "OrganizationStatements",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgPayoutAccounts_Organizations_OrganizationID",
                table: "OrgPayoutAccounts",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgSubscriptions_Organizations_OrganizationID",
                table: "OrgSubscriptions",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgSubscriptions_SubscriptionPlans_SubscriptionPlanID",
                table: "OrgSubscriptions",
                column: "SubscriptionPlanID",
                principalTable: "SubscriptionPlans",
                principalColumn: "SubscriptionPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocations_InvoiceDetails_InvoiceDetailID",
                table: "PaymentAllocations",
                column: "InvoiceDetailID",
                principalTable: "InvoiceDetails",
                principalColumn: "InvoiceDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocations_InvoiceMasters_InvoiceMasterID",
                table: "PaymentAllocations",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocations_Payments_PaymentID",
                table: "PaymentAllocations",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAttempts_PaymentIntents_PaymentIntentID",
                table: "PaymentAttempts",
                column: "PaymentIntentID",
                principalTable: "PaymentIntents",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_AutopayMandates_AutopayMandateID",
                table: "PaymentIntents",
                column: "AutopayMandateID",
                principalTable: "AutopayMandates",
                principalColumn: "AutopayMandateID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_InvoiceMasters_InvoiceMasterID",
                table: "PaymentIntents",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_Leases_LeaseID",
                table: "PaymentIntents",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_PaymentMethods_PaymentMethodID",
                table: "PaymentIntents",
                column: "PaymentMethodID",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntents_Tenants_TenantID",
                table: "PaymentIntents",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Tenants_TenantID",
                table: "PaymentMethods",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReminders_InvoiceMasters_InvoiceMasterID",
                table: "PaymentReminders",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentID",
                table: "Payments",
                column: "PaymentIntentID",
                principalTable: "PaymentIntents",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Tenants_TenantID",
                table: "Payments",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayoutItems_Payments_PaymentID",
                table: "PayoutItems",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayoutItems_Payouts_PayoutID",
                table: "PayoutItems",
                column: "PayoutID",
                principalTable: "Payouts",
                principalColumn: "PayoutID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payouts_OrgPayoutAccounts_OrgPayoutAccountID",
                table: "Payouts",
                column: "OrgPayoutAccountID",
                principalTable: "OrgPayoutAccounts",
                principalColumn: "OrgPayoutAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payouts_Organizations_OrganizationID",
                table: "Payouts",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Addresses_AddressID",
                table: "Properties",
                column: "AddressID",
                principalTable: "Addresses",
                principalColumn: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Organizations_OrganizationID",
                table: "Properties",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_ReviewerUserID",
                table: "Ratings",
                column: "ReviewerUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Leases_LeaseID",
                table: "Ratings",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptMasters_Payments_PaymentID",
                table: "ReceiptMasters",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptMasters_Tenants_TenantID",
                table: "ReceiptMasters",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentID",
                table: "Refunds",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Tenants_TenantID",
                table: "Refunds",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplications_Listings_ListingID",
                table: "RentalApplications",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplications_OrganizationMembers_ReviewedByOrganizationMemberID",
                table: "RentalApplications",
                column: "ReviewedByOrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplications_Organizations_OrganizationID",
                table: "RentalApplications",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplications_Tenants_TenantID",
                table: "RentalApplications",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalUnits_Properties_PropertyID",
                table: "RentalUnits",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalUnits_UnitTypes_UnitTypeID",
                table: "RentalUnits",
                column: "UnitTypeID",
                principalTable: "UnitTypes",
                principalColumn: "UnitTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHolds_Listings_ListingID",
                table: "ReservationHolds",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHolds_RentalApplications_RentalApplicationID",
                table: "ReservationHolds",
                column: "RentalApplicationID",
                principalTable: "RentalApplications",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHolds_Tenants_TenantID",
                table: "ReservationHolds",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposits_Leases_LeaseID",
                table: "SecurityDeposits",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposits_Organizations_OrganizationID",
                table: "SecurityDeposits",
                column: "OrganizationID",
                principalTable: "Organizations",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposits_Tenants_TenantID",
                table: "SecurityDeposits",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransactions_InvoiceDetails_InvoiceDetailID",
                table: "SecurityDepositTransactions",
                column: "InvoiceDetailID",
                principalTable: "InvoiceDetails",
                principalColumn: "InvoiceDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransactions_InvoiceMasters_InvoiceMasterID",
                table: "SecurityDepositTransactions",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMasters",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransactions_Payments_PaymentID",
                table: "SecurityDepositTransactions",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransactions_Refunds_RefundID",
                table: "SecurityDepositTransactions",
                column: "RefundID",
                principalTable: "Refunds",
                principalColumn: "RefundID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransactions_SecurityDeposits_SecurityDepositID",
                table: "SecurityDepositTransactions",
                column: "SecurityDepositID",
                principalTable: "SecurityDeposits",
                principalColumn: "SecurityDepositID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantEmergencyContacts_Tenants_TenantID",
                table: "TenantEmergencyContacts",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantEmployments_Tenants_TenantID",
                table: "TenantEmployments",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantGuarantors_AspNetUsers_UserID",
                table: "TenantGuarantors",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantGuarantors_Tenants_TenantID",
                table: "TenantGuarantors",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantInvitations_Leases_LeaseID",
                table: "TenantInvitations",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantInvitations_RentalApplications_RentalApplicationID",
                table: "TenantInvitations",
                column: "RentalApplicationID",
                principalTable: "RentalApplications",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_AspNetUsers_UserID",
                table: "Tenants",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantScreeningChecks_Tenants_TenantID",
                table: "TenantScreeningChecks",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_Listings_ListingID",
                table: "ViewingAppointments",
                column: "ListingID",
                principalTable: "Listings",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_OrganizationMembers_AssignedOrganizationMemberID",
                table: "ViewingAppointments",
                column: "AssignedOrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_Tenants_TenantID",
                table: "ViewingAppointments",
                column: "TenantID",
                principalTable: "Tenants",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Contractors_ContractorID",
                table: "WorkOrders",
                column: "ContractorID",
                principalTable: "Contractors",
                principalColumn: "ContractorID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Leases_LeaseID",
                table: "WorkOrders",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MaintenanceRequests_MaintenanceRequestID",
                table: "WorkOrders",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequests",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_OrganizationMembers_AssignedOrganizationMemberID",
                table: "WorkOrders",
                column: "AssignedOrganizationMemberID",
                principalTable: "OrganizationMembers",
                principalColumn: "OrganizationMemberID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Organizations_OrganizationID",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationOccupants_AspNetUsers_UserID",
                table: "ApplicationOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationOccupants_Tenants_TenantID",
                table: "ApplicationOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_ActorUserID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_OrganizationMembers_OrganizationMemberID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Organizations_OrganizationID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Tenants_TenantID",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayConsentAudits_AutopayMandates_AutopayMandateID",
                table: "AutopayConsentAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayConsentAudits_Tenants_TenantID",
                table: "AutopayConsentAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandates_Leases_LeaseID",
                table: "AutopayMandates");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandates_PaymentMethods_PaymentMethodID",
                table: "AutopayMandates");

            migrationBuilder.DropForeignKey(
                name: "FK_AutopayMandates_Tenants_TenantID",
                table: "AutopayMandates");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_Leases_LeaseID",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_Listings_ListingID",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_MaintenanceRequests_MaintenanceRequestID",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_RentalApplications_RentalApplicationID",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarEvents_ReservationHolds_ReservationHoldID",
                table: "CalendarEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargeBacks_Organizations_OrganizationID",
                table: "ChargeBacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargeBacks_Payments_PaymentID",
                table: "ChargeBacks");

            migrationBuilder.DropForeignKey(
                name: "FK_ChargeBacks_Tenants_TenantID",
                table: "ChargeBacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Contractors_Organizations_OrganizationID",
                table: "Contractors");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_AspNetUsers_SenderUserID",
                table: "ConversationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_ConversationMessages_ReplyToMessageID",
                table: "ConversationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_Conversations_ConversationID",
                table: "ConversationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_OrganizationMembers_SenderOrganizationMemberID",
                table: "ConversationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMessages_Tenants_SenderTenantID",
                table: "ConversationMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_AspNetUsers_UserID",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Conversations_ConversationID",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_OrganizationMembers_OrganizationMemberID",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Tenants_TenantID",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Disputes_DisputeID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Leases_LeaseID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_MaintenanceRequests_MaintenanceRequestID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingConsentAudits_CreditReportingEnrollments_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingConsentAudits_Tenants_TenantID",
                table: "CreditReportingConsentAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingEnrollments_Leases_LeaseID",
                table: "CreditReportingEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportingEnrollments_Tenants_TenantID",
                table: "CreditReportingEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportings_CreditReportingEnrollments_CreditReportingEnrollmentID",
                table: "CreditReportings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportings_InvoiceMasters_InvoiceMasterID",
                table: "CreditReportings");

            migrationBuilder.DropForeignKey(
                name: "FK_CreditReportings_Payments_PaymentID",
                table: "CreditReportings");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_ChargeBacks_ChargebackID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_InvoiceMasters_InvoiceMasterID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Leases_LeaseID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_MaintenanceRequests_MaintenanceRequestID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Organizations_OrganizationID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Payments_PaymentID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Disputes_Tenants_TenantID",
                table: "Disputes");

            migrationBuilder.DropForeignKey(
                name: "FK_Fees_FeeTypes_FeeTypeID",
                table: "Fees");

            migrationBuilder.DropForeignKey(
                name: "FK_Fees_Organizations_OrganizationID",
                table: "Fees");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_ChargeBacks_ChargebackID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_Leases_LeaseID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_Organizations_OrganizationID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_PaymentIntents_PaymentIntentID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_Payments_PaymentID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_FraudCases_Tenants_TenantID",
                table: "FraudCases");

            migrationBuilder.DropForeignKey(
                name: "FK_IdentityVerifications_AspNetUsers_UserID",
                table: "IdentityVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_InspectionItems_Inspections_InspectionID",
                table: "InspectionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Leases_LeaseID",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Properties_PropertyID",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_RentalUnits_RentalUnitID",
                table: "Inspections");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_Fees_FeeID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetails_InvoiceMasters_InvoiceMasterID",
                table: "InvoiceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMasters_Leases_LeaseID",
                table: "InvoiceMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMasters_Leases_LeaseRenewalID",
                table: "InvoiceMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMasters_Organizations_OrganizationID",
                table: "InvoiceMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceMasters_Tenants_TenantID",
                table: "InvoiceMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseDocuments_Leases_LeaseID",
                table: "LeaseDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseOccupants_Leases_LeaseID",
                table: "LeaseOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseOccupants_Tenants_TenantID",
                table: "LeaseOccupants");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRecurringCharges_Fees_FeeID",
                table: "LeaseRecurringCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRecurringCharges_Leases_LeaseID",
                table: "LeaseRecurringCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseRenewals_Leases_LeaseID",
                table: "LeaseRenewals");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Listings_ListingID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Organizations_OrganizationID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_RentalApplications_RentalApplicationID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_RentalUnits_RentalUnitID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_TenancyTypes_TenancyTypeID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Tenants_TenantID",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatories_AspNetUsers_UserID",
                table: "LeaseSignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatories_OrganizationMembers_OrganizationMemberID",
                table: "LeaseSignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatories_Organizations_OrganizationID",
                table: "LeaseSignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaseSignatories_Tenants_TenantID",
                table: "LeaseSignatories");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerAccounts_Organizations_OrganizationID",
                table: "LedgerAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntries_LedgerAccounts_LedgerAccountID",
                table: "LedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerEntries_LedgerTransactions_LedgerTransactionID",
                table: "LedgerEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_InvoiceMasters_InvoiceMasterID",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Organizations_OrganizationID",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Payments_PaymentID",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Payouts_PayoutID",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LedgerTransactions_Refunds_RefundID",
                table: "LedgerTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAccessInstructions_Leases_LeaseID",
                table: "ListingAccessInstructions");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAccessInstructions_Listings_ListingID",
                table: "ListingAccessInstructions");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAmenities_AmenityCatalogs_AmenityID",
                table: "ListingAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingAmenities_Listings_ListingID",
                table: "ListingAmenities");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingPhotos_Listings_ListingID",
                table: "ListingPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingPolicies_Listings_ListingID",
                table: "ListingPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingRules_Listings_ListingID",
                table: "ListingRules");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_ListingTypes_ListingTypeID",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_Organizations_OrganizationID",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_Listings_RentalUnits_RentalUnitID",
                table: "Listings");

            migrationBuilder.DropForeignKey(
                name: "FK_ListingTermPrices_Listings_ListingID",
                table: "ListingTermPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Categories_CategoryID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Leases_LeaseID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Listings_ListingID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Properties_PropertyID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_RentalUnits_RentalUnitID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_Tenants_SubmittedByTenantID",
                table: "MaintenanceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_RecipientUserID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_OrganizationMembers_OrganizationMemberID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Organizations_OrganizationID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Tenants_TenantID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_AspNetUsers_UserID",
                table: "OrganizationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Organizations_OrganizationID",
                table: "OrganizationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationStatements_Organizations_OrganizationID",
                table: "OrganizationStatements");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgPayoutAccounts_Organizations_OrganizationID",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgSubscriptions_Organizations_OrganizationID",
                table: "OrgSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgSubscriptions_SubscriptionPlans_SubscriptionPlanID",
                table: "OrgSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocations_InvoiceDetails_InvoiceDetailID",
                table: "PaymentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocations_InvoiceMasters_InvoiceMasterID",
                table: "PaymentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAllocations_Payments_PaymentID",
                table: "PaymentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentAttempts_PaymentIntents_PaymentIntentID",
                table: "PaymentAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_AutopayMandates_AutopayMandateID",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_InvoiceMasters_InvoiceMasterID",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_Leases_LeaseID",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_PaymentMethods_PaymentMethodID",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentIntents_Tenants_TenantID",
                table: "PaymentIntents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Tenants_TenantID",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentReminders_InvoiceMasters_InvoiceMasterID",
                table: "PaymentReminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentIntents_PaymentIntentID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Tenants_TenantID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PayoutItems_Payments_PaymentID",
                table: "PayoutItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PayoutItems_Payouts_PayoutID",
                table: "PayoutItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Payouts_OrgPayoutAccounts_OrgPayoutAccountID",
                table: "Payouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payouts_Organizations_OrganizationID",
                table: "Payouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Addresses_AddressID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Organizations_OrganizationID",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_ReviewerUserID",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Leases_LeaseID",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptMasters_Payments_PaymentID",
                table: "ReceiptMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptMasters_Tenants_TenantID",
                table: "ReceiptMasters");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentID",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Tenants_TenantID",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplications_Listings_ListingID",
                table: "RentalApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplications_OrganizationMembers_ReviewedByOrganizationMemberID",
                table: "RentalApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplications_Organizations_OrganizationID",
                table: "RentalApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalApplications_Tenants_TenantID",
                table: "RentalApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalUnits_Properties_PropertyID",
                table: "RentalUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalUnits_UnitTypes_UnitTypeID",
                table: "RentalUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHolds_Listings_ListingID",
                table: "ReservationHolds");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHolds_RentalApplications_RentalApplicationID",
                table: "ReservationHolds");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationHolds_Tenants_TenantID",
                table: "ReservationHolds");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposits_Leases_LeaseID",
                table: "SecurityDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposits_Organizations_OrganizationID",
                table: "SecurityDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDeposits_Tenants_TenantID",
                table: "SecurityDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransactions_InvoiceDetails_InvoiceDetailID",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransactions_InvoiceMasters_InvoiceMasterID",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransactions_Payments_PaymentID",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransactions_Refunds_RefundID",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityDepositTransactions_SecurityDeposits_SecurityDepositID",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantEmergencyContacts_Tenants_TenantID",
                table: "TenantEmergencyContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantEmployments_Tenants_TenantID",
                table: "TenantEmployments");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantGuarantors_AspNetUsers_UserID",
                table: "TenantGuarantors");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantGuarantors_Tenants_TenantID",
                table: "TenantGuarantors");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantInvitations_Leases_LeaseID",
                table: "TenantInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantInvitations_RentalApplications_RentalApplicationID",
                table: "TenantInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_AspNetUsers_UserID",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantScreeningChecks_Tenants_TenantID",
                table: "TenantScreeningChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_Listings_ListingID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_OrganizationMembers_AssignedOrganizationMemberID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_ViewingAppointments_Tenants_TenantID",
                table: "ViewingAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Contractors_ContractorID",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Leases_LeaseID",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MaintenanceRequests_MaintenanceRequestID",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_OrganizationMembers_AssignedOrganizationMemberID",
                table: "WorkOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkOrders",
                table: "WorkOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnitTypes",
                table: "UnitTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantScreeningChecks",
                table: "TenantScreeningChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantInvitations",
                table: "TenantInvitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantGuarantors",
                table: "TenantGuarantors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantEmployments",
                table: "TenantEmployments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantEmergencyContacts",
                table: "TenantEmergencyContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenancyTypes",
                table: "TenancyTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaxRates",
                table: "TaxRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionPlans",
                table: "SubscriptionPlans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityDepositTransactions",
                table: "SecurityDepositTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityDeposits",
                table: "SecurityDeposits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationHolds",
                table: "ReservationHolds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentalUnits",
                table: "RentalUnits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RentalApplications",
                table: "RentalApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptMasters",
                table: "ReceiptMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Preferences",
                table: "Preferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payouts",
                table: "Payouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PayoutItems",
                table: "PayoutItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentReminders",
                table: "PaymentReminders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentProviderEvents",
                table: "PaymentProviderEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentMethods",
                table: "PaymentMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentIntents",
                table: "PaymentIntents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentAttempts",
                table: "PaymentAttempts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaymentAllocations",
                table: "PaymentAllocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrgSubscriptions",
                table: "OrgSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrgPayoutAccounts",
                table: "OrgPayoutAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationStatements",
                table: "OrganizationStatements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organizations",
                table: "Organizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationMembers",
                table: "OrganizationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceRequests",
                table: "MaintenanceRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingTypes",
                table: "ListingTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingTermPrices",
                table: "ListingTermPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Listings",
                table: "Listings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingRules",
                table: "ListingRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingPolicies",
                table: "ListingPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingPhotos",
                table: "ListingPhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingAmenities",
                table: "ListingAmenities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListingAccessInstructions",
                table: "ListingAccessInstructions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerTransactions",
                table: "LedgerTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerEntries",
                table: "LedgerEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LedgerAccounts",
                table: "LedgerAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaseSignatories",
                table: "LeaseSignatories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Leases",
                table: "Leases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaseDocExtractedTerms",
                table: "LeaseDocExtractedTerms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceMasters",
                table: "InvoiceMasters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceDetails",
                table: "InvoiceDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inspections",
                table: "Inspections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InspectionItems",
                table: "InspectionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityVerifications",
                table: "IdentityVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FraudCases",
                table: "FraudCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeeTypes",
                table: "FeeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fees",
                table: "Fees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Disputes",
                table: "Disputes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReportings",
                table: "CreditReportings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReportingEnrollments",
                table: "CreditReportingEnrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreditReportingConsentAudits",
                table: "CreditReportingConsentAudits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationParticipants",
                table: "ConversationParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMessages",
                table: "ConversationMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contractors",
                table: "Contractors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChargeBacks",
                table: "ChargeBacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CalendarEvents",
                table: "CalendarEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutopayMandates",
                table: "AutopayMandates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutopayConsentAudits",
                table: "AutopayConsentAudits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationOccupants",
                table: "ApplicationOccupants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AmenityCatalogs",
                table: "AmenityCatalogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "WorkOrders",
                newName: "WorkOrder");

            migrationBuilder.RenameTable(
                name: "UnitTypes",
                newName: "UnitType");

            migrationBuilder.RenameTable(
                name: "TenantScreeningChecks",
                newName: "TenantScreeningCheck");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "Tenant");

            migrationBuilder.RenameTable(
                name: "TenantInvitations",
                newName: "TenantInvitation");

            migrationBuilder.RenameTable(
                name: "TenantGuarantors",
                newName: "TenantGuarantor");

            migrationBuilder.RenameTable(
                name: "TenantEmployments",
                newName: "TenantEmployment");

            migrationBuilder.RenameTable(
                name: "TenantEmergencyContacts",
                newName: "TenantEmergencyContact");

            migrationBuilder.RenameTable(
                name: "TenancyTypes",
                newName: "TenancyType");

            migrationBuilder.RenameTable(
                name: "TaxRates",
                newName: "TaxRate");

            migrationBuilder.RenameTable(
                name: "SubscriptionPlans",
                newName: "SubscriptionPlan");

            migrationBuilder.RenameTable(
                name: "SecurityDepositTransactions",
                newName: "SecurityDepositTransaction");

            migrationBuilder.RenameTable(
                name: "SecurityDeposits",
                newName: "SecurityDeposit");

            migrationBuilder.RenameTable(
                name: "ReservationHolds",
                newName: "ReservationHold");

            migrationBuilder.RenameTable(
                name: "RentalUnits",
                newName: "RentalUnit");

            migrationBuilder.RenameTable(
                name: "RentalApplications",
                newName: "RentalApplication");

            migrationBuilder.RenameTable(
                name: "Refunds",
                newName: "Refund");

            migrationBuilder.RenameTable(
                name: "ReceiptMasters",
                newName: "ReceiptMaster");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameTable(
                name: "Properties",
                newName: "Property");

            migrationBuilder.RenameTable(
                name: "Preferences",
                newName: "Preference");

            migrationBuilder.RenameTable(
                name: "Payouts",
                newName: "Payout");

            migrationBuilder.RenameTable(
                name: "PayoutItems",
                newName: "PayoutItem");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameTable(
                name: "PaymentReminders",
                newName: "PaymentReminder");

            migrationBuilder.RenameTable(
                name: "PaymentProviderEvents",
                newName: "PaymentProviderEvent");

            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "PaymentMethod");

            migrationBuilder.RenameTable(
                name: "PaymentIntents",
                newName: "PaymentIntent");

            migrationBuilder.RenameTable(
                name: "PaymentAttempts",
                newName: "PaymentAttempt");

            migrationBuilder.RenameTable(
                name: "PaymentAllocations",
                newName: "PaymentAllocation");

            migrationBuilder.RenameTable(
                name: "OrgSubscriptions",
                newName: "OrgSubscription");

            migrationBuilder.RenameTable(
                name: "OrgPayoutAccounts",
                newName: "OrgPayoutAccount");

            migrationBuilder.RenameTable(
                name: "OrganizationStatements",
                newName: "OrganizationStatement");

            migrationBuilder.RenameTable(
                name: "Organizations",
                newName: "Organization");

            migrationBuilder.RenameTable(
                name: "OrganizationMembers",
                newName: "OrganizationMember");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "MaintenanceRequests",
                newName: "MaintenanceRequest");

            migrationBuilder.RenameTable(
                name: "ListingTypes",
                newName: "ListingType");

            migrationBuilder.RenameTable(
                name: "ListingTermPrices",
                newName: "ListingTermPrice");

            migrationBuilder.RenameTable(
                name: "Listings",
                newName: "Listing");

            migrationBuilder.RenameTable(
                name: "ListingRules",
                newName: "ListingRule");

            migrationBuilder.RenameTable(
                name: "ListingPolicies",
                newName: "ListingPolicy");

            migrationBuilder.RenameTable(
                name: "ListingPhotos",
                newName: "ListingPhoto");

            migrationBuilder.RenameTable(
                name: "ListingAmenities",
                newName: "ListingAmenity");

            migrationBuilder.RenameTable(
                name: "ListingAccessInstructions",
                newName: "ListingAccessInstruction");

            migrationBuilder.RenameTable(
                name: "LedgerTransactions",
                newName: "LedgerTransaction");

            migrationBuilder.RenameTable(
                name: "LedgerEntries",
                newName: "LedgerEntry");

            migrationBuilder.RenameTable(
                name: "LedgerAccounts",
                newName: "LedgerAccount");

            migrationBuilder.RenameTable(
                name: "LeaseSignatories",
                newName: "LeaseSignatory");

            migrationBuilder.RenameTable(
                name: "Leases",
                newName: "Lease");

            migrationBuilder.RenameTable(
                name: "LeaseDocExtractedTerms",
                newName: "LeaseDocExtractedTerm");

            migrationBuilder.RenameTable(
                name: "InvoiceMasters",
                newName: "InvoiceMaster");

            migrationBuilder.RenameTable(
                name: "InvoiceDetails",
                newName: "InvoiceDetail");

            migrationBuilder.RenameTable(
                name: "Inspections",
                newName: "Inspection");

            migrationBuilder.RenameTable(
                name: "InspectionItems",
                newName: "InspectionItem");

            migrationBuilder.RenameTable(
                name: "IdentityVerifications",
                newName: "IdentityVerification");

            migrationBuilder.RenameTable(
                name: "FraudCases",
                newName: "FraudCase");

            migrationBuilder.RenameTable(
                name: "FeeTypes",
                newName: "FeeType");

            migrationBuilder.RenameTable(
                name: "Fees",
                newName: "Fee");

            migrationBuilder.RenameTable(
                name: "Disputes",
                newName: "Dispute");

            migrationBuilder.RenameTable(
                name: "CreditReportings",
                newName: "CreditReporting");

            migrationBuilder.RenameTable(
                name: "CreditReportingEnrollments",
                newName: "CreditReportingEnrollment");

            migrationBuilder.RenameTable(
                name: "CreditReportingConsentAudits",
                newName: "CreditReportingConsentAudit");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "Conversation");

            migrationBuilder.RenameTable(
                name: "ConversationParticipants",
                newName: "ConversationParticipant");

            migrationBuilder.RenameTable(
                name: "ConversationMessages",
                newName: "ConversationMessage");

            migrationBuilder.RenameTable(
                name: "Contractors",
                newName: "Contractor");

            migrationBuilder.RenameTable(
                name: "ChargeBacks",
                newName: "Chargeback");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameTable(
                name: "CalendarEvents",
                newName: "CalendarEvent");

            migrationBuilder.RenameTable(
                name: "AutopayMandates",
                newName: "AutopayMandate");

            migrationBuilder.RenameTable(
                name: "AutopayConsentAudits",
                newName: "AutopayConsentAudit");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLog");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameTable(
                name: "ApplicationOccupants",
                newName: "ApplicationOccupant");

            migrationBuilder.RenameTable(
                name: "AmenityCatalogs",
                newName: "AmenityCatalog");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_MaintenanceRequestID",
                table: "WorkOrder",
                newName: "IX_WorkOrder_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_LeaseID",
                table: "WorkOrder",
                newName: "IX_WorkOrder_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_ContractorID",
                table: "WorkOrder",
                newName: "IX_WorkOrder_ContractorID");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssignedOrganizationMemberID",
                table: "WorkOrder",
                newName: "IX_WorkOrder_AssignedOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantScreeningChecks_TenantID",
                table: "TenantScreeningCheck",
                newName: "IX_TenantScreeningCheck_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_UserID",
                table: "Tenant",
                newName: "IX_Tenant_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantInvitations_RentalApplicationID",
                table: "TenantInvitation",
                newName: "IX_TenantInvitation_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantInvitations_LeaseID",
                table: "TenantInvitation",
                newName: "IX_TenantInvitation_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantGuarantors_UserID",
                table: "TenantGuarantor",
                newName: "IX_TenantGuarantor_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantGuarantors_TenantID",
                table: "TenantGuarantor",
                newName: "IX_TenantGuarantor_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantEmployments_TenantID",
                table: "TenantEmployment",
                newName: "IX_TenantEmployment_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_TenantEmergencyContacts_TenantID",
                table: "TenantEmergencyContact",
                newName: "IX_TenantEmergencyContact_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransactions_SecurityDepositID",
                table: "SecurityDepositTransaction",
                newName: "IX_SecurityDepositTransaction_SecurityDepositID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransactions_RefundID",
                table: "SecurityDepositTransaction",
                newName: "IX_SecurityDepositTransaction_RefundID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransactions_PaymentID",
                table: "SecurityDepositTransaction",
                newName: "IX_SecurityDepositTransaction_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransactions_InvoiceMasterID",
                table: "SecurityDepositTransaction",
                newName: "IX_SecurityDepositTransaction_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDepositTransactions_InvoiceDetailID",
                table: "SecurityDepositTransaction",
                newName: "IX_SecurityDepositTransaction_InvoiceDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposits_TenantID",
                table: "SecurityDeposit",
                newName: "IX_SecurityDeposit_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposits_OrganizationID",
                table: "SecurityDeposit",
                newName: "IX_SecurityDeposit_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_SecurityDeposits_LeaseID",
                table: "SecurityDeposit",
                newName: "IX_SecurityDeposit_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHolds_TenantID",
                table: "ReservationHold",
                newName: "IX_ReservationHold_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHolds_RentalApplicationID",
                table: "ReservationHold",
                newName: "IX_ReservationHold_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationHolds_ListingID",
                table: "ReservationHold",
                newName: "IX_ReservationHold_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalUnits_UnitTypeID",
                table: "RentalUnit",
                newName: "IX_RentalUnit_UnitTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalUnits_PropertyID",
                table: "RentalUnit",
                newName: "IX_RentalUnit_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_TenantID",
                table: "RentalApplication",
                newName: "IX_RentalApplication_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_ReviewedByOrganizationMemberID",
                table: "RentalApplication",
                newName: "IX_RentalApplication_ReviewedByOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_OrganizationID",
                table: "RentalApplication",
                newName: "IX_RentalApplication_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_ListingID",
                table: "RentalApplication",
                newName: "IX_RentalApplication_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_TenantID",
                table: "Refund",
                newName: "IX_Refund_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_PaymentID",
                table: "Refund",
                newName: "IX_Refund_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptMasters_TenantID",
                table: "ReceiptMaster",
                newName: "IX_ReceiptMaster_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptMasters_PaymentID",
                table: "ReceiptMaster",
                newName: "IX_ReceiptMaster_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_ReviewerUserID",
                table: "Rating",
                newName: "IX_Rating_ReviewerUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_LeaseID",
                table: "Rating",
                newName: "IX_Rating_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_OrganizationID",
                table: "Property",
                newName: "IX_Property_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_AddressID",
                table: "Property",
                newName: "IX_Property_AddressID");

            migrationBuilder.RenameIndex(
                name: "IX_Payouts_OrgPayoutAccountID",
                table: "Payout",
                newName: "IX_Payout_OrgPayoutAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_Payouts_OrganizationID",
                table: "Payout",
                newName: "IX_Payout_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutItems_PayoutID",
                table: "PayoutItem",
                newName: "IX_PayoutItem_PayoutID");

            migrationBuilder.RenameIndex(
                name: "IX_PayoutItems_PaymentID",
                table: "PayoutItem",
                newName: "IX_PayoutItem_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_TenantID",
                table: "Payment",
                newName: "IX_Payment_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentIntentID",
                table: "Payment",
                newName: "IX_Payment_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentReminders_InvoiceMasterID",
                table: "PaymentReminder",
                newName: "IX_PaymentReminder_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentMethods_TenantID",
                table: "PaymentMethod",
                newName: "IX_PaymentMethod_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntents_TenantID",
                table: "PaymentIntent",
                newName: "IX_PaymentIntent_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntents_PaymentMethodID",
                table: "PaymentIntent",
                newName: "IX_PaymentIntent_PaymentMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntents_LeaseID",
                table: "PaymentIntent",
                newName: "IX_PaymentIntent_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntents_InvoiceMasterID",
                table: "PaymentIntent",
                newName: "IX_PaymentIntent_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentIntents_AutopayMandateID",
                table: "PaymentIntent",
                newName: "IX_PaymentIntent_AutopayMandateID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAttempts_PaymentIntentID",
                table: "PaymentAttempt",
                newName: "IX_PaymentAttempt_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocations_PaymentID",
                table: "PaymentAllocation",
                newName: "IX_PaymentAllocation_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocations_InvoiceMasterID",
                table: "PaymentAllocation",
                newName: "IX_PaymentAllocation_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_PaymentAllocations_InvoiceDetailID",
                table: "PaymentAllocation",
                newName: "IX_PaymentAllocation_InvoiceDetailID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgSubscriptions_SubscriptionPlanID",
                table: "OrgSubscription",
                newName: "IX_OrgSubscription_SubscriptionPlanID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgSubscriptions_OrganizationID",
                table: "OrgSubscription",
                newName: "IX_OrgSubscription_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrgPayoutAccounts_OrganizationID",
                table: "OrgPayoutAccount",
                newName: "IX_OrgPayoutAccount_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationStatements_OrganizationID",
                table: "OrganizationStatement",
                newName: "IX_OrganizationStatement_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMembers_UserID",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationMembers_OrganizationID",
                table: "OrganizationMember",
                newName: "IX_OrganizationMember_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TenantID",
                table: "Notification",
                newName: "IX_Notification_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RecipientUserID",
                table: "Notification",
                newName: "IX_Notification_RecipientUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_OrganizationMemberID",
                table: "Notification",
                newName: "IX_Notification_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_OrganizationID",
                table: "Notification",
                newName: "IX_Notification_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_SubmittedByTenantID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_SubmittedByTenantID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_RentalUnitID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_PropertyID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_ListingID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_LeaseID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceRequests_CategoryID",
                table: "MaintenanceRequest",
                newName: "IX_MaintenanceRequest_CategoryID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingTermPrices_ListingID",
                table: "ListingTermPrice",
                newName: "IX_ListingTermPrice_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_Listings_RentalUnitID",
                table: "Listing",
                newName: "IX_Listing_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Listings_OrganizationID",
                table: "Listing",
                newName: "IX_Listing_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Listings_ListingTypeID",
                table: "Listing",
                newName: "IX_Listing_ListingTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingRules_ListingID",
                table: "ListingRule",
                newName: "IX_ListingRule_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingPolicies_ListingID",
                table: "ListingPolicy",
                newName: "IX_ListingPolicy_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingPhotos_ListingID",
                table: "ListingPhoto",
                newName: "IX_ListingPhoto_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAmenities_ListingID",
                table: "ListingAmenity",
                newName: "IX_ListingAmenity_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAmenities_AmenityID",
                table: "ListingAmenity",
                newName: "IX_ListingAmenity_AmenityID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAccessInstructions_ListingID",
                table: "ListingAccessInstruction",
                newName: "IX_ListingAccessInstruction_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_ListingAccessInstructions_LeaseID",
                table: "ListingAccessInstruction",
                newName: "IX_ListingAccessInstruction_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransactions_RefundID",
                table: "LedgerTransaction",
                newName: "IX_LedgerTransaction_RefundID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransactions_PayoutID",
                table: "LedgerTransaction",
                newName: "IX_LedgerTransaction_PayoutID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransactions_PaymentID",
                table: "LedgerTransaction",
                newName: "IX_LedgerTransaction_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransactions_OrganizationID",
                table: "LedgerTransaction",
                newName: "IX_LedgerTransaction_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerTransactions_InvoiceMasterID",
                table: "LedgerTransaction",
                newName: "IX_LedgerTransaction_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntries_LedgerTransactionID",
                table: "LedgerEntry",
                newName: "IX_LedgerEntry_LedgerTransactionID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerEntries_LedgerAccountID",
                table: "LedgerEntry",
                newName: "IX_LedgerEntry_LedgerAccountID");

            migrationBuilder.RenameIndex(
                name: "IX_LedgerAccounts_OrganizationID",
                table: "LedgerAccount",
                newName: "IX_LedgerAccount_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatories_UserID",
                table: "LeaseSignatory",
                newName: "IX_LeaseSignatory_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatories_TenantID",
                table: "LeaseSignatory",
                newName: "IX_LeaseSignatory_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatories_OrganizationMemberID",
                table: "LeaseSignatory",
                newName: "IX_LeaseSignatory_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_LeaseSignatories_OrganizationID",
                table: "LeaseSignatory",
                newName: "IX_LeaseSignatory_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_TenantID",
                table: "Lease",
                newName: "IX_Lease_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_TenancyTypeID",
                table: "Lease",
                newName: "IX_Lease_TenancyTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_RentalUnitID",
                table: "Lease",
                newName: "IX_Lease_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_RentalApplicationID",
                table: "Lease",
                newName: "IX_Lease_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_OrganizationID",
                table: "Lease",
                newName: "IX_Lease_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Leases_ListingID",
                table: "Lease",
                newName: "IX_Lease_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMasters_TenantID",
                table: "InvoiceMaster",
                newName: "IX_InvoiceMaster_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMasters_OrganizationID",
                table: "InvoiceMaster",
                newName: "IX_InvoiceMaster_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMasters_LeaseRenewalID",
                table: "InvoiceMaster",
                newName: "IX_InvoiceMaster_LeaseRenewalID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceMasters_LeaseID",
                table: "InvoiceMaster",
                newName: "IX_InvoiceMaster_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetails_InvoiceMasterID",
                table: "InvoiceDetail",
                newName: "IX_InvoiceDetail_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceDetails_FeeID",
                table: "InvoiceDetail",
                newName: "IX_InvoiceDetail_FeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_RentalUnitID",
                table: "Inspection",
                newName: "IX_Inspection_RentalUnitID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_PropertyID",
                table: "Inspection",
                newName: "IX_Inspection_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_LeaseID",
                table: "Inspection",
                newName: "IX_Inspection_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_InspectionItems_InspectionID",
                table: "InspectionItem",
                newName: "IX_InspectionItem_InspectionID");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityVerifications_UserID",
                table: "IdentityVerification",
                newName: "IX_IdentityVerification_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_TenantID",
                table: "FraudCase",
                newName: "IX_FraudCase_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_PaymentIntentID",
                table: "FraudCase",
                newName: "IX_FraudCase_PaymentIntentID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_PaymentID",
                table: "FraudCase",
                newName: "IX_FraudCase_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_OrganizationID",
                table: "FraudCase",
                newName: "IX_FraudCase_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_LeaseID",
                table: "FraudCase",
                newName: "IX_FraudCase_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_FraudCases_ChargebackID",
                table: "FraudCase",
                newName: "IX_FraudCase_ChargebackID");

            migrationBuilder.RenameIndex(
                name: "IX_Fees_OrganizationID",
                table: "Fee",
                newName: "IX_Fee_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Fees_FeeTypeID",
                table: "Fee",
                newName: "IX_Fee_FeeTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_TenantID",
                table: "Dispute",
                newName: "IX_Dispute_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_PaymentID",
                table: "Dispute",
                newName: "IX_Dispute_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_OrganizationID",
                table: "Dispute",
                newName: "IX_Dispute_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_MaintenanceRequestID",
                table: "Dispute",
                newName: "IX_Dispute_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_LeaseID",
                table: "Dispute",
                newName: "IX_Dispute_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_InvoiceMasterID",
                table: "Dispute",
                newName: "IX_Dispute_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_Disputes_ChargebackID",
                table: "Dispute",
                newName: "IX_Dispute_ChargebackID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportings_PaymentID",
                table: "CreditReporting",
                newName: "IX_CreditReporting_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportings_InvoiceMasterID",
                table: "CreditReporting",
                newName: "IX_CreditReporting_InvoiceMasterID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportings_CreditReportingEnrollmentID",
                table: "CreditReporting",
                newName: "IX_CreditReporting_CreditReportingEnrollmentID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingEnrollments_TenantID",
                table: "CreditReportingEnrollment",
                newName: "IX_CreditReportingEnrollment_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingEnrollments_LeaseID",
                table: "CreditReportingEnrollment",
                newName: "IX_CreditReportingEnrollment_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingConsentAudits_TenantID",
                table: "CreditReportingConsentAudit",
                newName: "IX_CreditReportingConsentAudit_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_CreditReportingConsentAudits_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudit",
                newName: "IX_CreditReportingConsentAudit_CreditReportingEnrollmentID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_MaintenanceRequestID",
                table: "Conversation",
                newName: "IX_Conversation_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_LeaseID",
                table: "Conversation",
                newName: "IX_Conversation_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_DisputeID",
                table: "Conversation",
                newName: "IX_Conversation_DisputeID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_UserID",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_TenantID",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_OrganizationMemberID",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_ConversationID",
                table: "ConversationParticipant",
                newName: "IX_ConversationParticipant_ConversationID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessages_SenderUserID",
                table: "ConversationMessage",
                newName: "IX_ConversationMessage_SenderUserID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessages_SenderTenantID",
                table: "ConversationMessage",
                newName: "IX_ConversationMessage_SenderTenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessages_SenderOrganizationMemberID",
                table: "ConversationMessage",
                newName: "IX_ConversationMessage_SenderOrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessages_ReplyToMessageID",
                table: "ConversationMessage",
                newName: "IX_ConversationMessage_ReplyToMessageID");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMessages_ConversationID",
                table: "ConversationMessage",
                newName: "IX_ConversationMessage_ConversationID");

            migrationBuilder.RenameIndex(
                name: "IX_Contractors_OrganizationID",
                table: "Contractor",
                newName: "IX_Contractor_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_ChargeBacks_TenantID",
                table: "Chargeback",
                newName: "IX_Chargeback_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_ChargeBacks_PaymentID",
                table: "Chargeback",
                newName: "IX_Chargeback_PaymentID");

            migrationBuilder.RenameIndex(
                name: "IX_ChargeBacks_OrganizationID",
                table: "Chargeback",
                newName: "IX_Chargeback_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_ReservationHoldID",
                table: "CalendarEvent",
                newName: "IX_CalendarEvent_ReservationHoldID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_RentalApplicationID",
                table: "CalendarEvent",
                newName: "IX_CalendarEvent_RentalApplicationID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_MaintenanceRequestID",
                table: "CalendarEvent",
                newName: "IX_CalendarEvent_MaintenanceRequestID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_ListingID",
                table: "CalendarEvent",
                newName: "IX_CalendarEvent_ListingID");

            migrationBuilder.RenameIndex(
                name: "IX_CalendarEvents_LeaseID",
                table: "CalendarEvent",
                newName: "IX_CalendarEvent_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandates_TenantID",
                table: "AutopayMandate",
                newName: "IX_AutopayMandate_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandates_PaymentMethodID",
                table: "AutopayMandate",
                newName: "IX_AutopayMandate_PaymentMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayMandates_LeaseID",
                table: "AutopayMandate",
                newName: "IX_AutopayMandate_LeaseID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayConsentAudits_TenantID",
                table: "AutopayConsentAudit",
                newName: "IX_AutopayConsentAudit_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AutopayConsentAudits_AutopayMandateID",
                table: "AutopayConsentAudit",
                newName: "IX_AutopayConsentAudit_AutopayMandateID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_TenantID",
                table: "AuditLog",
                newName: "IX_AuditLog_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_OrganizationMemberID",
                table: "AuditLog",
                newName: "IX_AuditLog_OrganizationMemberID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_OrganizationID",
                table: "AuditLog",
                newName: "IX_AuditLog_OrganizationID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_ActorUserID",
                table: "AuditLog",
                newName: "IX_AuditLog_ActorUserID");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationOccupants_UserID",
                table: "ApplicationOccupant",
                newName: "IX_ApplicationOccupant_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationOccupants_TenantID",
                table: "ApplicationOccupant",
                newName: "IX_ApplicationOccupant_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_OrganizationID",
                table: "Address",
                newName: "IX_Address_OrganizationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkOrder",
                table: "WorkOrder",
                column: "WorkOrderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnitType",
                table: "UnitType",
                column: "UnitTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantScreeningCheck",
                table: "TenantScreeningCheck",
                column: "TenantScreeningCheckID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenant",
                table: "Tenant",
                column: "TenantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantInvitation",
                table: "TenantInvitation",
                column: "TenantInvitationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantGuarantor",
                table: "TenantGuarantor",
                column: "TenantGuarantorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantEmployment",
                table: "TenantEmployment",
                column: "TenantEmploymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantEmergencyContact",
                table: "TenantEmergencyContact",
                column: "TenantEmergencyContactID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenancyType",
                table: "TenancyType",
                column: "TenancyTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaxRate",
                table: "TaxRate",
                column: "TaxID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionPlan",
                table: "SubscriptionPlan",
                column: "SubscriptionPlanID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityDepositTransaction",
                table: "SecurityDepositTransaction",
                column: "SecurityDepositTransactionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityDeposit",
                table: "SecurityDeposit",
                column: "SecurityDepositID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationHold",
                table: "ReservationHold",
                column: "ReservationHoldID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentalUnit",
                table: "RentalUnit",
                column: "RentalUnitID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RentalApplication",
                table: "RentalApplication",
                column: "RentalApplicationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refund",
                table: "Refund",
                column: "RefundID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptMaster",
                table: "ReceiptMaster",
                column: "ReceiptMasterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "RatingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Property",
                table: "Property",
                column: "PropertyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Preference",
                table: "Preference",
                column: "PreferenceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payout",
                table: "Payout",
                column: "PayoutID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PayoutItem",
                table: "PayoutItem",
                column: "PayoutItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "PaymentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentReminder",
                table: "PaymentReminder",
                column: "PaymentReminderID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentProviderEvent",
                table: "PaymentProviderEvent",
                column: "PaymentProviderEventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentMethod",
                table: "PaymentMethod",
                column: "PaymentMethodID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentIntent",
                table: "PaymentIntent",
                column: "PaymentIntentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentAttempt",
                table: "PaymentAttempt",
                column: "PaymentAttemptID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaymentAllocation",
                table: "PaymentAllocation",
                column: "PaymentAllocationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrgSubscription",
                table: "OrgSubscription",
                column: "OrgSubscriptionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrgPayoutAccount",
                table: "OrgPayoutAccount",
                column: "OrgPayoutAccountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationStatement",
                table: "OrganizationStatement",
                column: "OrganizationStatementID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organization",
                table: "Organization",
                column: "OrganizationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationMember",
                table: "OrganizationMember",
                column: "OrganizationMemberID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceRequest",
                table: "MaintenanceRequest",
                column: "MaintenanceRequestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingType",
                table: "ListingType",
                column: "ListingTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingTermPrice",
                table: "ListingTermPrice",
                column: "ListingTermPriceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Listing",
                table: "Listing",
                column: "ListingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingRule",
                table: "ListingRule",
                column: "ListingRuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingPolicy",
                table: "ListingPolicy",
                column: "ListingPolicyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingPhoto",
                table: "ListingPhoto",
                column: "ListingPhotoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingAmenity",
                table: "ListingAmenity",
                column: "ListingAmenityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListingAccessInstruction",
                table: "ListingAccessInstruction",
                column: "ListingAccessInstructionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerTransaction",
                table: "LedgerTransaction",
                column: "LedgerTransactionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerEntry",
                table: "LedgerEntry",
                column: "LedgerEntryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LedgerAccount",
                table: "LedgerAccount",
                column: "LedgerAccountID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaseSignatory",
                table: "LeaseSignatory",
                column: "LeaseSignatoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lease",
                table: "Lease",
                column: "LeaseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaseDocExtractedTerm",
                table: "LeaseDocExtractedTerm",
                column: "LeaseDocExtractedTermID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceMaster",
                table: "InvoiceMaster",
                column: "InvoiceMasterID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceDetail",
                table: "InvoiceDetail",
                column: "InvoiceDetailID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inspection",
                table: "Inspection",
                column: "InspectionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InspectionItem",
                table: "InspectionItem",
                column: "InspectionItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityVerification",
                table: "IdentityVerification",
                column: "IdentityVerificationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FraudCase",
                table: "FraudCase",
                column: "FraudCaseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeeType",
                table: "FeeType",
                column: "FeeTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fee",
                table: "Fee",
                column: "FeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dispute",
                table: "Dispute",
                column: "DisputeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReporting",
                table: "CreditReporting",
                column: "CreditReportingID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReportingEnrollment",
                table: "CreditReportingEnrollment",
                column: "CreditReportingEnrollmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreditReportingConsentAudit",
                table: "CreditReportingConsentAudit",
                column: "ConsentAuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation",
                column: "ConversationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationParticipant",
                table: "ConversationParticipant",
                column: "ConversationParticipantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMessage",
                table: "ConversationMessage",
                column: "ConversationMessageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contractor",
                table: "Contractor",
                column: "ContractorID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chargeback",
                table: "Chargeback",
                column: "ChargebackID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "CategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CalendarEvent",
                table: "CalendarEvent",
                column: "CalendarEventID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutopayMandate",
                table: "AutopayMandate",
                column: "AutopayMandateID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutopayConsentAudit",
                table: "AutopayConsentAudit",
                column: "AutopayConsentAuditID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLog",
                table: "AuditLog",
                column: "AuditLogID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "AttachmentID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationOccupant",
                table: "ApplicationOccupant",
                column: "ApplicationOccupantID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AmenityCatalog",
                table: "AmenityCatalog",
                column: "AmenityID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Organization_OrganizationID",
                table: "Address",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationOccupant_AspNetUsers_UserID",
                table: "ApplicationOccupant",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationOccupant_Tenant_TenantID",
                table: "ApplicationOccupant",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_AspNetUsers_ActorUserID",
                table: "AuditLog",
                column: "ActorUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_OrganizationMember_OrganizationMemberID",
                table: "AuditLog",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_Organization_OrganizationID",
                table: "AuditLog",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLog_Tenant_TenantID",
                table: "AuditLog",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayConsentAudit_AutopayMandate_AutopayMandateID",
                table: "AutopayConsentAudit",
                column: "AutopayMandateID",
                principalTable: "AutopayMandate",
                principalColumn: "AutopayMandateID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayConsentAudit_Tenant_TenantID",
                table: "AutopayConsentAudit",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandate_Lease_LeaseID",
                table: "AutopayMandate",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandate_PaymentMethod_PaymentMethodID",
                table: "AutopayMandate",
                column: "PaymentMethodID",
                principalTable: "PaymentMethod",
                principalColumn: "PaymentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_AutopayMandate_Tenant_TenantID",
                table: "AutopayMandate",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_Lease_LeaseID",
                table: "CalendarEvent",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_Listing_ListingID",
                table: "CalendarEvent",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_MaintenanceRequest_MaintenanceRequestID",
                table: "CalendarEvent",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequest",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_RentalApplication_RentalApplicationID",
                table: "CalendarEvent",
                column: "RentalApplicationID",
                principalTable: "RentalApplication",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarEvent_ReservationHold_ReservationHoldID",
                table: "CalendarEvent",
                column: "ReservationHoldID",
                principalTable: "ReservationHold",
                principalColumn: "ReservationHoldID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargeback_Organization_OrganizationID",
                table: "Chargeback",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargeback_Payment_PaymentID",
                table: "Chargeback",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chargeback_Tenant_TenantID",
                table: "Chargeback",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contractor_Organization_OrganizationID",
                table: "Contractor",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Dispute_DisputeID",
                table: "Conversation",
                column: "DisputeID",
                principalTable: "Dispute",
                principalColumn: "DisputeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_Lease_LeaseID",
                table: "Conversation",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_MaintenanceRequest_MaintenanceRequestID",
                table: "Conversation",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequest",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessage_AspNetUsers_SenderUserID",
                table: "ConversationMessage",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessage_ConversationMessage_ReplyToMessageID",
                table: "ConversationMessage",
                column: "ReplyToMessageID",
                principalTable: "ConversationMessage",
                principalColumn: "ConversationMessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessage_Conversation_ConversationID",
                table: "ConversationMessage",
                column: "ConversationID",
                principalTable: "Conversation",
                principalColumn: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessage_OrganizationMember_SenderOrganizationMemberID",
                table: "ConversationMessage",
                column: "SenderOrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMessage_Tenant_SenderTenantID",
                table: "ConversationMessage",
                column: "SenderTenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_AspNetUsers_UserID",
                table: "ConversationParticipant",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Conversation_ConversationID",
                table: "ConversationParticipant",
                column: "ConversationID",
                principalTable: "Conversation",
                principalColumn: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_OrganizationMember_OrganizationMemberID",
                table: "ConversationParticipant",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipant_Tenant_TenantID",
                table: "ConversationParticipant",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReporting_CreditReportingEnrollment_CreditReportingEnrollmentID",
                table: "CreditReporting",
                column: "CreditReportingEnrollmentID",
                principalTable: "CreditReportingEnrollment",
                principalColumn: "CreditReportingEnrollmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReporting_InvoiceMaster_InvoiceMasterID",
                table: "CreditReporting",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReporting_Payment_PaymentID",
                table: "CreditReporting",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingConsentAudit_CreditReportingEnrollment_CreditReportingEnrollmentID",
                table: "CreditReportingConsentAudit",
                column: "CreditReportingEnrollmentID",
                principalTable: "CreditReportingEnrollment",
                principalColumn: "CreditReportingEnrollmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingConsentAudit_Tenant_TenantID",
                table: "CreditReportingConsentAudit",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingEnrollment_Lease_LeaseID",
                table: "CreditReportingEnrollment",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditReportingEnrollment_Tenant_TenantID",
                table: "CreditReportingEnrollment",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Chargeback_ChargebackID",
                table: "Dispute",
                column: "ChargebackID",
                principalTable: "Chargeback",
                principalColumn: "ChargebackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_InvoiceMaster_InvoiceMasterID",
                table: "Dispute",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Lease_LeaseID",
                table: "Dispute",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_MaintenanceRequest_MaintenanceRequestID",
                table: "Dispute",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequest",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Organization_OrganizationID",
                table: "Dispute",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Payment_PaymentID",
                table: "Dispute",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispute_Tenant_TenantID",
                table: "Dispute",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fee_FeeType_FeeTypeID",
                table: "Fee",
                column: "FeeTypeID",
                principalTable: "FeeType",
                principalColumn: "FeeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fee_Organization_OrganizationID",
                table: "Fee",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_Chargeback_ChargebackID",
                table: "FraudCase",
                column: "ChargebackID",
                principalTable: "Chargeback",
                principalColumn: "ChargebackID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_Lease_LeaseID",
                table: "FraudCase",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_Organization_OrganizationID",
                table: "FraudCase",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_PaymentIntent_PaymentIntentID",
                table: "FraudCase",
                column: "PaymentIntentID",
                principalTable: "PaymentIntent",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_Payment_PaymentID",
                table: "FraudCase",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_FraudCase_Tenant_TenantID",
                table: "FraudCase",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityVerification_AspNetUsers_UserID",
                table: "IdentityVerification",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Lease_LeaseID",
                table: "Inspection",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Property_PropertyID",
                table: "Inspection",
                column: "PropertyID",
                principalTable: "Property",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_RentalUnit_RentalUnitID",
                table: "Inspection",
                column: "RentalUnitID",
                principalTable: "RentalUnit",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionItem_Inspection_InspectionID",
                table: "InspectionItem",
                column: "InspectionID",
                principalTable: "Inspection",
                principalColumn: "InspectionID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetail_Fee_FeeID",
                table: "InvoiceDetail",
                column: "FeeID",
                principalTable: "Fee",
                principalColumn: "FeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetail_InvoiceMaster_InvoiceMasterID",
                table: "InvoiceDetail",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMaster_Lease_LeaseID",
                table: "InvoiceMaster",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMaster_Lease_LeaseRenewalID",
                table: "InvoiceMaster",
                column: "LeaseRenewalID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMaster_Organization_OrganizationID",
                table: "InvoiceMaster",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceMaster_Tenant_TenantID",
                table: "InvoiceMaster",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_Listing_ListingID",
                table: "Lease",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_Organization_OrganizationID",
                table: "Lease",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_RentalApplication_RentalApplicationID",
                table: "Lease",
                column: "RentalApplicationID",
                principalTable: "RentalApplication",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_RentalUnit_RentalUnitID",
                table: "Lease",
                column: "RentalUnitID",
                principalTable: "RentalUnit",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_TenancyType_TenancyTypeID",
                table: "Lease",
                column: "TenancyTypeID",
                principalTable: "TenancyType",
                principalColumn: "TenancyTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Lease_Tenant_TenantID",
                table: "Lease",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseDocuments_Lease_LeaseID",
                table: "LeaseDocuments",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseOccupants_Lease_LeaseID",
                table: "LeaseOccupants",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseOccupants_Tenant_TenantID",
                table: "LeaseOccupants",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRecurringCharges_Fee_FeeID",
                table: "LeaseRecurringCharges",
                column: "FeeID",
                principalTable: "Fee",
                principalColumn: "FeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRecurringCharges_Lease_LeaseID",
                table: "LeaseRecurringCharges",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseRenewals_Lease_LeaseID",
                table: "LeaseRenewals",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatory_AspNetUsers_UserID",
                table: "LeaseSignatory",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatory_OrganizationMember_OrganizationMemberID",
                table: "LeaseSignatory",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatory_Organization_OrganizationID",
                table: "LeaseSignatory",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaseSignatory_Tenant_TenantID",
                table: "LeaseSignatory",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerAccount_Organization_OrganizationID",
                table: "LedgerAccount",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_LedgerAccount_LedgerAccountID",
                table: "LedgerEntry",
                column: "LedgerAccountID",
                principalTable: "LedgerAccount",
                principalColumn: "LedgerAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerEntry_LedgerTransaction_LedgerTransactionID",
                table: "LedgerEntry",
                column: "LedgerTransactionID",
                principalTable: "LedgerTransaction",
                principalColumn: "LedgerTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransaction_InvoiceMaster_InvoiceMasterID",
                table: "LedgerTransaction",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransaction_Organization_OrganizationID",
                table: "LedgerTransaction",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransaction_Payment_PaymentID",
                table: "LedgerTransaction",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransaction_Payout_PayoutID",
                table: "LedgerTransaction",
                column: "PayoutID",
                principalTable: "Payout",
                principalColumn: "PayoutID");

            migrationBuilder.AddForeignKey(
                name: "FK_LedgerTransaction_Refund_RefundID",
                table: "LedgerTransaction",
                column: "RefundID",
                principalTable: "Refund",
                principalColumn: "RefundID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listing_ListingType_ListingTypeID",
                table: "Listing",
                column: "ListingTypeID",
                principalTable: "ListingType",
                principalColumn: "ListingTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listing_Organization_OrganizationID",
                table: "Listing",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Listing_RentalUnit_RentalUnitID",
                table: "Listing",
                column: "RentalUnitID",
                principalTable: "RentalUnit",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAccessInstruction_Lease_LeaseID",
                table: "ListingAccessInstruction",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAccessInstruction_Listing_ListingID",
                table: "ListingAccessInstruction",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAmenity_AmenityCatalog_AmenityID",
                table: "ListingAmenity",
                column: "AmenityID",
                principalTable: "AmenityCatalog",
                principalColumn: "AmenityID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingAmenity_Listing_ListingID",
                table: "ListingAmenity",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingPhoto_Listing_ListingID",
                table: "ListingPhoto",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingPolicy_Listing_ListingID",
                table: "ListingPolicy",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingRule_Listing_ListingID",
                table: "ListingRule",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingTermPrice_Listing_ListingID",
                table: "ListingTermPrice",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_Category_CategoryID",
                table: "MaintenanceRequest",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_Lease_LeaseID",
                table: "MaintenanceRequest",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_Listing_ListingID",
                table: "MaintenanceRequest",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_Property_PropertyID",
                table: "MaintenanceRequest",
                column: "PropertyID",
                principalTable: "Property",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_RentalUnit_RentalUnitID",
                table: "MaintenanceRequest",
                column: "RentalUnitID",
                principalTable: "RentalUnit",
                principalColumn: "RentalUnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequest_Tenant_SubmittedByTenantID",
                table: "MaintenanceRequest",
                column: "SubmittedByTenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_RecipientUserID",
                table: "Notification",
                column: "RecipientUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_OrganizationMember_OrganizationMemberID",
                table: "Notification",
                column: "OrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Organization_OrganizationID",
                table: "Notification",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Tenant_TenantID",
                table: "Notification",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMember_AspNetUsers_UserID",
                table: "OrganizationMember",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMember_Organization_OrganizationID",
                table: "OrganizationMember",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationStatement_Organization_OrganizationID",
                table: "OrganizationStatement",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgPayoutAccount_Organization_OrganizationID",
                table: "OrgPayoutAccount",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgSubscription_Organization_OrganizationID",
                table: "OrgSubscription",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgSubscription_SubscriptionPlan_SubscriptionPlanID",
                table: "OrgSubscription",
                column: "SubscriptionPlanID",
                principalTable: "SubscriptionPlan",
                principalColumn: "SubscriptionPlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentIntent_PaymentIntentID",
                table: "Payment",
                column: "PaymentIntentID",
                principalTable: "PaymentIntent",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Tenant_TenantID",
                table: "Payment",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocation_InvoiceDetail_InvoiceDetailID",
                table: "PaymentAllocation",
                column: "InvoiceDetailID",
                principalTable: "InvoiceDetail",
                principalColumn: "InvoiceDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocation_InvoiceMaster_InvoiceMasterID",
                table: "PaymentAllocation",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAllocation_Payment_PaymentID",
                table: "PaymentAllocation",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentAttempt_PaymentIntent_PaymentIntentID",
                table: "PaymentAttempt",
                column: "PaymentIntentID",
                principalTable: "PaymentIntent",
                principalColumn: "PaymentIntentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntent_AutopayMandate_AutopayMandateID",
                table: "PaymentIntent",
                column: "AutopayMandateID",
                principalTable: "AutopayMandate",
                principalColumn: "AutopayMandateID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntent_InvoiceMaster_InvoiceMasterID",
                table: "PaymentIntent",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntent_Lease_LeaseID",
                table: "PaymentIntent",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntent_PaymentMethod_PaymentMethodID",
                table: "PaymentIntent",
                column: "PaymentMethodID",
                principalTable: "PaymentMethod",
                principalColumn: "PaymentMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentIntent_Tenant_TenantID",
                table: "PaymentIntent",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_Tenant_TenantID",
                table: "PaymentMethod",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentReminder_InvoiceMaster_InvoiceMasterID",
                table: "PaymentReminder",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payout_OrgPayoutAccount_OrgPayoutAccountID",
                table: "Payout",
                column: "OrgPayoutAccountID",
                principalTable: "OrgPayoutAccount",
                principalColumn: "OrgPayoutAccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payout_Organization_OrganizationID",
                table: "Payout",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayoutItem_Payment_PaymentID",
                table: "PayoutItem",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_PayoutItem_Payout_PayoutID",
                table: "PayoutItem",
                column: "PayoutID",
                principalTable: "Payout",
                principalColumn: "PayoutID");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_Address_AddressID",
                table: "Property",
                column: "AddressID",
                principalTable: "Address",
                principalColumn: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_Property_Organization_OrganizationID",
                table: "Property",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_ReviewerUserID",
                table: "Rating",
                column: "ReviewerUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Lease_LeaseID",
                table: "Rating",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptMaster_Payment_PaymentID",
                table: "ReceiptMaster",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptMaster_Tenant_TenantID",
                table: "ReceiptMaster",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Payment_PaymentID",
                table: "Refund",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Tenant_TenantID",
                table: "Refund",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplication_Listing_ListingID",
                table: "RentalApplication",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplication_OrganizationMember_ReviewedByOrganizationMemberID",
                table: "RentalApplication",
                column: "ReviewedByOrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplication_Organization_OrganizationID",
                table: "RentalApplication",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalApplication_Tenant_TenantID",
                table: "RentalApplication",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalUnit_Property_PropertyID",
                table: "RentalUnit",
                column: "PropertyID",
                principalTable: "Property",
                principalColumn: "PropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalUnit_UnitType_UnitTypeID",
                table: "RentalUnit",
                column: "UnitTypeID",
                principalTable: "UnitType",
                principalColumn: "UnitTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHold_Listing_ListingID",
                table: "ReservationHold",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHold_RentalApplication_RentalApplicationID",
                table: "ReservationHold",
                column: "RentalApplicationID",
                principalTable: "RentalApplication",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationHold_Tenant_TenantID",
                table: "ReservationHold",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposit_Lease_LeaseID",
                table: "SecurityDeposit",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposit_Organization_OrganizationID",
                table: "SecurityDeposit",
                column: "OrganizationID",
                principalTable: "Organization",
                principalColumn: "OrganizationID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDeposit_Tenant_TenantID",
                table: "SecurityDeposit",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransaction_InvoiceDetail_InvoiceDetailID",
                table: "SecurityDepositTransaction",
                column: "InvoiceDetailID",
                principalTable: "InvoiceDetail",
                principalColumn: "InvoiceDetailID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransaction_InvoiceMaster_InvoiceMasterID",
                table: "SecurityDepositTransaction",
                column: "InvoiceMasterID",
                principalTable: "InvoiceMaster",
                principalColumn: "InvoiceMasterID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransaction_Payment_PaymentID",
                table: "SecurityDepositTransaction",
                column: "PaymentID",
                principalTable: "Payment",
                principalColumn: "PaymentID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransaction_Refund_RefundID",
                table: "SecurityDepositTransaction",
                column: "RefundID",
                principalTable: "Refund",
                principalColumn: "RefundID");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityDepositTransaction_SecurityDeposit_SecurityDepositID",
                table: "SecurityDepositTransaction",
                column: "SecurityDepositID",
                principalTable: "SecurityDeposit",
                principalColumn: "SecurityDepositID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenant_AspNetUsers_UserID",
                table: "Tenant",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantEmergencyContact_Tenant_TenantID",
                table: "TenantEmergencyContact",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantEmployment_Tenant_TenantID",
                table: "TenantEmployment",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantGuarantor_AspNetUsers_UserID",
                table: "TenantGuarantor",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantGuarantor_Tenant_TenantID",
                table: "TenantGuarantor",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantInvitation_Lease_LeaseID",
                table: "TenantInvitation",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantInvitation_RentalApplication_RentalApplicationID",
                table: "TenantInvitation",
                column: "RentalApplicationID",
                principalTable: "RentalApplication",
                principalColumn: "RentalApplicationID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantScreeningCheck_Tenant_TenantID",
                table: "TenantScreeningCheck",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_Listing_ListingID",
                table: "ViewingAppointments",
                column: "ListingID",
                principalTable: "Listing",
                principalColumn: "ListingID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_OrganizationMember_AssignedOrganizationMemberID",
                table: "ViewingAppointments",
                column: "AssignedOrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewingAppointments_Tenant_TenantID",
                table: "ViewingAppointments",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Contractor_ContractorID",
                table: "WorkOrder",
                column: "ContractorID",
                principalTable: "Contractor",
                principalColumn: "ContractorID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Lease_LeaseID",
                table: "WorkOrder",
                column: "LeaseID",
                principalTable: "Lease",
                principalColumn: "LeaseID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_MaintenanceRequest_MaintenanceRequestID",
                table: "WorkOrder",
                column: "MaintenanceRequestID",
                principalTable: "MaintenanceRequest",
                principalColumn: "MaintenanceRequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_OrganizationMember_AssignedOrganizationMemberID",
                table: "WorkOrder",
                column: "AssignedOrganizationMemberID",
                principalTable: "OrganizationMember",
                principalColumn: "OrganizationMemberID");
        }
    }
}
