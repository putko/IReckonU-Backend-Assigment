using CsvHelper;
using System.Collections.Generic;
using System.IO;

namespace IReckonUAssigment.Business
{
    /// <summary>
    /// CVSParser library helper class. takes the responsibility of parsing file from Service class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Object" />
    internal class CVSParser<T> where T : class
    {
        /// <summary>
        /// Parses the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        internal IEnumerable<T> Parse(string filePath)
        {
            List<T> records = new List<T>();
            using (TextReader reader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    T record = csv.GetRecord<T>();
                    records.Add(record);
                }
            }

            return records;
        }
    }
}