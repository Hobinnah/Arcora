// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPayoutItemRepository : IRepository<PayoutItem>
    {
        Task<List<PayoutItem>> GetPayoutItemAsync();
        Task<bool> HasPayoutItemsAsync();
    }
}