// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAutopayMandateRepository : IRepository<AutopayMandate>
    {
        Task<List<AutopayMandate>> GetAutopayMandateAsync();
        Task<bool> HasAutopayMandatesAsync();
    }
}