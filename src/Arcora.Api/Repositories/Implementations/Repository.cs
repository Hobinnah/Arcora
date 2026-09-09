// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Arcora.Api.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Name of the audit column used to surface the most recent record first.
        /// </summary>
        private const string CapturedDateProperty = "CapturedDate";

        /// <summary>
        /// Caches, per entity type, the property metadata used to order by <see cref="CapturedDateProperty"/>.
        /// A value of <c>null</c> means the entity has no CapturedDate column, so no ordering is applied.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _capturedDateProperties = new();

        protected ArcoraDbContext _context;
        public Repository(ArcoraDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Applies a default ordering that puts the most recent record on top using the
        /// <see cref="CapturedDateProperty"/> column, when the entity exposes it. Entities without a
        /// CapturedDate column are returned unordered so behaviour is preserved for them.
        /// </summary>
        public static IQueryable<T> ApplyDefaultOrder(IQueryable<T> query)
        {
            var property = _capturedDateProperties.GetOrAdd(typeof(T), static entityType =>
                entityType.GetProperty(CapturedDateProperty, BindingFlags.Public | BindingFlags.Instance));

            if (property == null)
            {
                return query;
            }

            // Build "x => x.CapturedDate" with the property's real type so EF Core translates it to SQL.
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var keySelector = Expression.Lambda(propertyAccess, parameter);

            var orderByDescending = typeof(Queryable).GetMethods()
                .First(m => m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType);

            return (IQueryable<T>)orderByDescending.Invoke(null, new object[] { query, keySelector })!;
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
            return await ApplyDefaultOrder(_context.Set<T>().AsNoTracking()).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate)
        {
            return await ApplyDefaultOrder(_context.Set<T>().AsNoTracking().Where(predicate)).ToListAsync();
        }

        public async Task<IEnumerable<T?>> FindWhere(Expression<Func<T, bool>> predicate)
        {
            return await ApplyDefaultOrder(_context.Set<T>().AsNoTracking().Where(predicate)).ToListAsync();
        }

        public async Task<T?> Single(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T?>> GetAll()
        {
            return await ApplyDefaultOrder(_context.Set<T>().AsNoTracking()).ToListAsync();
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