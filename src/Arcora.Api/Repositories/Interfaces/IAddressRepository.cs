// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<List<Address>> GetAddressAsync();
        Task<bool> HasAddressesAsync();
    }
}