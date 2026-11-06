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
    public class InspectionService : IInspectionService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<InspectionService> logger;
        private readonly IInspectionRepository inspectionRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public InspectionService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<InspectionService> logger, IInspectionRepository inspectionRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.inspectionRepository = inspectionRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<InspectionDto>> GetAll(Paging paging)
        {
            IEnumerable<Inspection> entities;
            try
            {
                entities = await cache.GetOrCreateAsync(
                    Cache.INSPECTIONS.ToString(),
                    async cancel =>
                        (await this.inspectionRepository.GetInspectionAsync())?.Where(x => x != null).ToList() ?? new List<Inspection>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Inspection by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<InspectionDto> { Data = new List<InspectionDto>(), TotalCount = 0 };
            }

            IEnumerable<Inspection> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.InspectionType) && x.InspectionType.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.InspectionID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<InspectionDto>>(pagedEntities);
            return new PagedResult<InspectionDto> { Data = pagedDtos, TotalCount = totalCount };
        }

        /// <inheritdoc/>
        public async Task<InspectionDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.INSPECTIONS.ToString(),
                    async cancel =>
                        (await this.inspectionRepository.GetInspectionAsync())?.Where(x => x != null).ToList() ?? new List<Inspection>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });

                var match = entities.FirstOrDefault(x => x.InspectionID == ID);
                if (match == null)
                    match = await this.inspectionRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<InspectionDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Inspection by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InspectionDto> CreateInspection(InspectionDto inspectionDto)
        {
            Inspection inspection = new Inspection();
            IEnumerable<Inspection?> checkEntity;
            try
            {
                checkEntity = await this.inspectionRepository.Find(x => x.InspectionType!.ToLower().Trim() == inspectionDto.InspectionType!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    inspection = this.mapper.Map<Inspection>(inspectionDto);
                    inspection.InspectionID = Guid.NewGuid();
                    inspection.PropertyID = inspectionDto.PropertyID == Guid.Empty ? null : inspectionDto.PropertyID;
                    inspection.RentalUnitID = inspectionDto.RentalUnitID == Guid.Empty ? null : inspectionDto.RentalUnitID;
                    inspection.LeaseID = inspectionDto.LeaseID == Guid.Empty ? null : inspectionDto.LeaseID;
                    inspection.LeaseRenewalID = inspectionDto.LeaseRenewalID == Guid.Empty ? null : inspectionDto.LeaseRenewalID;
                    inspection.CapturedDate = DateTime.UtcNow;
                    inspection = await inspectionRepository.Create(inspection) ?? new Inspection();
                    await inspectionRepository.Save();
                    await cache.RemoveAsync(Cache.INSPECTIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Inspection. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<InspectionDto>(inspection);
        }

        /// <inheritdoc/>
        public async Task<InspectionDto?> UpdateInspection(Guid id, InspectionDto inspectionDto)
        {
            try
            {
                var existing = await this.inspectionRepository.GetByID(id);
                if (existing == null)
                    return null;
                Inspection inspection = this.mapper.Map<Inspection>(inspectionDto);
                inspection = await inspectionRepository.Update(inspection) ?? new Inspection();
                await inspectionRepository.Save();
                await cache.RemoveAsync(Cache.INSPECTIONS.ToString());
                inspectionDto = this.mapper.Map<InspectionDto>(inspection);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Inspection. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return inspectionDto;
        }

        /// <inheritdoc/>
        public async Task DeleteInspection(Guid ID)
        {
            try
            {
                var inspection = await this.inspectionRepository.GetByID(ID);
                if (inspection == null)
                    throw new KeyNotFoundException("Inspection with the specified ID was not found.");
                await inspectionRepository.Delete(inspection);
                await inspectionRepository.Save();
                await cache.RemoveAsync(Cache.INSPECTIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Inspection . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<InspectionDto?> UpdateInspectionStatus(Guid id, string status)
        {
            var inspection = await inspectionRepository.GetByID(id);
            if (inspection == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                inspection.Status = "Pending";
            }
            else
            {
                inspection.Status = status;
            }

            await inspectionRepository.Update(inspection);
            await inspectionRepository.Save();
            await cache.RemoveAsync(Cache.INSPECTIONS.ToString());
            return this.mapper.Map<InspectionDto>(inspection);
        }
    }
}