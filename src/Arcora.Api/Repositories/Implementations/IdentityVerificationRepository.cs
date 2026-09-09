// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class IdentityVerificationRepository : Repository<IdentityVerification>, IIdentityVerificationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public IdentityVerificationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<IdentityVerification>> GetIdentityVerificationAsync()
        {
            return await ApplyDefaultOrder(this.context.IdentityVerifications.AsNoTracking().Include(x => x.User)).ToListAsync();
        }

        public async Task<bool> HasIdentityVerificationsAsync()
        {
            return await this.context.Set<IdentityVerification>().AnyAsync();
        }
    }
}