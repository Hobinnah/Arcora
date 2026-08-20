// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> GetNotificationAsync();
        Task<bool> HasNotificationsAsync();
    }
}