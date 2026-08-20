// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IReservationHoldRepository : IRepository<ReservationHold>
    {
        Task<List<ReservationHold>> GetReservationHoldAsync();
        Task<bool> HasReservationHoldsAsync();
    }
}