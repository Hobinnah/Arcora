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
    public class PaymentIntentService : IPaymentIntentService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentIntentService> logger;
        private readonly IPaymentIntentRepository paymentintentRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentIntentService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentIntentService> logger, IPaymentIntentRepository paymentintentRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentintentRepository = paymentintentRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentIntentDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentIntent> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentIntent>>(Cache.PAYMENTINTENTS.ToString()) ?? new List<PaymentIntent>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentintentRepository.GetPaymentIntentAsync())?.Where(x => x != null) ?? new List<PaymentIntent>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentIntent>>(Cache.PAYMENTINTENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentIntent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentIntentDto>
                {
                    Data = new List<PaymentIntentDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentIntent> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderPaymentIntentID) && x.ProviderPaymentIntentID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentIntentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentIntentDto>>(pagedEntities);
            return new PagedResult<PaymentIntentDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<PaymentIntent> entities = cache.Get<IEnumerable<PaymentIntent>>(Cache.PAYMENTINTENTS.ToString()) ?? new List<PaymentIntent>();
                PaymentIntent? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentIntentID == ID);
                }
                else
                {
                    match = await this.paymentintentRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentIntentDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentIntent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto> CreatePaymentIntent(PaymentIntentDto paymentintentDto)
        {
            PaymentIntent paymentIntent = new PaymentIntent();
            IEnumerable<PaymentIntent?> checkEntity;
            try
            {
                checkEntity = await this.paymentintentRepository.Find(x => x.ProviderPaymentIntentID!.ToLower().Trim() == paymentintentDto.ProviderPaymentIntentID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentIntent = this.mapper.Map<PaymentIntent>(paymentintentDto);
                    paymentIntent.PaymentIntentID = Guid.NewGuid();
                    paymentIntent.InvoiceMasterID = paymentintentDto.InvoiceMasterID == Guid.Empty ? null : paymentintentDto.InvoiceMasterID;
                    paymentIntent.AutopayMandateID = paymentintentDto.AutopayMandateID == Guid.Empty ? null : paymentintentDto.AutopayMandateID;
                    paymentIntent.CapturedDate = DateTime.UtcNow;
                    paymentIntent = await paymentintentRepository.Create(paymentIntent) ?? new PaymentIntent();
                    await paymentintentRepository.Save();
                    cache.Remove(Cache.PAYMENTINTENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentIntent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentIntentDto>(paymentIntent);
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto?> UpdatePaymentIntent(Guid id, PaymentIntentDto paymentintentDto)
        {
            try
            {
                var existing = await this.paymentintentRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentIntent paymentIntent = this.mapper.Map<PaymentIntent>(paymentintentDto);
                paymentIntent = await paymentintentRepository.Update(paymentIntent) ?? new PaymentIntent();
                await paymentintentRepository.Save();
                cache.Remove(Cache.PAYMENTINTENTS.ToString());
                paymentintentDto = this.mapper.Map<PaymentIntentDto>(paymentIntent);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentIntent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentintentDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentIntent(Guid ID)
        {
            try
            {
                var paymentIntent = await this.paymentintentRepository.GetByID(ID);
                if (paymentIntent == null)
                    throw new KeyNotFoundException("PaymentIntent with the specified ID was not found.");
                await paymentintentRepository.Delete(paymentIntent);
                await paymentintentRepository.Save();
                cache.Remove(Cache.PAYMENTINTENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentIntent . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentIntentDto?> UpdatePaymentIntentStatus(Guid id, string status)
        {
            var paymentIntent = await paymentintentRepository.GetByID(id);
            if (paymentIntent == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                paymentIntent.Status = "Pending";
            }
            else
            {
                paymentIntent.Status = status;
            }

            await paymentintentRepository.Update(paymentIntent);
            await paymentintentRepository.Save();
            cache.Remove(Cache.PAYMENTINTENTS.ToString());
            return this.mapper.Map<PaymentIntentDto>(paymentIntent);
        }
    }
}