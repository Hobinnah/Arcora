// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public CategoryRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasCategoriesAsync()
        {
            return await this.context.Set<Category>().AnyAsync();
        }
    }
}