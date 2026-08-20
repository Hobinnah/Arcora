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
    public class PaymentAllocationService : IPaymentAllocationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentAllocationService> logger;
        private readonly IPaymentAllocationRepository paymentallocationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentAllocationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentAllocationService> logger, IPaymentAllocationRepository paymentallocationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentallocationRepository = paymentallocationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentAllocationDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentAllocation> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentAllocation>>(Cache.PAYMENTALLOCATIONS.ToString()) ?? new List<PaymentAllocation>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentallocationRepository.GetPaymentAllocationAsync())?.Where(x => x != null) ?? new List<PaymentAllocation>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentAllocation>>(Cache.PAYMENTALLOCATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentAllocation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentAllocationDto>
                {
                    Data = new List<PaymentAllocationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentAllocation> filteredEntities = entities!;
            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentAllocationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentAllocationDto>>(pagedEntities);
            return new PagedResult<PaymentAllocationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentAllocationDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<PaymentAllocation> entities = cache.Get<IEnumerable<PaymentAllocation>>(Cache.PAYMENTALLOCATIONS.ToString()) ?? new List<PaymentAllocation>();
                PaymentAllocation? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentAllocationID == ID);
                }
                else
                {
                    match = await this.paymentallocationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentAllocationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentAllocation by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentAllocationDto> CreatePaymentAllocation(PaymentAllocationDto paymentallocationDto)
        {
            PaymentAllocation paymentAllocation = new PaymentAllocation();
            IEnumerable<PaymentAllocation?> checkEntity;
            try
            {
                checkEntity = await this.paymentallocationRepository.Find(x => x.AllocationType!.ToLower().Trim() == paymentallocationDto.AllocationType!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentAllocation = this.mapper.Map<PaymentAllocation>(paymentallocationDto);
                    paymentAllocation.InvoiceDetailID = paymentallocationDto.InvoiceDetailID == Guid.Empty ? null : paymentallocationDto.InvoiceDetailID;
                    paymentAllocation.CapturedDate = DateTime.UtcNow;
                    paymentAllocation = await paymentallocationRepository.Create(paymentAllocation) ?? new PaymentAllocation();
                    await paymentallocationRepository.Save();
                    cache.Remove(Cache.PAYMENTALLOCATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentAllocation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentAllocationDto>(paymentAllocation);
        }

        /// <inheritdoc/>
        public async Task<PaymentAllocationDto?> UpdatePaymentAllocation(long id, PaymentAllocationDto paymentallocationDto)
        {
            try
            {
                var existing = await this.paymentallocationRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentAllocation paymentAllocation = this.mapper.Map<PaymentAllocation>(paymentallocationDto);
                paymentAllocation = await paymentallocationRepository.Update(paymentAllocation) ?? new PaymentAllocation();
                await paymentallocationRepository.Save();
                cache.Remove(Cache.PAYMENTALLOCATIONS.ToString());
                paymentallocationDto = this.mapper.Map<PaymentAllocationDto>(paymentAllocation);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentAllocation. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentallocationDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentAllocation(long ID)
        {
            try
            {
                var paymentAllocation = await this.paymentallocationRepository.GetByID(ID);
                if (paymentAllocation == null)
                    throw new KeyNotFoundException("PaymentAllocation with the specified ID was not found.");
                await paymentallocationRepository.Delete(paymentAllocation);
                await paymentallocationRepository.Save();
                cache.Remove(Cache.PAYMENTALLOCATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentAllocation . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}