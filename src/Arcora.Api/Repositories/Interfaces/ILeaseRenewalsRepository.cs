// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseRenewalsRepository : IRepository<LeaseRenewals>
    {
        Task<List<LeaseRenewals>> GetLeaseRenewalsAsync();
        Task<bool> HasLeaseRenewalsAsync();
    }
}