// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class FraudCaseProfile : Profile
    {
        public FraudCaseProfile()
        {
            CreateMap<FraudCase, FraudCaseDto>().ReverseMap();
        }
    }
}