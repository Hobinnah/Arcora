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
    public class NotificationService : INotificationService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<NotificationService> logger;
        private readonly INotificationRepository notificationRepository;
        private readonly IOptions<CacheConfiguration> _options;
        public NotificationService(IMapper mapper, IMemoryCache cache, IOptions<CacheConfiguration> options, ILogger<NotificationService> logger, INotificationRepository notificationRepository)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.notificationRepository = notificationRepository;
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<NotificationDto>> GetAll(Paging paging)
        {
            IEnumerable<Notification> entities;
            try
            {
                entities = cache.Get<IEnumerable<Notification>>(Cache.NOTIFICATIONS.ToString()) ?? new List<Notification>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.notificationRepository.GetNotificationAsync())?.Where(x => x != null) ?? new List<Notification>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<Notification>>(Cache.NOTIFICATIONS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Notification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<NotificationDto>
                {
                    Data = new List<NotificationDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<Notification> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.Subject) && x.Subject.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.NotificationID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<NotificationDto>>(pagedEntities);
            return new PagedResult<NotificationDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<NotificationDto?> GetID(Guid ID)
        {
            try
            {
                IEnumerable<Notification> entities = cache.Get<IEnumerable<Notification>>(Cache.NOTIFICATIONS.ToString()) ?? new List<Notification>();
                Notification? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.NotificationID == ID);
                }
                else
                {
                    match = await this.notificationRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<NotificationDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching Notification by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<NotificationDto> CreateNotification(NotificationDto notificationDto)
        {
            Notification notification = new Notification();
            IEnumerable<Notification?> checkEntity;
            try
            {
                checkEntity = await this.notificationRepository.Find(x => x.Subject!.ToLower().Trim() == notificationDto.Subject!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    notification = this.mapper.Map<Notification>(notificationDto);
                    notification.NotificationID = Guid.NewGuid();
                    notification.RecipientUserID = notificationDto.RecipientUserID == 0 ? null : notificationDto.RecipientUserID;
                    notification.TenantID = notificationDto.TenantID == Guid.Empty ? null : notificationDto.TenantID;
                    notification.OrganizationID = notificationDto.OrganizationID == Guid.Empty ? null : notificationDto.OrganizationID;
                    notification.OrganizationMemberID = notificationDto.OrganizationMemberID == Guid.Empty ? null : notificationDto.OrganizationMemberID;
                    notification.ProviderMessageID = string.IsNullOrEmpty(notificationDto.ProviderMessageID) ? null : notificationDto.ProviderMessageID;
                    notification.CapturedDate = DateTime.UtcNow;
                    notification = await notificationRepository.Create(notification) ?? new Notification();
                    await notificationRepository.Save();
                    cache.Remove(Cache.NOTIFICATIONS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating Notification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<NotificationDto>(notification);
        }

        /// <inheritdoc/>
        public async Task<NotificationDto?> UpdateNotification(Guid id, NotificationDto notificationDto)
        {
            try
            {
                var existing = await this.notificationRepository.GetByID(id);
                if (existing == null)
                    return null;
                Notification notification = this.mapper.Map<Notification>(notificationDto);
                notification = await notificationRepository.Update(notification) ?? new Notification();
                await notificationRepository.Save();
                cache.Remove(Cache.NOTIFICATIONS.ToString());
                notificationDto = this.mapper.Map<NotificationDto>(notification);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating Notification. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return notificationDto;
        }

        /// <inheritdoc/>
        public async Task DeleteNotification(Guid ID)
        {
            try
            {
                var notification = await this.notificationRepository.GetByID(ID);
                if (notification == null)
                    throw new KeyNotFoundException("Notification with the specified ID was not found.");
                await notificationRepository.Delete(notification);
                await notificationRepository.Save();
                cache.Remove(Cache.NOTIFICATIONS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting Notification . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<NotificationDto?> UpdateNotificationStatus(Guid id, string status)
        {
            var notification = await notificationRepository.GetByID(id);
            if (notification == null)
                return null;
            if (status.Equals("Revert", StringComparison.OrdinalIgnoreCase))
            {
                notification.Status = "Pending";
            }
            else
            {
                notification.Status = status;
            }

            await notificationRepository.Update(notification);
            await notificationRepository.Save();
            cache.Remove(Cache.NOTIFICATIONS.ToString());
            return this.mapper.Map<NotificationDto>(notification);
        }
    }
}