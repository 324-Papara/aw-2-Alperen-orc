using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using Para.Base.Entity;
using Para.Data.Context;

namespace Para.Data.GenericRepository;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ParaSqlDbContext dbContext;

    public GenericRepository(ParaSqlDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Save()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<TEntity> GetById(long Id)
    {
        return await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == Id);
    }

    public async Task Insert(TEntity entity)
    {
        entity.IsActive = true;
        entity.InsertDate = DateTime.UtcNow;
        entity.InsertUser = "System";
        await dbContext.Set<TEntity>().AddAsync(entity);
    }

    public async Task Update(TEntity entity)
    {
        dbContext.Set<TEntity>().Update(entity);
    }

    public async Task Delete(TEntity entity)
    {
        dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task Delete(long Id)
    {
        var entity = await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == Id);
        dbContext.Set<TEntity>().Remove(entity);
    }

    //Bu metod ile birlikte Include kullanımı sağlanmıştır. Belirli bir varlığı ilişkili tüm varlıkları ile birlikte listelemektedir.
    public async Task<List<TEntity>> GetAll(params Expression<Func<TEntity, object>>[] includes)
    {
        if (includes != null && includes.Length > 0)
        {
            return await Include(includes);
        }
        return await dbContext.Set<TEntity>().ToListAsync();
    }
    //Include metodunun dinamik olarak yazılması. Alınan parametrelerle hangi ilişkili varlıkların yüklenmesi gerektiğini belirtir. Eager loading sağlanmış olunur.
    public async Task<List<TEntity>> Include(params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    //Where metodunun dinamik olarak yazılması. Bu metod ile belirli bir koşulu sağlayan varlığın ilişkili varlıklarla beraber bir liste şeklinde dönmesini sağlar. Eager loading sağlanmış olunur.
    public async Task<List<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = dbContext.Set<TEntity>().Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
}