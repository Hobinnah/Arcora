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
    public class RentalApplicationService : IRentalApplicationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<RentalApplicationService> logger;
        private readonly IRentalApplicationRepository rentalapplicationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public RentalApplicationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<RentalApplicationService> logger, IRentalApplicationRepository rentalapplicationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.rentalapplicationRepository = rentalapplicationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<RentalApplicationDto>> GetAll(Paging paging)
        {
            IEnumerable<RentalApplication> entities;
            try
            {
                entities = cache.Get<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString()) ?? new List<RentalApplication>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.rentalapplicationRepository.GetRentalApplicationAsync())?.Where(x => x != null) ?? new List<RentalApplication>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalApplication by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<RentalApplicationDto>
                {
                    Data = new List<RentalApplicationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<RentalApplication> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ApplicationCode) && x.ApplicationCode.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.RentalApplicationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<RentalApplicationDto>>(pagedEntities);
            return new PagedResult<RentalApplicationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<RentalApplication> entities = cache.Get<IEnumerable<RentalApplication>>(Cache.RENTALAPPLICATIONS.ToString()) ?? new List<RentalApplication>();
                RentalApplication? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.RentalApplicationID == ID);
                }
                else
                {
                    match = await this.rentalapplicationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<RentalApplicationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching RentalApplication by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto> CreateRentalApplication(RentalApplicationDto rentalapplicationDto)
        {
            RentalApplication rentalApplication = new RentalApplication();
            IEnumerable<RentalApplication?> checkEntity;
            try
            {
                checkEntity = await this.rentalapplicationRepository.Find(x => x.ApplicationCode!.ToLower().Trim() == rentalapplicationDto.ApplicationCode!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    rentalApplication = this.mapper.Map<RentalApplication>(rentalapplicationDto);
                    rentalApplication.RentalApplicationID = Guid.NewGuid();
                    rentalApplication.ReviewedByOrganizationMemberID = rentalapplicationDto.ReviewedByOrganizationMemberID == Guid.Empty ? null : rentalapplicationDto.ReviewedByOrganizationMemberID;
                    rentalApplication.CapturedDate = DateTime.UtcNow;
                    rentalApplication = await rentalapplicationRepository.Create(rentalApplication) ?? new RentalApplication();
                    await rentalapplicationRepository.Save();
                    cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating RentalApplication. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<RentalApplicationDto>(rentalApplication);
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> UpdateRentalApplication(Guid id, RentalApplicationDto rentalapplicationDto)
        {
            try
            {
                var existing = await this.rentalapplicationRepository.GetByID(id);
                if (existing == null)
                    return null;
                RentalApplication rentalApplication = this.mapper.Map<RentalApplication>(rentalapplicationDto);
                rentalApplication = await rentalapplicationRepository.Update(rentalApplication) ?? new RentalApplication();
                await rentalapplicationRepository.Save();
                cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
                rentalapplicationDto = this.mapper.Map<RentalApplicationDto>(rentalApplication);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating RentalApplication. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return rentalapplicationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteRentalApplication(Guid ID)
        {
            try
            {
                var rentalApplication = await this.rentalapplicationRepository.GetByID(ID);
                if (rentalApplication == null)
                    throw new KeyNotFoundException("RentalApplication with the specified ID was not found.");
                await rentalapplicationRepository.Delete(rentalApplication);
                await rentalapplicationRepository.Save();
                cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting RentalApplication . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RentalApplicationDto?> UpdateRentalApplicationStatus(Guid id, string status)
        {
            var rentalApplication = await rentalapplicationRepository.GetByID(id);
            if (rentalApplication == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                rentalApplication.Status = "Pending";
            }
            else
            {
                rentalApplication.Status = status;
            }

            await rentalapplicationRepository.Update(rentalApplication);
            await rentalapplicationRepository.Save();
            cache.Remove(Cache.RENTALAPPLICATIONS.ToString());
            return this.mapper.Map<RentalApplicationDto>(rentalApplication);
        }
    }
}