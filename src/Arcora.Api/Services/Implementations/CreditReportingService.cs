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
    public class CreditReportingService : ICreditReportingService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CreditReportingService> logger;
        private readonly ICreditReportingRepository creditreportingRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public CreditReportingService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<CreditReportingService> logger, ICreditReportingRepository creditreportingRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.creditreportingRepository = creditreportingRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<CreditReportingDto>> GetAll(Paging paging)
        {
            IEnumerable<CreditReporting> entities;
            try
            {
                entities = cache.Get<IEnumerable<CreditReporting>>(Cache.CREDITREPORTINGS.ToString()) ?? new List<CreditReporting>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.creditreportingRepository.GetCreditReportingAsync())?.Where(x => x != null) ?? new List<CreditReporting>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<CreditReporting>>(Cache.CREDITREPORTINGS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReporting by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<CreditReportingDto>
                {
                    Data = new List<CreditReportingDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<CreditReporting> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderResponse) && x.ProviderResponse.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CreditReportingID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<CreditReportingDto>>(pagedEntities);
            return new PagedResult<CreditReportingDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<CreditReportingDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<CreditReporting> entities = cache.Get<IEnumerable<CreditReporting>>(Cache.CREDITREPORTINGS.ToString()) ?? new List<CreditReporting>();
                CreditReporting? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.CreditReportingID == ID);
                }
                else
                {
                    match = await this.creditreportingRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<CreditReportingDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReporting by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CreditReportingDto> CreateCreditReporting(CreditReportingDto creditreportingDto)
        {
            CreditReporting creditReporting = new CreditReporting();
            IEnumerable<CreditReporting?> checkEntity;
            try
            {
                checkEntity = await this.creditreportingRepository.Find(x => x.ProviderResponse!.ToLower().Trim() == creditreportingDto.ProviderResponse!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    creditReporting = this.mapper.Map<CreditReporting>(creditreportingDto);
                    creditReporting.CreditReportingID = Guid.NewGuid();
                    creditReporting.InvoiceMasterID = creditreportingDto.InvoiceMasterID == Guid.Empty ? null : creditreportingDto.InvoiceMasterID;
                    creditReporting.PaymentID = creditreportingDto.PaymentID == Guid.Empty ? null : creditreportingDto.PaymentID;
                    creditReporting.ProviderReferenceID = string.IsNullOrEmpty(creditreportingDto.ProviderReferenceID) ? null : creditreportingDto.ProviderReferenceID;
                    creditReporting.CapturedDate = DateTime.UtcNow;
                    creditReporting = await creditreportingRepository.Create(creditReporting) ?? new CreditReporting();
                    await creditreportingRepository.Save();
                    cache.Remove(Cache.CREDITREPORTINGS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating CreditReporting. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<CreditReportingDto>(creditReporting);
        }

        /// <inheritdoc/>
        public async Task<CreditReportingDto?> UpdateCreditReporting(Guid id, CreditReportingDto creditreportingDto)
        {
            try
            {
                var existing = await this.creditreportingRepository.GetByID(id);
                if (existing == null)
                    return null;
                CreditReporting creditReporting = this.mapper.Map<CreditReporting>(creditreportingDto);
                creditReporting = await creditreportingRepository.Update(creditReporting) ?? new CreditReporting();
                await creditreportingRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGS.ToString());
                creditreportingDto = this.mapper.Map<CreditReportingDto>(creditReporting);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating CreditReporting. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return creditreportingDto;
        }

        /// <inheritdoc/>
        public async Task DeleteCreditReporting(Guid ID)
        {
            try
            {
                var creditReporting = await this.creditreportingRepository.GetByID(ID);
                if (creditReporting == null)
                    throw new KeyNotFoundException("CreditReporting with the specified ID was not found.");
                await creditreportingRepository.Delete(creditReporting);
                await creditreportingRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting CreditReporting . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}