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
    public class RefundService : IRefundService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<RefundService> logger;
        private readonly IRefundRepository refundRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public RefundService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<RefundService> logger, IRefundRepository refundRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.refundRepository = refundRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<RefundDto>> GetAll(Paging paging)
        {
            IEnumerable<Refund> entities;
            try
            {
                entities = cache.Get<IEnumerable<Refund>>(Cache.REFUNDS.ToString()) ?? new List<Refund>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.refundRepository.GetRefundAsync())?.Where(x => x != null) ?? new List<Refund>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Refund>>(Cache.REFUNDS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Refund by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<RefundDto>
                {
                    Data = new List<RefundDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Refund> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderRefundID) && x.ProviderRefundID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.RefundID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<RefundDto>>(pagedEntities);
            return new PagedResult<RefundDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<RefundDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Refund> entities = cache.Get<IEnumerable<Refund>>(Cache.REFUNDS.ToString()) ?? new List<Refund>();
                Refund? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.RefundID == ID);
                }
                else
                {
                    match = await this.refundRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<RefundDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Refund by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RefundDto> CreateRefund(RefundDto refundDto)
        {
            Refund refund = new Refund();
            IEnumerable<Refund?> checkEntity;
            try
            {
                checkEntity = await this.refundRepository.Find(x => x.ProviderRefundID!.ToLower().Trim() == refundDto.ProviderRefundID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    refund = this.mapper.Map<Refund>(refundDto);
                    refund.RefundID = Guid.NewGuid();
                    refund.CapturedDate = DateTime.UtcNow;
                    refund = await refundRepository.Create(refund) ?? new Refund();
                    await refundRepository.Save();
                    cache.Remove(Cache.REFUNDS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Refund. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<RefundDto>(refund);
        }

        /// <inheritdoc/>
        public async Task<RefundDto?> UpdateRefund(Guid id, RefundDto refundDto)
        {
            try
            {
                var existing = await this.refundRepository.GetByID(id);
                if (existing == null)
                    return null;
                Refund refund = this.mapper.Map<Refund>(refundDto);
                refund = await refundRepository.Update(refund) ?? new Refund();
                await refundRepository.Save();
                cache.Remove(Cache.REFUNDS.ToString());
                refundDto = this.mapper.Map<RefundDto>(refund);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Refund. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return refundDto;
        }

        /// <inheritdoc/>
        public async Task DeleteRefund(Guid ID)
        {
            try
            {
                var refund = await this.refundRepository.GetByID(ID);
                if (refund == null)
                    throw new KeyNotFoundException("Refund with the specified ID was not found.");
                await refundRepository.Delete(refund);
                await refundRepository.Save();
                cache.Remove(Cache.REFUNDS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Refund . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RefundDto?> UpdateRefundStatus(Guid id, string status)
        {
            var refund = await refundRepository.GetByID(id);
            if (refund == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                refund.Status = "Pending";
            }
            else
            {
                refund.Status = status;
            }

            await refundRepository.Update(refund);
            await refundRepository.Save();
            cache.Remove(Cache.REFUNDS.ToString());
            return this.mapper.Map<RefundDto>(refund);
        }
    }
}