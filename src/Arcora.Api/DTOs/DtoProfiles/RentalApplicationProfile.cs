// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class RentalApplicationProfile : Profile
    {
        public RentalApplicationProfile()
        {
            CreateMap<RentalApplication, RentalApplicationDto>()
                .ReverseMap()
                // These related collections are populated for reads only. Ignore them on the
                // DTO -> entity map so a rental application create/update never accidentally
                // inserts or mutates child records.
                .ForMember(dest => dest.ApplicationOccupants, opt => opt.Ignore())
                .ForMember(dest => dest.LeaseDocuments, opt => opt.Ignore())
                .ForMember(dest => dest.TenantGuarantors, opt => opt.Ignore())
                .ForMember(dest => dest.TenantEmergencyContacts, opt => opt.Ignore())
                .ForMember(dest => dest.TenantEmployments, opt => opt.Ignore())
                .ForMember(dest => dest.TenantScreeningChecks, opt => opt.Ignore())
                .ForMember(dest => dest.TenantInvitations, opt => opt.Ignore())
                .ForMember(dest => dest.ReservationHolds, opt => opt.Ignore())
                .ForMember(dest => dest.ViewingAppointments, opt => opt.Ignore())
                .ForMember(dest => dest.CalendarEvents, opt => opt.Ignore())
                .ForMember(dest => dest.Leases, opt => opt.Ignore());
        }
    }
}