using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace UnitySC.DataAccess.Base.Implementation
{
    public class UnitOfWorkBase : IUnitOfWorkBase
    {
#pragma warning disable IDE1006 // Naming Styles
        protected readonly DbContext _context;
#pragma warning restore IDE1006 // Naming Styles

        public UnitOfWorkBase(DbContext context)
        {
            _context = context;

            _context.Configuration.LazyLoadingEnabled = false;
        }

        private readonly SortedDictionary<string, object> _repositoriesInstances = new SortedDictionary<string, object>();

        protected IRepositoryBase<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var t = typeof(TEntity);

            RepositoryBase<TEntity> retour;
            if (_repositoriesInstances.Keys.Contains(t.FullName))
            {
                retour = _repositoriesInstances[t.FullName] as RepositoryBase<TEntity>;

            }
            else
            {
                retour = new RepositoryBase<TEntity>(_context);
                _repositoriesInstances.Add(t.FullName, retour);
            }

            return retour;
        }


        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            finally
            {
                DetachAllEntities();
            }
        }

        public void DetachAllEntities()
        {
            foreach (var entity in _context.ChangeTracker.Entries())
            {
                _context.Entry(entity.Entity).State = EntityState.Detached;
            }
        }

        public string GetConnectionString()
        {
            return _context.Database.Connection.ConnectionString;
        }

        public DbConnection GetDatabaseConnection()
        {
            return _context.Database.Connection;
        }
    }
}
