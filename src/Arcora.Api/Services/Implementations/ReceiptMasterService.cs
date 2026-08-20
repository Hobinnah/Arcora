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
    public class ReceiptMasterService : IReceiptMasterService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ReceiptMasterService> logger;
        private readonly IReceiptMasterRepository receiptmasterRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ReceiptMasterService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ReceiptMasterService> logger, IReceiptMasterRepository receiptmasterRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.receiptmasterRepository = receiptmasterRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ReceiptMasterDto>> GetAll(Paging paging)
        {
            IEnumerable<ReceiptMaster> entities;
            try
            {
                entities = cache.Get<IEnumerable<ReceiptMaster>>(Cache.RECEIPTMASTERS.ToString()) ?? new List<ReceiptMaster>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.receiptmasterRepository.GetReceiptMasterAsync())?.Where(x => x != null) ?? new List<ReceiptMaster>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ReceiptMaster>>(Cache.RECEIPTMASTERS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ReceiptMaster by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ReceiptMasterDto>
                {
                    Data = new List<ReceiptMasterDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ReceiptMaster> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ReceiptNumber) && x.ReceiptNumber.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ReceiptMasterID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ReceiptMasterDto>>(pagedEntities);
            return new PagedResult<ReceiptMasterDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ReceiptMasterDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<ReceiptMaster> entities = cache.Get<IEnumerable<ReceiptMaster>>(Cache.RECEIPTMASTERS.ToString()) ?? new List<ReceiptMaster>();
                ReceiptMaster? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ReceiptMasterID == ID);
                }
                else
                {
                    match = await this.receiptmasterRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ReceiptMasterDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ReceiptMaster by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ReceiptMasterDto> CreateReceiptMaster(ReceiptMasterDto receiptmasterDto)
        {
            ReceiptMaster receiptMaster = new ReceiptMaster();
            IEnumerable<ReceiptMaster?> checkEntity;
            try
            {
                checkEntity = await this.receiptmasterRepository.Find(x => x.ReceiptNumber!.ToLower().Trim() == receiptmasterDto.ReceiptNumber!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    receiptMaster = this.mapper.Map<ReceiptMaster>(receiptmasterDto);
                    receiptMaster.CapturedDate = DateTime.UtcNow;
                    receiptMaster = await receiptmasterRepository.Create(receiptMaster) ?? new ReceiptMaster();
                    await receiptmasterRepository.Save();
                    cache.Remove(Cache.RECEIPTMASTERS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ReceiptMaster. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ReceiptMasterDto>(receiptMaster);
        }

        /// <inheritdoc/>
        public async Task<ReceiptMasterDto?> UpdateReceiptMaster(long id, ReceiptMasterDto receiptmasterDto)
        {
            try
            {
                var existing = await this.receiptmasterRepository.GetByID(id);
                if (existing == null)
                    return null;
                ReceiptMaster receiptMaster = this.mapper.Map<ReceiptMaster>(receiptmasterDto);
                receiptMaster = await receiptmasterRepository.Update(receiptMaster) ?? new ReceiptMaster();
                await receiptmasterRepository.Save();
                cache.Remove(Cache.RECEIPTMASTERS.ToString());
                receiptmasterDto = this.mapper.Map<ReceiptMasterDto>(receiptMaster);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ReceiptMaster. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return receiptmasterDto;
        }

        /// <inheritdoc/>
        public async Task DeleteReceiptMaster(long ID)
        {
            try
            {
                var receiptMaster = await this.receiptmasterRepository.GetByID(ID);
                if (receiptMaster == null)
                    throw new KeyNotFoundException("ReceiptMaster with the specified ID was not found.");
                await receiptmasterRepository.Delete(receiptMaster);
                await receiptmasterRepository.Save();
                cache.Remove(Cache.RECEIPTMASTERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ReceiptMaster . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}