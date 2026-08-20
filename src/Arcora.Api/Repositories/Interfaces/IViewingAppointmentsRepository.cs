// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IViewingAppointmentsRepository : IRepository<ViewingAppointments>
    {
        Task<List<ViewingAppointments>> GetViewingAppointmentsAsync();
        Task<bool> HasViewingAppointmentsAsync();
    }
}