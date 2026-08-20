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
    public class PropertyService : IPropertyService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<PropertyService> logger;
        private readonly IPropertyRepository propertyRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public PropertyService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<PropertyService> logger, IPropertyRepository propertyRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.propertyRepository = propertyRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PropertyDto>> GetAll(Paging paging)
        {
            IEnumerable<Property> entities;
            try
            {
                entities = cache.Get<IEnumerable<Property>>(Cache.PROPERTIES.ToString()) ?? new List<Property>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.propertyRepository.GetPropertyAsync())?.Where(x => x != null) ?? new List<Property>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Property>>(Cache.PROPERTIES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Property by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<PropertyDto>
                {
                    Data = new List<PropertyDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Property> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.PropertyID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<PropertyDto>>(pagedEntities);
            return new PagedResult<PropertyDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<PropertyDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Property> entities = cache.Get<IEnumerable<Property>>(Cache.PROPERTIES.ToString()) ?? new List<Property>();
                Property? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.PropertyID == ID);
                }
                else
                {
                    match = await this.propertyRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<PropertyDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Property by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PropertyDto> CreateProperty(PropertyDto propertyDto)
        {
            Property @property = new Property();
            IEnumerable<Property?> checkEntity;
            try
            {
                checkEntity = await this.propertyRepository.Find(x => x.Name!.ToLower().Trim() == propertyDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    @property = this.mapper.Map<Property>(propertyDto);
                    @property.PropertyID = Guid.NewGuid();
                    @property.CapturedDate = DateTime.UtcNow;
                    @property = await propertyRepository.Create(@property) ?? new Property();
                    await propertyRepository.Save();
                    cache.Remove(Cache.PROPERTIES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Property. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<PropertyDto>(@property);
        }

        /// <inheritdoc/>
        public async Task<PropertyDto?> UpdateProperty(Guid id, PropertyDto propertyDto)
        {
            try
            {
                var existing = await this.propertyRepository.GetByID(id);
                if (existing == null)
                    return null;
                Property @property = this.mapper.Map<Property>(propertyDto);
                @property = await propertyRepository.Update(@property) ?? new Property();
                await propertyRepository.Save();
                cache.Remove(Cache.PROPERTIES.ToString());
                propertyDto = this.mapper.Map<PropertyDto>(@property);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Property. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return propertyDto;
        }

        /// <inheritdoc/>
        public async Task DeleteProperty(Guid ID)
        {
            try
            {
                var @property = await this.propertyRepository.GetByID(ID);
                if (@property == null)
                    throw new KeyNotFoundException("Property with the specified ID was not found.");
                await propertyRepository.Delete(@property);
                await propertyRepository.Save();
                cache.Remove(Cache.PROPERTIES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Property . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PropertyDto?> UpdatePropertyStatus(Guid id, string status)
        {
            var @property = await propertyRepository.GetByID(id);
            if (@property == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                @property.Status = "Pending";
            }
            else
            {
                @property.Status = status;
            }

            await propertyRepository.Update(@property);
            await propertyRepository.Save();
            cache.Remove(Cache.PROPERTIES.ToString());
            return this.mapper.Map<PropertyDto>(@property);
        }
    }
}