// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IInspectionRepository : IRepository<Inspection>
    {
        Task<List<Inspection>> GetInspectionAsync();
        Task<bool> HasInspectionsAsync();
    }
}