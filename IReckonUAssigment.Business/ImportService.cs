using AutoMapper;
using IReckonUAssignment.DataAccessLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IReckonUAssigment.Business
{
    public class ImportService : IImportService
    {
        /// <summary>
        /// The mapper
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The unit of work
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        public ImportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Imports the data from CSV.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public async Task ImportDataFromCSV(string filePath)
        {
            var parser = new CVSParser<ArticleDTO>();
            var articles = parser.Parse(filePath);
            var articleEntities = this.mapper.Map<List<ArticleEntity>>(articles);
            foreach (var item in articleEntities)
            {
                this.unitOfWork.ArticleSqlRepository.Insert(item);
                this.unitOfWork.ArticleFileRepository.Insert(item);
            }


            await this.unitOfWork.Save();
        }
    }
}