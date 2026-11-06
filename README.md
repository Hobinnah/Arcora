# Arcora
Generated project for Arcora by Hobinnah

## Frontend checks

From `arcora-client`, run `npm test` to execute all Node tests under `tests/`, and
`npm run build` to type-check and create the production bundle. `npm run lint`
checks ESLint diagnostics against the per-file/rule baseline in
`arcora-client/scripts/lint-baseline.json`: existing diagnostics are reported,
and any increase fails the command. The baseline currently captures substantial
legacy lint debt; it does not mean all existing diagnostics have been fixed.
Update it only after intentionally reducing or reviewing that debt, with
`npm run lint:baseline:update`.

## Marketplace map and address privacy

`GET /api/Listing/MapSearch` returns lightweight price pins for **all** matching
homes, independent of card pagination. It shares the SQL search filters with
`GET /api/Listing/Search`: public listing status, stay dates/length, occupants,
price, furnishings, pets and `UnitTypeID`. Availability excludes overlapping
leases, unexpired reservation holds and non-cancelled blocking calendar events.
`South`, `West`, `North`, `East` bounds must be supplied together; west greater
than east represents a viewport crossing the international date line.

Public pins are fixed 0.01-degree cell centers (approximately neighbourhood-level).
Bounds apply to those same **public** positions, not private coordinates, to
prevent repeated narrow searches from locating a home. Street/postal-code matching
is excluded from public location search. `missingLocationCount` reports matching
homes without valid coordinates; they remain in the list, never receive invented
pins. `ListingDto.publicLocation` is safe to use in public clients.

The response boundary redacts exact street/unit/postal/coordinate/provider-reference
fields from nested listing/property/unit/address graphs unless the viewer is an
active organization member, an administrator, or the home's tenant with an
`ACTIVE` lease and a completed, not fully refunded **initial** payment. An intent
flag alone or a different recurring payment does not qualify. Messages and
check-in emails enforce the same tenant condition. Necessary property addresses
in privately authorized lease contracts remain available before signing, as
required by the lease workflow. Hosts must not put exact addresses/access codes
in public listing titles, descriptions or photos.

Tenant disclosure is scoped to the leased unit/listing, not every unit sharing
the same building address. Deactivated organization members have no management
exception. Raw payment/payment-intent create, update and status endpoints and
raw lease-status updates are administrator-only: tenants/hosts must use the
server-verified signing/payment workflows rather than setting paid/active flags.

Azure Maps setup:

- Set `Azure:Maps:ClientId` to the Azure Maps account's unique client ID
  (JSON: `"Azure": { "Maps": { "ClientId": "..." } }`;
  environment variable: `Azure__Maps__ClientId`).
- Give a **dedicated** managed identity **Azure Maps Data Reader** on that account
  only. Set `Azure:Maps:ManagedIdentityClientId` for a user-assigned identity.
  Production always uses managed identity. In Development, configure
  `Azure:Maps:TenantId`, `Azure:Maps:ApplicationClientId` and
  `Azure:Maps:ClientSecret` together in the API's .NET User Secrets to use a
  dedicated app registration with the same account-scoped reader role.
  `ApplicationClientId` identifies the app registration; `ClientId` identifies
  the Maps account. Partial credentials fail explicitly without displaying values.
  With none of these three settings, Development uses `DefaultAzureCredential`
  (including environment credentials or Azure CLI login). Do not expose a
  developer/admin identity with broad map permissions through a publicly reachable
  deployment. User Secrets are not encrypted; never commit them or copy them to
  the React environment. Restart the API after changing credentials.
- Anonymous, rate-limited `/api/Maps/Configuration` and `/api/Maps/Token` supply the
  browser with a read-only, expiring Azure access token. Tokens are not cached by
  HTTP intermediaries. No subscription/master key belongs in a `VITE_` variable.
