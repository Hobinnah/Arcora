// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs.DtoProfiles
{
    public class ConversationMessageProfile : Profile
    {
        public ConversationMessageProfile()
        {
            CreateMap<ConversationMessage, ConversationMessageDto>()
                .ForMember(dto => dto.Reactions, options => options.Ignore())
                .ForMember(dto => dto.AttachmentUrl, options => options.Ignore())
                .ForMember(dto => dto.ReplyToPreview, options => options.Ignore())
                .ForMember(dto => dto.ReplyToSenderName, options => options.Ignore())
                .ForMember(dto => dto.SenderName, options => options.MapFrom((src, _) => SenderFirstName(src)))
                .ReverseMap()
                .ForMember(entity => entity.AttachmentSendKey, options => options.Ignore())
                .ForMember(entity => entity.AttachmentSenderScope, options => options.Ignore())
                .ForMember(entity => entity.AttachmentPayloadHash, options => options.Ignore());
            CreateMap<ConversationMessageReaction, ConversationMessageReactionDto>();
        }

        private static string? SenderFirstName(ConversationMessage message)
        {
            var name = message.SenderTenant?.User?.FirstName
                ?? message.SenderOrganizationMember?.User?.FirstName
                ?? message.SenderUser?.FirstName;
            return string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        }
    }
}