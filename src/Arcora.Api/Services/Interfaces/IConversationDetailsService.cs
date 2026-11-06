using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    /// <summary>
    /// Resolves the reservation/enquiry side pane for a conversation and manages the host's private notes.
    /// Throws <see cref="KeyNotFoundException"/> for unknown conversations and
    /// <see cref="UnauthorizedAccessException"/> when the actor is not a participant.
    /// </summary>
    public interface IConversationDetailsService
    {
        Task<ConversationDetailsDto> GetDetailsAsync(Guid conversationId, ConversationActor actor, CancellationToken cancellationToken = default);

        /// <summary>Creates, replaces or (when blank) clears the shared host note. Returns the saved note, or null when cleared.</summary>
        Task<ConversationNoteDto?> SaveNoteAsync(Guid conversationId, ConversationActor actor, string? note, CancellationToken cancellationToken = default);
    }
}
