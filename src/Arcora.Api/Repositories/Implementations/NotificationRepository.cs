// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public NotificationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Notification>> GetNotificationAsync()
        {
            return await ApplyDefaultOrder(this.context.Notifications.AsNoTracking().Include(x => x.RecipientUser) // FK to User
            .Include(x => x.Tenant).Include(x => x.Organization).Include(x => x.OrganizationMember)).ToListAsync();
        }

        public async Task<bool> HasNotificationsAsync()
        {
            return await this.context.Set<Notification>().AnyAsync();
        }
    }
}