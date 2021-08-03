using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Repositories
{
    /// <summary>
    /// Base class for all data repositories. Provides functionality common to all repositories.
    /// </summary>
    public class RepositoryBase
    {
        private readonly DbConfig _dbConfig;

        public RepositoryBase(DbConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        /// <summary>
        /// Returns an open connection to the database.
        /// </summary>
        protected async Task<IDbConnection> GetDbConnectionAsync()
        {
            // This application will use SQL Server for the database, but we could use another.

            SqlConnection connection = new(_dbConfig.ConnectionString);
            await connection.OpenAsync();

            return connection;
        }

        /// <summary>
        /// To pass a collection of primitive values to a SQL query or procedure, you can use a .Net DataTable or a
        /// collection of SqlDataRecord objects. SqlDataRecord is slightly faster. A SqlDataRecord object describes
        /// the value's data type and holds the value.
        /// This method converts a collection of integers into a collection of SqlDataRecord objects.
        /// </summary>
        /// <param name="items">A collection of integers to convert to SqlDataRecord objects</param>
        /// <returns>A collection of SqlDataRecord objects that can be passed to SQL Server</returns>
        protected static IEnumerable<SqlDataRecord> MakeSqlDataRecords(IEnumerable<int> items)
        {
            SqlMetaData[] metaData = new[]
            {
                new SqlMetaData("Item", SqlDbType.Int)
            };
            SqlDataRecord record = new(metaData);

            foreach (int id in items)
            {
                record.SetInt32(0, id);
                yield return record;
            }
        }

        protected static IEnumerable<SqlDataRecord> MakeSqlDataRecords(IEnumerable<string> items)
        {
            SqlMetaData[] metaData = new[]
            {
                new SqlMetaData("Item", SqlDbType.NVarChar, 120)
            };
            SqlDataRecord record = new(metaData);

            foreach (string item in items)
            {
                record.SetString(0, item);
                yield return record;
            }
        }

        protected static bool IsDuplicateError(int dbErrorNumber)
        {
            // Determines if the given error number is because of duplicate key or unique constraint violation.
            // The numbers are documented here (old article but applys to latest server versions):
            // https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/cc645728(v=sql.105)

            return dbErrorNumber == 2601 || dbErrorNumber == 2627;
        }
    }
}
