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
    public class ViewingAppointmentsService : IViewingAppointmentsService
    {
        private readonly IMapper mapper;
        private readonly HybridCache cache;
        private readonly ILogger<ViewingAppointmentsService> logger;
        private readonly IViewingAppointmentsRepository viewingappointmentsRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public ViewingAppointmentsService(IMapper mapper, HybridCache cache, IOptions<CacheConfiguration> options, ILogger<ViewingAppointmentsService> logger, IViewingAppointmentsRepository viewingappointmentsRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.viewingappointmentsRepository = viewingappointmentsRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<ViewingAppointmentsDto>> GetAll(Paging paging)
        {
            IEnumerable<ViewingAppointments> entities;
            try
            {
                entities = await cache.GetOrCreateAsync(
                    Cache.VIEWINGAPPOINTMENTS.ToString(),
                    async cancel =>
                        (await this.viewingappointmentsRepository.GetViewingAppointmentsAsync())?.Where(x => x != null).ToList() ?? new List<ViewingAppointments>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ViewingAppointments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<ViewingAppointmentsDto>
                {
                    Data = new List<ViewingAppointmentsDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<ViewingAppointments> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Notes) && x.Notes.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.ViewingAppointmentID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<ViewingAppointmentsDto>>(pagedEntities);
            return new PagedResult<ViewingAppointmentsDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<ViewingAppointmentsDto?> GetID(Guid ID)
        {
            try
            {
                var entities = await cache.GetOrCreateAsync(
                    Cache.VIEWINGAPPOINTMENTS.ToString(),
                    async cancel =>
                        (await this.viewingappointmentsRepository.GetViewingAppointmentsAsync())?.Where(x => x != null).ToList() ?? new List<ViewingAppointments>(),
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(this._options.Value.ExpirationTimeInMinutes) });

                var match = entities.FirstOrDefault(x => x.ViewingAppointmentID == ID);
                if (match == null)
                    match = await this.viewingappointmentsRepository.GetByID(ID);

                return match == null ? null : this.mapper.Map<ViewingAppointmentsDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching ViewingAppointments by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ViewingAppointmentsDto> CreateViewingAppointments(ViewingAppointmentsDto viewingappointmentsDto)
        {
            ViewingAppointments viewingAppointments = new ViewingAppointments();
            IEnumerable<ViewingAppointments?> checkEntity;
            try
            {
                checkEntity = await this.viewingappointmentsRepository.Find(x => x.Notes!.ToLower().Trim() == viewingappointmentsDto.Notes!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    viewingAppointments = this.mapper.Map<ViewingAppointments>(viewingappointmentsDto);
                    viewingAppointments.ViewingAppointmentID = Guid.NewGuid();
                    viewingAppointments.TenantID = viewingappointmentsDto.TenantID == Guid.Empty ? null : viewingappointmentsDto.TenantID;
                    viewingAppointments.AssignedOrganizationMemberID = viewingappointmentsDto.AssignedOrganizationMemberID == Guid.Empty ? null : viewingappointmentsDto.AssignedOrganizationMemberID;
                    viewingAppointments.CapturedDate = DateTime.UtcNow;
                    viewingAppointments = await viewingappointmentsRepository.Create(viewingAppointments) ?? new ViewingAppointments();
                    await viewingappointmentsRepository.Save();
                    await cache.RemoveAsync(Cache.VIEWINGAPPOINTMENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating ViewingAppointments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<ViewingAppointmentsDto>(viewingAppointments);
        }

        /// <inheritdoc/>
        public async Task<ViewingAppointmentsDto?> UpdateViewingAppointments(Guid id, ViewingAppointmentsDto viewingappointmentsDto)
        {
            try
            {
                var existing = await this.viewingappointmentsRepository.GetByID(id);
                if (existing == null)
                    return null;
                ViewingAppointments viewingAppointments = this.mapper.Map<ViewingAppointments>(viewingappointmentsDto);
                viewingAppointments = await viewingappointmentsRepository.Update(viewingAppointments) ?? new ViewingAppointments();
                await viewingappointmentsRepository.Save();
                await cache.RemoveAsync(Cache.VIEWINGAPPOINTMENTS.ToString());
                viewingappointmentsDto = this.mapper.Map<ViewingAppointmentsDto>(viewingAppointments);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating ViewingAppointments. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return viewingappointmentsDto;
        }

        /// <inheritdoc/>
        public async Task DeleteViewingAppointments(Guid ID)
        {
            try
            {
                var viewingAppointments = await this.viewingappointmentsRepository.GetByID(ID);
                if (viewingAppointments == null)
                    throw new KeyNotFoundException("ViewingAppointments with the specified ID was not found.");
                await viewingappointmentsRepository.Delete(viewingAppointments);
                await viewingappointmentsRepository.Save();
                await cache.RemoveAsync(Cache.VIEWINGAPPOINTMENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting ViewingAppointments . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<ViewingAppointmentsDto?> UpdateViewingAppointmentsStatus(Guid id, string status)
        {
            var viewingAppointments = await viewingappointmentsRepository.GetByID(id);
            if (viewingAppointments == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                viewingAppointments.Status = "Pending";
            }
            else
            {
                viewingAppointments.Status = status;
            }

            await viewingappointmentsRepository.Update(viewingAppointments);
            await viewingappointmentsRepository.Save();
            await cache.RemoveAsync(Cache.VIEWINGAPPOINTMENTS.ToString());
            return this.mapper.Map<ViewingAppointmentsDto>(viewingAppointments);
        }
    }
}