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
        private readonly IRatingRepository ratingRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ListingService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ListingService> logger, IListingRepository listingRepository, IRatingRepository ratingRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.listingRepository = listingRepository;
            this.ratingRepository = ratingRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <summary>
        /// Populates the aggregated <see cref="ListingDto.Rating"/> and <see cref="ListingDto.Reviews"/>
        /// values from public ratings, grouped by the listing they belong to via the lease.
        /// The aggregate dictionary is cached so it is only recalculated when the cache expires
        /// or is invalidated.
        /// </summary>
        private async Task ApplyRatingAggregatesAsync(IEnumerable<ListingDto> listings)
        {
            var targets = listings?.Where(x => x?.ListingID != null).ToList();
            if (targets == null || targets.Count == 0)
                return;

            var aggregates = await this.GetRatingAggregatesAsync();

            foreach (var listing in targets)
            {
                if (aggregates.TryGetValue(listing.ListingID!.Value, out var agg))
                {
                    listing.Rating = agg.Average;
                    listing.Reviews = agg.Count;
                }
            }
        }

        /// <summary>
        /// Returns the per-listing rating aggregates, using a cached copy when available
        /// and rebuilding from the Rating table only when the cache is empty.
        /// </summary>
        private async Task<IReadOnlyDictionary<Guid, (decimal Average, int Count)>> GetRatingAggregatesAsync()
        {
            var cached = cache.Get<IReadOnlyDictionary<Guid, (decimal Average, int Count)>>(Cache.LISTINGRATINGAGGREGATES.ToString());
            if (cached != null)
                return cached;

            IEnumerable<Rating> ratings;
            try
            {
                ratings = await this.ratingRepository.GetRatingAsync() ?? new List<Rating>();
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Ratings for listing aggregates. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new Dictionary<Guid, (decimal Average, int Count)>();
            }

            var aggregates = ratings
                .Where(r => r.IsPublic && r.Lease != null)
                .GroupBy(r => r.Lease!.ListingID)
                .ToDictionary(
                    g => g.Key,
                    g => (Average: Math.Round((decimal)g.Average(r => r.OverallRating), 1), Count: g.Count()));

            cache.Set<IReadOnlyDictionary<Guid, (decimal Average, int Count)>>(
                Cache.LISTINGRATINGAGGREGATES.ToString(),
                aggregates,
                DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));

            return aggregates;
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
                    entities = (await this.listingRepository.GetListingsAsync())?.Where(x => x != null) ?? new List<Listing>();
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
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CapturedDate).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ListingDto>>(pagedEntities).ToList();
            await this.ApplyRatingAggregatesAsync(pagedDtos);
            return new PagedResult<ListingDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ListingDto>> SearchListings(ListingSearchCriteria criteria)
        {
            try
            {
                var (items, totalCount) = await this.listingRepository.SearchListingsAsync(criteria ?? new ListingSearchCriteria());
                var dtos = this.mapper.Map<IEnumerable<ListingDto>>(items).ToList();
                await this.ApplyRatingAggregatesAsync(dtos);
                return new PagedResult<ListingDto>
                {
                    Data = dtos,
                    TotalCount = totalCount
                };
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while searching Listings. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ListingDto>
                {
                    Data = new List<ListingDto>(),
                    TotalCount = 0
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ListingDto?> GetID(Guid ID)
        {
            try
            {
                // The LISTINGS cache is populated by GetAll via GetListingsAsync, which only
                // eager-loads photos for performance. The detail view needs the full child graph
                // (amenities, rules, policies, term prices, calendar events), so always load it
                // through GetListingAsync rather than the partially-populated list cache.
                Listing? match = await this.listingRepository.GetListingAsync(ID);

                if (match == null)
                    return null;

                var dto = this.mapper.Map<ListingDto>(match);
                await this.ApplyRatingAggregatesAsync(new[] { dto });
                return dto;
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