// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ReservationHoldRepository : Repository<ReservationHold>, IReservationHoldRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ReservationHoldRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ReservationHold>> GetReservationHoldAsync()
        {
            return await ApplyDefaultOrder(this.context.ReservationHolds.AsNoTracking().Include(x => x.Listing).Include(x => x.RentalApplication).Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasReservationHoldsAsync()
        {
            return await this.context.Set<ReservationHold>().AnyAsync();
        }
    }
}