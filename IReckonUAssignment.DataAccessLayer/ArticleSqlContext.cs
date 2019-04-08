using Microsoft.EntityFrameworkCore;

namespace IReckonUAssignment.DataAccessLayer
{
    public class ArticleSqlContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleSqlContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ArticleSqlContext(DbContextOptions<ArticleSqlContext> options) : base(options)
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
            //    optionsBuilder.UseSqlServer(
            //        @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=IReckonUDb;Data Source=AMSLTDEVATOPAL");
            //}
        }
    }
}