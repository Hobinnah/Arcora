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
    public class ListingRuleService : IListingRuleService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingRuleService> logger;
        private readonly IListingRuleRepository listingruleRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingRuleService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingRuleService> logger, IListingRuleRepository listingruleRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingruleRepository = listingruleRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingRuleDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingRule> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingRule>>(Cache.LISTINGRULES.ToString()) ?? new List<ListingRule>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingruleRepository.GetListingRuleAsync())?.Where(x => x != null) ?? new List<ListingRule>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingRule>>(Cache.LISTINGRULES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingRule by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingRuleDto>
                {
                    Data = new List<ListingRuleDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingRule> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.RuleTitle) && x.RuleTitle.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingRuleID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingRuleDto>>(pagedEntities);
            return new PagedResult<ListingRuleDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingRuleDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingRule> entities = cache.Get<IEnumerable<ListingRule>>(Cache.LISTINGRULES.ToString()) ?? new List<ListingRule>();
                ListingRule? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingRuleID == ID);
                }
                else
                {
                    match = await this.listingruleRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingRuleDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingRule by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingRuleDto> CreateListingRule(ListingRuleDto listingruleDto)
        {
            ListingRule listingRule = new ListingRule();
            IEnumerable<ListingRule?> checkEntity;
            try
            {
                checkEntity = await this.listingruleRepository.Find(x => x.RuleTitle!.ToLower().Trim() == listingruleDto.RuleTitle!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingRule = this.mapper.Map<ListingRule>(listingruleDto);
                    listingRule.ListingRuleID = Guid.NewGuid();
                    listingRule.CapturedDate = DateTime.UtcNow;
                    listingRule = await listingruleRepository.Create(listingRule) ?? new ListingRule();
                    await listingruleRepository.Save();
                    cache.Remove(Cache.LISTINGRULES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingRule. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingRuleDto>(listingRule);
        }

        /// <inheritdoc/>
        public async Task<ListingRuleDto?> UpdateListingRule(Guid id, ListingRuleDto listingruleDto)
        {
            try
            {
                var existing = await this.listingruleRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingRule listingRule = this.mapper.Map<ListingRule>(listingruleDto);
                listingRule = await listingruleRepository.Update(listingRule) ?? new ListingRule();
                await listingruleRepository.Save();
                cache.Remove(Cache.LISTINGRULES.ToString());
                listingruleDto = this.mapper.Map<ListingRuleDto>(listingRule);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingRule. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingruleDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingRule(Guid ID)
        {
            try
            {
                var listingRule = await this.listingruleRepository.GetByID(ID);
                if (listingRule == null)
                    throw new KeyNotFoundException("ListingRule with the specified ID was not found.");
                await listingruleRepository.Delete(listingRule);
                await listingruleRepository.Save();
                cache.Remove(Cache.LISTINGRULES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingRule . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}