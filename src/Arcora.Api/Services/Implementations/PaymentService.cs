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
    public class PaymentService : IPaymentService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentService> logger;
        private readonly IPaymentRepository paymentRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentService> logger, IPaymentRepository paymentRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentRepository = paymentRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentDto>> GetAll(Paging paging)
        {
            IEnumerable<Payment> entities;
            try
            {
                entities = cache.Get<IEnumerable<Payment>>(Cache.PAYMENTS.ToString()) ?? new List<Payment>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentRepository.GetPaymentAsync())?.Where(x => x != null) ?? new List<Payment>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Payment>>(Cache.PAYMENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Payment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentDto>
                {
                    Data = new List<PaymentDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Payment> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderChargeID) && x.ProviderChargeID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentDto>>(pagedEntities);
            return new PagedResult<PaymentDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Payment> entities = cache.Get<IEnumerable<Payment>>(Cache.PAYMENTS.ToString()) ?? new List<Payment>();
                Payment? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentID == ID);
                }
                else
                {
                    match = await this.paymentRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Payment by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentDto> CreatePayment(PaymentDto paymentDto)
        {
            Payment payment = new Payment();
            IEnumerable<Payment?> checkEntity;
            try
            {
                checkEntity = await this.paymentRepository.Find(x => x.ProviderChargeID!.ToLower().Trim() == paymentDto.ProviderChargeID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    payment = this.mapper.Map<Payment>(paymentDto);
                    payment.PaymentID = Guid.NewGuid();
                    payment.CapturedDate = DateTime.UtcNow;
                    payment = await paymentRepository.Create(payment) ?? new Payment();
                    await paymentRepository.Save();
                    cache.Remove(Cache.PAYMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Payment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentDto>(payment);
        }

        /// <inheritdoc/>
        public async Task<PaymentDto?> UpdatePayment(Guid id, PaymentDto paymentDto)
        {
            try
            {
                var existing = await this.paymentRepository.GetByID(id);
                if (existing == null)
                    return null;
                Payment payment = this.mapper.Map<Payment>(paymentDto);
                payment = await paymentRepository.Update(payment) ?? new Payment();
                await paymentRepository.Save();
                cache.Remove(Cache.PAYMENTS.ToString());
                paymentDto = this.mapper.Map<PaymentDto>(payment);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Payment. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentDto;
        }

        /// <inheritdoc/>
        public async Task DeletePayment(Guid ID)
        {
            try
            {
                var payment = await this.paymentRepository.GetByID(ID);
                if (payment == null)
                    throw new KeyNotFoundException("Payment with the specified ID was not found.");
                await paymentRepository.Delete(payment);
                await paymentRepository.Save();
                cache.Remove(Cache.PAYMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Payment . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentDto?> UpdatePaymentStatus(Guid id, string status)
        {
            var payment = await paymentRepository.GetByID(id);
            if (payment == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                payment.Status = "Pending";
            }
            else
            {
                payment.Status = status;
            }

            await paymentRepository.Update(payment);
            await paymentRepository.Save();
            cache.Remove(Cache.PAYMENTS.ToString());
            return this.mapper.Map<PaymentDto>(payment);
        }
    }
}