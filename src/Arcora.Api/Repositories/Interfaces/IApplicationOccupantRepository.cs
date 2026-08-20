// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IApplicationOccupantRepository : IRepository<ApplicationOccupant>
    {
        Task<List<ApplicationOccupant>> GetApplicationOccupantAsync();
        Task<bool> HasApplicationOccupantsAsync();
    }
}