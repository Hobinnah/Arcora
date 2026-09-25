using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CohostInvitationRepository : Repository<CohostInvitation>, ICohostInvitationRepository
    {
        private readonly ArcoraDbContext context;

        public CohostInvitationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<CohostInvitation>> GetByOrganizationIDAsync(Guid organizationID)
        {
            return await this.context.CohostInvitations
                .AsNoTracking()
                .Where(x => x.OrganizationID == organizationID)
                .OrderByDescending(x => x.CapturedDate)
                .ThenByDescending(x => x.ExpiresAt)
                .ToListAsync();
        }
    }
}
