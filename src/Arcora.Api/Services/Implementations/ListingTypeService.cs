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
    public class ListingTypeService : IListingTypeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingTypeService> logger;
        private readonly IListingTypeRepository listingtypeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingTypeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingTypeService> logger, IListingTypeRepository listingtypeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingtypeRepository = listingtypeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingTypeDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingType> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingType>>(Cache.LISTINGTYPES.ToString()) ?? new List<ListingType>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingtypeRepository.GetAll())?.Where(x => x != null).Cast<ListingType>().ToList() ?? new List<ListingType>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingType>>(Cache.LISTINGTYPES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingTypeDto>
                {
                    Data = new List<ListingTypeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingType> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingTypeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingTypeDto>>(pagedEntities);
            return new PagedResult<ListingTypeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingTypeDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingType> entities = cache.Get<IEnumerable<ListingType>>(Cache.LISTINGTYPES.ToString()) ?? new List<ListingType>();
                ListingType? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingTypeID == ID);
                }
                else
                {
                    match = await this.listingtypeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingTypeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingTypeDto> CreateListingType(ListingTypeDto listingtypeDto)
        {
            ListingType listingType = new ListingType();
            IEnumerable<ListingType?> checkEntity;
            try
            {
                var normalizedName = listingtypeDto.Name?.ToLower().Trim();
                checkEntity = await this.listingtypeRepository.Find(x => x.Name != null && x.Name.ToLower().Trim() == normalizedName);
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingType = this.mapper.Map<ListingType>(listingtypeDto);
                    listingType.ListingTypeID = Guid.NewGuid();
                    listingType.CapturedDate = DateTime.UtcNow;
                    listingType = await listingtypeRepository.Create(listingType) ?? new ListingType();
                    await listingtypeRepository.Save();
                    cache.Remove(Cache.LISTINGTYPES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingTypeDto>(listingType);
        }

        /// <inheritdoc/>
        public async Task<ListingTypeDto?> UpdateListingType(Guid id, ListingTypeDto listingtypeDto)
        {
            try
            {
                var existing = await this.listingtypeRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingType listingType = this.mapper.Map<ListingType>(listingtypeDto);
                listingType = await listingtypeRepository.Update(listingType) ?? new ListingType();
                await listingtypeRepository.Save();
                cache.Remove(Cache.LISTINGTYPES.ToString());
                listingtypeDto = this.mapper.Map<ListingTypeDto>(listingType);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingtypeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingType(Guid ID)
        {
            try
            {
                var listingType = await this.listingtypeRepository.GetByID(ID);
                if (listingType == null)
                    throw new KeyNotFoundException("ListingType with the specified ID was not found.");
                await listingtypeRepository.Delete(listingType);
                await listingtypeRepository.Save();
                cache.Remove(Cache.LISTINGTYPES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingType . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}