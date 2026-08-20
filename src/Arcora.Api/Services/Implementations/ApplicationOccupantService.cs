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
    public class ApplicationOccupantService : IApplicationOccupantService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<ApplicationOccupantService> logger;
        private readonly IApplicationOccupantRepository applicationoccupantRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ApplicationOccupantService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<ApplicationOccupantService> logger, IApplicationOccupantRepository applicationoccupantRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.applicationoccupantRepository = applicationoccupantRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ApplicationOccupantDto>> GetAll(Paging paging)
        {
            IEnumerable<ApplicationOccupant> entities;
            try
            {
                entities = cache.Get<IEnumerable<ApplicationOccupant>>(Cache.APPLICATIONOCCUPANTS.ToString()) ?? new List<ApplicationOccupant>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.applicationoccupantRepository.GetApplicationOccupantAsync())?.Where(x => x != null) ?? new List<ApplicationOccupant>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<ApplicationOccupant>>(Cache.APPLICATIONOCCUPANTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }

                if (entities is not null && paging.UserID > 0)
                {
                    entities = entities.Where(c => c.UserID == paging.UserID);
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ApplicationOccupant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ApplicationOccupantDto>
                {
                    Data = new List<ApplicationOccupantDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ApplicationOccupant> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ApplicationOccupantID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ApplicationOccupantDto>>(pagedEntities);
            return new PagedResult<ApplicationOccupantDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ApplicationOccupantDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<ApplicationOccupant> entities = cache.Get<IEnumerable<ApplicationOccupant>>(Cache.APPLICATIONOCCUPANTS.ToString()) ?? new List<ApplicationOccupant>();
                ApplicationOccupant? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.ApplicationOccupantID == ID);
                }
                else
                {
                    match = await this.applicationoccupantRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<ApplicationOccupantDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ApplicationOccupant by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ApplicationOccupantDto> CreateApplicationOccupant(ApplicationOccupantDto applicationoccupantDto)
        {
            ApplicationOccupant applicationOccupant = new ApplicationOccupant();
            IEnumerable<ApplicationOccupant?> checkEntity;
            try
            {
                checkEntity = await this.applicationoccupantRepository.Find(x => x.LastName!.ToLower().Trim() == applicationoccupantDto.LastName!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    applicationOccupant = this.mapper.Map<ApplicationOccupant>(applicationoccupantDto);
                    applicationOccupant.ApplicationOccupantID = Guid.NewGuid();
                    applicationOccupant.TenantID = applicationoccupantDto.TenantID == Guid.Empty ? null : applicationoccupantDto.TenantID;
                    applicationOccupant.UserID = applicationoccupantDto.UserID == 0 ? null : applicationoccupantDto.UserID;
                    applicationOccupant.CapturedDate = DateTime.UtcNow;
                    applicationOccupant = await applicationoccupantRepository.Create(applicationOccupant) ?? new ApplicationOccupant();
                    await applicationoccupantRepository.Save();
                    cache.Remove(Cache.APPLICATIONOCCUPANTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ApplicationOccupant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ApplicationOccupantDto>(applicationOccupant);
        }

        /// <inheritdoc/>
        public async Task<ApplicationOccupantDto?> UpdateApplicationOccupant(Guid id, ApplicationOccupantDto applicationoccupantDto)
        {
            try
            {
                var existing = await this.applicationoccupantRepository.GetByID(id);
                if (existing == null)
                    return null;
                ApplicationOccupant applicationOccupant = this.mapper.Map<ApplicationOccupant>(applicationoccupantDto);
                applicationOccupant = await applicationoccupantRepository.Update(applicationOccupant) ?? new ApplicationOccupant();
                await applicationoccupantRepository.Save();
                cache.Remove(Cache.APPLICATIONOCCUPANTS.ToString());
                applicationoccupantDto = this.mapper.Map<ApplicationOccupantDto>(applicationOccupant);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ApplicationOccupant. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return applicationoccupantDto;
        }

        /// <inheritdoc/>
        public async Task DeleteApplicationOccupant(Guid ID)
        {
            try
            {
                var applicationOccupant = await this.applicationoccupantRepository.GetByID(ID);
                if (applicationOccupant == null)
                    throw new KeyNotFoundException("ApplicationOccupant with the specified ID was not found.");
                await applicationoccupantRepository.Delete(applicationOccupant);
                await applicationoccupantRepository.Save();
                cache.Remove(Cache.APPLICATIONOCCUPANTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ApplicationOccupant . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ApplicationOccupantDto?> UpdateApplicationOccupantStatus(Guid id, string status)
        {
            var applicationOccupant = await applicationoccupantRepository.GetByID(id);
            if (applicationOccupant == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                applicationOccupant.Status = "Pending";
            }
            else
            {
                applicationOccupant.Status = status;
            }

            await applicationoccupantRepository.Update(applicationOccupant);
            await applicationoccupantRepository.Save();
            cache.Remove(Cache.APPLICATIONOCCUPANTS.ToString());
            return this.mapper.Map<ApplicationOccupantDto>(applicationOccupant);
        }
    }
}