- Host-only `POST /api/Address/Geocode` uses Azure Maps structured geocoding
  (`2025-01-01`) with a ten-second timeout. Optionally configure the server-only
  `Azure:Maps:SubscriptionKey` in environment/User Secrets/Key Vault for geocoding;
  it is sent in a header, never a URL, and geocoding HTTP logs are disabled.
  Only good street-address matches of high/medium confidence are accepted.
  Hosts confirm/correct the returned coordinates before saving. No match is 422;
  unavailable/unconfigured geocoding is 503, not a fabricated location.
- Address edits invalidate old coordinates unless `locationConfirmed=true` and
  a valid coordinate pair is submitted. Geocoding runs on host action, not search.
  Existing listings without coordinates require host location confirmation.

Without Azure setup the home list remains usable and the map reports that it is
unavailable. Configure the Azure account before claiming live map/geocoder
end-to-end verification.

Run focused API regressions:
`dotnet test test/Arcora.Api.Tests --filter "FullyQualifiedName~ListingMapPrivacyTests|FullyQualifiedName~AzureMapsGeocodingTests|FullyQualifiedName~ConversationDetailsTests"`.

## Tenant payment verification

Payment onboarding is ready only when the primary PAD, its mandate, and the backup
credit card are verified. `PaymentOnboarding/Status/{tenantId}` reads pending PAD
SetupIntents from the payment provider, returns `hostedBankVerificationUrl` when
microdeposit verification is required, and reconciles successful verification.
Refreshing status also works in local Docker without a public webhook receiver.
Provider errors propagate; saved methods are never locally marked verified.

Password reset emails use `FrontendUrl` and the frontend `/reset-password` route.
Configure `FrontendUrl` to the browser-accessible deployment origin.

For non-binding signing tests, explicitly set `Signwell:TestMode=true` (Docker:
`Signwell__TestMode=true`). The client sends SignWell's `test_mode` field with every
create request. The default remains `false` for existing live deployments. Confirm
the provider document is marked test mode before applying any test signature.

The signing worker requires Playwright Chromium and its Linux dependencies
(`playwright install --with-deps chromium` using the deployed Playwright CLI).
Preparation failures are persisted as `FAILED` so the tenant can see the failure
and retry after the runtime dependency or template issue is corrected.

Refreshing a completed signing request reconciles the lease into
`PENDING_FIRST_PAYMENT` (or `ACTIVE` if its first payment is already paid), even
without a public SignWell webhook. Refreshing status does not initiate a charge.
Signed agreement links come from SignWell's completed-PDF endpoint, never the
unsigned file uploaded for signing. Cancelled or otherwise closed leases are not
reopened by a late completion event.

## First lease payment

The tenant's signed-lease page loads `GET /api/leases/{leaseId}/first-payment`
without creating a charge. Its authenticated, tenant-owned POST action checks the
displayed amount against the server quote, completed signatures, verified PAD and
backup card, and an active mandate. SignWell completion webhooks use the same service.
Tenant first payments include rent, deposit, booking fees (on rent only), and tenant
placement fees. Host placement and host unit subscription fees are excluded from
the tenant debit and deducted from the host's rent earnings instead.

Apply migration `20261029000100_AddPaymentIntentIdempotencyUniqueIndex` before
enabling this action. Existing duplicate idempotency keys must be investigated,
not deleted or recharged automatically. Repeated/concurrent submissions reuse one
persisted intent; an uncertain provider response never triggers the backup card.
`POST /api/leases/{leaseId}/first-payment/refresh` reads the real provider outcome.
Pending bank debits do not activate the lease or fund its deposit. Confirmed
success records the payment/deposit and activates the lease. The recurring scheduler
does not charge again in the initial lease month.
First-payment and monthly rent collection also create an `ISSUED` invoice with
rent, security deposit (first payment only), and individual tenant-fee lines before
the provider is charged. The invoice stays unpaid while the debit is pending or
failed; confirmed success creates payment allocations and updates its paid balance
in the same transaction. The current rent quote does not calculate tax, so these
invoices do not add unquoted tax.

