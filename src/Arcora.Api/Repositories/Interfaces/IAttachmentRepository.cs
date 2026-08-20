// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        Task<bool> HasAttachmentsAsync();
    }
}