// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class RatingRepository : Repository<Rating>, IRatingRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public RatingRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Rating>> GetRatingAsync()
        {
            return await ApplyDefaultOrder(this.context.Ratings.AsNoTracking().Include(x => x.Lease).Include(x => x.ReviewerUser) // FK to User
            ).ToListAsync();
        }

        public async Task<bool> HasRatingsAsync()
        {
            return await this.context.Set<Rating>().AnyAsync();
        }
    }
}