// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrganizationStatementRepository : IRepository<OrganizationStatement>
    {
        Task<List<OrganizationStatement>> GetOrganizationStatementAsync();
        Task<bool> HasOrganizationStatementsAsync();
    }
}