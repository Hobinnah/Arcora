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
    public class AuditLogService : IAuditLogService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<AuditLogService> logger;
        private readonly IAuditLogRepository auditlogRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public AuditLogService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<AuditLogService> logger, IAuditLogRepository auditlogRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.auditlogRepository = auditlogRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<AuditLogDto>> GetAll(Paging paging)
        {
            IEnumerable<AuditLog> entities;
            try
            {
                entities = cache.Get<IEnumerable<AuditLog>>(Cache.AUDITLOGS.ToString()) ?? new List<AuditLog>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.auditlogRepository.GetAuditLogAsync())?.Where(x => x != null) ?? new List<AuditLog>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<AuditLog>>(Cache.AUDITLOGS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AuditLog by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<AuditLogDto>
                {
                    Data = new List<AuditLogDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<AuditLog> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Action) && x.Action.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.AuditLogID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<AuditLogDto>>(pagedEntities);
            return new PagedResult<AuditLogDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<AuditLogDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<AuditLog> entities = cache.Get<IEnumerable<AuditLog>>(Cache.AUDITLOGS.ToString()) ?? new List<AuditLog>();
                AuditLog? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.AuditLogID == ID);
                }
                else
                {
                    match = await this.auditlogRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<AuditLogDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching AuditLog by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AuditLogDto> CreateAuditLog(AuditLogDto auditlogDto)
        {
            AuditLog auditLog = new AuditLog();
            IEnumerable<AuditLog?> checkEntity;
            try
            {
                checkEntity = await this.auditlogRepository.Find(x => x.Action!.ToLower().Trim() == auditlogDto.Action!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    auditLog = this.mapper.Map<AuditLog>(auditlogDto);
                    auditLog.AuditLogID = Guid.NewGuid();
                    auditLog.ActorUserID = auditlogDto.ActorUserID == 0 ? null : auditlogDto.ActorUserID;
                    auditLog.TenantID = auditlogDto.TenantID == Guid.Empty ? null : auditlogDto.TenantID;
                    auditLog.OrganizationMemberID = auditlogDto.OrganizationMemberID == Guid.Empty ? null : auditlogDto.OrganizationMemberID;
                    auditLog.OrganizationID = auditlogDto.OrganizationID == Guid.Empty ? null : auditlogDto.OrganizationID;
                    auditLog.CorrelationID = string.IsNullOrEmpty(auditlogDto.CorrelationID) ? null : auditlogDto.CorrelationID;
                    auditLog.CapturedDate = DateTime.UtcNow;
                    auditLog = await auditlogRepository.Create(auditLog) ?? new AuditLog();
                    await auditlogRepository.Save();
                    cache.Remove(Cache.AUDITLOGS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating AuditLog. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<AuditLogDto>(auditLog);
        }

        /// <inheritdoc/>
        public async Task<AuditLogDto?> UpdateAuditLog(Guid id, AuditLogDto auditlogDto)
        {
            try
            {
                var existing = await this.auditlogRepository.GetByID(id);
                if (existing == null)
                    return null;
                AuditLog auditLog = this.mapper.Map<AuditLog>(auditlogDto);
                auditLog = await auditlogRepository.Update(auditLog) ?? new AuditLog();
                await auditlogRepository.Save();
                cache.Remove(Cache.AUDITLOGS.ToString());
                auditlogDto = this.mapper.Map<AuditLogDto>(auditLog);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating AuditLog. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return auditlogDto;
        }

        /// <inheritdoc/>
        public async Task DeleteAuditLog(Guid ID)
        {
            try
            {
                var auditLog = await this.auditlogRepository.GetByID(ID);
                if (auditLog == null)
                    throw new KeyNotFoundException("AuditLog with the specified ID was not found.");
                await auditlogRepository.Delete(auditLog);
                await auditlogRepository.Save();
                cache.Remove(Cache.AUDITLOGS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting AuditLog . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }
    }
}