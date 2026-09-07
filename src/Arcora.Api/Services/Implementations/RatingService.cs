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
    public class RatingService : IRatingService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<RatingService> logger;
        private readonly IRatingRepository ratingRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public RatingService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<RatingService> logger, IRatingRepository ratingRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.ratingRepository = ratingRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<RatingDto>> GetAll(Paging paging)
        {
            IEnumerable<Rating> entities;
            try
            {
                entities = cache.Get<IEnumerable<Rating>>(Cache.RATINGS.ToString()) ?? new List<Rating>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.ratingRepository.GetRatingAsync())?.Where(x => x != null) ?? new List<Rating>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Rating>>(Cache.RATINGS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Rating by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<RatingDto>
                {
                    Data = new List<RatingDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Rating> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ReviewBody) && x.ReviewBody.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.RatingID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<RatingDto>>(pagedEntities);
            return new PagedResult<RatingDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<RatingDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Rating> entities = cache.Get<IEnumerable<Rating>>(Cache.RATINGS.ToString()) ?? new List<Rating>();
                Rating? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.RatingID == ID);
                }
                else
                {
                    match = await this.ratingRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<RatingDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Rating by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RatingDto> CreateRating(RatingDto ratingDto)
        {
            Rating rating = new Rating();
            IEnumerable<Rating?> checkEntity;
            try
            {
                checkEntity = await this.ratingRepository.Find(x => x.ReviewBody!.ToLower().Trim() == ratingDto.ReviewBody!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    rating = this.mapper.Map<Rating>(ratingDto);
                    rating.RatingID = Guid.NewGuid();
                    rating.CapturedDate = DateTime.UtcNow;
                    rating = await ratingRepository.Create(rating) ?? new Rating();
                    await ratingRepository.Save();
                    cache.Remove(Cache.RATINGS.ToString());
                    cache.Remove(Cache.LISTINGRATINGAGGREGATES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Rating. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<RatingDto>(rating);
        }

        /// <inheritdoc/>
        public async Task<RatingDto?> UpdateRating(Guid id, RatingDto ratingDto)
        {
            try
            {
                var existing = await this.ratingRepository.GetByID(id);
                if (existing == null)
                    return null;
                Rating rating = this.mapper.Map<Rating>(ratingDto);
                rating = await ratingRepository.Update(rating) ?? new Rating();
                await ratingRepository.Save();
                cache.Remove(Cache.RATINGS.ToString());
                cache.Remove(Cache.LISTINGRATINGAGGREGATES.ToString());
                ratingDto = this.mapper.Map<RatingDto>(rating);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Rating. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return ratingDto;
        }

        /// <inheritdoc/>
        public async Task DeleteRating(Guid ID)
        {
            try
            {
                var rating = await this.ratingRepository.GetByID(ID);
                if (rating == null)
                    throw new KeyNotFoundException("Rating with the specified ID was not found.");
                await ratingRepository.Delete(rating);
                await ratingRepository.Save();
                cache.Remove(Cache.RATINGS.ToString());
                cache.Remove(Cache.LISTINGRATINGAGGREGATES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Rating . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}