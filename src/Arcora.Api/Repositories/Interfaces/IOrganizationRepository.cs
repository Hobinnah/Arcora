// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrganizationRepository : IRepository<Organization>
    {
        Task<bool> HasOrganizationsAsync();
    }
}