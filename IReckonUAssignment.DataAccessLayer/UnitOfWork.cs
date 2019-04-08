using System;
using System.Threading.Tasks;
using System.Transactions;

namespace IReckonUAssignment.DataAccessLayer
{
    /// <summary>
    /// Unit of work pattern implementation.
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="IReckonUAssignment.DataAccessLayer.IUnitOfWork" />
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        /// <summary>
        /// The article file repository
        /// </summary>
        private GenericRepository<ArticleEntity> articleFileRepository;

        /// <summary>
        /// The article SQL repository
        /// </summary>
        private GenericRepository<ArticleEntity> articleSqlRepository;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The file context
        /// </summary>
        private readonly ArticleFileContext fileContext;

        /// <summary>
        /// The SQL context
        /// </summary>
        private readonly ArticleSqlContext sqlContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="fileContext">The file context.</param>
        /// <param name="sqlContext">The SQL context.</param>
        public UnitOfWork(ArticleFileContext fileContext, ArticleSqlContext sqlContext)
        {
            this.fileContext = fileContext;
            this.sqlContext = sqlContext;
        }

        /// <summary>
        /// Gets the article file repository.
        /// </summary>
        /// <value>
        /// The article file repository.
        /// </value>
        public GenericRepository<ArticleEntity> ArticleFileRepository
        {
            get
            {
                if (this.articleFileRepository == null)
                {
                    this.articleFileRepository = new GenericRepository<ArticleEntity>(this.fileContext);
                }

                return this.articleFileRepository;
            }
        }

        /// <summary>
        /// Gets the article SQL repository.
        /// </summary>
        /// <value>
        /// The article SQL repository.
        /// </value>
        public GenericRepository<ArticleEntity> ArticleSqlRepository
        {
            get
            {
                if (this.articleSqlRepository == null)
                {
                    this.articleSqlRepository = new GenericRepository<ArticleEntity>(this.sqlContext);
                }

                return this.articleSqlRepository;
            }
        }

        /// <summary>
        /// Saves changes made on repositories.
        /// </summary>
        /// <returns></returns>
        public async Task Save()
        {
            // TransactionScopeAsyncFlowOption is enabled because of the async/await calls.
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sqlTask = this.sqlContext.SaveChangesAsync();
                var fileTask = this.fileContext.SaveChangesAsync();
                await Task.WhenAll(sqlTask, fileTask);
                scope.Complete();
            }

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.sqlContext.Dispose();
                    this.fileContext.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}