using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UnitySC.DataAccess.Base
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        TEntity Add(TEntity entity);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        void Remove(TEntity entity);
        void RemoveById(int id);
        void RemoveRange(IEnumerable<TEntity> entities);

        IRepositoryBase<TEntity> AddDefaultInclude(Expression<Func<TEntity, object>> include);

        IQueryable<TEntity> CreateQuery(bool tracker = false, params Expression<Func<TEntity, object>>[] includeProperties);

        int Count();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity FindById(int id);
        TEntity FindByLongId(long id);
    }
}
