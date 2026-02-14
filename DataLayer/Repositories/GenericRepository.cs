using Ardalis.GuardClauses;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity, new()
    {
        #region Dependency Injection

        private readonly AppDbContext _context;
        protected readonly DbSet<T> _db;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        #endregion

        public async Task AddAsync(T entity) => await _db.AddAsync(entity);

        //Hard delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            Guard.Against.Null(entity, nameof(entity));
            _db.Remove(entity);
        }

        //Soft delete - int value
        public async Task SoftDeleteAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            Guard.Against.Null(entity, nameof(entity));
            entity.DeletedAt = DateTime.UtcNow;
        }

        //Soft delete - string value
        public async Task SoftDeleteAsync(string id)
        {
            var entity = await _db.FindAsync(id);
            Guard.Against.Null(entity, nameof(entity));
            entity.DeletedAt = DateTime.UtcNow;
        }

        //Full update
        public async Task UpdateAsync(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        //Partial update
        public async Task UpdateAsync(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            //Check if entity is being tracked
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                // Attach the entity to the context
                _db.Attach(entity);
            }

            // Loop through all the properties that need to be updated
            foreach (var property in updatedProperties)
            {
                // Get the property name from the expression
                var propertyName = ((MemberExpression)property.Body).Member.Name;

                // Mark the property as modified
                _context.Entry(entity).Property(propertyName).IsModified = true;
            }
        }

        ////Return one entity by id
        //public async Task<T> GetByIdAsync(int id) => await _db.FirstOrDefaultAsync(n => n.Id == id);

        //Return one entity
        public async Task<T> GetAsync(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync(expression);
        }

        //Return several entities
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }
    }
}
