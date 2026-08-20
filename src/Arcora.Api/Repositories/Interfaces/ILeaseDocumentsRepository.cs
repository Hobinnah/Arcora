// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseDocumentsRepository : IRepository<LeaseDocuments>
    {
        Task<List<LeaseDocuments>> GetLeaseDocumentsAsync();
        Task<bool> HasLeaseDocumentsAsync();
    }
}