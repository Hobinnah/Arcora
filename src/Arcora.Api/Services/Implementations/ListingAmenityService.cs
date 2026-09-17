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
    public class ListingAmenityService : IListingAmenityService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingAmenityService> logger;
        private readonly IListingAmenityRepository listingamenityRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingAmenityService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingAmenityService> logger, IListingAmenityRepository listingamenityRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingamenityRepository = listingamenityRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingAmenityDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingAmenity> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingAmenity>>(Cache.LISTINGAMENITIES.ToString()) ?? new List<ListingAmenity>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingamenityRepository.GetListingAmenityAsync())?.Where(x => x != null) ?? new List<ListingAmenity>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingAmenity>>(Cache.LISTINGAMENITIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingAmenity by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingAmenityDto>
                {
                    Data = new List<ListingAmenityDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingAmenity> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Notes) && x.Notes.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingAmenityID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingAmenityDto>>(pagedEntities);
            return new PagedResult<ListingAmenityDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingAmenityDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingAmenity> entities = cache.Get<IEnumerable<ListingAmenity>>(Cache.LISTINGAMENITIES.ToString()) ?? new List<ListingAmenity>();
                ListingAmenity? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingAmenityID == ID);
                }
                else
                {
                    match = await this.listingamenityRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingAmenityDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingAmenity by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingAmenityDto> CreateListingAmenity(ListingAmenityDto listingamenityDto)
        {
            ListingAmenity listingAmenity = new ListingAmenity();
            IEnumerable<ListingAmenity?> checkEntity;
            try
            {
                var normalizedNotes = listingamenityDto.Notes?.ToLower().Trim();
                checkEntity = await this.listingamenityRepository.Find(x => x.Notes != null && x.Notes.ToLower().Trim() == normalizedNotes);
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingAmenity = this.mapper.Map<ListingAmenity>(listingamenityDto);
                    listingAmenity.ListingAmenityID = Guid.NewGuid();
                    listingAmenity.CapturedDate = DateTime.UtcNow;
                    listingAmenity = await listingamenityRepository.Create(listingAmenity) ?? new ListingAmenity();
                    await listingamenityRepository.Save();
                    cache.Remove(Cache.LISTINGAMENITIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingAmenity. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingAmenityDto>(listingAmenity);
        }

        /// <inheritdoc/>
        public async Task<ListingAmenityDto?> UpdateListingAmenity(Guid id, ListingAmenityDto listingamenityDto)
        {
            try
            {
                var existing = await this.listingamenityRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingAmenity listingAmenity = this.mapper.Map<ListingAmenity>(listingamenityDto);
                listingAmenity = await listingamenityRepository.Update(listingAmenity) ?? new ListingAmenity();
                await listingamenityRepository.Save();
                cache.Remove(Cache.LISTINGAMENITIES.ToString());
                listingamenityDto = this.mapper.Map<ListingAmenityDto>(listingAmenity);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingAmenity. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingamenityDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingAmenity(Guid ID)
        {
            try
            {
                var listingAmenity = await this.listingamenityRepository.GetByID(ID);
                if (listingAmenity == null)
                    throw new KeyNotFoundException("ListingAmenity with the specified ID was not found.");
                await listingamenityRepository.Delete(listingAmenity);
                await listingamenityRepository.Save();
                cache.Remove(Cache.LISTINGAMENITIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingAmenity . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}