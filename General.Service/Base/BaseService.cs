using General.IRepository;
using General.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace General.Service
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {

        public IBaseRepository<TEntity> baseRepository;//通过在子类的构造函数中注入，这里是基类，不用构造函数

        public async Task<int> CreateAsync(TEntity entity)
        {
            return await baseRepository.CreateAsync(entity);
        }

        public async Task<int> EditAsync(TEntity entity)
        {
            return await baseRepository.EditAsync(entity);
        }

        public async Task<TEntity> GetById(object id)
        {
            return await baseRepository.GetById(id);
        }

        public async Task<int> RemoveAsync(TEntity entity)
        {
            return await baseRepository.RemoveAsync(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return baseRepository.GetAll();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return baseRepository.GetAll(predicate);
        }

        //public DbContext DbContext => baseRepository.DbContext;

        //public DbSet<TEntity> Entities => baseRepository.Entities;

        //public IQueryable<TEntity> Table => baseRepository.Table;

        //public void Delete(TEntity entity, bool isSave = true)
        //{
        //    baseRepository.Delete(entity,isSave);
        //}

        //public TEntity GetById(object id)
        //{
        //    return baseRepository.GetById(id);
        //}

        //public void Insert(TEntity entity, bool isSave = true)
        //{
        //    baseRepository.Insert(entity,isSave);
        //}

        //public void Update(TEntity entity, bool isSave = true)
        //{
        //    baseRepository.Update(entity,isSave);
        //}
    }
}
