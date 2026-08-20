// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> HasCategoriesAsync();
    }
}