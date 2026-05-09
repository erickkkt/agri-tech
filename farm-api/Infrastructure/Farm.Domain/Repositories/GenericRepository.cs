using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Farm.Domain.Repositories
{
    public abstract class GenericRepository<TEntity, DbContextT> : IGenericRepository<TEntity> where TEntity : class where DbContextT : DbContext
    {
        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;

        internal GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Queries all entities of type TEntity.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> QueryAll()
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Queries all entities of type TEntity with optional include expressions.
        /// </summary>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        public IQueryable<TEntity> QueryAll(IEnumerable<Expression<Func<TEntity, object>>> includeExpressions)
        {
            var qry = _context.Set<TEntity>().AsQueryable();
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    qry = qry.Include(includeExpression);
                }
            }
            return qry;
        }

        /// <summary>
        /// Queries many entities of type TEntity that match the specified condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> where)
        {
            return _context.Set<TEntity>().Where(where);
        }

        /// <summary>
        /// Queries all entities of type TEntity with optional include expressions.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        public IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> where, IEnumerable<Expression<Func<TEntity, object>>> includeExpressions = null)
        {
            var qry = _context.Set<TEntity>().Where(where);
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    qry = qry.Include(includeExpression);
                }
            }

            return qry;
        }


        /// <summary>
        /// Counts the total number of records in the context.
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountTotalRecordsAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }


        /// <summary>
        /// Counts the number of records that match the specified condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<int> CountManyRecordsAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _context.Set<TEntity>().Where(where).CountAsync();
        }


        /// <summary>
        /// Gets a single entity that matches the specified condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _context.Set<TEntity>().Where(where).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Gets a single entity that matches the specified condition, including related entities.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, IEnumerable<Expression<Func<TEntity, object>>> includeExpressions)
        {
            var qry = _context.Set<TEntity>().Where(where);
            foreach (var includeExpression in includeExpressions)
            {
                qry = qry.Include(includeExpression);
            }
            return await qry.FirstOrDefaultAsync();
        }


        /// <summary>
        /// Creates a new entity in the context and saves changes asynchronously.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public async Task<TEntity> CreateAsync(TEntity o)
        {
            var result = _context.Set<TEntity>().Add(o);
            await _context.SaveChangesAsync();
            return result.Entity;
        }


        /// <summary>
        /// Deletes an entity from the context and saves changes asynchronously.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(TEntity o)
        {
            var result = _context.Set<TEntity>().Remove(o);
            var returnValue = result.State == EntityState.Deleted;
            await _context.SaveChangesAsync();
            return returnValue;
        }

        /// <summary>
        /// Updates an existing entity in the context and saves changes asynchronously.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public async Task<TEntity> UpdateAsync(TEntity o)
        {
            var result = _context.Set<TEntity>().Update(o);
            result.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        /// <summary>
        /// Saves all changes made in the context asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the context and releases resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}
