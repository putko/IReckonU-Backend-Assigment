using System.Threading.Tasks;

namespace IReckonUAssignment.DataAccessLayer
{
    /// <summary>
    /// Unit of work interface. 
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the article file repository.
        /// </summary>
        /// <value>
        /// The article file repository.
        /// </value>
        GenericRepository<ArticleEntity> ArticleFileRepository { get; }

        /// <summary>
        /// Gets the article SQL repository.
        /// </summary>
        /// <value>
        /// The article SQL repository.
        /// </value>
        GenericRepository<ArticleEntity> ArticleSqlRepository { get; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Saves the changes made on repositories.
        /// </summary>
        /// <returns></returns>
        Task Save();
    }
}