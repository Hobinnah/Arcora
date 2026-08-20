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
    public class RentalUnitService : IRentalUnitService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<RentalUnitService> logger;
        private readonly IRentalUnitRepository rentalunitRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public RentalUnitService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<RentalUnitService> logger, IRentalUnitRepository rentalunitRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.rentalunitRepository = rentalunitRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<RentalUnitDto>> GetAll(Paging paging)
        {
            IEnumerable<RentalUnit> entities;
            try
            {
                entities = cache.Get<IEnumerable<RentalUnit>>(Cache.RENTALUNITS.ToString()) ?? new List<RentalUnit>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.rentalunitRepository.GetRentalUnitAsync())?.Where(x => x != null) ?? new List<RentalUnit>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<RentalUnit>>(Cache.RENTALUNITS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalUnit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<RentalUnitDto>
                {
                    Data = new List<RentalUnitDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<RentalUnit> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.UnitNumber) && x.UnitNumber.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.RentalUnitID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<RentalUnitDto>>(pagedEntities);
            return new PagedResult<RentalUnitDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<RentalUnitDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<RentalUnit> entities = cache.Get<IEnumerable<RentalUnit>>(Cache.RENTALUNITS.ToString()) ?? new List<RentalUnit>();
                RentalUnit? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.RentalUnitID == ID);
                }
                else
                {
                    match = await this.rentalunitRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<RentalUnitDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalUnit by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalUnitDto> CreateRentalUnit(RentalUnitDto rentalunitDto)
        {
            RentalUnit rentalUnit = new RentalUnit();
            IEnumerable<RentalUnit?> checkEntity;
            try
            {
                checkEntity = await this.rentalunitRepository.Find(x => x.UnitNumber!.ToLower().Trim() == rentalunitDto.UnitNumber!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    rentalUnit = this.mapper.Map<RentalUnit>(rentalunitDto);
                    rentalUnit.RentalUnitID = Guid.NewGuid();
                    rentalUnit.UnitTypeID = rentalunitDto.UnitTypeID == Guid.Empty ? null : rentalunitDto.UnitTypeID;
                    rentalUnit.CapturedDate = DateTime.UtcNow;
                    rentalUnit = await rentalunitRepository.Create(rentalUnit) ?? new RentalUnit();
                    await rentalunitRepository.Save();
                    cache.Remove(Cache.RENTALUNITS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating RentalUnit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<RentalUnitDto>(rentalUnit);
        }

        /// <inheritdoc/>
        public async Task<RentalUnitDto?> UpdateRentalUnit(Guid id, RentalUnitDto rentalunitDto)
        {
            try
            {
                var existing = await this.rentalunitRepository.GetByID(id);
                if (existing == null)
                    return null;
                RentalUnit rentalUnit = this.mapper.Map<RentalUnit>(rentalunitDto);
                rentalUnit = await rentalunitRepository.Update(rentalUnit) ?? new RentalUnit();
                await rentalunitRepository.Save();
                cache.Remove(Cache.RENTALUNITS.ToString());
                rentalunitDto = this.mapper.Map<RentalUnitDto>(rentalUnit);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating RentalUnit. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return rentalunitDto;
        }

        /// <inheritdoc/>
        public async Task DeleteRentalUnit(Guid ID)
        {
            try
            {
                var rentalUnit = await this.rentalunitRepository.GetByID(ID);
                if (rentalUnit == null)
                    throw new KeyNotFoundException("RentalUnit with the specified ID was not found.");
                await rentalunitRepository.Delete(rentalUnit);
                await rentalunitRepository.Save();
                cache.Remove(Cache.RENTALUNITS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting RentalUnit . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalUnitDto?> UpdateRentalUnitStatus(Guid id, string status)
        {
            var rentalUnit = await rentalunitRepository.GetByID(id);
            if (rentalUnit == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                rentalUnit.Status = "Pending";
            }
            else
            {
                rentalUnit.Status = status;
            }

            await rentalunitRepository.Update(rentalUnit);
            await rentalunitRepository.Save();
            cache.Remove(Cache.RENTALUNITS.ToString());
            return this.mapper.Map<RentalUnitDto>(rentalUnit);
        }
    }
}