Run the bank-status regressions with
`dotnet test test/Arcora.Api.Tests --filter FullyQualifiedName~PaymentOnboardingStatusTests`.

## Host rent accounting

Apply migration `20261029213000_AddRentPaymentAccounting`. Supported first-payment
and monthly scheduler collections snapshot rent, deposit, tenant fees and effective
host fee amounts before contacting the payment provider. Missing or ambiguous host
fee configuration blocks a new collection explicitly. Organization-specific host
fees override the corresponding global definition.

Only confirmed payment success posts earnings: host placement is deducted once per
lease, and the active-unit fee once per rental unit/month, including the first
payment. Pending/failed debits post no earnings or deductions. Settlement and
lease/deposit updates share a database transaction; lease locks and filtered unique
indexes protect against duplicate/concurrent fee posting. Saved rates and amounts
are not recalculated when fee configuration changes. No separate host debit,
subscription, transfer or bank payout is created by this accounting.

For CAD 1,500 rent with configured CAD 99.99 placement and CAD 24.99 monthly fees,
net host earnings are CAD 1,375.02 initially and CAD 1,475.01 in later rent periods.
Deposits and tenant fees are never host rent. Summary, listing breakdown and
statement exports read the same posted allocations. Net earnings are historical
accounting, not a provider-confirmed available balance. Refunds flag accounting
review; fee refunds/reversals and tax treatment require explicit policy and are not
inferred from a refund amount.

Legacy paid collections are not silently backfilled using today's mutable fees.
An authorized operator must verify the original breakdown and explicitly reconcile:

```text
dotnet Arcora.Api.dll --reconcile-rent-payment PAYMENT_GUID 1500 750 124.99 99.99 24.99 AUDIT_ACTOR
```

The command requires the existing payment to be paid and unrefunded, verifies the
exact approved amounts against the supported collection and recorded deposit
funding, and persists an audit actor. A repeated matching command is idempotent.
It never calls the payment provider or initiates a payout. Reconcile only records
whose original amounts/rates have been verified; mismatches require investigation.

Run accounting regressions with
`dotnet test test/Arcora.Api.Tests --filter FullyQualifiedName~LeaseFirstPaymentTests`.

## Host test transfers and bank payouts

The host earnings page has an opt-in test payout panel. Apply migration
`20261029220000_AddRentDisbursements` and explicitly configure
`HostPayouts:EnableTestPayouts=true` (environment variable
`HostPayouts__EnableTestPayouts=true`). The default is disabled; the service and
provider both reject live Stripe credentials/charges. This is not a live payout
rollout or an automatic payout scheduler.

An authorized organization manager must confirm the saved net rent amount with
`POST /api/organizations/{orgId}/rent-payouts/{paymentId}` (`expectedAmount`).
The server reserves that payment once, records a payout item with rent-only gross
and host deductions, and pins the destination account and amount. The provider
validates the original successful test charge, configures the test connected
account for manual payouts before its first transfer, and creates a transfer using
`source_transaction`. Deposits and tenant fees are never transferred to the host.
Transfer completion is not bank payout completion.

`POST .../{paymentId}/refresh` reconciles the original transfer and, once the source
charge and connected balance are available, progresses the consented bank payout.
It first returns `READY_FOR_PAYOUT`; the next refresh durably records its bank-payout
attempt clock before calling Stripe. Pending PAD settlement returns `WAITING_FUNDS`,
not `PAID`. No synthetic funding, extra tenant charge or forced provider success is
used to overcome a settlement delay.

Provider operations use deterministic idempotency keys and provider references.
After a 23-hour uncertain retry window, missing operations require explicit
support review instead of reissuing a possibly expired Stripe idempotency key.
Already-existing transfers/payouts are read regardless of that window. Failed or
reversed operations do not generate replacement transfers automatically. Refunds
and disputes block disbursement and need separate accounting policy.

