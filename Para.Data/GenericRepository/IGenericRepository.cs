using System.Linq;
using System.Linq.Expressions;

namespace Para.Data.GenericRepository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task Save();
    Task<TEntity?> GetById(long Id);
    Task Insert(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
    Task Delete(long Id);
    Task<List<TEntity>> GetAll(params Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> Include(params Expression<Func<TEntity, object>>[] includes);
    Task<List<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);
}