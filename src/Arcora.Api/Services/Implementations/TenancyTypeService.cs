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
    public class TenancyTypeService : ITenancyTypeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<TenancyTypeService> logger;
        private readonly ITenancyTypeRepository tenancytypeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public TenancyTypeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<TenancyTypeService> logger, ITenancyTypeRepository tenancytypeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.tenancytypeRepository = tenancytypeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<TenancyTypeDto>> GetAll(Paging paging)
        {
            IEnumerable<TenancyType> entities;
            try
            {
                entities = cache.Get<IEnumerable<TenancyType>>(Cache.TENANCYTYPES.ToString()) ?? new List<TenancyType>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.tenancytypeRepository.GetAll())?.Where(x => x != null).Cast<TenancyType>().ToList() ?? new List<TenancyType>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<TenancyType>>(Cache.TENANCYTYPES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenancyType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<TenancyTypeDto>
                {
                    Data = new List<TenancyTypeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<TenancyType> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.TenancyTypeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<TenancyTypeDto>>(pagedEntities);
            return new PagedResult<TenancyTypeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<TenancyTypeDto?> GetID(int ID)
        {
            try
            {
                IEnumerable<TenancyType> entities = cache.Get<IEnumerable<TenancyType>>(Cache.TENANCYTYPES.ToString()) ?? new List<TenancyType>();
                TenancyType? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.TenancyTypeID == ID);
                }
                else
                {
                    match = await this.tenancytypeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<TenancyTypeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching TenancyType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TenancyTypeDto> CreateTenancyType(TenancyTypeDto tenancytypeDto)
        {
            TenancyType tenancyType = new TenancyType();
            IEnumerable<TenancyType?> checkEntity;
            try
            {
                checkEntity = await this.tenancytypeRepository.Find(x => x.Name!.ToLower().Trim() == tenancytypeDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    tenancyType = this.mapper.Map<TenancyType>(tenancytypeDto);
                    tenancyType.CapturedDate = DateTime.UtcNow;
                    tenancyType = await tenancytypeRepository.Create(tenancyType) ?? new TenancyType();
                    await tenancytypeRepository.Save();
                    cache.Remove(Cache.TENANCYTYPES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating TenancyType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<TenancyTypeDto>(tenancyType);
        }

        /// <inheritdoc/>
        public async Task<TenancyTypeDto?> UpdateTenancyType(int id, TenancyTypeDto tenancytypeDto)
        {
            try
            {
                var existing = await this.tenancytypeRepository.GetByID(id);
                if (existing == null)
                    return null;
                TenancyType tenancyType = this.mapper.Map<TenancyType>(tenancytypeDto);
                tenancyType = await tenancytypeRepository.Update(tenancyType) ?? new TenancyType();
                await tenancytypeRepository.Save();
                cache.Remove(Cache.TENANCYTYPES.ToString());
                tenancytypeDto = this.mapper.Map<TenancyTypeDto>(tenancyType);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating TenancyType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return tenancytypeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteTenancyType(int ID)
        {
            try
            {
                var tenancyType = await this.tenancytypeRepository.GetByID(ID);
                if (tenancyType == null)
                    throw new KeyNotFoundException("TenancyType with the specified ID was not found.");
                await tenancytypeRepository.Delete(tenancyType);
                await tenancytypeRepository.Save();
                cache.Remove(Cache.TENANCYTYPES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting TenancyType . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}