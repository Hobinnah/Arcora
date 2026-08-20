// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ListingTermPriceProfile : Profile
    {
        public ListingTermPriceProfile()
        {
            CreateMap<ListingTermPrice, ListingTermPriceDto>().ReverseMap();
        }
    }
}