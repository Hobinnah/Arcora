// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IWorkOrderRepository : IRepository<WorkOrder>
    {
        Task<List<WorkOrder>> GetWorkOrderAsync();
        Task<bool> HasWorkOrdersAsync();
    }
}