using FileContextCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace IReckonUAssignment.DataAccessLayer
{
    public class ArticleFileContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleFileContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ArticleFileContext(DbContextOptions<ArticleFileContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the articles.
        /// </summary>
        /// <value>
        /// The articles.
        /// </value>
        public DbSet<ArticleEntity> Articles { get; set; }

        /// <summary>
        /// Called when [configuring].
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
            //{
            //    optionsBuilder.UseFileContext("json");
            //}
        }
    }
}