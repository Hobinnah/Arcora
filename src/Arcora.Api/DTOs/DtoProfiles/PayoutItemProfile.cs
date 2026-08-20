// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class PayoutItemProfile : Profile
    {
        public PayoutItemProfile()
        {
            CreateMap<PayoutItem, PayoutItemDto>().ReverseMap();
        }
    }
}