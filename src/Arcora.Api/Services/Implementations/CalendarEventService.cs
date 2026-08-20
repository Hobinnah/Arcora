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
    public class CalendarEventService : ICalendarEventService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<CalendarEventService> logger;
        private readonly ICalendarEventRepository calendareventRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public CalendarEventService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<CalendarEventService> logger, ICalendarEventRepository calendareventRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.calendareventRepository = calendareventRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<CalendarEventDto>> GetAll(Paging paging)
        {
            IEnumerable<CalendarEvent> entities;
            try
            {
                entities = cache.Get<IEnumerable<CalendarEvent>>(Cache.CALENDAREVENTS.ToString()) ?? new List<CalendarEvent>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.calendareventRepository.GetCalendarEventAsync())?.Where(x => x != null) ?? new List<CalendarEvent>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<CalendarEvent>>(Cache.CALENDAREVENTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CalendarEvent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<CalendarEventDto>
                {
                    Data = new List<CalendarEventDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<CalendarEvent> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.CalendarEventID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<CalendarEventDto>>(pagedEntities);
            return new PagedResult<CalendarEventDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<CalendarEventDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<CalendarEvent> entities = cache.Get<IEnumerable<CalendarEvent>>(Cache.CALENDAREVENTS.ToString()) ?? new List<CalendarEvent>();
                CalendarEvent? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.CalendarEventID == ID);
                }
                else
                {
                    match = await this.calendareventRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<CalendarEventDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching CalendarEvent by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CalendarEventDto> CreateCalendarEvent(CalendarEventDto calendareventDto)
        {
            CalendarEvent calendarEvent = new CalendarEvent();
            IEnumerable<CalendarEvent?> checkEntity;
            try
            {
                checkEntity = await this.calendareventRepository.Find(x => x.Title!.ToLower().Trim() == calendareventDto.Title!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    calendarEvent = this.mapper.Map<CalendarEvent>(calendareventDto);
                    calendarEvent.CalendarEventID = Guid.NewGuid();
                    calendarEvent.LeaseID = calendareventDto.LeaseID == Guid.Empty ? null : calendareventDto.LeaseID;
                    calendarEvent.RentalApplicationID = calendareventDto.RentalApplicationID == Guid.Empty ? null : calendareventDto.RentalApplicationID;
                    calendarEvent.ReservationHoldID = calendareventDto.ReservationHoldID == Guid.Empty ? null : calendareventDto.ReservationHoldID;
                    calendarEvent.MaintenanceRequestID = calendareventDto.MaintenanceRequestID == Guid.Empty ? null : calendareventDto.MaintenanceRequestID;
                    calendarEvent.SourceReferenceID = string.IsNullOrEmpty(calendareventDto.SourceReferenceID) ? null : calendareventDto.SourceReferenceID;
                    calendarEvent.ExternalCalendarID = string.IsNullOrEmpty(calendareventDto.ExternalCalendarID) ? null : calendareventDto.ExternalCalendarID;
                    calendarEvent.CapturedDate = DateTime.UtcNow;
                    calendarEvent = await calendareventRepository.Create(calendarEvent) ?? new CalendarEvent();
                    await calendareventRepository.Save();
                    cache.Remove(Cache.CALENDAREVENTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating CalendarEvent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<CalendarEventDto>(calendarEvent);
        }

        /// <inheritdoc/>
        public async Task<CalendarEventDto?> UpdateCalendarEvent(Guid id, CalendarEventDto calendareventDto)
        {
            try
            {
                var existing = await this.calendareventRepository.GetByID(id);
                if (existing == null)
                    return null;
                CalendarEvent calendarEvent = this.mapper.Map<CalendarEvent>(calendareventDto);
                calendarEvent = await calendareventRepository.Update(calendarEvent) ?? new CalendarEvent();
                await calendareventRepository.Save();
                cache.Remove(Cache.CALENDAREVENTS.ToString());
                calendareventDto = this.mapper.Map<CalendarEventDto>(calendarEvent);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating CalendarEvent. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return calendareventDto;
        }

        /// <inheritdoc/>
        public async Task DeleteCalendarEvent(Guid ID)
        {
            try
            {
                var calendarEvent = await this.calendareventRepository.GetByID(ID);
                if (calendarEvent == null)
                    throw new KeyNotFoundException("CalendarEvent with the specified ID was not found.");
                await calendareventRepository.Delete(calendarEvent);
                await calendareventRepository.Save();
                cache.Remove(Cache.CALENDAREVENTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting CalendarEvent . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<CalendarEventDto?> UpdateCalendarEventStatus(Guid id, string status)
        {
            var calendarEvent = await calendareventRepository.GetByID(id);
            if (calendarEvent == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                calendarEvent.Status = "Pending";
            }
            else
            {
                calendarEvent.Status = status;
            }

            await calendareventRepository.Update(calendarEvent);
            await calendareventRepository.Save();
            cache.Remove(Cache.CALENDAREVENTS.ToString());
            return this.mapper.Map<CalendarEventDto>(calendarEvent);
        }
    }
}