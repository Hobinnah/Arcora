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
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<WorkOrderService> logger;
        private readonly IWorkOrderRepository workorderRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public WorkOrderService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<WorkOrderService> logger, IWorkOrderRepository workorderRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.workorderRepository = workorderRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<WorkOrderDto>> GetAll(Paging paging)
        {
            IEnumerable<WorkOrder> entities;
            try
            {
                entities = cache.Get<IEnumerable<WorkOrder>>(Cache.WORKORDERS.ToString()) ?? new List<WorkOrder>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.workorderRepository.GetWorkOrderAsync())?.Where(x => x != null) ?? new List<WorkOrder>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<WorkOrder>>(Cache.WORKORDERS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching WorkOrder by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<WorkOrderDto>
                {
                    Data = new List<WorkOrderDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<WorkOrder> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.WorkOrderID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<WorkOrderDto>>(pagedEntities);
            return new PagedResult<WorkOrderDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<WorkOrderDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<WorkOrder> entities = cache.Get<IEnumerable<WorkOrder>>(Cache.WORKORDERS.ToString()) ?? new List<WorkOrder>();
                WorkOrder? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.WorkOrderID == ID);
                }
                else
                {
                    match = await this.workorderRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<WorkOrderDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching WorkOrder by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<WorkOrderDto> CreateWorkOrder(WorkOrderDto workorderDto)
        {
            WorkOrder workOrder = new WorkOrder();
            IEnumerable<WorkOrder?> checkEntity;
            try
            {
                checkEntity = await this.workorderRepository.Find(x => x.Title!.ToLower().Trim() == workorderDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    workOrder = this.mapper.Map<WorkOrder>(workorderDto);
                    workOrder.WorkOrderID = Guid.NewGuid();
                    workOrder.ContractorID = workorderDto.ContractorID == Guid.Empty ? null : workorderDto.ContractorID;
                    workOrder.AssignedOrganizationMemberID = workorderDto.AssignedOrganizationMemberID == Guid.Empty ? null : workorderDto.AssignedOrganizationMemberID;
                    workOrder.LeaseID = workorderDto.LeaseID == Guid.Empty ? null : workorderDto.LeaseID;
                    workOrder.LeaseRenewalID = workorderDto.LeaseRenewalID == Guid.Empty ? null : workorderDto.LeaseRenewalID;
                    workOrder.CapturedDate = DateTime.UtcNow;
                    workOrder = await workorderRepository.Create(workOrder) ?? new WorkOrder();
                    await workorderRepository.Save();
                    cache.Remove(Cache.WORKORDERS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating WorkOrder. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<WorkOrderDto>(workOrder);
        }

        /// <inheritdoc/>
        public async Task<WorkOrderDto?> UpdateWorkOrder(Guid id, WorkOrderDto workorderDto)
        {
            try
            {
                var existing = await this.workorderRepository.GetByID(id);
                if (existing == null)
                    return null;
                WorkOrder workOrder = this.mapper.Map<WorkOrder>(workorderDto);
                workOrder = await workorderRepository.Update(workOrder) ?? new WorkOrder();
                await workorderRepository.Save();
                cache.Remove(Cache.WORKORDERS.ToString());
                workorderDto = this.mapper.Map<WorkOrderDto>(workOrder);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating WorkOrder. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return workorderDto;
        }

        /// <inheritdoc/>
        public async Task DeleteWorkOrder(Guid ID)
        {
            try
            {
                var workOrder = await this.workorderRepository.GetByID(ID);
                if (workOrder == null)
                    throw new KeyNotFoundException("WorkOrder with the specified ID was not found.");
                await workorderRepository.Delete(workOrder);
                await workorderRepository.Save();
                cache.Remove(Cache.WORKORDERS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting WorkOrder . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<WorkOrderDto?> UpdateWorkOrderStatus(Guid id, string status)
        {
            var workOrder = await workorderRepository.GetByID(id);
            if (workOrder == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                workOrder.Status = "Pending";
            }
            else
            {
                workOrder.Status = status;
            }

            await workorderRepository.Update(workOrder);
            await workorderRepository.Save();
            cache.Remove(Cache.WORKORDERS.ToString());
            return this.mapper.Map<WorkOrderDto>(workOrder);
        }
    }
}