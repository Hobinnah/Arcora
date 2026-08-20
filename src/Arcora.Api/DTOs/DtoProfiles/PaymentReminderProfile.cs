// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class PaymentReminderProfile : Profile
    {
        public PaymentReminderProfile()
        {
            CreateMap<PaymentReminder, PaymentReminderDto>().ReverseMap();
        }
    }
}