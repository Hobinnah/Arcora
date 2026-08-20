// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class PaymentAllocationProfile : Profile
    {
        public PaymentAllocationProfile()
        {
            CreateMap<PaymentAllocation, PaymentAllocationDto>().ReverseMap();
        }
    }
}