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
    public class UnitTypeService : IUnitTypeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<UnitTypeService> logger;
        private readonly IUnitTypeRepository unittypeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public UnitTypeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<UnitTypeService> logger, IUnitTypeRepository unittypeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.unittypeRepository = unittypeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<UnitTypeDto>> GetAll(Paging paging)
        {
            IEnumerable<UnitType> entities;
            try
            {
                entities = cache.Get<IEnumerable<UnitType>>(Cache.UNITTYPES.ToString()) ?? new List<UnitType>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.unittypeRepository.GetAll())?.Where(x => x != null).Cast<UnitType>().ToList() ?? new List<UnitType>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<UnitType>>(Cache.UNITTYPES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching UnitType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<UnitTypeDto>
                {
                    Data = new List<UnitTypeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<UnitType> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.UnitTypeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<UnitTypeDto>>(pagedEntities);
            return new PagedResult<UnitTypeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<UnitTypeDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<UnitType> entities = cache.Get<IEnumerable<UnitType>>(Cache.UNITTYPES.ToString()) ?? new List<UnitType>();
                UnitType? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.UnitTypeID == ID);
                }
                else
                {
                    match = await this.unittypeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<UnitTypeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching UnitType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitTypeDto> CreateUnitType(UnitTypeDto unittypeDto)
        {
            UnitType unitType = new UnitType();
            IEnumerable<UnitType?> checkEntity;
            try
            {
                var normalizedName = unittypeDto.Name?.ToLower().Trim();
                checkEntity = await this.unittypeRepository.Find(x => x.Name != null && x.Name.ToLower().Trim() == normalizedName);
                if (checkEntity == null || !checkEntity.Any())
                {
                    unitType = this.mapper.Map<UnitType>(unittypeDto);
                    unitType.UnitTypeID = Guid.NewGuid();
                    unitType.CapturedDate = DateTime.UtcNow;
                    unitType = await unittypeRepository.Create(unitType) ?? new UnitType();
                    await unittypeRepository.Save();
                    cache.Remove(Cache.UNITTYPES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating UnitType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<UnitTypeDto>(unitType);
        }

        /// <inheritdoc/>
        public async Task<UnitTypeDto?> UpdateUnitType(Guid id, UnitTypeDto unittypeDto)
        {
            try
            {
                var existing = await this.unittypeRepository.GetByID(id);
                if (existing == null)
                    return null;
                UnitType unitType = this.mapper.Map<UnitType>(unittypeDto);
                unitType = await unittypeRepository.Update(unitType) ?? new UnitType();
                await unittypeRepository.Save();
                cache.Remove(Cache.UNITTYPES.ToString());
                unittypeDto = this.mapper.Map<UnitTypeDto>(unitType);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating UnitType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return unittypeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteUnitType(Guid ID)
        {
            try
            {
                var unitType = await this.unittypeRepository.GetByID(ID);
                if (unitType == null)
                    throw new KeyNotFoundException("UnitType with the specified ID was not found.");
                await unittypeRepository.Delete(unitType);
                await unittypeRepository.Save();
                cache.Remove(Cache.UNITTYPES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting UnitType . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}