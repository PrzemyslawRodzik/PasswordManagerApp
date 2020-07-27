using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PasswordManagerApp.Repositories
{
    public interface IRepositoryBase
    {
        //finding objects

        TEntity GetById<TEntity>(int id) where TEntity : class;
        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;
        IEnumerable<TEntity> FindByCondition<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        TEntity Find<TEntity>(int id) where TEntity : class;

        TEntity SingleOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        // adding objects
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;

        // removing objects
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        
        

    }
}
