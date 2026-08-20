// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAmenityCatalogRepository : IRepository<AmenityCatalog>
    {
        Task<bool> HasAmenityCatalogsAsync();
    }
}