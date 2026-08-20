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
    public class SecurityDepositService : ISecurityDepositService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<SecurityDepositService> logger;
        private readonly ISecurityDepositRepository securitydepositRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public SecurityDepositService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<SecurityDepositService> logger, ISecurityDepositRepository securitydepositRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.securitydepositRepository = securitydepositRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<SecurityDepositDto>> GetAll(Paging paging)
        {
            IEnumerable<SecurityDeposit> entities;
            try
            {
                entities = cache.Get<IEnumerable<SecurityDeposit>>(Cache.SECURITYDEPOSITS.ToString()) ?? new List<SecurityDeposit>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.securitydepositRepository.GetSecurityDepositAsync())?.Where(x => x != null) ?? new List<SecurityDeposit>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<SecurityDeposit>>(Cache.SECURITYDEPOSITS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SecurityDeposit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<SecurityDepositDto>
                {
                    Data = new List<SecurityDepositDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<SecurityDeposit> filteredEntities = entities!;
            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.SecurityDepositID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<SecurityDepositDto>>(pagedEntities);
            return new PagedResult<SecurityDepositDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<SecurityDeposit> entities = cache.Get<IEnumerable<SecurityDeposit>>(Cache.SECURITYDEPOSITS.ToString()) ?? new List<SecurityDeposit>();
                SecurityDeposit? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.SecurityDepositID == ID);
                }
                else
                {
                    match = await this.securitydepositRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<SecurityDepositDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SecurityDeposit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositDto> CreateSecurityDeposit(SecurityDepositDto securitydepositDto)
        {
            SecurityDeposit securityDeposit = new SecurityDeposit();
            IEnumerable<SecurityDeposit?> checkEntity;
            try
            {
                checkEntity = await this.securitydepositRepository.Find(x => x.Currency!.ToLower().Trim() == securitydepositDto.Currency!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    securityDeposit = this.mapper.Map<SecurityDeposit>(securitydepositDto);
                    securityDeposit.SecurityDepositID = Guid.NewGuid();
                    securityDeposit.CapturedDate = DateTime.UtcNow;
                    securityDeposit = await securitydepositRepository.Create(securityDeposit) ?? new SecurityDeposit();
                    await securitydepositRepository.Save();
                    cache.Remove(Cache.SECURITYDEPOSITS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating SecurityDeposit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<SecurityDepositDto>(securityDeposit);
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositDto?> UpdateSecurityDeposit(Guid id, SecurityDepositDto securitydepositDto)
        {
            try
            {
                var existing = await this.securitydepositRepository.GetByID(id);
                if (existing == null)
                    return null;
                SecurityDeposit securityDeposit = this.mapper.Map<SecurityDeposit>(securitydepositDto);
                securityDeposit = await securitydepositRepository.Update(securityDeposit) ?? new SecurityDeposit();
                await securitydepositRepository.Save();
                cache.Remove(Cache.SECURITYDEPOSITS.ToString());
                securitydepositDto = this.mapper.Map<SecurityDepositDto>(securityDeposit);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating SecurityDeposit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return securitydepositDto;
        }

        /// <inheritdoc/>
        public async Task DeleteSecurityDeposit(Guid ID)
        {
            try
            {
                var securityDeposit = await this.securitydepositRepository.GetByID(ID);
                if (securityDeposit == null)
                    throw new KeyNotFoundException("SecurityDeposit with the specified ID was not found.");
                await securitydepositRepository.Delete(securityDeposit);
                await securitydepositRepository.Save();
                cache.Remove(Cache.SECURITYDEPOSITS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting SecurityDeposit . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositDto?> UpdateSecurityDepositStatus(Guid id, string status)
        {
            var securityDeposit = await securitydepositRepository.GetByID(id);
            if (securityDeposit == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                securityDeposit.Status = "Pending";
            }
            else
            {
                securityDeposit.Status = status;
            }

            await securitydepositRepository.Update(securityDeposit);
            await securitydepositRepository.Save();
            cache.Remove(Cache.SECURITYDEPOSITS.ToString());
            return this.mapper.Map<SecurityDepositDto>(securityDeposit);
        }
    }
}