Connected-account payout webhooks retrieve current Stripe state, associate tagged
rent payouts with their existing reservation, and update bank-payout history.
Duplicate/late events cannot replace current provider state with an old payload.
Provider-backed records cannot be manually changed through generic payout CRUD.
GET collection listings and earnings are read-only and membership-scoped.

Tests:
`dotnet test test/Arcora.Api.Tests --filter "FullyQualifiedName~LeaseFirstPaymentTests|FullyQualifiedName~StripeHostPayoutTests"`.
These cover net amounts, source-charge linkage, consent/access, pending/failed/paid
outcomes, uncertain responses, expiry, live-mode rejection and duplicate posting.

## Verification, review and messaging concurrency

Changing or removing an account phone number clears pending SMS codes and resets
phone confirmation. Request a fresh code for the current number. Identity
concurrency checks reject an in-flight verification if a concurrent phone edit
has already changed the account.

Listing decisions atomically claim a listing that is still `PENDING_REVIEW`.
The decision and conversation record commit together; only the winning reviewer
triggers host notifications.

Attachment sends require a UUID `AttachmentSendKey`. The client creates it once
per logical send and reuses it on retries. The server scopes the key to the
authorized sender and conversation, serializes matching sends, and replays the
persisted message without another upload or notification. Reusing the key with a
different attachment payload returns HTTP 409. Apply migration
`20261030010000_AddAttachmentSendIdempotency` before deploying the updated client.

## Resilient Key Vault startup secrets

Key Vault integration is **opt-in** (`KeyVault:Enabled`, default false). Disabled
mode preserves existing environment/User Secrets configuration unless unresolved
`akv:` references are present, in which case startup is refused. When enabled,
the API loads only explicitly referenced secrets before registering database,
authentication, payments, storage or SignalR services. Values are applied as
one in-memory configuration layer only after the entire set succeeds. There
is no Key Vault configuration reload provider, secret enumeration, per-request
retrieval, plaintext disk cache, database cache or secret values in health output.
The cache is ordinary process memory protected by OS/process isolation, not
encrypted .NET strings or a cache that survives restarts. Protect process access,
crash dumps, environment access and diagnostic configuration dumps accordingly.

Non-secret deployment configuration example:

```json
{
  "KeyVault": {
    "Enabled": true,
    "VaultUri": "https://YOUR-VAULT.vault.azure.net/",
    "AllowEnvironmentFallback": true,
    "StartupTimeoutSeconds": 30,
    "ConnectivityIntervalSeconds": 300
  },
  "ConnectionStrings": {
    "DefaultAppConnection": "akv:database-connection"
  },
  "Authentication": {
    "JwtSettings": {
      "Key": "akv:arcora-jwt-secret"
    }
  },
  "Stripe": {
    "SecretKey": "akv:arcora-stripe-secret"
  },
  "Azure": {
    "SignalR": {
      "ConnectionString": "akv:arcora-signalr-connection"
    }
  }
}
```

Reference every secret the deployment needs (for example provider API/webhook secrets,
SMTP password, storage connection string if not using managed identity, and
authentication secrets). All configured mappings are required. The sample is
illustrative, not a complete list for every deployment; confirm actual option
names in the service configuration. Secret names and configuration keys are
separate so no Key Vault name enumeration or naming conversion is needed.
Only effective configuration values beginning with `akv:` (case-insensitive)
are resolved; ordinary literal values are unchanged. The suffix must be a valid
Key Vault secret name, not a URI or version. References cannot configure the
`KeyVault` loader itself or its fallback section. Configuration provider precedence
still applies: an environment/User Secrets override replaces a file reference.
The existing `KeyVault:Secrets` array of `{ "ConfigurationKey": "...",
"SecretName": "..." }` remains supported, but must not target a key that also
contains a reference. There must be 1-100 references/mappings in total.
Production/non-Development uses `ManagedIdentityCredential`; assign the API's
identity least-privilege permission to **get** the mapped secrets (for example
an appropriate Key Vault Secrets User RBAC assignment). It does not need
permission to create/update secrets. `KeyVault:ManagedIdentityClientId` optionally
selects a user-assigned managed identity. Development uses
`DefaultAzureCredential` with interactive browser authentication disabled.
Never put Azure client secrets or application secret values in source control.

