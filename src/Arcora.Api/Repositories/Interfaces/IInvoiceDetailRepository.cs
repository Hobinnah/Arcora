// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IInvoiceDetailRepository : IRepository<InvoiceDetail>
    {
        Task<List<InvoiceDetail>> GetInvoiceDetailAsync();
        Task<bool> HasInvoiceDetailsAsync();
    }
}