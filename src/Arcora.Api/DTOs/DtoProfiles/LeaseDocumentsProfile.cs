// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class LeaseDocumentsProfile : Profile
    {
        public LeaseDocumentsProfile()
        {
            CreateMap<LeaseDocuments, LeaseDocumentsDto>()
                .ReverseMap()
                // Url on the DTO is a transient, short-lived SAS URL generated on read.
                // It must never overwrite the persisted raw blob URL on the entity.
                .ForMember(dest => dest.Url, opt => opt.Ignore());
        }
    }
}