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
    public class FeeTypeService : IFeeTypeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<FeeTypeService> logger;
        private readonly IFeeTypeRepository feetypeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public FeeTypeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<FeeTypeService> logger, IFeeTypeRepository feetypeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.feetypeRepository = feetypeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<FeeTypeDto>> GetAll(Paging paging)
        {
            IEnumerable<FeeType> entities;
            try
            {
                entities = cache.Get<IEnumerable<FeeType>>(Cache.FEETYPES.ToString()) ?? new List<FeeType>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.feetypeRepository.GetAll())?.Where(x => x != null).Cast<FeeType>().ToList() ?? new List<FeeType>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<FeeType>>(Cache.FEETYPES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching FeeType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<FeeTypeDto>
                {
                    Data = new List<FeeTypeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<FeeType> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.FeeTypeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<FeeTypeDto>>(pagedEntities);
            return new PagedResult<FeeTypeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<FeeTypeDto?> GetID(int ID)
        {
            try
            {
                IEnumerable<FeeType> entities = cache.Get<IEnumerable<FeeType>>(Cache.FEETYPES.ToString()) ?? new List<FeeType>();
                FeeType? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.FeeTypeID == ID);
                }
                else
                {
                    match = await this.feetypeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<FeeTypeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching FeeType by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FeeTypeDto> CreateFeeType(FeeTypeDto feetypeDto)
        {
            FeeType feeType = new FeeType();
            IEnumerable<FeeType?> checkEntity;
            try
            {
                checkEntity = await this.feetypeRepository.Find(x => x.Name!.ToLower().Trim() == feetypeDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    feeType = this.mapper.Map<FeeType>(feetypeDto);
                    feeType.CapturedDate = DateTime.UtcNow;
                    feeType = await feetypeRepository.Create(feeType) ?? new FeeType();
                    await feetypeRepository.Save();
                    cache.Remove(Cache.FEETYPES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating FeeType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<FeeTypeDto>(feeType);
        }

        /// <inheritdoc/>
        public async Task<FeeTypeDto?> UpdateFeeType(int id, FeeTypeDto feetypeDto)
        {
            try
            {
                var existing = await this.feetypeRepository.GetByID(id);
                if (existing == null)
                    return null;
                FeeType feeType = this.mapper.Map<FeeType>(feetypeDto);
                feeType = await feetypeRepository.Update(feeType) ?? new FeeType();
                await feetypeRepository.Save();
                cache.Remove(Cache.FEETYPES.ToString());
                feetypeDto = this.mapper.Map<FeeTypeDto>(feeType);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating FeeType. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return feetypeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteFeeType(int ID)
        {
            try
            {
                var feeType = await this.feetypeRepository.GetByID(ID);
                if (feeType == null)
                    throw new KeyNotFoundException("FeeType with the specified ID was not found.");
                await feetypeRepository.Delete(feeType);
                await feetypeRepository.Save();
                cache.Remove(Cache.FEETYPES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting FeeType . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}