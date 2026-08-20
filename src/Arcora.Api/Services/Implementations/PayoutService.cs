// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class PayoutService : IPayoutService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PayoutService> logger;
        private readonly IPayoutRepository payoutRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PayoutService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PayoutService> logger, IPayoutRepository payoutRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.payoutRepository = payoutRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PayoutDto>> GetAll(Paging paging)
        {
            IEnumerable<Payout> entities;
            try
            {
                entities = cache.Get<IEnumerable<Payout>>(Cache.PAYOUTS.ToString()) ?? new List<Payout>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.payoutRepository.GetPayoutAsync())?.Where(x => x != null) ?? new List<Payout>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Payout>>(Cache.PAYOUTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Payout by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PayoutDto>
                {
                    Data = new List<PayoutDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Payout> filteredEntities = entities!;
            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PayoutID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PayoutDto>>(pagedEntities);
            return new PagedResult<PayoutDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
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
    }
}