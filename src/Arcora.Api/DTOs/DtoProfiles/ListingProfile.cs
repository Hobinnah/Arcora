// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ListingProfile : Profile
    {
        public ListingProfile()
        {
            CreateMap<Listing, ListingDto>()
                .ForMember(dest => dest.ReviewList, opt => opt.MapFrom(src =>
                    src.Leases == null
                        ? new List<Rating>()
                        : src.Leases
                            .Where(l => l.Ratings != null)
                            .SelectMany(l => l.Ratings!)
                            .Where(r => r.IsPublic)
                            .ToList()))
                .ReverseMap()
                .ForMember(dest => dest.Leases, opt => opt.Ignore());
        }
    }
}