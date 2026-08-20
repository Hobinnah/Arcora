// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AttachmentRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasAttachmentsAsync()
        {
            return await this.context.Set<Attachment>().AnyAsync();
        }
    }
}