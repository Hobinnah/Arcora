// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITaxRateRepository : IRepository<TaxRate>
    {
        Task<bool> HasTaxRatesAsync();
    }
}