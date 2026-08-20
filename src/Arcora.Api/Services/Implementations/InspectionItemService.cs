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
    public class InspectionItemService : IInspectionItemService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<InspectionItemService> logger;
        private readonly IInspectionItemRepository inspectionitemRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public InspectionItemService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<InspectionItemService> logger, IInspectionItemRepository inspectionitemRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.inspectionitemRepository = inspectionitemRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<InspectionItemDto>> GetAll(Paging paging)
        {
            IEnumerable<InspectionItem> entities;
            try
            {
                entities = cache.Get<IEnumerable<InspectionItem>>(Cache.INSPECTIONITEMS.ToString()) ?? new List<InspectionItem>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.inspectionitemRepository.GetInspectionItemAsync())?.Where(x => x != null) ?? new List<InspectionItem>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<InspectionItem>>(Cache.INSPECTIONITEMS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InspectionItem by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<InspectionItemDto>
                {
                    Data = new List<InspectionItemDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<InspectionItem> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ItemName) && x.ItemName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.InspectionItemID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<InspectionItemDto>>(pagedEntities);
            return new PagedResult<InspectionItemDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<InspectionItemDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<InspectionItem> entities = cache.Get<IEnumerable<InspectionItem>>(Cache.INSPECTIONITEMS.ToString()) ?? new List<InspectionItem>();
                InspectionItem? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.InspectionItemID == ID);
                }
                else
                {
                    match = await this.inspectionitemRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<InspectionItemDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching InspectionItem by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InspectionItemDto> CreateInspectionItem(InspectionItemDto inspectionitemDto)
        {
            InspectionItem inspectionItem = new InspectionItem();
            IEnumerable<InspectionItem?> checkEntity;
            try
            {
                checkEntity = await this.inspectionitemRepository.Find(x => x.ItemName!.ToLower().Trim() == inspectionitemDto.ItemName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    inspectionItem = this.mapper.Map<InspectionItem>(inspectionitemDto);
                    inspectionItem.InspectionItemID = Guid.NewGuid();
                    inspectionItem.CapturedDate = DateTime.UtcNow;
                    inspectionItem = await inspectionitemRepository.Create(inspectionItem) ?? new InspectionItem();
                    await inspectionitemRepository.Save();
                    cache.Remove(Cache.INSPECTIONITEMS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating InspectionItem. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<InspectionItemDto>(inspectionItem);
        }

        /// <inheritdoc/>
        public async Task<InspectionItemDto?> UpdateInspectionItem(Guid id, InspectionItemDto inspectionitemDto)
        {
            try
            {
                var existing = await this.inspectionitemRepository.GetByID(id);
                if (existing == null)
                    return null;
                InspectionItem inspectionItem = this.mapper.Map<InspectionItem>(inspectionitemDto);
                inspectionItem = await inspectionitemRepository.Update(inspectionItem) ?? new InspectionItem();
                await inspectionitemRepository.Save();
                cache.Remove(Cache.INSPECTIONITEMS.ToString());
                inspectionitemDto = this.mapper.Map<InspectionItemDto>(inspectionItem);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating InspectionItem. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return inspectionitemDto;
        }

        /// <inheritdoc/>
        public async Task DeleteInspectionItem(Guid ID)
        {
            try
            {
                var inspectionItem = await this.inspectionitemRepository.GetByID(ID);
                if (inspectionItem == null)
                    throw new KeyNotFoundException("InspectionItem with the specified ID was not found.");
                await inspectionitemRepository.Delete(inspectionItem);
                await inspectionitemRepository.Save();
                cache.Remove(Cache.INSPECTIONITEMS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting InspectionItem . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}