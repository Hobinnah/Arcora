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
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentMethodService> logger;
        private readonly IPaymentMethodRepository paymentmethodRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentMethodService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentMethodService> logger, IPaymentMethodRepository paymentmethodRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentmethodRepository = paymentmethodRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentMethodDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentMethod> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentMethod>>(Cache.PAYMENTMETHODS.ToString()) ?? new List<PaymentMethod>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentmethodRepository.GetPaymentMethodAsync())?.Where(x => x != null) ?? new List<PaymentMethod>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentMethod>>(Cache.PAYMENTMETHODS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentMethod by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentMethodDto>
                {
                    Data = new List<PaymentMethodDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentMethod> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentMethodID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentMethodDto>>(pagedEntities);
            return new PagedResult<PaymentMethodDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentMethodDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<PaymentMethod> entities = cache.Get<IEnumerable<PaymentMethod>>(Cache.PAYMENTMETHODS.ToString()) ?? new List<PaymentMethod>();
                PaymentMethod? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentMethodID == ID);
                }
                else
                {
                    match = await this.paymentmethodRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentMethodDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentMethod by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentMethodDto> CreatePaymentMethod(PaymentMethodDto paymentmethodDto)
        {
            PaymentMethod paymentMethod = new PaymentMethod();
            IEnumerable<PaymentMethod?> checkEntity;
            try
            {
                checkEntity = await this.paymentmethodRepository.Find(x => x.DisplayName!.ToLower().Trim() == paymentmethodDto.DisplayName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentMethod = this.mapper.Map<PaymentMethod>(paymentmethodDto);
                    paymentMethod.PaymentMethodID = Guid.NewGuid();
                    paymentMethod.CapturedDate = DateTime.UtcNow;
                    paymentMethod = await paymentmethodRepository.Create(paymentMethod) ?? new PaymentMethod();
                    await paymentmethodRepository.Save();
                    cache.Remove(Cache.PAYMENTMETHODS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentMethod. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentMethodDto>(paymentMethod);
        }

        /// <inheritdoc/>
        public async Task<PaymentMethodDto?> UpdatePaymentMethod(Guid id, PaymentMethodDto paymentmethodDto)
        {
            try
            {
                var existing = await this.paymentmethodRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentMethod paymentMethod = this.mapper.Map<PaymentMethod>(paymentmethodDto);
                paymentMethod = await paymentmethodRepository.Update(paymentMethod) ?? new PaymentMethod();
                await paymentmethodRepository.Save();
                cache.Remove(Cache.PAYMENTMETHODS.ToString());
                paymentmethodDto = this.mapper.Map<PaymentMethodDto>(paymentMethod);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentMethod. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentmethodDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentMethod(Guid ID)
        {
            try
            {
                var paymentMethod = await this.paymentmethodRepository.GetByID(ID);
                if (paymentMethod == null)
                    throw new KeyNotFoundException("PaymentMethod with the specified ID was not found.");
                await paymentmethodRepository.Delete(paymentMethod);
                await paymentmethodRepository.Save();
                cache.Remove(Cache.PAYMENTMETHODS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentMethod . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}