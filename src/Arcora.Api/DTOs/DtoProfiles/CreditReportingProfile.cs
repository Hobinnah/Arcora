// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class CreditReportingProfile : Profile
    {
        public CreditReportingProfile()
        {
            CreateMap<CreditReporting, CreditReportingDto>().ReverseMap();
        }
    }
}