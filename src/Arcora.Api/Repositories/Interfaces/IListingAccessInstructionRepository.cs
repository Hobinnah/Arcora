// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingAccessInstructionRepository : IRepository<ListingAccessInstruction>
    {
        Task<List<ListingAccessInstruction>> GetListingAccessInstructionAsync();
        Task<bool> HasListingAccessInstructionsAsync();
    }
}