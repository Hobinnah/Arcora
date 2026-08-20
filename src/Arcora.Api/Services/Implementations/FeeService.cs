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
    public class FeeService : IFeeService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<FeeService> logger;
        private readonly IFeeRepository feeRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public FeeService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<FeeService> logger, IFeeRepository feeRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.feeRepository = feeRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<FeeDto>> GetAll(Paging paging)
        {
            IEnumerable<Fee> entities;
            try
            {
                entities = cache.Get<IEnumerable<Fee>>(Cache.FEES.ToString()) ?? new List<Fee>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.feeRepository.GetFeeAsync())?.Where(x => x != null) ?? new List<Fee>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Fee>>(Cache.FEES.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Fee by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<FeeDto>
                {
                    Data = new List<FeeDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Fee> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.FeeID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<FeeDto>>(pagedEntities);
            return new PagedResult<FeeDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<FeeDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Fee> entities = cache.Get<IEnumerable<Fee>>(Cache.FEES.ToString()) ?? new List<Fee>();
                Fee? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.FeeID == ID);
                }
                else
                {
                    match = await this.feeRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<FeeDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Fee by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<FeeDto> CreateFee(FeeDto feeDto)
        {
            Fee fee = new Fee();
            IEnumerable<Fee?> checkEntity;
            try
            {
                checkEntity = await this.feeRepository.Find(x => x.Name!.ToLower().Trim() == feeDto.Name!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    fee = this.mapper.Map<Fee>(feeDto);
                    fee.FeeID = Guid.NewGuid();
                    fee.OrganizationID = feeDto.OrganizationID == Guid.Empty ? null : feeDto.OrganizationID;
                    fee.CapturedDate = DateTime.UtcNow;
                    fee = await feeRepository.Create(fee) ?? new Fee();
                    await feeRepository.Save();
                    cache.Remove(Cache.FEES.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Fee. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<FeeDto>(fee);
        }

        /// <inheritdoc/>
        public async Task<FeeDto?> UpdateFee(Guid id, FeeDto feeDto)
        {
            try
            {
                var existing = await this.feeRepository.GetByID(id);
                if (existing == null)
                    return null;
                Fee fee = this.mapper.Map<Fee>(feeDto);
                fee = await feeRepository.Update(fee) ?? new Fee();
                await feeRepository.Save();
                cache.Remove(Cache.FEES.ToString());
                feeDto = this.mapper.Map<FeeDto>(fee);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Fee. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return feeDto;
        }

        /// <inheritdoc/>
        public async Task DeleteFee(Guid ID)
        {
            try
            {
                var fee = await this.feeRepository.GetByID(ID);
                if (fee == null)
                    throw new KeyNotFoundException("Fee with the specified ID was not found.");
                await feeRepository.Delete(fee);
                await feeRepository.Save();
                cache.Remove(Cache.FEES.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Fee . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}