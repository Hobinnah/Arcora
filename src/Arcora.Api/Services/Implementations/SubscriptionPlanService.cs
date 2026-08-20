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
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<SubscriptionPlanService> logger;
        private readonly ISubscriptionPlanRepository subscriptionplanRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public SubscriptionPlanService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<SubscriptionPlanService> logger, ISubscriptionPlanRepository subscriptionplanRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.subscriptionplanRepository = subscriptionplanRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<SubscriptionPlanDto>> GetAll(Paging paging)
        {
            IEnumerable<SubscriptionPlan> entities;
            try
            {
                entities = cache.Get<IEnumerable<SubscriptionPlan>>(Cache.SUBSCRIPTIONPLANS.ToString()) ?? new List<SubscriptionPlan>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.subscriptionplanRepository.GetAll())?.Where(x => x != null).Cast<SubscriptionPlan>().ToList() ?? new List<SubscriptionPlan>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<SubscriptionPlan>>(Cache.SUBSCRIPTIONPLANS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SubscriptionPlan by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<SubscriptionPlanDto>
                {
                    Data = new List<SubscriptionPlanDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<SubscriptionPlan> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.SubscriptionPlanID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<SubscriptionPlanDto>>(pagedEntities);
            return new PagedResult<SubscriptionPlanDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<SubscriptionPlanDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<SubscriptionPlan> entities = cache.Get<IEnumerable<SubscriptionPlan>>(Cache.SUBSCRIPTIONPLANS.ToString()) ?? new List<SubscriptionPlan>();
                SubscriptionPlan? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.SubscriptionPlanID == ID);
                }
                else
                {
                    match = await this.subscriptionplanRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<SubscriptionPlanDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SubscriptionPlan by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<SubscriptionPlanDto> CreateSubscriptionPlan(SubscriptionPlanDto subscriptionplanDto)
        {
            SubscriptionPlan subscriptionPlan = new SubscriptionPlan();
            IEnumerable<SubscriptionPlan?> checkEntity;
            try
            {
                checkEntity = await this.subscriptionplanRepository.Find(x => x.Name!.ToLower().Trim() == subscriptionplanDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    subscriptionPlan = this.mapper.Map<SubscriptionPlan>(subscriptionplanDto);
                    subscriptionPlan.SubscriptionPlanID = Guid.NewGuid();
                    subscriptionPlan.ProviderProductID = string.IsNullOrEmpty(subscriptionplanDto.ProviderProductID) ? null : subscriptionplanDto.ProviderProductID;
                    subscriptionPlan.ProviderMonthlyPriceID = string.IsNullOrEmpty(subscriptionplanDto.ProviderMonthlyPriceID) ? null : subscriptionplanDto.ProviderMonthlyPriceID;
                    subscriptionPlan.ProviderAnnualPriceID = string.IsNullOrEmpty(subscriptionplanDto.ProviderAnnualPriceID) ? null : subscriptionplanDto.ProviderAnnualPriceID;
                    subscriptionPlan.CapturedDate = DateTime.UtcNow;
                    subscriptionPlan = await subscriptionplanRepository.Create(subscriptionPlan) ?? new SubscriptionPlan();
                    await subscriptionplanRepository.Save();
                    cache.Remove(Cache.SUBSCRIPTIONPLANS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating SubscriptionPlan. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<SubscriptionPlanDto>(subscriptionPlan);
        }

        /// <inheritdoc/>
        public async Task<SubscriptionPlanDto?> UpdateSubscriptionPlan(Guid id, SubscriptionPlanDto subscriptionplanDto)
        {
            try
            {
                var existing = await this.subscriptionplanRepository.GetByID(id);
                if (existing == null)
                    return null;
                SubscriptionPlan subscriptionPlan = this.mapper.Map<SubscriptionPlan>(subscriptionplanDto);
                subscriptionPlan = await subscriptionplanRepository.Update(subscriptionPlan) ?? new SubscriptionPlan();
                await subscriptionplanRepository.Save();
                cache.Remove(Cache.SUBSCRIPTIONPLANS.ToString());
                subscriptionplanDto = this.mapper.Map<SubscriptionPlanDto>(subscriptionPlan);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating SubscriptionPlan. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return subscriptionplanDto;
        }

        /// <inheritdoc/>
        public async Task DeleteSubscriptionPlan(Guid ID)
        {
            try
            {
                var subscriptionPlan = await this.subscriptionplanRepository.GetByID(ID);
                if (subscriptionPlan == null)
                    throw new KeyNotFoundException("SubscriptionPlan with the specified ID was not found.");
                await subscriptionplanRepository.Delete(subscriptionPlan);
                await subscriptionplanRepository.Save();
                cache.Remove(Cache.SUBSCRIPTIONPLANS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting SubscriptionPlan . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}