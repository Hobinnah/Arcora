// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IReceiptMasterRepository : IRepository<ReceiptMaster>
    {
        Task<List<ReceiptMaster>> GetReceiptMasterAsync();
        Task<bool> HasReceiptMastersAsync();
    }
}