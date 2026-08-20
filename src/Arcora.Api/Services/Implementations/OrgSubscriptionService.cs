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
    public class OrgSubscriptionService : IOrgSubscriptionService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrgSubscriptionService> logger;
        private readonly IOrgSubscriptionRepository orgsubscriptionRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public OrgSubscriptionService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<OrgSubscriptionService> logger, IOrgSubscriptionRepository orgsubscriptionRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.orgsubscriptionRepository = orgsubscriptionRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrgSubscriptionDto>> GetAll(Paging paging)
        {
            IEnumerable<OrgSubscription> entities;
            try
            {
                entities = cache.Get<IEnumerable<OrgSubscription>>(Cache.ORGSUBSCRIPTIONS.ToString()) ?? new List<OrgSubscription>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.orgsubscriptionRepository.GetOrgSubscriptionAsync())?.Where(x => x != null) ?? new List<OrgSubscription>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<OrgSubscription>>(Cache.ORGSUBSCRIPTIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgSubscription by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrgSubscriptionDto>
                {
                    Data = new List<OrgSubscriptionDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<OrgSubscription> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderSubscriptionID) && x.ProviderSubscriptionID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrgSubscriptionID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrgSubscriptionDto>>(pagedEntities);
            return new PagedResult<OrgSubscriptionDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrgSubscriptionDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<OrgSubscription> entities = cache.Get<IEnumerable<OrgSubscription>>(Cache.ORGSUBSCRIPTIONS.ToString()) ?? new List<OrgSubscription>();
                OrgSubscription? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrgSubscriptionID == ID);
                }
                else
                {
                    match = await this.orgsubscriptionRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrgSubscriptionDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgSubscription by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrgSubscriptionDto> CreateOrgSubscription(OrgSubscriptionDto orgsubscriptionDto)
        {
            OrgSubscription orgSubscription = new OrgSubscription();
            IEnumerable<OrgSubscription?> checkEntity;
            try
            {
                checkEntity = await this.orgsubscriptionRepository.Find(x => x.ProviderSubscriptionID!.ToLower().Trim() == orgsubscriptionDto.ProviderSubscriptionID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    orgSubscription = this.mapper.Map<OrgSubscription>(orgsubscriptionDto);
                    orgSubscription.OrgSubscriptionID = Guid.NewGuid();
                    orgSubscription.ProviderCustomerID = string.IsNullOrEmpty(orgsubscriptionDto.ProviderCustomerID) ? null : orgsubscriptionDto.ProviderCustomerID;
                    orgSubscription.ProviderSubscriptionID = string.IsNullOrEmpty(orgsubscriptionDto.ProviderSubscriptionID) ? null : orgsubscriptionDto.ProviderSubscriptionID;
                    orgSubscription.CapturedDate = DateTime.UtcNow;
                    orgSubscription = await orgsubscriptionRepository.Create(orgSubscription) ?? new OrgSubscription();
                    await orgsubscriptionRepository.Save();
                    cache.Remove(Cache.ORGSUBSCRIPTIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating OrgSubscription. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrgSubscriptionDto>(orgSubscription);
        }

        /// <inheritdoc/>
        public async Task<OrgSubscriptionDto?> UpdateOrgSubscription(Guid id, OrgSubscriptionDto orgsubscriptionDto)
        {
            try
            {
                var existing = await this.orgsubscriptionRepository.GetByID(id);
                if (existing == null)
                    return null;
                OrgSubscription orgSubscription = this.mapper.Map<OrgSubscription>(orgsubscriptionDto);
                orgSubscription = await orgsubscriptionRepository.Update(orgSubscription) ?? new OrgSubscription();
                await orgsubscriptionRepository.Save();
                cache.Remove(Cache.ORGSUBSCRIPTIONS.ToString());
                orgsubscriptionDto = this.mapper.Map<OrgSubscriptionDto>(orgSubscription);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating OrgSubscription. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return orgsubscriptionDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrgSubscription(Guid ID)
        {
            try
            {
                var orgSubscription = await this.orgsubscriptionRepository.GetByID(ID);
                if (orgSubscription == null)
                    throw new KeyNotFoundException("OrgSubscription with the specified ID was not found.");
                await orgsubscriptionRepository.Delete(orgSubscription);
                await orgsubscriptionRepository.Save();
                cache.Remove(Cache.ORGSUBSCRIPTIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting OrgSubscription . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrgSubscriptionDto?> UpdateOrgSubscriptionStatus(Guid id, string status)
        {
            var orgSubscription = await orgsubscriptionRepository.GetByID(id);
            if (orgSubscription == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                orgSubscription.Status = "Pending";
            }
            else
            {
                orgSubscription.Status = status;
            }

            await orgsubscriptionRepository.Update(orgSubscription);
            await orgsubscriptionRepository.Save();
            cache.Remove(Cache.ORGSUBSCRIPTIONS.ToString());
            return this.mapper.Map<OrgSubscriptionDto>(orgSubscription);
        }
    }
}