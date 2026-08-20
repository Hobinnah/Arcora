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
    public class PaymentProviderEventService : IPaymentProviderEventService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentProviderEventService> logger;
        private readonly IPaymentProviderEventRepository paymentprovidereventRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentProviderEventService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentProviderEventService> logger, IPaymentProviderEventRepository paymentprovidereventRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentprovidereventRepository = paymentprovidereventRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentProviderEventDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentProviderEvent> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentProviderEvent>>(Cache.PAYMENTPROVIDEREVENTS.ToString()) ?? new List<PaymentProviderEvent>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentprovidereventRepository.GetAll())?.Where(x => x != null).Cast<PaymentProviderEvent>().ToList() ?? new List<PaymentProviderEvent>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentProviderEvent>>(Cache.PAYMENTPROVIDEREVENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentProviderEvent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentProviderEventDto>
                {
                    Data = new List<PaymentProviderEventDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentProviderEvent> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderEventID) && x.ProviderEventID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentProviderEventID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentProviderEventDto>>(pagedEntities);
            return new PagedResult<PaymentProviderEventDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentProviderEventDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<PaymentProviderEvent> entities = cache.Get<IEnumerable<PaymentProviderEvent>>(Cache.PAYMENTPROVIDEREVENTS.ToString()) ?? new List<PaymentProviderEvent>();
                PaymentProviderEvent? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentProviderEventID == ID);
                }
                else
                {
                    match = await this.paymentprovidereventRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentProviderEventDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentProviderEvent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentProviderEventDto> CreatePaymentProviderEvent(PaymentProviderEventDto paymentprovidereventDto)
        {
            PaymentProviderEvent paymentProviderEvent = new PaymentProviderEvent();
            IEnumerable<PaymentProviderEvent?> checkEntity;
            try
            {
                checkEntity = await this.paymentprovidereventRepository.Find(x => x.ProviderEventID!.ToLower().Trim() == paymentprovidereventDto.ProviderEventID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentProviderEvent = this.mapper.Map<PaymentProviderEvent>(paymentprovidereventDto);
                    paymentProviderEvent.PaymentProviderEventID = Guid.NewGuid();
                    paymentProviderEvent.CapturedDate = DateTime.UtcNow;
                    paymentProviderEvent = await paymentprovidereventRepository.Create(paymentProviderEvent) ?? new PaymentProviderEvent();
                    await paymentprovidereventRepository.Save();
                    cache.Remove(Cache.PAYMENTPROVIDEREVENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentProviderEvent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentProviderEventDto>(paymentProviderEvent);
        }

        /// <inheritdoc/>
        public async Task<PaymentProviderEventDto?> UpdatePaymentProviderEvent(Guid id, PaymentProviderEventDto paymentprovidereventDto)
        {
            try
            {
                var existing = await this.paymentprovidereventRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentProviderEvent paymentProviderEvent = this.mapper.Map<PaymentProviderEvent>(paymentprovidereventDto);
                paymentProviderEvent = await paymentprovidereventRepository.Update(paymentProviderEvent) ?? new PaymentProviderEvent();
                await paymentprovidereventRepository.Save();
                cache.Remove(Cache.PAYMENTPROVIDEREVENTS.ToString());
                paymentprovidereventDto = this.mapper.Map<PaymentProviderEventDto>(paymentProviderEvent);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentProviderEvent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentprovidereventDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentProviderEvent(Guid ID)
        {
            try
            {
                var paymentProviderEvent = await this.paymentprovidereventRepository.GetByID(ID);
                if (paymentProviderEvent == null)
                    throw new KeyNotFoundException("PaymentProviderEvent with the specified ID was not found.");
                await paymentprovidereventRepository.Delete(paymentProviderEvent);
                await paymentprovidereventRepository.Save();
                cache.Remove(Cache.PAYMENTPROVIDEREVENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentProviderEvent . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}