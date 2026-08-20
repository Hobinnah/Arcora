// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IIdentityVerificationRepository : IRepository<IdentityVerification>
    {
        Task<List<IdentityVerification>> GetIdentityVerificationAsync();
        Task<bool> HasIdentityVerificationsAsync();
    }
}