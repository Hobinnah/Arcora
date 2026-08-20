// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingPhotoRepository : IRepository<ListingPhoto>
    {
        Task<List<ListingPhoto>> GetListingPhotoAsync();
        Task<bool> HasListingPhotosAsync();
    }
}