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
    public class ChargebackService : IChargebackService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ChargebackService> logger;
        private readonly IChargebackRepository chargebackRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ChargebackService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ChargebackService> logger, IChargebackRepository chargebackRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.chargebackRepository = chargebackRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ChargebackDto>> GetAll(Paging paging)
        {
            IEnumerable<Chargeback> entities;
            try
            {
                entities = cache.Get<IEnumerable<Chargeback>>(Cache.CHARGEBACKS.ToString()) ?? new List<Chargeback>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.chargebackRepository.GetChargebackAsync())?.Where(x => x != null) ?? new List<Chargeback>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Chargeback>>(Cache.CHARGEBACKS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Chargeback by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ChargebackDto>
                {
                    Data = new List<ChargebackDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Chargeback> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderDisputeID) && x.ProviderDisputeID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ChargebackID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ChargebackDto>>(pagedEntities);
            return new PagedResult<ChargebackDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ChargebackDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Chargeback> entities = cache.Get<IEnumerable<Chargeback>>(Cache.CHARGEBACKS.ToString()) ?? new List<Chargeback>();
                Chargeback? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ChargebackID == ID);
                }
                else
                {
                    match = await this.chargebackRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ChargebackDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Chargeback by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ChargebackDto> CreateChargeback(ChargebackDto chargebackDto)
        {
            Chargeback chargeback = new Chargeback();
            IEnumerable<Chargeback?> checkEntity;
            try
            {
                checkEntity = await this.chargebackRepository.Find(x => x.ProviderDisputeID!.ToLower().Trim() == chargebackDto.ProviderDisputeID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    chargeback = this.mapper.Map<Chargeback>(chargebackDto);
                    chargeback.ChargebackID = Guid.NewGuid();
                    chargeback.TenantID = chargebackDto.TenantID == Guid.Empty ? null : chargebackDto.TenantID;
                    chargeback.OrganizationID = chargebackDto.OrganizationID == Guid.Empty ? null : chargebackDto.OrganizationID;
                    chargeback.CapturedDate = DateTime.UtcNow;
                    chargeback = await chargebackRepository.Create(chargeback) ?? new Chargeback();
                    await chargebackRepository.Save();
                    cache.Remove(Cache.CHARGEBACKS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Chargeback. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ChargebackDto>(chargeback);
        }

        /// <inheritdoc/>
        public async Task<ChargebackDto?> UpdateChargeback(Guid id, ChargebackDto chargebackDto)
        {
            try
            {
                var existing = await this.chargebackRepository.GetByID(id);
                if (existing == null)
                    return null;
                Chargeback chargeback = this.mapper.Map<Chargeback>(chargebackDto);
                chargeback = await chargebackRepository.Update(chargeback) ?? new Chargeback();
                await chargebackRepository.Save();
                cache.Remove(Cache.CHARGEBACKS.ToString());
                chargebackDto = this.mapper.Map<ChargebackDto>(chargeback);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Chargeback. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return chargebackDto;
        }

        /// <inheritdoc/>
        public async Task DeleteChargeback(Guid ID)
        {
            try
            {
                var chargeback = await this.chargebackRepository.GetByID(ID);
                if (chargeback == null)
                    throw new KeyNotFoundException("Chargeback with the specified ID was not found.");
                await chargebackRepository.Delete(chargeback);
                await chargebackRepository.Save();
                cache.Remove(Cache.CHARGEBACKS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Chargeback . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ChargebackDto?> UpdateChargebackStatus(Guid id, string status)
        {
            var chargeback = await chargebackRepository.GetByID(id);
            if (chargeback == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                chargeback.Status = "Pending";
            }
            else
            {
                chargeback.Status = status;
            }

            await chargebackRepository.Update(chargeback);
            await chargebackRepository.Save();
            cache.Remove(Cache.CHARGEBACKS.ToString());
            return this.mapper.Map<ChargebackDto>(chargeback);
        }
    }
}