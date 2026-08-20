// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Retrieves all notifications with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<NotificationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a notification by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<NotificationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new notification entry.
        /// </summary>
        /// <param name = "notificationDto"></param>
        /// <returns></returns>
        Task<NotificationDto> CreateNotification(NotificationDto notificationDto);
        /// <summary>
        /// Updates an existing notification entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "notificationDto"></param>
        /// <returns></returns>
        Task<NotificationDto?> UpdateNotification(Guid id, NotificationDto notificationDto);
        /// <summary>
        /// Deletes a notification entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteNotification(Guid ID);
        /// <summary>
        /// Updates the status of a notification by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the notification to update.</param>
        /// <param name = "status">The new status value to assign to the notification.</param>
        /// <returns>
        /// Returns <see cref = "NotificationDto"/> with the updated notification if successful, or null if not found.
        /// </returns>
        Task<NotificationDto?> UpdateNotificationStatus(Guid id, string status);
    }
}