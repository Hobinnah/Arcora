// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class PayoutService : IPayoutService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<PayoutService> logger;
        private readonly IPayoutRepository payoutRepository;
        private readonly ArcoraDbContext dbContext;
        private readonly IOptions<CacheConfiguration> _options;
        public PayoutService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<PayoutService> logger, IPayoutRepository payoutRepository, ArcoraDbContext dbContext)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.payoutRepository = payoutRepository;
            this.dbContext = dbContext;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PayoutDto>> GetAll(Paging paging)
            => await GetFiltered(null, null, paging);

        /// <inheritdoc/>
        public async Task<PagedResult<PayoutDto>> GetFiltered(Guid? organizationID, string? period, Paging paging)
        {
            try
            {
                var pagingModel = paging ?? new Paging();
                var (periodStart, periodEndExclusive, _) = ResolvePeriod(period);

                var query = this.dbContext.Payouts
                    .AsNoTracking()
                    .Include(x => x.Organization)
                    .Include(x => x.OrgPayoutAccount)
                    .AsQueryable();

                if (organizationID.HasValue)
                    query = query.Where(x => x.OrganizationID == organizationID.Value);

                if (periodStart.HasValue && periodEndExclusive.HasValue)
                {
                    query = query.Where(x =>
                        (x.PaidAt ?? x.ScheduledAt ?? x.RequestedAt) >= periodStart.Value &&
                        (x.PaidAt ?? x.ScheduledAt ?? x.RequestedAt) < periodEndExclusive.Value);
                }

                var totalCount = await query.CountAsync();
                var pagedEntities = await query
                    .OrderByDescending(x => x.PaidAt ?? x.ScheduledAt ?? x.RequestedAt)
                    .ThenByDescending(x => x.PayoutID)
                    .Skip((pagingModel.PageNumber - 1) * pagingModel.PageSize)
                    .Take(pagingModel.PageSize)
                    .ToListAsync();

                return new PagedResult<PayoutDto>
                {
                    Data = this.mapper.Map<IEnumerable<PayoutDto>>(pagedEntities),
                    TotalCount = totalCount
                };
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching payouts. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PayoutDto>
                {
                    Data = new List<PayoutDto>(),
                    TotalCount = 0
                };
            }
        }

        /// <inheritdoc/>
        public async Task<EarningsSummaryDto> GetEarningsSummary(Guid organizationID, string? period)
        {
            var (periodStart, periodEndExclusive, normalizedPeriod) = ResolvePeriod(period);

            var allocations = await AccountingQuery(organizationID, periodStart, periodEndExclusive).ToListAsync();
            var rentCollected = allocations.Sum(x => x.RentAmount);
            var placementFees = allocations.Sum(x => x.HostPlacementFee);
            var monthlyFees = allocations.Sum(x => x.HostMonthlyFee);
            var paymentIds = allocations.Select(x => x.PaymentID).ToList();
            var unallocatedQuery = from payment in dbContext.Payments.AsNoTracking()
                                   join intent in dbContext.PaymentIntents.AsNoTracking() on payment.PaymentIntentID equals intent.PaymentIntentID
                                   join lease in dbContext.Leases.AsNoTracking() on intent.LeaseID equals lease.LeaseID
                                   where lease.OrganizationID == organizationID && payment.Status == "PAID" &&
                                       !dbContext.RentPaymentAccountings.Any(x =>
                                           x.PaymentID == payment.PaymentID && x.SettledAt != null)
                                   select payment;
            if (periodStart.HasValue && periodEndExclusive.HasValue)
                unallocatedQuery = unallocatedQuery.Where(x => x.PaidAt >= periodStart && x.PaidAt < periodEndExclusive);
            var refundReviewRequired = await dbContext.Payments.AsNoTracking().AnyAsync(
                x => paymentIds.Contains(x.PaymentID) && x.RefundedAmount > 0m) ||
                await dbContext.Refunds.AsNoTracking().AnyAsync(
                    x => paymentIds.Contains(x.PaymentID) && x.Status != "FAILED" && x.Status != "CANCELLED");

            var pastDueQuery = this.dbContext.InvoiceMasters
                .AsNoTracking()
                .Where(x => x.OrganizationID == organizationID && x.BalanceDue > 0 && x.DueDate < DateTime.UtcNow && x.Status != "PAID" && x.Status != "VOID" && x.Status != "VOIDED");

            if (periodStart.HasValue && periodEndExclusive.HasValue)
                pastDueQuery = pastDueQuery.Where(x => x.DueDate >= periodStart.Value && x.DueDate < periodEndExclusive.Value);

            var pastDue = (await pastDueQuery.Select(x => x.BalanceDue).ToListAsync()).Sum();

            return new EarningsSummaryDto
            {
                OrganizationID = organizationID,
                Period = normalizedPeriod,
                PeriodStart = periodStart,
                PeriodEnd = periodEndExclusive?.AddTicks(-1),
                RentCollected = rentCollected,
                ServiceFees = placementFees + monthlyFees,
                HostPlacementFees = placementFees,
                HostMonthlyFees = monthlyFees,
                RefundReviewRequired = refundReviewRequired,
                UnallocatedPaymentCount = await unallocatedQuery.CountAsync(),
                PastDue = pastDue,
                NetEarnings = allocations.Sum(x => x.HostNetAmount)
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ListingEarningsBreakdownDto>> GetListingBreakdown(Guid organizationID, string? period)
        {
            var (periodStart, periodEndExclusive, _) = ResolvePeriod(period);

            var query = from allocation in AccountingQuery(organizationID, periodStart, periodEndExclusive)
                        join listing in this.dbContext.Listings.AsNoTracking() on allocation.ListingID equals listing.ListingID
                        select new
                        {
                            listing.ListingID,
                            ListingName = listing.Title,
                            allocation.RentAmount,
                            allocation.HostPlacementFee,
                            allocation.HostMonthlyFee,
                            allocation.HostNetAmount
                        };

            return (await query.ToListAsync())
                .GroupBy(x => new { x.ListingID, x.ListingName })
                .Select(g => new ListingEarningsBreakdownDto
                {
                    ListingID = g.Key.ListingID,
                    ListingName = g.Key.ListingName ?? string.Empty,
                    RentCollected = g.Sum(x => x.RentAmount),
                    ServiceFees = g.Sum(x => x.HostPlacementFee + x.HostMonthlyFee),
                    HostPlacementFees = g.Sum(x => x.HostPlacementFee),
                    HostMonthlyFees = g.Sum(x => x.HostMonthlyFee),
                    NetEarnings = g.Sum(x => x.HostNetAmount)
                })
                .OrderByDescending(x => x.NetEarnings)
                .ToList();
        }

        private IQueryable<RentPaymentAccounting> AccountingQuery(Guid organizationID, DateTime? start, DateTime? end)
        {
            var query = dbContext.RentPaymentAccountings.AsNoTracking()
                .Where(x => x.OrganizationID == organizationID && x.SettledAt != null);
            if (start.HasValue && end.HasValue)
                query = query.Where(x => x.SettledAt >= start.Value && x.SettledAt < end.Value);
            return query;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EarningsTransactionDto>> GetStatementTransactions(Guid organizationID, string? period)
        {
            var (periodStart, periodEndExclusive, _) = ResolvePeriod(period);
            var allocatedPaymentIds = dbContext.RentPaymentAccountings.AsNoTracking()
                .Where(x => x.OrganizationID == organizationID && x.SettledAt != null)
                .Select(x => x.PaymentID);

            var query = from transaction in this.dbContext.LedgerTransactions.AsNoTracking()
                        join payment in this.dbContext.Payments.AsNoTracking() on transaction.PaymentID equals payment.PaymentID into paymentJoin
                        from payment in paymentJoin.DefaultIfEmpty()
                        join payout in this.dbContext.Payouts.AsNoTracking() on transaction.PayoutID equals payout.PayoutID into payoutJoin
                        from payout in payoutJoin.DefaultIfEmpty()
                        join invoice in this.dbContext.InvoiceMasters.AsNoTracking() on transaction.InvoiceMasterID equals invoice.InvoiceMasterID into invoiceJoin
                        from invoice in invoiceJoin.DefaultIfEmpty()
                        where transaction.OrganizationID == organizationID &&
                            (transaction.PaymentID == null || transaction.RefundID != null || transaction.PayoutID != null ||
                                !allocatedPaymentIds.Contains(transaction.PaymentID))
                        select new EarningsTransactionDto
                        {
                            TransactionDate = transaction.TransactionDate,
                            TransactionType = transaction.TransactionType,
                            Status = transaction.Status,
                            Description = transaction.Description,
                            ReferenceNumber = transaction.ReferenceNumber,
                            PayoutID = transaction.PayoutID,
                            PaymentID = transaction.PaymentID,
                            InvoiceMasterID = transaction.InvoiceMasterID,
                            Amount = payout != null
                                ? payout.Amount
                                : payment != null
                                    ? payment.NetAmount
                                    : invoice != null
                                        ? invoice.TotalAmount
                                        : 0,
                            Currency = payout != null
                                ? payout.Currency
                                : payment != null
                                    ? payment.Currency
                                    : invoice != null
                                        ? invoice.Currency
                                        : null
                        };

            if (periodStart.HasValue && periodEndExclusive.HasValue)
                query = query.Where(x => x.TransactionDate >= periodStart.Value && x.TransactionDate < periodEndExclusive.Value);

            var rows = await query
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();
            var allocations = await AccountingQuery(organizationID, periodStart, periodEndExclusive).ToListAsync();
            foreach (var allocation in allocations)
            {
                rows.Add(AccountingRow(allocation, "RENT_COLLECTED", allocation.RentAmount, "Rent collected (excludes deposit and tenant fees)"));
                if (allocation.HostPlacementFee != 0m)
                    rows.Add(AccountingRow(allocation, "HOST_PLACEMENT_FEE", -allocation.HostPlacementFee, "One-time host placement deduction"));
                if (allocation.HostMonthlyFee != 0m)
                    rows.Add(AccountingRow(allocation, "HOST_MONTHLY_FEE", -allocation.HostMonthlyFee, "Monthly active-unit deduction"));
            }
            return rows.OrderByDescending(x => x.TransactionDate).ToList();
        }

        private static EarningsTransactionDto AccountingRow(RentPaymentAccounting allocation, string type, decimal amount, string description) =>
            new()
            {
                TransactionDate = allocation.SettledAt!.Value, TransactionType = type, Status = "POSTED",
                Amount = amount, Currency = allocation.Currency, Description = description,
                PaymentID = allocation.PaymentID, ReferenceNumber = allocation.PaymentIntentID.ToString()
            };

        private static (DateTime? PeriodStart, DateTime? PeriodEndExclusive, string NormalizedPeriod) ResolvePeriod(string? period)
        {
            if (string.IsNullOrWhiteSpace(period))
                return (null, null, "all");

            var normalized = period.Trim().ToLowerInvariant();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            return normalized switch
            {
                "this month" or "thismonth" => (startOfMonth, startOfMonth.AddMonths(1), "this_month"),
                "last 3 months" or "last3months" => (startOfMonth.AddMonths(-2), startOfMonth.AddMonths(1), "last_3_months"),
                "year to date" or "ytd" or "yeartodate" => (new DateTime(now.Year, 1, 1), now.Date.AddDays(1), "year_to_date"),
                _ => throw new ArgumentException("Invalid period. Use 'This month', 'Last 3 months', or 'Year to date'.", nameof(period))
            };
        }

        /// <inheritdoc/>
        public async Task<PayoutDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<Payout> entities = cache.Get<IEnumerable<Payout>>(Cache.PAYOUTS.ToString()) ?? new List<Payout>();
                Payout? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PayoutID == ID);
                }
                else
                {
                    match = await this.payoutRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PayoutDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Payout by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PayoutDto> CreatePayout(PayoutDto payoutDto)
        {
            Payout payout = new Payout();
            IEnumerable<Payout?> checkEntity;
            try
            {
                checkEntity = await this.payoutRepository.Find(x => x.Currency!.ToLower().Trim() == payoutDto.Currency!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    payout = this.mapper.Map<Payout>(payoutDto);
                    payout.CapturedDate = DateTime.UtcNow;
                    payout = await payoutRepository.Create(payout) ?? new Payout();
                    await payoutRepository.Save();
                    cache.Remove(Cache.PAYOUTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Payout. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PayoutDto>(payout);
        }

        /// <inheritdoc/>
        public async Task<PayoutDto?> UpdatePayout(long id, PayoutDto payoutDto)
        {
            await EnsureNotProviderManaged(id);
            try
            {
                var existing = await this.payoutRepository.GetByID(id);
                if (existing == null)
                    return null;
                Payout payout = this.mapper.Map<Payout>(payoutDto);
                payout = await payoutRepository.Update(payout) ?? new Payout();
                await payoutRepository.Save();
                cache.Remove(Cache.PAYOUTS.ToString());
                payoutDto = this.mapper.Map<PayoutDto>(payout);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Payout. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return payoutDto;
        }

        /// <inheritdoc/>
        public async Task DeletePayout(long ID)
        {
            await EnsureNotProviderManaged(ID);
            try
            {
                var payout = await this.payoutRepository.GetByID(ID);
                if (payout == null)
                    throw new KeyNotFoundException("Payout with the specified ID was not found.");
                await payoutRepository.Delete(payout);
                await payoutRepository.Save();
                cache.Remove(Cache.PAYOUTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Payout . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PayoutDto?> UpdatePayoutStatus(long id, string status)
        {
            await EnsureNotProviderManaged(id);
            var payout = await payoutRepository.GetByID(id);
            if (payout == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                payout.Status = "Pending";
            }
            else
            {
                payout.Status = status;
            }

            await payoutRepository.Update(payout);
            await payoutRepository.Save();
            cache.Remove(Cache.PAYOUTS.ToString());
            return this.mapper.Map<PayoutDto>(payout);
        }

        private async Task EnsureNotProviderManaged(long id)
        {
            if (await dbContext.RentDisbursements.AnyAsync(x => x.PayoutID == id))
                throw new InvalidOperationException("Provider-backed rent payouts must be reconciled with Stripe, not edited manually.");
        }
    }
}