Startup performs up to four concurrent reads, a bounded 30-second total budget
and at most two SDK retries per read. An unreachable endpoint, transport failure,
timeout, HTTP 408/429 or server error can use the fallback **only** when explicitly
enabled. The fallback is a complete, separate set of process environment
variables, provisioned securely by your deployment:

```text
KeyVaultFallback__ConnectionStrings__DefaultAppConnection
KeyVaultFallback__Authentication__JwtSettings__Key
KeyVaultFallback__Stripe__SecretKey
KeyVaultFallback__Azure__SignalR__ConnectionString
```

Use the exact casing on Linux. Each variable corresponds to the mapped
configuration key with `:` replaced by `__`. Ordinary appsettings values,
User Secrets or ordinary environment values are **not** silently treated as
fallbacks when the feature is enabled. Missing any mapped fallback value refuses
startup without applying a partial vault/fallback mixture. Explicit access denial,
authentication failure, missing secrets, or empty/disabled/expired/not-yet-valid
vault secrets also refuse startup, rather than masking a permission or rotation
error with fallback. Startup cancellation never chooses fallback.

Once running, API requests use loaded configuration, not Key Vault. A vault
outage does not erase the loaded values or trigger new retrievals; dependent
services can continue while those credentials remain valid. This cannot make
expired/revoked database/provider credentials work. Expiration metadata from
vault secrets is tracked: expired loaded secrets fail the vault health entry
and readiness; explicit fallback has no vault expiration metadata and must be
rotated/validated by the deployment owner.

Operations Health includes **Key Vault startup secrets / transport**. At most
once per five minutes it checks DNS/TCP/certificate-validated TLS, with **no
authenticated Key Vault REST call or secret fetch**. It clearly distinguishes
transport connectivity from authentication. A runtime outage is Degraded (it
does not withdraw database-ready traffic merely because the vault is down).
Fallback startup is Degraded even after transport recovery, until an intentional
restart successfully loads vault secrets. Existing incident/email monitoring
reports these conditions. Monitor externally as well: an instance that refuses
startup cannot deliver an in-app alert.

Rotation is deliberate: update vault secrets and separately provision matching,
valid deployment fallback values, then roll/restart instances. The configuration
layer does not automatically replace keys underneath already-created JWT,
database, Stripe or SignalR clients. Protect the fallback delivery mechanism
separately so it does not depend on the same unavailable vault at startup;
platform Key Vault references that cannot resolve during an outage are not an
independent fallback. No vault resources or real secret copies are provisioned
automatically by this code.

Standard Key Vault supports ordinary secrets; Premium is not required solely
for API keys, passwords and connection strings. Runtime transport monitoring
does not incur secret-read operations. Startup reads (and retries) occur for
each instance/restart and each mapped secret. Consult Azure's current regional
pricing and your billing agreement rather than relying on a fixed CAD quote.

Tests:
`dotnet test test/Arcora.Api.Tests --filter "FullyQualifiedName~KeyVaultStartupTests|FullyQualifiedName~OperationsHealthTests"`.
They exercise startup/atomic publication, bounded timeout, explicit fallback,
permission failures, invalid secrets, cancellation, cached transport, and secret
expiration without connecting to a real vault.

## Central operations monitoring

The admin-only client route `/admin/operations` reads the cached
`GET /api/operations/health` snapshot. Reading or refreshing the dashboard does
not run probes or initiate business operations. It polls every 30 seconds and
shows stale/error states, UTC timestamps, worker runs, processed/failed counts,
queue backlogs, dependency results, and persistent incident/recovery history.

