using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IReckonUAssignment.DataAccessLayer
{
    /// <summary>
    /// Generic Repository pattern implementation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Object" />
    /// <seealso cref="IReckonUAssignment.DataAccessLayer.IRepository{TEntity}" />
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// The context
        /// </summary>
        internal DbContext context;

        /// <summary>
        /// The database set
        /// </summary>
        internal DbSet<TEntity> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets the entities specified by the given filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The sort expression.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>Entities matching the filter</returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = this.dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        /// <summary>
        /// Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Entity that corresponds to the given id</returns>
        public virtual TEntity GetByID(object id)
        {
            return this.dbSet.Find(id);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Insert(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        /// <summary>
        /// Deletes the  entity specified by the identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = this.dbSet.Find(id);
            this.Delete(entityToDelete);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (this.context.Entry(entityToDelete).State == EntityState.Detached)
            {
                this.dbSet.Attach(entityToDelete);
            }

            this.dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            this.dbSet.Attach(entityToUpdate);
            this.context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}