// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<List<Rating>> GetRatingAsync();
        Task<bool> HasRatingsAsync();
    }
}