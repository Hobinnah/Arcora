// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IRentalApplicationRepository : IRepository<RentalApplication>
    {
        Task<List<RentalApplication>> GetRentalApplicationAsync();
        Task<bool> HasRentalApplicationsAsync();
    }
}