The monitor runs independently of the dashboard, defaults to a 60-second cycle,
limits concurrency to four probes and network probes to ten seconds. PDF rendering
has a separate 60-second cold-start budget, is checked every 15 minutes after
success, and retries on the normal interval after failure. Six existing workers are instrumented: recurring
rent, invitation email dispatch, general email dispatch, signing preparation,
move-in details email, and expired move-in draft cleanup. Disabled rent collection
is reported as Disabled; monitoring never enables it.

Dependency coverage includes SQL queries/pending migrations, required blob
containers, SMTP connection/STARTTLS, Stripe authenticated balance reads,
SignWell TLS plus an existing-document read where available, the selected
Persona/Onfido and Twilio/Infobip providers, web availability, Chromium PDF
generation, SignalR registration/Azure transport, Google/OIDC discovery, and
Canada Post TLS. Descriptions state exactly what was verified. Storage probes
do not write/delete blobs; SMTP probes do not authenticate or send email.
Canada Post connectivity does not verify its API token, and SignalR registration
does not prove authenticated end-to-end message delivery. Those require
separate synthetic journey monitoring. No probe charges tenants, creates
payouts, sends OTPs, creates verifications, or submits signing requests.

API traffic metrics retain only per-minute aggregate counts/durations (no URLs,
users or request bodies). Over the current and previous four UTC minutes, at
least five server errors and a 5% error rate degrade health; a 50% error rate
fails it. At least twenty requests averaging over two seconds also degrade
health. Non-API routes, including public health checks, are excluded.

Read-only business checks flag failed/stuck payments, overdue current-month
recurring collections, disbursements needing review, failed/queued signing,
aging identity sessions, failed/stuck payment webhooks, invitation retries and
aged queue items. Thresholds distinguish ordinary bank settlement/human waiting
from an outage; these advisory checks never retry or repair financial operations.
The rent-due check covers the current month only, not a historical arrears ledger.

Configuration (standard configuration/environment variables, no credentials
or recipient addresses belong in source control):

```json
{
  "OperationsHealth": {
    "Enabled": true,
    "InstanceId": "api-replica-1",
    "IntervalSeconds": 60,
    "TimeoutSeconds": 10,
    "PdfTimeoutSeconds": 60,
    "FailureThreshold": 2,
    "StuckMinutes": 30,
    "PaymentPendingHours": 120,
    "VerificationPendingHours": 48,
    "PayoutPendingHours": 240,
    "EmailEnabled": false,
    "EmailRecipients": []
  }
}
```

`WebUrl` optionally overrides `FrontendUrl` with an address reachable from the
API container. Keep it deployment-controlled, not user supplied. Give each
replica its own stable `InstanceId` (default: machine name); snapshots and incident
history are scoped to that instance, not a fleet-wide aggregate. Poll each replica
when operating multiple instances. Worker telemetry resets to Unknown on process
restart; incident history survives in SQL. A missed scheduler run or exceeded
processing deadline is detected; queue workers remain healthy while idle.
Worker failures are retained until incident persistence acknowledges them, even
if another job succeeds before the next monitor scan.

In-app incidents are enabled without email configuration. Set `EmailEnabled`
and valid `EmailRecipients` to deliver failure and recovery emails using the
existing `EmailSettings` SMTP service. Two consecutive unhealthy dependency
observations open one incident by default; an actual failed worker job is retained
and opens an incident on the next scan. A healthy observation resolves it.
Unknown/disabled/not-configured results do not falsely announce recovery.
Repeated observations do not produce repeated emails. Failed sends retry with
exponential backoff, capped at 60 minutes; SMTP acceptance is not proof of inbox
delivery. If an incident resolves before its first email succeeds, delivery still
retries and sends a combined recovered notification including the opening and
recovery times. Alert persistence and SMTP delivery run in a separate background
worker, so slow email delivery cannot block dependency probe refresh or readiness.
API error metrics observe the final exception-mapped HTTP response, not every
business exception as a server error.
Emails are at-least-once: a process crash after SMTP accepts a message
but before SQL records acceptance can produce a duplicate. Do not configure two
running replicas with the same instance ID.

