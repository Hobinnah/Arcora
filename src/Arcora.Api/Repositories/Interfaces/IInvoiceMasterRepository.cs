// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IInvoiceMasterRepository : IRepository<InvoiceMaster>
    {
        Task<List<InvoiceMaster>> GetInvoiceMasterAsync();
        Task<bool> HasInvoiceMastersAsync();
    }
}