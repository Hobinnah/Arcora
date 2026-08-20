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
    public class ListingPolicyService : IListingPolicyService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingPolicyService> logger;
        private readonly IListingPolicyRepository listingpolicyRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingPolicyService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingPolicyService> logger, IListingPolicyRepository listingpolicyRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingpolicyRepository = listingpolicyRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingPolicyDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingPolicy> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingPolicy>>(Cache.LISTINGPOLICIES.ToString()) ?? new List<ListingPolicy>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingpolicyRepository.GetListingPolicyAsync())?.Where(x => x != null) ?? new List<ListingPolicy>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingPolicy>>(Cache.LISTINGPOLICIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPolicy by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingPolicyDto>
                {
                    Data = new List<ListingPolicyDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingPolicy> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ApplicationInstructions) && x.ApplicationInstructions.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingPolicyID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingPolicyDto>>(pagedEntities);
            return new PagedResult<ListingPolicyDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingPolicyDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingPolicy> entities = cache.Get<IEnumerable<ListingPolicy>>(Cache.LISTINGPOLICIES.ToString()) ?? new List<ListingPolicy>();
                ListingPolicy? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingPolicyID == ID);
                }
                else
                {
                    match = await this.listingpolicyRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingPolicyDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPolicy by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingPolicyDto> CreateListingPolicy(ListingPolicyDto listingpolicyDto)
        {
            ListingPolicy listingPolicy = new ListingPolicy();
            IEnumerable<ListingPolicy?> checkEntity;
            try
            {
                checkEntity = await this.listingpolicyRepository.Find(x => x.ApplicationInstructions!.ToLower().Trim() == listingpolicyDto.ApplicationInstructions!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingPolicy = this.mapper.Map<ListingPolicy>(listingpolicyDto);
                    listingPolicy.ListingPolicyID = Guid.NewGuid();
                    listingPolicy.CapturedDate = DateTime.UtcNow;
                    listingPolicy = await listingpolicyRepository.Create(listingPolicy) ?? new ListingPolicy();
                    await listingpolicyRepository.Save();
                    cache.Remove(Cache.LISTINGPOLICIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingPolicy. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingPolicyDto>(listingPolicy);
        }

        /// <inheritdoc/>
        public async Task<ListingPolicyDto?> UpdateListingPolicy(Guid id, ListingPolicyDto listingpolicyDto)
        {
            try
            {
                var existing = await this.listingpolicyRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingPolicy listingPolicy = this.mapper.Map<ListingPolicy>(listingpolicyDto);
                listingPolicy = await listingpolicyRepository.Update(listingPolicy) ?? new ListingPolicy();
                await listingpolicyRepository.Save();
                cache.Remove(Cache.LISTINGPOLICIES.ToString());
                listingpolicyDto = this.mapper.Map<ListingPolicyDto>(listingPolicy);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingPolicy. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingpolicyDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingPolicy(Guid ID)
        {
            try
            {
                var listingPolicy = await this.listingpolicyRepository.GetByID(ID);
                if (listingPolicy == null)
                    throw new KeyNotFoundException("ListingPolicy with the specified ID was not found.");
                await listingpolicyRepository.Delete(listingPolicy);
                await listingpolicyRepository.Save();
                cache.Remove(Cache.LISTINGPOLICIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingPolicy . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}