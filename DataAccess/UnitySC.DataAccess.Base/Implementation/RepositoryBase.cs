using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace UnitySC.DataAccess.Base.Implementation
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {

        public DbContext Context { get; private set; }

        public RepositoryBase(DbContext context)
        {
            Context = context;
        }

        private List<Expression<Func<TEntity, object>>> _defaultInclude = new List<Expression<Func<TEntity, object>>>();

        public IRepositoryBase<TEntity> AddDefaultInclude(Expression<Func<TEntity, object>> include)
        {

            if (include != null)
                _defaultInclude.Add(include);

            return this;
        }

        /// <summary>
        /// Renvois Context.Set<TEntity> avec les include par default. Les entités retournées ne sont pas suivit par le Tracker d'EF6
        /// </summary>
        /// <returns></returns>
        private IQueryable<TEntity> GetQueryable(bool tracker = false)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();

            foreach (var includeProperty in _defaultInclude)
            {
                query = query.Include(includeProperty);
            }

            if (tracker)
                return query;
            else
                return query.AsNoTracking();
        }



        public TEntity FindFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQueryable().FirstOrDefault(predicate);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQueryable().Where(predicate);
        }

        public TEntity FindById(int id)
        {
            var found = Context.Set<TEntity>().Find(id);
            if (found != null)
                Context.Entry(found).State = EntityState.Detached;

            return found;
        }

        public TEntity FindByLongId(long id)
        {
            var found = Context.Set<TEntity>().Find(id);
            if (found != null)
                Context.Entry(found).State = EntityState.Detached;

            return found;
        }


        public int Count()
        {
            return GetQueryable().Count();
        }

        public int CountBy(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQueryable().Count(predicate);
        }

        /// <summary xml:lang="fr">
        /// Create a query with included properties
        /// </summary>
        /// <param xml:lang="fr" name="includeProperties">included properties</param>
        /// <returns xml:lang="fr"></returns>
        public IQueryable<TEntity> CreateQuery(bool tracker = false, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetQueryable(tracker);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public TEntity Add(TEntity entity)
        {
            return Context.Set<TEntity>().Add(entity);
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            return Context.Set<TEntity>().AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void RemoveById(int id)
        {
            var entity = FindById(id);
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public void Remove(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Context.Set<TEntity>().Attach(entity);
            }

            Context.Set<TEntity>().RemoveRange(entities);
        }
    }
}