If incident storage is unavailable, the dashboard retains cached incidents and
shows a persistence failure. A separate deduplicated in-memory outage email is
attempted directly through SMTP, with recovery notification when storage returns.
That fallback cannot survive a restart and cannot notify through a broken SMTP
service. Monitor the app from outside its process as well.

Public endpoints expose only minimal status, never provider errors or credentials:

- `/health/live`: process liveness, HTTP 200.
- `/health/ready`: fresh SQL/migration readiness, HTTP 200 or 503.
- `/health/status`: aggregate monitored status, HTTP 200 or 503.

Use `/health/live` for container liveness, `/health/ready` for traffic readiness,
and `/health/status` for external incident monitoring. Do not restart healthy
containers merely because an external vendor is unavailable. External monitoring
must also cover the public web URL, TLS expiry and authenticated websocket/journey
checks; an in-process monitor cannot report its own complete outage.

Apply migration `20261029230000_AddOperationsIncidents` before deployment.
Regression coverage: `dotnet test test/Arcora.Api.Tests --filter FullyQualifiedName~OperationsHealthTests`.

## Move-in submission

Move-in submission saves the inspection, arrival timestamp, evidence links and
in-app notifications before invoking messaging repositories, which can clear the
shared change tracker. Recipient notification lookup is awaited before messaging
returns so it cannot leave database work running during the check-in transaction's
commit. Email delivery still uses the configured queue.

Run the transaction and notification regressions with
`dotnet test test/Arcora.Api.Tests --filter "FullyQualifiedName~LeaseCheckOutTests|FullyQualifiedName~MessagingEnquiryTests"`.

## Isolated recurring rent verification

Run `dotnet test test\Arcora.Api.Tests --filter FullyQualifiedName~RecurringRentCollectionTests`.
By default, 15 deterministic cases execute the real scheduler cycle, repositories,
collection orchestrator and accounting against isolated SQLite with a synthetic
payment-provider boundary. Two real Stripe sandbox cases are skipped unless opted in.
An instance-local clock exercises anniversary/hour/month-end rules without changing
the system clock or enabling the application's background scheduler.

For Docker SQL Server coverage, privately set `ARCORA_RECURRING_TEST_SQLSERVER` to a
connection targeting a **nonexistent** database named `ArcoraRecurringTests_<32-character GUID>`.
The tests reject existing databases, create/drop their own schema per case, and disable
foreign-key checks only in that isolated fixture database. Never point this setting
at a shared journey database.

### Opt-in real Stripe recurring payments

Privately supply `ARCORA_RECURRING_TEST_STRIPE_KEY` with an `sk_test_` secret, then run:

```powershell
dotnet test test\Arcora.Api.Tests --filter FullyQualifiedName~RecurringRentCollectionTests.StripeSandbox
```

Do not paste credentials into source, command history or test output. Without a test
key these cases skip. They create only new synthetic Stripe customers, methods,
mandates and CAD 1,500 test payments; assert `livemode=false`; and execute the actual
`StripePaymentProvider`, scheduler cycle, orchestrator, repositories and accounting.
The first test verifies a genuine successful monthly PAD debit. The second uses
Stripe's documented insufficient-funds ACSS instrument and verifies the definitive
PAD failure followed by a genuine successful backup-card charge. Both first create
a successful payment for that new lease, then assert recurring monthly host fee
CAD 24.99, no repeated placement fee, and host net CAD 1,475.01. Repeated scheduler
cycles must not create additional Stripe PaymentIntents.

SQLite is the default isolated store; supply the separate SQL variable above to
exercise Docker SQL Server as well. No shared scheduler is enabled. Provider status
is polled in a bounded window, independently retrieved from Stripe, and passed
directly to the orchestrator's result handler. These tests do **not** exercise signed
HTTP webhook delivery, the running Docker API, browser/UI journeys, bank settlement,
or host payouts. No provider clock is advanced and no success is fabricated.
Synthetic customers are deleted afterward; Stripe test payment history remains.
