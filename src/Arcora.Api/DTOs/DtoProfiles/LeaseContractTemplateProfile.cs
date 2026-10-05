using AutoMapper;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs.DtoProfiles;

public class LeaseContractTemplateProfile : Profile
{
    public LeaseContractTemplateProfile()
    {
        CreateMap<LeaseContractTemplate, LeaseContractTemplateDto>()
            .ForMember(dest => dest.CurrentVersionNumber,
                opt => opt.MapFrom(src => src.Versions != null && src.Versions.Any() ? src.Versions.Max(v => v.VersionNumber) : 0));

        CreateMap<LeaseContractTemplateVersion, LeaseContractTemplateVersionDto>();
    }
}
