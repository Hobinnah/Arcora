// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ListingPolicyProfile : Profile
    {
        public ListingPolicyProfile()
        {
            CreateMap<ListingPolicy, ListingPolicyDto>().ReverseMap();
        }
    }
}