using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IReckonUAssignment.DataAccessLayer
{
    /// <summary>
    /// Generic repository interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete(T entityToDelete);

        IEnumerable<T> Get(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");

        /// <summary>
        /// Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The entity corresponded to given id.</returns>
        T GetByID(object id);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(T entity);

        /// <summary>
        /// Updates the specified entity to update.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        void Update(T entityToUpdate);
    }
}