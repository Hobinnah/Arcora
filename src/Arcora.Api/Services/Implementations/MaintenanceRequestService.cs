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
    public class MaintenanceRequestService : IMaintenanceRequestService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<MaintenanceRequestService> logger;
        private readonly IMaintenanceRequestRepository maintenancerequestRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public MaintenanceRequestService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<MaintenanceRequestService> logger, IMaintenanceRequestRepository maintenancerequestRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.maintenancerequestRepository = maintenancerequestRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<MaintenanceRequestDto>> GetAll(Paging paging)
        {
            IEnumerable<MaintenanceRequest> entities;
            try
            {
                entities = cache.Get<IEnumerable<MaintenanceRequest>>(Cache.MAINTENANCEREQUESTS.ToString()) ?? new List<MaintenanceRequest>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.maintenancerequestRepository.GetMaintenanceRequestAsync())?.Where(x => x != null) ?? new List<MaintenanceRequest>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<MaintenanceRequest>>(Cache.MAINTENANCEREQUESTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching MaintenanceRequest by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<MaintenanceRequestDto>
                {
                    Data = new List<MaintenanceRequestDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<MaintenanceRequest> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.MaintenanceRequestID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<MaintenanceRequestDto>>(pagedEntities);
            return new PagedResult<MaintenanceRequestDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<MaintenanceRequestDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<MaintenanceRequest> entities = cache.Get<IEnumerable<MaintenanceRequest>>(Cache.MAINTENANCEREQUESTS.ToString()) ?? new List<MaintenanceRequest>();
                MaintenanceRequest? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.MaintenanceRequestID == ID);
                }
                else
                {
                    match = await this.maintenancerequestRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<MaintenanceRequestDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching MaintenanceRequest by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<MaintenanceRequestDto> CreateMaintenanceRequest(MaintenanceRequestDto maintenancerequestDto)
        {
            MaintenanceRequest maintenanceRequest = new MaintenanceRequest();
            IEnumerable<MaintenanceRequest?> checkEntity;
            try
            {
                checkEntity = await this.maintenancerequestRepository.Find(x => x.Title!.ToLower().Trim() == maintenancerequestDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    maintenanceRequest = this.mapper.Map<MaintenanceRequest>(maintenancerequestDto);
                    maintenanceRequest.MaintenanceRequestID = Guid.NewGuid();
                    maintenanceRequest.PropertyID = maintenancerequestDto.PropertyID == Guid.Empty ? null : maintenancerequestDto.PropertyID;
                    maintenanceRequest.RentalUnitID = maintenancerequestDto.RentalUnitID == Guid.Empty ? null : maintenancerequestDto.RentalUnitID;
                    maintenanceRequest.ListingID = maintenancerequestDto.ListingID == Guid.Empty ? null : maintenancerequestDto.ListingID;
                    maintenanceRequest.LeaseID = maintenancerequestDto.LeaseID == Guid.Empty ? null : maintenancerequestDto.LeaseID;
                    maintenanceRequest.LeaseRenewalID = maintenancerequestDto.LeaseRenewalID == Guid.Empty ? null : maintenancerequestDto.LeaseRenewalID;
                    maintenanceRequest.SubmittedByTenantID = maintenancerequestDto.SubmittedByTenantID == Guid.Empty ? null : maintenancerequestDto.SubmittedByTenantID;
                    maintenanceRequest.CapturedDate = DateTime.UtcNow;
                    maintenanceRequest = await maintenancerequestRepository.Create(maintenanceRequest) ?? new MaintenanceRequest();
                    await maintenancerequestRepository.Save();
                    cache.Remove(Cache.MAINTENANCEREQUESTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating MaintenanceRequest. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<MaintenanceRequestDto>(maintenanceRequest);
        }

        /// <inheritdoc/>
        public async Task<MaintenanceRequestDto?> UpdateMaintenanceRequest(Guid id, MaintenanceRequestDto maintenancerequestDto)
        {
            try
            {
                var existing = await this.maintenancerequestRepository.GetByID(id);
                if (existing == null)
                    return null;
                MaintenanceRequest maintenanceRequest = this.mapper.Map<MaintenanceRequest>(maintenancerequestDto);
                maintenanceRequest = await maintenancerequestRepository.Update(maintenanceRequest) ?? new MaintenanceRequest();
                await maintenancerequestRepository.Save();
                cache.Remove(Cache.MAINTENANCEREQUESTS.ToString());
                maintenancerequestDto = this.mapper.Map<MaintenanceRequestDto>(maintenanceRequest);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating MaintenanceRequest. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return maintenancerequestDto;
        }

        /// <inheritdoc/>
        public async Task DeleteMaintenanceRequest(Guid ID)
        {
            try
            {
                var maintenanceRequest = await this.maintenancerequestRepository.GetByID(ID);
                if (maintenanceRequest == null)
                    throw new KeyNotFoundException("MaintenanceRequest with the specified ID was not found.");
                await maintenancerequestRepository.Delete(maintenanceRequest);
                await maintenancerequestRepository.Save();
                cache.Remove(Cache.MAINTENANCEREQUESTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting MaintenanceRequest . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<MaintenanceRequestDto?> UpdateMaintenanceRequestStatus(Guid id, string status)
        {
            var maintenanceRequest = await maintenancerequestRepository.GetByID(id);
            if (maintenanceRequest == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                maintenanceRequest.Status = "Pending";
            }
            else
            {
                maintenanceRequest.Status = status;
            }

            await maintenancerequestRepository.Update(maintenanceRequest);
            await maintenancerequestRepository.Save();
            cache.Remove(Cache.MAINTENANCEREQUESTS.ToString());
            return this.mapper.Map<MaintenanceRequestDto>(maintenanceRequest);
        }
    }
}