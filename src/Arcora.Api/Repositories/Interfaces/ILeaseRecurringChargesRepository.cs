// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseRecurringChargesRepository : IRepository<LeaseRecurringCharges>
    {
        Task<List<LeaseRecurringCharges>> GetLeaseRecurringChargesAsync();
        Task<bool> HasLeaseRecurringChargesAsync();
    }
}