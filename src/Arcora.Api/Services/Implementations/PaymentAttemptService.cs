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
    public class PaymentAttemptService : IPaymentAttemptService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentAttemptService> logger;
        private readonly IPaymentAttemptRepository paymentattemptRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentAttemptService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentAttemptService> logger, IPaymentAttemptRepository paymentattemptRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentattemptRepository = paymentattemptRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentAttemptDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentAttempt> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentAttempt>>(Cache.PAYMENTATTEMPTS.ToString()) ?? new List<PaymentAttempt>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentattemptRepository.GetPaymentAttemptAsync())?.Where(x => x != null) ?? new List<PaymentAttempt>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentAttempt>>(Cache.PAYMENTATTEMPTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentAttempt by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentAttemptDto>
                {
                    Data = new List<PaymentAttemptDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentAttempt> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderAttemptID) && x.ProviderAttemptID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentAttemptID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentAttemptDto>>(pagedEntities);
            return new PagedResult<PaymentAttemptDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentAttemptDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<PaymentAttempt> entities = cache.Get<IEnumerable<PaymentAttempt>>(Cache.PAYMENTATTEMPTS.ToString()) ?? new List<PaymentAttempt>();
                PaymentAttempt? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentAttemptID == ID);
                }
                else
                {
                    match = await this.paymentattemptRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentAttemptDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentAttempt by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentAttemptDto> CreatePaymentAttempt(PaymentAttemptDto paymentattemptDto)
        {
            PaymentAttempt paymentAttempt = new PaymentAttempt();
            IEnumerable<PaymentAttempt?> checkEntity;
            try
            {
                checkEntity = await this.paymentattemptRepository.Find(x => x.ProviderAttemptID!.ToLower().Trim() == paymentattemptDto.ProviderAttemptID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentAttempt = this.mapper.Map<PaymentAttempt>(paymentattemptDto);
                    paymentAttempt.PaymentAttemptID = Guid.NewGuid();
                    paymentAttempt.ProviderAttemptID = string.IsNullOrEmpty(paymentattemptDto.ProviderAttemptID) ? null : paymentattemptDto.ProviderAttemptID;
                    paymentAttempt.CapturedDate = DateTime.UtcNow;
                    paymentAttempt = await paymentattemptRepository.Create(paymentAttempt) ?? new PaymentAttempt();
                    await paymentattemptRepository.Save();
                    cache.Remove(Cache.PAYMENTATTEMPTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentAttempt. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentAttemptDto>(paymentAttempt);
        }

        /// <inheritdoc/>
        public async Task<PaymentAttemptDto?> UpdatePaymentAttempt(Guid id, PaymentAttemptDto paymentattemptDto)
        {
            try
            {
                var existing = await this.paymentattemptRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentAttempt paymentAttempt = this.mapper.Map<PaymentAttempt>(paymentattemptDto);
                paymentAttempt = await paymentattemptRepository.Update(paymentAttempt) ?? new PaymentAttempt();
                await paymentattemptRepository.Save();
                cache.Remove(Cache.PAYMENTATTEMPTS.ToString());
                paymentattemptDto = this.mapper.Map<PaymentAttemptDto>(paymentAttempt);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentAttempt. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentattemptDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentAttempt(Guid ID)
        {
            try
            {
                var paymentAttempt = await this.paymentattemptRepository.GetByID(ID);
                if (paymentAttempt == null)
                    throw new KeyNotFoundException("PaymentAttempt with the specified ID was not found.");
                await paymentattemptRepository.Delete(paymentAttempt);
                await paymentattemptRepository.Save();
                cache.Remove(Cache.PAYMENTATTEMPTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentAttempt . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentAttemptDto?> UpdatePaymentAttemptStatus(Guid id, string status)
        {
            var paymentAttempt = await paymentattemptRepository.GetByID(id);
            if (paymentAttempt == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                paymentAttempt.Status = "Pending";
            }
            else
            {
                paymentAttempt.Status = status;
            }

            await paymentattemptRepository.Update(paymentAttempt);
            await paymentattemptRepository.Save();
            cache.Remove(Cache.PAYMENTATTEMPTS.ToString());
            return this.mapper.Map<PaymentAttemptDto>(paymentAttempt);
        }
    }
}