using System.Linq.Expressions;

namespace Farm.Domain.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<int> CountTotalRecordsAsync();
        Task<int> CountManyRecordsAsync(Expression<Func<TEntity, bool>> where);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, IEnumerable<Expression<Func<TEntity, object>>> includeExpressions);

        Task<TEntity> CreateAsync(TEntity o);
        Task<bool> DeleteAsync(TEntity o);
        Task<TEntity> UpdateAsync(TEntity o);

        // Query methods
        IQueryable<TEntity> QueryAll();
        IQueryable<TEntity> QueryAll(IEnumerable<Expression<Func<TEntity, object>>> includeExpressions);
        IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> where);
        IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> where, IEnumerable<Expression<Func<TEntity, object>>> includeExpressions = null);

        Task SaveChangesAsync();
    }
}
