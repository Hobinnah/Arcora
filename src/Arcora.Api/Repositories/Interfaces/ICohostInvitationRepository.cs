using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICohostInvitationRepository : IRepository<CohostInvitation>
    {
        Task<List<CohostInvitation>> GetByOrganizationIDAsync(Guid organizationID);
    }
}
