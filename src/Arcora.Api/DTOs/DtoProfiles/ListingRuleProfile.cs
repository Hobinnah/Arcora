// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ListingRuleProfile : Profile
    {
        public ListingRuleProfile()
        {
            CreateMap<ListingRule, ListingRuleDto>().ReverseMap();
        }
    }
}