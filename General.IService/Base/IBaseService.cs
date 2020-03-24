using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace General.IService
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        ///// <summary>
        ///// 上下文
        ///// </summary>
        //DbContext DbContext { get; }

        ///// <summary>
        ///// 实体对象
        ///// </summary>
        //DbSet<TEntity> Entities { get; }

        ///// <summary>
        ///// 实体对象表
        ///// </summary>
        //IQueryable<TEntity> Table { get; }

        ///// <summary>
        ///// 通过主键ID获取数据
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //TEntity GetById(object id);

        //void Insert(TEntity entity, bool isSave = true);

        //void Update(TEntity entity, bool isSave = true);

        //void Delete(TEntity entity, bool isSave = true);

        Task<TEntity> GetById(object id);

        Task<int> CreateAsync(TEntity entity);

        Task<int> EditAsync(TEntity entity);

        Task<int> RemoveAsync(TEntity entity);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
    }
}
