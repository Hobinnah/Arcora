using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations;

public class LeaseContractTemplateRepository : Repository<LeaseContractTemplate>, ILeaseContractTemplateRepository
{
    private readonly ArcoraDbContext context;

    public LeaseContractTemplateRepository(ArcoraDbContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<List<LeaseContractTemplate>> GetByOrganizationIDAsync(Guid organizationID)
    {
        return await context.LeaseContractTemplates
            .AsNoTracking()
            .Include(x => x.Versions)
            .Where(x => x.OrganizationID == organizationID)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.UpdatedDate ?? x.CapturedDate)
            .ToListAsync();
    }

    public async Task<LeaseContractTemplate?> GetByTemplateIDAsync(Guid templateID)
    {
        return await context.LeaseContractTemplates
            .Include(x => x.Versions)
            .FirstOrDefaultAsync(x => x.LeaseContractTemplateID == templateID);
    }

    public async Task<LeaseContractTemplate?> GetDefaultByOrganizationIDAsync(Guid organizationID)
    {
        return await context.LeaseContractTemplates
            .AsNoTracking()
            .Include(x => x.Versions)
            .Where(x => x.OrganizationID == organizationID && x.IsDefault && x.IsActive)
            .OrderByDescending(x => x.UpdatedDate ?? x.CapturedDate)
            .FirstOrDefaultAsync();
    }

    public async Task<List<LeaseContractTemplateVersion>> GetVersionsAsync(Guid templateID)
    {
        return await context.LeaseContractTemplateVersions
            .AsNoTracking()
            .Where(x => x.LeaseContractTemplateID == templateID)
            .OrderByDescending(x => x.VersionNumber)
            .ToListAsync();
    }

    public async Task<int> GetLatestVersionNumberAsync(Guid templateID)
    {
        return await context.LeaseContractTemplateVersions
            .Where(x => x.LeaseContractTemplateID == templateID)
            .Select(x => (int?)x.VersionNumber)
            .MaxAsync() ?? 0;
    }

    public async Task ClearDefaultFlagsAsync(Guid organizationID, Guid? excludeTemplateID = null)
    {
        var templates = context.LeaseContractTemplates.Where(x => x.OrganizationID == organizationID && x.IsDefault);
        if (excludeTemplateID.HasValue)
            templates = templates.Where(x => x.LeaseContractTemplateID != excludeTemplateID.Value);

        var entities = await templates.ToListAsync();
        foreach (var entity in entities)
        {
            entity.IsDefault = false;
            entity.UpdatedDate = DateTime.UtcNow;
        }
    }

    public async Task AddVersionAsync(LeaseContractTemplateVersion version)
    {
        await context.LeaseContractTemplateVersions.AddAsync(version);
    }
}
