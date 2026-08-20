// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class FeeTypeProfile : Profile
    {
        public FeeTypeProfile()
        {
            CreateMap<FeeType, FeeTypeDto>().ReverseMap();
        }
    }
}