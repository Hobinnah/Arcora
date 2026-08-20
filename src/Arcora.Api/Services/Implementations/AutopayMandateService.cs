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
    public class AutopayMandateService : IAutopayMandateService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<AutopayMandateService> logger;
        private readonly IAutopayMandateRepository autopaymandateRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AutopayMandateService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<AutopayMandateService> logger, IAutopayMandateRepository autopaymandateRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.autopaymandateRepository = autopaymandateRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AutopayMandateDto>> GetAll(Paging paging)
        {
            IEnumerable<AutopayMandate> entities;
            try
            {
                entities = cache.Get<IEnumerable<AutopayMandate>>(Cache.AUTOPAYMANDATES.ToString()) ?? new List<AutopayMandate>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.autopaymandateRepository.GetAutopayMandateAsync())?.Where(x => x != null) ?? new List<AutopayMandate>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<AutopayMandate>>(Cache.AUTOPAYMANDATES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AutopayMandate by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AutopayMandateDto>
                {
                    Data = new List<AutopayMandateDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<AutopayMandate> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderName) && x.ProviderName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AutopayMandateID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AutopayMandateDto>>(pagedEntities);
            return new PagedResult<AutopayMandateDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AutopayMandateDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<AutopayMandate> entities = cache.Get<IEnumerable<AutopayMandate>>(Cache.AUTOPAYMANDATES.ToString()) ?? new List<AutopayMandate>();
                AutopayMandate? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.AutopayMandateID == ID);
                }
                else
                {
                    match = await this.autopaymandateRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<AutopayMandateDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AutopayMandate by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AutopayMandateDto> CreateAutopayMandate(AutopayMandateDto autopaymandateDto)
        {
            AutopayMandate autopayMandate = new AutopayMandate();
            IEnumerable<AutopayMandate?> checkEntity;
            try
            {
                checkEntity = await this.autopaymandateRepository.Find(x => x.ProviderName!.ToLower().Trim() == autopaymandateDto.ProviderName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    autopayMandate = this.mapper.Map<AutopayMandate>(autopaymandateDto);
                    autopayMandate.AutopayMandateID = Guid.NewGuid();
                    autopayMandate.LeaseRenewalID = autopaymandateDto.LeaseRenewalID == Guid.Empty ? null : autopaymandateDto.LeaseRenewalID;
                    autopayMandate.ProviderMandateID = string.IsNullOrEmpty(autopaymandateDto.ProviderMandateID) ? null : autopaymandateDto.ProviderMandateID;
                    autopayMandate.CapturedDate = DateTime.UtcNow;
                    autopayMandate = await autopaymandateRepository.Create(autopayMandate) ?? new AutopayMandate();
                    await autopaymandateRepository.Save();
                    cache.Remove(Cache.AUTOPAYMANDATES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating AutopayMandate. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AutopayMandateDto>(autopayMandate);
        }

        /// <inheritdoc/>
        public async Task<AutopayMandateDto?> UpdateAutopayMandate(Guid id, AutopayMandateDto autopaymandateDto)
        {
            try
            {
                var existing = await this.autopaymandateRepository.GetByID(id);
                if (existing == null)
                    return null;
                AutopayMandate autopayMandate = this.mapper.Map<AutopayMandate>(autopaymandateDto);
                autopayMandate = await autopaymandateRepository.Update(autopayMandate) ?? new AutopayMandate();
                await autopaymandateRepository.Save();
                cache.Remove(Cache.AUTOPAYMANDATES.ToString());
                autopaymandateDto = this.mapper.Map<AutopayMandateDto>(autopayMandate);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating AutopayMandate. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return autopaymandateDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAutopayMandate(Guid ID)
        {
            try
            {
                var autopayMandate = await this.autopaymandateRepository.GetByID(ID);
                if (autopayMandate == null)
                    throw new KeyNotFoundException("AutopayMandate with the specified ID was not found.");
                await autopaymandateRepository.Delete(autopayMandate);
                await autopaymandateRepository.Save();
                cache.Remove(Cache.AUTOPAYMANDATES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting AutopayMandate . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AutopayMandateDto?> UpdateAutopayMandateStatus(Guid id, string status)
        {
            var autopayMandate = await autopaymandateRepository.GetByID(id);
            if (autopayMandate == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                autopayMandate.Status = "Pending";
            }
            else
            {
                autopayMandate.Status = status;
            }

            await autopaymandateRepository.Update(autopayMandate);
            await autopaymandateRepository.Save();
            cache.Remove(Cache.AUTOPAYMANDATES.ToString());
            return this.mapper.Map<AutopayMandateDto>(autopayMandate);
        }
    }
}