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
    public class PayoutItemService : IPayoutItemService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PayoutItemService> logger;
        private readonly IPayoutItemRepository payoutitemRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PayoutItemService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PayoutItemService> logger, IPayoutItemRepository payoutitemRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.payoutitemRepository = payoutitemRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PayoutItemDto>> GetAll(Paging paging)
        {
            IEnumerable<PayoutItem> entities;
            try
            {
                entities = cache.Get<IEnumerable<PayoutItem>>(Cache.PAYOUTITEMS.ToString()) ?? new List<PayoutItem>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.payoutitemRepository.GetPayoutItemAsync())?.Where(x => x != null) ?? new List<PayoutItem>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PayoutItem>>(Cache.PAYOUTITEMS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PayoutItem by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PayoutItemDto>
                {
                    Data = new List<PayoutItemDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PayoutItem> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PayoutItemID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PayoutItemDto>>(pagedEntities);
            return new PagedResult<PayoutItemDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PayoutItemDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<PayoutItem> entities = cache.Get<IEnumerable<PayoutItem>>(Cache.PAYOUTITEMS.ToString()) ?? new List<PayoutItem>();
                PayoutItem? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PayoutItemID == ID);
                }
                else
                {
                    match = await this.payoutitemRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PayoutItemDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PayoutItem by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PayoutItemDto> CreatePayoutItem(PayoutItemDto payoutitemDto)
        {
            PayoutItem payoutItem = new PayoutItem();
            IEnumerable<PayoutItem?> checkEntity;
            try
            {
                checkEntity = await this.payoutitemRepository.Find(x => x.Description!.ToLower().Trim() == payoutitemDto.Description!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    payoutItem = this.mapper.Map<PayoutItem>(payoutitemDto);
                    payoutItem.CapturedDate = DateTime.UtcNow;
                    payoutItem = await payoutitemRepository.Create(payoutItem) ?? new PayoutItem();
                    await payoutitemRepository.Save();
                    cache.Remove(Cache.PAYOUTITEMS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PayoutItem. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PayoutItemDto>(payoutItem);
        }

        /// <inheritdoc/>
        public async Task<PayoutItemDto?> UpdatePayoutItem(long id, PayoutItemDto payoutitemDto)
        {
            try
            {
                var existing = await this.payoutitemRepository.GetByID(id);
                if (existing == null)
                    return null;
                PayoutItem payoutItem = this.mapper.Map<PayoutItem>(payoutitemDto);
                payoutItem = await payoutitemRepository.Update(payoutItem) ?? new PayoutItem();
                await payoutitemRepository.Save();
                cache.Remove(Cache.PAYOUTITEMS.ToString());
                payoutitemDto = this.mapper.Map<PayoutItemDto>(payoutItem);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PayoutItem. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return payoutitemDto;
        }

        /// <inheritdoc/>
        public async Task DeletePayoutItem(long ID)
        {
            try
            {
                var payoutItem = await this.payoutitemRepository.GetByID(ID);
                if (payoutItem == null)
                    throw new KeyNotFoundException("PayoutItem with the specified ID was not found.");
                await payoutitemRepository.Delete(payoutItem);
                await payoutitemRepository.Save();
                cache.Remove(Cache.PAYOUTITEMS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PayoutItem . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}