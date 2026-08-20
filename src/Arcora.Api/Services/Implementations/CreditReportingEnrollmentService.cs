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
    public class CreditReportingEnrollmentService : ICreditReportingEnrollmentService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CreditReportingEnrollmentService> logger;
        private readonly ICreditReportingEnrollmentRepository creditreportingenrollmentRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public CreditReportingEnrollmentService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<CreditReportingEnrollmentService> logger, ICreditReportingEnrollmentRepository creditreportingenrollmentRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.creditreportingenrollmentRepository = creditreportingenrollmentRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<CreditReportingEnrollmentDto>> GetAll(Paging paging)
        {
            IEnumerable<CreditReportingEnrollment> entities;
            try
            {
                entities = cache.Get<IEnumerable<CreditReportingEnrollment>>(Cache.CREDITREPORTINGENROLLMENTS.ToString()) ?? new List<CreditReportingEnrollment>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.creditreportingenrollmentRepository.GetCreditReportingEnrollmentAsync())?.Where(x => x != null) ?? new List<CreditReportingEnrollment>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<CreditReportingEnrollment>>(Cache.CREDITREPORTINGENROLLMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReportingEnrollment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<CreditReportingEnrollmentDto>
                {
                    Data = new List<CreditReportingEnrollmentDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<CreditReportingEnrollment> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderName) && x.ProviderName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CreditReportingEnrollmentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<CreditReportingEnrollmentDto>>(pagedEntities);
            return new PagedResult<CreditReportingEnrollmentDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<CreditReportingEnrollmentDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<CreditReportingEnrollment> entities = cache.Get<IEnumerable<CreditReportingEnrollment>>(Cache.CREDITREPORTINGENROLLMENTS.ToString()) ?? new List<CreditReportingEnrollment>();
                CreditReportingEnrollment? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.CreditReportingEnrollmentID == ID);
                }
                else
                {
                    match = await this.creditreportingenrollmentRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<CreditReportingEnrollmentDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CreditReportingEnrollment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CreditReportingEnrollmentDto> CreateCreditReportingEnrollment(CreditReportingEnrollmentDto creditreportingenrollmentDto)
        {
            CreditReportingEnrollment creditReportingEnrollment = new CreditReportingEnrollment();
            IEnumerable<CreditReportingEnrollment?> checkEntity;
            try
            {
                checkEntity = await this.creditreportingenrollmentRepository.Find(x => x.ProviderName!.ToLower().Trim() == creditreportingenrollmentDto.ProviderName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    creditReportingEnrollment = this.mapper.Map<CreditReportingEnrollment>(creditreportingenrollmentDto);
                    creditReportingEnrollment.CreditReportingEnrollmentID = Guid.NewGuid();
                    creditReportingEnrollment.LeaseID = creditreportingenrollmentDto.LeaseID == Guid.Empty ? null : creditreportingenrollmentDto.LeaseID;
                    creditReportingEnrollment.LeaseRenewalID = creditreportingenrollmentDto.LeaseRenewalID == Guid.Empty ? null : creditreportingenrollmentDto.LeaseRenewalID;
                    creditReportingEnrollment.ProviderReferenceID = string.IsNullOrEmpty(creditreportingenrollmentDto.ProviderReferenceID) ? null : creditreportingenrollmentDto.ProviderReferenceID;
                    creditReportingEnrollment.CapturedDate = DateTime.UtcNow;
                    creditReportingEnrollment = await creditreportingenrollmentRepository.Create(creditReportingEnrollment) ?? new CreditReportingEnrollment();
                    await creditreportingenrollmentRepository.Save();
                    cache.Remove(Cache.CREDITREPORTINGENROLLMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating CreditReportingEnrollment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<CreditReportingEnrollmentDto>(creditReportingEnrollment);
        }

        /// <inheritdoc/>
        public async Task<CreditReportingEnrollmentDto?> UpdateCreditReportingEnrollment(Guid id, CreditReportingEnrollmentDto creditreportingenrollmentDto)
        {
            try
            {
                var existing = await this.creditreportingenrollmentRepository.GetByID(id);
                if (existing == null)
                    return null;
                CreditReportingEnrollment creditReportingEnrollment = this.mapper.Map<CreditReportingEnrollment>(creditreportingenrollmentDto);
                creditReportingEnrollment = await creditreportingenrollmentRepository.Update(creditReportingEnrollment) ?? new CreditReportingEnrollment();
                await creditreportingenrollmentRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGENROLLMENTS.ToString());
                creditreportingenrollmentDto = this.mapper.Map<CreditReportingEnrollmentDto>(creditReportingEnrollment);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating CreditReportingEnrollment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return creditreportingenrollmentDto;
        }

        /// <inheritdoc/>
        public async Task DeleteCreditReportingEnrollment(Guid ID)
        {
            try
            {
                var creditReportingEnrollment = await this.creditreportingenrollmentRepository.GetByID(ID);
                if (creditReportingEnrollment == null)
                    throw new KeyNotFoundException("CreditReportingEnrollment with the specified ID was not found.");
                await creditreportingenrollmentRepository.Delete(creditReportingEnrollment);
                await creditreportingenrollmentRepository.Save();
                cache.Remove(Cache.CREDITREPORTINGENROLLMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting CreditReportingEnrollment . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CreditReportingEnrollmentDto?> UpdateCreditReportingEnrollmentStatus(Guid id, string status)
        {
            var creditReportingEnrollment = await creditreportingenrollmentRepository.GetByID(id);
            if (creditReportingEnrollment == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                creditReportingEnrollment.Status = "Pending";
            }
            else
            {
                creditReportingEnrollment.Status = status;
            }

            await creditreportingenrollmentRepository.Update(creditReportingEnrollment);
            await creditreportingenrollmentRepository.Save();
            cache.Remove(Cache.CREDITREPORTINGENROLLMENTS.ToString());
            return this.mapper.Map<CreditReportingEnrollmentDto>(creditReportingEnrollment);
        }
    }
}