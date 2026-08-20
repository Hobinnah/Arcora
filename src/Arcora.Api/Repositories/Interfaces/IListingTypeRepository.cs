// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingTypeRepository : IRepository<ListingType>
    {
        Task<bool> HasListingTypesAsync();
    }
}