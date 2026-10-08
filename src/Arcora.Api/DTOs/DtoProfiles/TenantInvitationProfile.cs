// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class TenantInvitationProfile : Profile
    {
        public TenantInvitationProfile()
        {
            CreateMap<TenantInvitation, TenantInvitationDto>()
                .ForMember(dto => dto.Price, options => options.MapFrom(entity => entity.MonthlyRentAmount))
                .ReverseMap()
                .ForMember(entity => entity.MonthlyRentAmount, options => options.Ignore())
                .ForMember(entity => entity.SecurityDepositAmount, options => options.Ignore())
                .ForMember(entity => entity.Currency, options => options.Ignore())
                .ForMember(entity => entity.StartDate, options => options.Ignore())
                .ForMember(entity => entity.EndDate, options => options.Ignore())
                .ForMember(entity => entity.LeaseTermMonths, options => options.Ignore())
                .ForMember(entity => entity.ReservationHoldID, options => options.Ignore());
        }
    }
}