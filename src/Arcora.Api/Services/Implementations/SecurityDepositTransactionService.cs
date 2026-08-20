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
    public class SecurityDepositTransactionService : ISecurityDepositTransactionService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<SecurityDepositTransactionService> logger;
        private readonly ISecurityDepositTransactionRepository securitydeposittransactionRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public SecurityDepositTransactionService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<SecurityDepositTransactionService> logger, ISecurityDepositTransactionRepository securitydeposittransactionRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.securitydeposittransactionRepository = securitydeposittransactionRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<SecurityDepositTransactionDto>> GetAll(Paging paging)
        {
            IEnumerable<SecurityDepositTransaction> entities;
            try
            {
                entities = cache.Get<IEnumerable<SecurityDepositTransaction>>(Cache.SECURITYDEPOSITTRANSACTIONS.ToString()) ?? new List<SecurityDepositTransaction>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.securitydeposittransactionRepository.GetSecurityDepositTransactionAsync())?.Where(x => x != null) ?? new List<SecurityDepositTransaction>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<SecurityDepositTransaction>>(Cache.SECURITYDEPOSITTRANSACTIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SecurityDepositTransaction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<SecurityDepositTransactionDto>
                {
                    Data = new List<SecurityDepositTransactionDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<SecurityDepositTransaction> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.SecurityDepositTransactionID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<SecurityDepositTransactionDto>>(pagedEntities);
            return new PagedResult<SecurityDepositTransactionDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositTransactionDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<SecurityDepositTransaction> entities = cache.Get<IEnumerable<SecurityDepositTransaction>>(Cache.SECURITYDEPOSITTRANSACTIONS.ToString()) ?? new List<SecurityDepositTransaction>();
                SecurityDepositTransaction? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.SecurityDepositTransactionID == ID);
                }
                else
                {
                    match = await this.securitydeposittransactionRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<SecurityDepositTransactionDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching SecurityDepositTransaction by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositTransactionDto> CreateSecurityDepositTransaction(SecurityDepositTransactionDto securitydeposittransactionDto)
        {
            SecurityDepositTransaction securityDepositTransaction = new SecurityDepositTransaction();
            IEnumerable<SecurityDepositTransaction?> checkEntity;
            try
            {
                checkEntity = await this.securitydeposittransactionRepository.Find(x => x.Description!.ToLower().Trim() == securitydeposittransactionDto.Description!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    securityDepositTransaction = this.mapper.Map<SecurityDepositTransaction>(securitydeposittransactionDto);
                    securityDepositTransaction.SecurityDepositTransactionID = Guid.NewGuid();
                    securityDepositTransaction.PaymentID = securitydeposittransactionDto.PaymentID == Guid.Empty ? null : securitydeposittransactionDto.PaymentID;
                    securityDepositTransaction.RefundID = securitydeposittransactionDto.RefundID == Guid.Empty ? null : securitydeposittransactionDto.RefundID;
                    securityDepositTransaction.InvoiceMasterID = securitydeposittransactionDto.InvoiceMasterID == Guid.Empty ? null : securitydeposittransactionDto.InvoiceMasterID;
                    securityDepositTransaction.InvoiceDetailID = securitydeposittransactionDto.InvoiceDetailID == Guid.Empty ? null : securitydeposittransactionDto.InvoiceDetailID;
                    securityDepositTransaction.CapturedDate = DateTime.UtcNow;
                    securityDepositTransaction = await securitydeposittransactionRepository.Create(securityDepositTransaction) ?? new SecurityDepositTransaction();
                    await securitydeposittransactionRepository.Save();
                    cache.Remove(Cache.SECURITYDEPOSITTRANSACTIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating SecurityDepositTransaction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<SecurityDepositTransactionDto>(securityDepositTransaction);
        }

        /// <inheritdoc/>
        public async Task<SecurityDepositTransactionDto?> UpdateSecurityDepositTransaction(Guid id, SecurityDepositTransactionDto securitydeposittransactionDto)
        {
            try
            {
                var existing = await this.securitydeposittransactionRepository.GetByID(id);
                if (existing == null)
                    return null;
                SecurityDepositTransaction securityDepositTransaction = this.mapper.Map<SecurityDepositTransaction>(securitydeposittransactionDto);
                securityDepositTransaction = await securitydeposittransactionRepository.Update(securityDepositTransaction) ?? new SecurityDepositTransaction();
                await securitydeposittransactionRepository.Save();
                cache.Remove(Cache.SECURITYDEPOSITTRANSACTIONS.ToString());
                securitydeposittransactionDto = this.mapper.Map<SecurityDepositTransactionDto>(securityDepositTransaction);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating SecurityDepositTransaction. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return securitydeposittransactionDto;
        }

        /// <inheritdoc/>
        public async Task DeleteSecurityDepositTransaction(Guid ID)
        {
            try
            {
                var securityDepositTransaction = await this.securitydeposittransactionRepository.GetByID(ID);
                if (securityDepositTransaction == null)
                    throw new KeyNotFoundException("SecurityDepositTransaction with the specified ID was not found.");
                await securitydeposittransactionRepository.Delete(securityDepositTransaction);
                await securitydeposittransactionRepository.Save();
                cache.Remove(Cache.SECURITYDEPOSITTRANSACTIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting SecurityDepositTransaction . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}