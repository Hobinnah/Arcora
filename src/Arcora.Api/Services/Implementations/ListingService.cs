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
    public class ListingService : IListingService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingService> logger;
        private readonly IListingRepository listingRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingService> logger, IListingRepository listingRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingRepository = listingRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingDto>> GetAll(Paging paging)
        {
            IEnumerable<Listing> entities;
            try
            {
                entities = cache.Get<IEnumerable<Listing>>(Cache.LISTINGS.ToString()) ?? new List<Listing>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingRepository.GetListingAsync())?.Where(x => x != null) ?? new List<Listing>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Listing>>(Cache.LISTINGS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Listing by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingDto>
                {
                    Data = new List<ListingDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Listing> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingDto>>(pagedEntities);
            return new PagedResult<ListingDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Listing> entities = cache.Get<IEnumerable<Listing>>(Cache.LISTINGS.ToString()) ?? new List<Listing>();
                Listing? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingID == ID);
                }
                else
                {
                    match = await this.listingRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Listing by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto> CreateListing(ListingDto listingDto)
        {
            Listing listing = new Listing();
            IEnumerable<Listing?> checkEntity;
            try
            {
                checkEntity = await this.listingRepository.Find(x => x.Title!.ToLower().Trim() == listingDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listing = this.mapper.Map<Listing>(listingDto);
                    listing.ListingID = Guid.NewGuid();
                    listing.CapturedDate = DateTime.UtcNow;
                    listing = await listingRepository.Create(listing) ?? new Listing();
                    await listingRepository.Save();
                    cache.Remove(Cache.LISTINGS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Listing. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingDto>(listing);
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> UpdateListing(Guid id, ListingDto listingDto)
        {
            try
            {
                var existing = await this.listingRepository.GetByID(id);
                if (existing == null)
                    return null;
                Listing listing = this.mapper.Map<Listing>(listingDto);
                listing = await listingRepository.Update(listing) ?? new Listing();
                await listingRepository.Save();
                cache.Remove(Cache.LISTINGS.ToString());
                listingDto = this.mapper.Map<ListingDto>(listing);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Listing. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListing(Guid ID)
        {
            try
            {
                var listing = await this.listingRepository.GetByID(ID);
                if (listing == null)
                    throw new KeyNotFoundException("Listing with the specified ID was not found.");
                await listingRepository.Delete(listing);
                await listingRepository.Save();
                cache.Remove(Cache.LISTINGS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Listing . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> UpdateListingStatus(Guid id, string status)
        {
            var listing = await listingRepository.GetByID(id);
            if (listing == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                listing.Status = "Pending";
            }
            else
            {
                listing.Status = status;
            }

            await listingRepository.Update(listing);
            await listingRepository.Save();
            cache.Remove(Cache.LISTINGS.ToString());
            return this.mapper.Map<ListingDto>(listing);
        }
    }
}