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
    public class ListingTermPriceService : IListingTermPriceService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ListingTermPriceService> logger;
        private readonly IListingTermPriceRepository listingtermpriceRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingTermPriceService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingTermPriceService> logger, IListingTermPriceRepository listingtermpriceRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingtermpriceRepository = listingtermpriceRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingTermPriceDto>> GetAll(Paging paging)
        {
            IEnumerable<ListingTermPrice> entities;
            try
            {
                entities = cache.Get<IEnumerable<ListingTermPrice>>(Cache.LISTINGTERMPRICES.ToString()) ?? new List<ListingTermPrice>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.listingtermpriceRepository.GetListingTermPriceAsync())?.Where(x => x != null) ?? new List<ListingTermPrice>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ListingTermPrice>>(Cache.LISTINGTERMPRICES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingTermPrice by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingTermPriceDto>
                {
                    Data = new List<ListingTermPriceDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ListingTermPrice> filteredEntities = entities!;
            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ListingTermPriceID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingTermPriceDto>>(pagedEntities);
            return new PagedResult<ListingTermPriceDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ListingTermPriceDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ListingTermPrice> entities = cache.Get<IEnumerable<ListingTermPrice>>(Cache.LISTINGTERMPRICES.ToString()) ?? new List<ListingTermPrice>();
                ListingTermPrice? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ListingTermPriceID == ID);
                }
                else
                {
                    match = await this.listingtermpriceRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ListingTermPriceDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ListingTermPrice by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ListingTermPriceDto> CreateListingTermPrice(ListingTermPriceDto listingtermpriceDto)
        {
            ListingTermPrice listingTermPrice = new ListingTermPrice();
            IEnumerable<ListingTermPrice?> checkEntity;
            try
            {
                checkEntity = await this.listingtermpriceRepository.Find(x => x.CapturedBy!.ToLower().Trim() == listingtermpriceDto.CapturedBy!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    listingTermPrice = this.mapper.Map<ListingTermPrice>(listingtermpriceDto);
                    listingTermPrice.ListingTermPriceID = Guid.NewGuid();
                    listingTermPrice.CapturedDate = DateTime.UtcNow;
                    listingTermPrice = await listingtermpriceRepository.Create(listingTermPrice) ?? new ListingTermPrice();
                    await listingtermpriceRepository.Save();
                    cache.Remove(Cache.LISTINGTERMPRICES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ListingTermPrice. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ListingTermPriceDto>(listingTermPrice);
        }

        /// <inheritdoc/>
        public async Task<ListingTermPriceDto?> UpdateListingTermPrice(Guid id, ListingTermPriceDto listingtermpriceDto)
        {
            try
            {
                var existing = await this.listingtermpriceRepository.GetByID(id);
                if (existing == null)
                    return null;
                ListingTermPrice listingTermPrice = this.mapper.Map<ListingTermPrice>(listingtermpriceDto);
                listingTermPrice = await listingtermpriceRepository.Update(listingTermPrice) ?? new ListingTermPrice();
                await listingtermpriceRepository.Save();
                cache.Remove(Cache.LISTINGTERMPRICES.ToString());
                listingtermpriceDto = this.mapper.Map<ListingTermPriceDto>(listingTermPrice);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ListingTermPrice. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return listingtermpriceDto;
        }

        /// <inheritdoc/>
        public async Task DeleteListingTermPrice(Guid ID)
        {
            try
            {
                var listingTermPrice = await this.listingtermpriceRepository.GetByID(ID);
                if (listingTermPrice == null)
                    throw new KeyNotFoundException("ListingTermPrice with the specified ID was not found.");
                await listingtermpriceRepository.Delete(listingTermPrice);
                await listingtermpriceRepository.Save();
                cache.Remove(Cache.LISTINGTERMPRICES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ListingTermPrice . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}