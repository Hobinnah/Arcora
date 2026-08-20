// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AmenityCatalogRepository : Repository<AmenityCatalog>, IAmenityCatalogRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AmenityCatalogRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasAmenityCatalogsAsync()
        {
            return await this.context.Set<AmenityCatalog>().AnyAsync();
        }
    }
}