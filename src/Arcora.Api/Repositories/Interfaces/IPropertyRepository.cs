// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<List<Property>> GetPropertyAsync();
        Task<bool> HasPropertiesAsync();
    }
}