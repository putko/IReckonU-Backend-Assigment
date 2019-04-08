using System.Threading.Tasks;

namespace IReckonUAssigment.Business
{
    public interface IImportService
    {
        /// <summary>
        /// Imports the data from CSV.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        Task ImportDataFromCSV(string filePath);
    }
}