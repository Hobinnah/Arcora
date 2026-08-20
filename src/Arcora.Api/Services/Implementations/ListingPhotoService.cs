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
    public class ListingPhotoService : IListingPhotoService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingPhotoService> logger;
        private readonly IListingPhotoRepository listingphotoRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingPhotoService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingPhotoService> logger, IListingPhotoRepository listingphotoRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingphotoRepository = listingphotoRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingPhotoDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingPhoto> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString()) ?? new List<ListingPhoto>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingphotoRepository.GetListingPhotoAsync())?.Where(x => x != null) ?? new List<ListingPhoto>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPhoto by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingPhotoDto>
                {
                    Data = new List<ListingPhotoDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingPhoto> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Url) && x.Url.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingPhotoID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingPhotoDto>>(pagedEntities);
            return new PagedResult<ListingPhotoDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingPhoto> entities = cache.Get<IEnumerable<ListingPhoto>>(Cache.LISTINGPHOTOS.ToString()) ?? new List<ListingPhoto>();
                ListingPhoto? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingPhotoID == ID);
                }
                else
                {
                    match = await this.listingphotoRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingPhotoDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingPhoto by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto> CreateListingPhoto(ListingPhotoDto listingphotoDto)
        {
            ListingPhoto listingPhoto = new ListingPhoto();
            IEnumerable<ListingPhoto?> checkEntity;
            try
            {
                checkEntity = await this.listingphotoRepository.Find(x => x.Url!.ToLower().Trim() == listingphotoDto.Url!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingPhoto = this.mapper.Map<ListingPhoto>(listingphotoDto);
                    listingPhoto.ListingPhotoID = Guid.NewGuid();
                    listingPhoto.CapturedDate = DateTime.UtcNow;
                    listingPhoto = await listingphotoRepository.Create(listingPhoto) ?? new ListingPhoto();
                    await listingphotoRepository.Save();
                    cache.Remove(Cache.LISTINGPHOTOS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingPhoto. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingPhotoDto>(listingPhoto);
        }

        /// <inheritdoc/>
        public async Task<ListingPhotoDto?> UpdateListingPhoto(Guid id, ListingPhotoDto listingphotoDto)
        {
            try
            {
                var existing = await this.listingphotoRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingPhoto listingPhoto = this.mapper.Map<ListingPhoto>(listingphotoDto);
                listingPhoto = await listingphotoRepository.Update(listingPhoto) ?? new ListingPhoto();
                await listingphotoRepository.Save();
                cache.Remove(Cache.LISTINGPHOTOS.ToString());
                listingphotoDto = this.mapper.Map<ListingPhotoDto>(listingPhoto);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingPhoto. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingphotoDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingPhoto(Guid ID)
        {
            try
            {
                var listingPhoto = await this.listingphotoRepository.GetByID(ID);
                if (listingPhoto == null)
                    throw new KeyNotFoundException("ListingPhoto with the specified ID was not found.");
                await listingphotoRepository.Delete(listingPhoto);
                await listingphotoRepository.Save();
                cache.Remove(Cache.LISTINGPHOTOS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingPhoto . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}