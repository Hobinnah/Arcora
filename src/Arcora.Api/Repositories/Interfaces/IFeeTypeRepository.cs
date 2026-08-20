// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IFeeTypeRepository : IRepository<FeeType>
    {
        Task<bool> HasFeeTypesAsync();
    }
}