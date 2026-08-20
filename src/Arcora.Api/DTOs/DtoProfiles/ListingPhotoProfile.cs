// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ListingPhotoProfile : Profile
    {
        public ListingPhotoProfile()
        {
            CreateMap<ListingPhoto, ListingPhotoDto>().ReverseMap();
        }
    }
}