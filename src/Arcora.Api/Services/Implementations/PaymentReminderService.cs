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
    public class PaymentReminderService : IPaymentReminderService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PaymentReminderService> logger;
        private readonly IPaymentReminderRepository paymentreminderRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PaymentReminderService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PaymentReminderService> logger, IPaymentReminderRepository paymentreminderRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.paymentreminderRepository = paymentreminderRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PaymentReminderDto>> GetAll(Paging paging)
        {
            IEnumerable<PaymentReminder> entities;
            try
            {
                entities = cache.Get<IEnumerable<PaymentReminder>>(Cache.PAYMENTREMINDERS.ToString()) ?? new List<PaymentReminder>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.paymentreminderRepository.GetPaymentReminderAsync())?.Where(x => x != null) ?? new List<PaymentReminder>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<PaymentReminder>>(Cache.PAYMENTREMINDERS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentReminder by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PaymentReminderDto>
                {
                    Data = new List<PaymentReminderDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<PaymentReminder> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.MessageSubject) && x.MessageSubject.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PaymentReminderID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PaymentReminderDto>>(pagedEntities);
            return new PagedResult<PaymentReminderDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentReminderDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<PaymentReminder> entities = cache.Get<IEnumerable<PaymentReminder>>(Cache.PAYMENTREMINDERS.ToString()) ?? new List<PaymentReminder>();
                PaymentReminder? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PaymentReminderID == ID);
                }
                else
                {
                    match = await this.paymentreminderRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PaymentReminderDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching PaymentReminder by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentReminderDto> CreatePaymentReminder(PaymentReminderDto paymentreminderDto)
        {
            PaymentReminder paymentReminder = new PaymentReminder();
            IEnumerable<PaymentReminder?> checkEntity;
            try
            {
                checkEntity = await this.paymentreminderRepository.Find(x => x.MessageSubject!.ToLower().Trim() == paymentreminderDto.MessageSubject!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    paymentReminder = this.mapper.Map<PaymentReminder>(paymentreminderDto);
                    paymentReminder.PaymentReminderID = Guid.NewGuid();
                    paymentReminder.CapturedDate = DateTime.UtcNow;
                    paymentReminder = await paymentreminderRepository.Create(paymentReminder) ?? new PaymentReminder();
                    await paymentreminderRepository.Save();
                    cache.Remove(Cache.PAYMENTREMINDERS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating PaymentReminder. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PaymentReminderDto>(paymentReminder);
        }

        /// <inheritdoc/>
        public async Task<PaymentReminderDto?> UpdatePaymentReminder(Guid id, PaymentReminderDto paymentreminderDto)
        {
            try
            {
                var existing = await this.paymentreminderRepository.GetByID(id);
                if (existing == null)
                    return null;
                PaymentReminder paymentReminder = this.mapper.Map<PaymentReminder>(paymentreminderDto);
                paymentReminder = await paymentreminderRepository.Update(paymentReminder) ?? new PaymentReminder();
                await paymentreminderRepository.Save();
                cache.Remove(Cache.PAYMENTREMINDERS.ToString());
                paymentreminderDto = this.mapper.Map<PaymentReminderDto>(paymentReminder);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating PaymentReminder. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return paymentreminderDto;
        }

        /// <inheritdoc/>
        public async Task DeletePaymentReminder(Guid ID)
        {
            try
            {
                var paymentReminder = await this.paymentreminderRepository.GetByID(ID);
                if (paymentReminder == null)
                    throw new KeyNotFoundException("PaymentReminder with the specified ID was not found.");
                await paymentreminderRepository.Delete(paymentReminder);
                await paymentreminderRepository.Save();
                cache.Remove(Cache.PAYMENTREMINDERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting PaymentReminder . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PaymentReminderDto?> UpdatePaymentReminderStatus(Guid id, string status)
        {
            var paymentReminder = await paymentreminderRepository.GetByID(id);
            if (paymentReminder == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                paymentReminder.Status = "Pending";
            }
            else
            {
                paymentReminder.Status = status;
            }

            await paymentreminderRepository.Update(paymentReminder);
            await paymentreminderRepository.Save();
            cache.Remove(Cache.PAYMENTREMINDERS.ToString());
            return this.mapper.Map<PaymentReminderDto>(paymentReminder);
        }
    }
}