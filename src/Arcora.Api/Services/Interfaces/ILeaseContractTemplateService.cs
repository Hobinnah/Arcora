using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces;

public interface ILeaseContractTemplateService
{
    Task<PagedResult<LeaseContractTemplateDto>> GetByOrganizationID(Guid organizationID, Paging paging);
    Task<LeaseContractTemplateDto?> GetByID(Guid templateID);
    Task<IEnumerable<LeaseContractTemplateVersionDto>> GetVersions(Guid templateID);
    Task<LeaseContractTemplateDto> CreateTemplate(LeaseContractTemplateUpsertDto request);
    Task<LeaseContractTemplateDto?> UpdateTemplate(Guid templateID, LeaseContractTemplateUpsertDto request);
    Task<LeaseContractTemplateDto?> SetDefaultTemplate(Guid templateID, string? updatedBy);
    Task<LeaseContractRenderResultDto?> RenderContract(LeaseContractRenderRequestDto request);
    Task<string> GetStandardTemplateHtml();
}
