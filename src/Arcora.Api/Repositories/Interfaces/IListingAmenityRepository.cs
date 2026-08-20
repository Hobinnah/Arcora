// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingAmenityRepository : IRepository<ListingAmenity>
    {
        Task<List<ListingAmenity>> GetListingAmenityAsync();
        Task<bool> HasListingAmenitiesAsync();
    }
}