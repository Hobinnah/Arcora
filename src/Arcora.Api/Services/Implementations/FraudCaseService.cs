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
    public class FraudCaseService : IFraudCaseService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<FraudCaseService> logger;
        private readonly IFraudCaseRepository fraudcaseRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public FraudCaseService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<FraudCaseService> logger, IFraudCaseRepository fraudcaseRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.fraudcaseRepository = fraudcaseRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<FraudCaseDto>> GetAll(Paging paging)
        {
            IEnumerable<FraudCase> entities;
            try
            {
                entities = cache.Get<IEnumerable<FraudCase>>(Cache.FRAUDCASES.ToString()) ?? new List<FraudCase>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.fraudcaseRepository.GetFraudCaseAsync())?.Where(x => x != null) ?? new List<FraudCase>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<FraudCase>>(Cache.FRAUDCASES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching FraudCase by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<FraudCaseDto>
                {
                    Data = new List<FraudCaseDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<FraudCase> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Reason) && x.Reason.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.FraudCaseID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<FraudCaseDto>>(pagedEntities);
            return new PagedResult<FraudCaseDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<FraudCaseDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<FraudCase> entities = cache.Get<IEnumerable<FraudCase>>(Cache.FRAUDCASES.ToString()) ?? new List<FraudCase>();
                FraudCase? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.FraudCaseID == ID);
                }
                else
                {
                    match = await this.fraudcaseRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<FraudCaseDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching FraudCase by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FraudCaseDto> CreateFraudCase(FraudCaseDto fraudcaseDto)
        {
            FraudCase fraudCase = new FraudCase();
            IEnumerable<FraudCase?> checkEntity;
            try
            {
                checkEntity = await this.fraudcaseRepository.Find(x => x.Reason!.ToLower().Trim() == fraudcaseDto.Reason!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    fraudCase = this.mapper.Map<FraudCase>(fraudcaseDto);
                    fraudCase.FraudCaseID = Guid.NewGuid();
                    fraudCase.TenantID = fraudcaseDto.TenantID == Guid.Empty ? null : fraudcaseDto.TenantID;
                    fraudCase.OrganizationID = fraudcaseDto.OrganizationID == Guid.Empty ? null : fraudcaseDto.OrganizationID;
                    fraudCase.LeaseID = fraudcaseDto.LeaseID == Guid.Empty ? null : fraudcaseDto.LeaseID;
                    fraudCase.LeaseRenewalID = fraudcaseDto.LeaseRenewalID == Guid.Empty ? null : fraudcaseDto.LeaseRenewalID;
                    fraudCase.PaymentIntentID = fraudcaseDto.PaymentIntentID == Guid.Empty ? null : fraudcaseDto.PaymentIntentID;
                    fraudCase.PaymentID = fraudcaseDto.PaymentID == Guid.Empty ? null : fraudcaseDto.PaymentID;
                    fraudCase.ChargebackID = fraudcaseDto.ChargebackID == Guid.Empty ? null : fraudcaseDto.ChargebackID;
                    fraudCase.CapturedDate = DateTime.UtcNow;
                    fraudCase = await fraudcaseRepository.Create(fraudCase) ?? new FraudCase();
                    await fraudcaseRepository.Save();
                    cache.Remove(Cache.FRAUDCASES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating FraudCase. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<FraudCaseDto>(fraudCase);
        }

        /// <inheritdoc/>
        public async Task<FraudCaseDto?> UpdateFraudCase(Guid id, FraudCaseDto fraudcaseDto)
        {
            try
            {
                var existing = await this.fraudcaseRepository.GetByID(id);
                if (existing == null)
                    return null;
                FraudCase fraudCase = this.mapper.Map<FraudCase>(fraudcaseDto);
                fraudCase = await fraudcaseRepository.Update(fraudCase) ?? new FraudCase();
                await fraudcaseRepository.Save();
                cache.Remove(Cache.FRAUDCASES.ToString());
                fraudcaseDto = this.mapper.Map<FraudCaseDto>(fraudCase);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating FraudCase. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return fraudcaseDto;
        }

        /// <inheritdoc/>
        public async Task DeleteFraudCase(Guid ID)
        {
            try
            {
                var fraudCase = await this.fraudcaseRepository.GetByID(ID);
                if (fraudCase == null)
                    throw new KeyNotFoundException("FraudCase with the specified ID was not found.");
                await fraudcaseRepository.Delete(fraudCase);
                await fraudcaseRepository.Save();
                cache.Remove(Cache.FRAUDCASES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting FraudCase . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FraudCaseDto?> UpdateFraudCaseStatus(Guid id, string status)
        {
            var fraudCase = await fraudcaseRepository.GetByID(id);
            if (fraudCase == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                fraudCase.Status = "Pending";
            }
            else
            {
                fraudCase.Status = status;
            }

            await fraudcaseRepository.Update(fraudCase);
            await fraudcaseRepository.Save();
            cache.Remove(Cache.FRAUDCASES.ToString());
            return this.mapper.Map<FraudCaseDto>(fraudCase);
        }
    }
}