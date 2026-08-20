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
    public class OrgPayoutAccountService : IOrgPayoutAccountService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrgPayoutAccountService> logger;
        private readonly IOrgPayoutAccountRepository orgpayoutaccountRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public OrgPayoutAccountService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<OrgPayoutAccountService> logger, IOrgPayoutAccountRepository orgpayoutaccountRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.orgpayoutaccountRepository = orgpayoutaccountRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrgPayoutAccountDto>> GetAll(Paging paging)
        {
            IEnumerable<OrgPayoutAccount> entities;
            try
            {
                entities = cache.Get<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString()) ?? new List<OrgPayoutAccount>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.orgpayoutaccountRepository.GetOrgPayoutAccountAsync())?.Where(x => x != null) ?? new List<OrgPayoutAccount>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgPayoutAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrgPayoutAccountDto>
                {
                    Data = new List<OrgPayoutAccountDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<OrgPayoutAccount> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderAccountID) && x.ProviderAccountID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrgPayoutAccountID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrgPayoutAccountDto>>(pagedEntities);
            return new PagedResult<OrgPayoutAccountDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<OrgPayoutAccount> entities = cache.Get<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString()) ?? new List<OrgPayoutAccount>();
                OrgPayoutAccount? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrgPayoutAccountID == ID);
                }
                else
                {
                    match = await this.orgpayoutaccountRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrgPayoutAccountDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgPayoutAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto> CreateOrgPayoutAccount(OrgPayoutAccountDto orgpayoutaccountDto)
        {
            OrgPayoutAccount orgPayoutAccount = new OrgPayoutAccount();
            IEnumerable<OrgPayoutAccount?> checkEntity;
            try
            {
                checkEntity = await this.orgpayoutaccountRepository.Find(x => x.ProviderAccountID!.ToLower().Trim() == orgpayoutaccountDto.ProviderAccountID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    orgPayoutAccount = this.mapper.Map<OrgPayoutAccount>(orgpayoutaccountDto);
                    orgPayoutAccount.CapturedDate = DateTime.UtcNow;
                    orgPayoutAccount = await orgpayoutaccountRepository.Create(orgPayoutAccount) ?? new OrgPayoutAccount();
                    await orgpayoutaccountRepository.Save();
                    cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating OrgPayoutAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrgPayoutAccountDto>(orgPayoutAccount);
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto?> UpdateOrgPayoutAccount(long id, OrgPayoutAccountDto orgpayoutaccountDto)
        {
            try
            {
                var existing = await this.orgpayoutaccountRepository.GetByID(id);
                if (existing == null)
                    return null;
                OrgPayoutAccount orgPayoutAccount = this.mapper.Map<OrgPayoutAccount>(orgpayoutaccountDto);
                orgPayoutAccount = await orgpayoutaccountRepository.Update(orgPayoutAccount) ?? new OrgPayoutAccount();
                await orgpayoutaccountRepository.Save();
                cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
                orgpayoutaccountDto = this.mapper.Map<OrgPayoutAccountDto>(orgPayoutAccount);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating OrgPayoutAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return orgpayoutaccountDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrgPayoutAccount(long ID)
        {
            try
            {
                var orgPayoutAccount = await this.orgpayoutaccountRepository.GetByID(ID);
                if (orgPayoutAccount == null)
                    throw new KeyNotFoundException("OrgPayoutAccount with the specified ID was not found.");
                await orgpayoutaccountRepository.Delete(orgPayoutAccount);
                await orgpayoutaccountRepository.Save();
                cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting OrgPayoutAccount . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}