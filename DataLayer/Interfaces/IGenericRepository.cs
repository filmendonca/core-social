using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interfaces
{
    public interface IGenericRepository<T> where T : class, IBaseEntity, new()
    {
        //Get a single entity
        Task<T> GetAsync(
            Expression<Func<T, bool>> expression,
            List<string> includes = null
        );

        //Get all entities
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
        );

        //Task<T> GetByIdAsync(int id);

        //Add entity
        Task AddAsync(T entity);

        //Edit entity
        Task UpdateAsync(T entity);

        Task UpdateAsync(T entity, params Expression<Func<T, object>>[] updatedProperties);

        //Delete entity
        Task DeleteAsync(int id);

        Task SoftDeleteAsync(int id);

        Task SoftDeleteAsync(string id);
    }
}
