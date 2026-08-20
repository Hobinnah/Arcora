// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IMaintenanceRequestRepository : IRepository<MaintenanceRequest>
    {
        Task<List<MaintenanceRequest>> GetMaintenanceRequestAsync();
        Task<bool> HasMaintenanceRequestsAsync();
    }
}