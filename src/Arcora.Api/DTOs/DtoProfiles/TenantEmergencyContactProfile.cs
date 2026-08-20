// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class TenantEmergencyContactProfile : Profile
    {
        public TenantEmergencyContactProfile()
        {
            CreateMap<TenantEmergencyContact, TenantEmergencyContactDto>().ReverseMap();
        }
    }
}