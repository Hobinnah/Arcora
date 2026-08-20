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
    public class DisputeService : IDisputeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<DisputeService> logger;
        private readonly IDisputeRepository disputeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public DisputeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<DisputeService> logger, IDisputeRepository disputeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.disputeRepository = disputeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<DisputeDto>> GetAll(Paging paging)
        {
            IEnumerable<Dispute> entities;
            try
            {
                entities = cache.Get<IEnumerable<Dispute>>(Cache.DISPUTES.ToString()) ?? new List<Dispute>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.disputeRepository.GetDisputeAsync())?.Where(x => x != null) ?? new List<Dispute>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Dispute>>(Cache.DISPUTES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Dispute by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<DisputeDto>
                {
                    Data = new List<DisputeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Dispute> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.DisputeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<DisputeDto>>(pagedEntities);
            return new PagedResult<DisputeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<DisputeDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Dispute> entities = cache.Get<IEnumerable<Dispute>>(Cache.DISPUTES.ToString()) ?? new List<Dispute>();
                Dispute? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.DisputeID == ID);
                }
                else
                {
                    match = await this.disputeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<DisputeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Dispute by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DisputeDto> CreateDispute(DisputeDto disputeDto)
        {
            Dispute dispute = new Dispute();
            IEnumerable<Dispute?> checkEntity;
            try
            {
                checkEntity = await this.disputeRepository.Find(x => x.Title!.ToLower().Trim() == disputeDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    dispute = this.mapper.Map<Dispute>(disputeDto);
                    dispute.DisputeID = Guid.NewGuid();
                    dispute.LeaseID = disputeDto.LeaseID == Guid.Empty ? null : disputeDto.LeaseID;
                    dispute.LeaseRenewalID = disputeDto.LeaseRenewalID == Guid.Empty ? null : disputeDto.LeaseRenewalID;
                    dispute.InvoiceMasterID = disputeDto.InvoiceMasterID == Guid.Empty ? null : disputeDto.InvoiceMasterID;
                    dispute.PaymentID = disputeDto.PaymentID == Guid.Empty ? null : disputeDto.PaymentID;
                    dispute.ChargebackID = disputeDto.ChargebackID == Guid.Empty ? null : disputeDto.ChargebackID;
                    dispute.MaintenanceRequestID = disputeDto.MaintenanceRequestID == Guid.Empty ? null : disputeDto.MaintenanceRequestID;
                    dispute.CapturedDate = DateTime.UtcNow;
                    dispute = await disputeRepository.Create(dispute) ?? new Dispute();
                    await disputeRepository.Save();
                    cache.Remove(Cache.DISPUTES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Dispute. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<DisputeDto>(dispute);
        }

        /// <inheritdoc/>
        public async Task<DisputeDto?> UpdateDispute(Guid id, DisputeDto disputeDto)
        {
            try
            {
                var existing = await this.disputeRepository.GetByID(id);
                if (existing == null)
                    return null;
                Dispute dispute = this.mapper.Map<Dispute>(disputeDto);
                dispute = await disputeRepository.Update(dispute) ?? new Dispute();
                await disputeRepository.Save();
                cache.Remove(Cache.DISPUTES.ToString());
                disputeDto = this.mapper.Map<DisputeDto>(dispute);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Dispute. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return disputeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteDispute(Guid ID)
        {
            try
            {
                var dispute = await this.disputeRepository.GetByID(ID);
                if (dispute == null)
                    throw new KeyNotFoundException("Dispute with the specified ID was not found.");
                await disputeRepository.Delete(dispute);
                await disputeRepository.Save();
                cache.Remove(Cache.DISPUTES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Dispute . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<DisputeDto?> UpdateDisputeStatus(Guid id, string status)
        {
            var dispute = await disputeRepository.GetByID(id);
            if (dispute == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                dispute.Status = "Pending";
            }
            else
            {
                dispute.Status = status;
            }

            await disputeRepository.Update(dispute);
            await disputeRepository.Save();
            cache.Remove(Cache.DISPUTES.ToString());
            return this.mapper.Map<DisputeDto>(dispute);
        }
    }
}