using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces;

public interface ILeaseContractTemplateRepository : IRepository<LeaseContractTemplate>
{
    Task<List<LeaseContractTemplate>> GetByOrganizationIDAsync(Guid organizationID);
    Task<LeaseContractTemplate?> GetByTemplateIDAsync(Guid templateID);
    Task<LeaseContractTemplate?> GetDefaultByOrganizationIDAsync(Guid organizationID);
    Task<List<LeaseContractTemplateVersion>> GetVersionsAsync(Guid templateID);
    Task<int> GetLatestVersionNumberAsync(Guid templateID);
    Task ClearDefaultFlagsAsync(Guid organizationID, Guid? excludeTemplateID = null);
    Task AddVersionAsync(LeaseContractTemplateVersion version);
}
