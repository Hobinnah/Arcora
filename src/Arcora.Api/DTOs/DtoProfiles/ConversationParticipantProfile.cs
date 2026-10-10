// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ConversationParticipantProfile : Profile
    {
        public ConversationParticipantProfile()
        {
            CreateMap<ConversationParticipant, ConversationParticipantDto>().ReverseMap()
                .ForMember(x => x.Conversation, options => options.Ignore())
                .ForMember(x => x.User, options => options.Ignore())
                .ForMember(x => x.Tenant, options => options.Ignore())
                .ForMember(x => x.OrganizationMember, options => options.Ignore());
        }
    }
}