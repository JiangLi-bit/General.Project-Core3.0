using General.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace General.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private readonly GeneralDbContext dbContext;

        public BaseRepository(GeneralDbContext context)
        {
            this.dbContext = context;
        }

        //public DbContext DbContext => dbContext;

        //public DbSet<TEntity> Entities => dbContext.Set<TEntity>();

        //public IQueryable<TEntity> Table => Entities;

        //public void Delete(TEntity entity, bool isSave = true)
        //{
        //    Entities.Remove(entity);
        //    if (isSave)
        //        dbContext.SaveChanges();
        //}

        //public TEntity GetById(object id)
        //{
        //    return dbContext.Set<TEntity>().Find(id);
        //}

        //public void Insert(TEntity entity, bool isSave = true)
        //{
        //    Entities.Add(entity);
        //    if (isSave)
        //        dbContext.SaveChanges();
        //}

        //public void Update(TEntity entity, bool isSave = true)
        //{
        //    Entities.Update(entity);
        //    if (isSave)
        //        dbContext.SaveChanges();
        //}
 
        public async Task<TEntity> GetById(object id)
        {
            return await dbContext.Set<TEntity>().FindAsync(id);
        }

        public IQueryable<TEntity> GetAll()
        {
            return dbContext.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }

        public async Task<int> CreateAsync(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> EditAsync(TEntity entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> RemoveAsync(TEntity entity)
        {
            dbContext.Entry(entity).State = EntityState.Deleted;
            return await dbContext.SaveChangesAsync();
        }
    }
}
