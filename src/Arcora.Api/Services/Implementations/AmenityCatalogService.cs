// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    public class AmenityCatalogService : IAmenityCatalogService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<AmenityCatalogService> logger;
        private readonly IAmenityCatalogRepository amenitycatalogRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AmenityCatalogService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<AmenityCatalogService> logger, IAmenityCatalogRepository amenitycatalogRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.amenitycatalogRepository = amenitycatalogRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AmenityCatalogDto>> GetAll(Paging paging)
        {
            IEnumerable<AmenityCatalog> entities;
            try
            {
                var cacheOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes)
                };

                entities = await cache.GetOrCreateAsync(
                    Cache.AMENITYCATALOGS.ToString(),
                    async cancel =>
                        (await this.amenitycatalogRepository.GetAll())?.Where(x => x != null).Cast<AmenityCatalog>().ToList() ?? new List<AmenityCatalog>(),
                    cacheOptions);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AmenityCatalog by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AmenityCatalogDto>
                {
                    Data = new List<AmenityCatalogDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<AmenityCatalog> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AmenityID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AmenityCatalogDto>>(pagedEntities);
            return new PagedResult<AmenityCatalogDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AmenityCatalogDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.AMENITYCATALOGS.ToString(),
                    async cancel =>
                        (await this.amenitycatalogRepository.GetAll())?.Where(x => x != null).Cast<AmenityCatalog>().ToList() ?? new List<AmenityCatalog>(),
                    new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes)
                    });

                var match = entities.FirstOrDefault(x => x.AmenityID == ID);
                if (match == null)
                    match = await this.amenitycatalogRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<AmenityCatalogDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AmenityCatalog by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AmenityCatalogDto> CreateAmenityCatalog(AmenityCatalogDto amenitycatalogDto)
        {
            AmenityCatalog amenityCatalog = new AmenityCatalog();
            IEnumerable<AmenityCatalog?> checkEntity;
            try
            {
                var normalizedName = amenitycatalogDto.Name?.ToLower().Trim();
                checkEntity = await this.amenitycatalogRepository.Find(x => x.Name != null && x.Name.ToLower().Trim() == normalizedName);
                if (checkEntity == null || !checkEntity.Any())
                {
                    amenityCatalog = this.mapper.Map<AmenityCatalog>(amenitycatalogDto);
                    amenityCatalog.AmenityID = Guid.NewGuid();
                    amenityCatalog.CapturedDate = DateTime.UtcNow;
                    amenityCatalog = await amenitycatalogRepository.Create(amenityCatalog) ?? new AmenityCatalog();
                    await amenitycatalogRepository.Save();
                    await cache.RemoveAsync(Cache.AMENITYCATALOGS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating AmenityCatalog. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AmenityCatalogDto>(amenityCatalog);
        }

        /// <inheritdoc/>
        public async Task<AmenityCatalogDto?> UpdateAmenityCatalog(Guid id, AmenityCatalogDto amenitycatalogDto)
        {
            try
            {
                var existing = await this.amenitycatalogRepository.GetByID(id);
                if (existing == null)
                    return null;
                AmenityCatalog amenityCatalog = this.mapper.Map<AmenityCatalog>(amenitycatalogDto);
                amenityCatalog = await amenitycatalogRepository.Update(amenityCatalog) ?? new AmenityCatalog();
                await amenitycatalogRepository.Save();
                await cache.RemoveAsync(Cache.AMENITYCATALOGS.ToString());
                amenitycatalogDto = this.mapper.Map<AmenityCatalogDto>(amenityCatalog);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating AmenityCatalog. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return amenitycatalogDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAmenityCatalog(Guid ID)
        {
            try
            {
                var amenityCatalog = await this.amenitycatalogRepository.GetByID(ID);
                if (amenityCatalog == null)
                    throw new KeyNotFoundException("AmenityCatalog with the specified ID was not found.");
                await amenitycatalogRepository.Delete(amenityCatalog);
                await amenitycatalogRepository.Save();
                await cache.RemoveAsync(Cache.AMENITYCATALOGS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting AmenityCatalog . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}