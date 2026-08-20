// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        Task<IEnumerable<T?>> GetAll();
        Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate);
        Task<T?> GetByID(int id);
        Task<T?> GetByID(long id);
        Task<T?> GetByID(string id);
        Task<T?> GetByID(Guid id);
        Task<bool> Any(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefault();
        Task Save();
        Task<T?> Create(T entity);
        Task<T?> Update(T entity);
        Task Delete(T entity);
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<T?> Single(Expression<Func<T, bool>> predicate);
        Task ExecuteScript(string sqlScript, params SqlParameter[] parameters);
        Task<IEnumerable<T?>> FindWhere(Expression<Func<T, bool>> predicate);
    }
}