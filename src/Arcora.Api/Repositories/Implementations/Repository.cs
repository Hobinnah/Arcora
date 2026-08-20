// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Arcora.Api.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ArcoraDbContext _context;
        public Repository(ArcoraDbContext context)
        {
            this._context = context;
        }

        public Task Save() => _context.SaveChangesAsync();
        public async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).AnyAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).CountAsync();
        }

        public async Task<T?> Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T?> Update(T entity)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(entity).State = EntityState.Modified;
            await Task.CompletedTask;
            return entity;
        }

        public async Task Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task ExecuteScript(string sqlScript, params SqlParameter[] parameters)
        {
            await _context.Database.ExecuteSqlRawAsync(sqlScript, parameters);
        }

        public async Task<T?> FirstOrDefault()
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T?>> FindWhere(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> Single(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T?>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByID(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByID(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByID(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByID(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}