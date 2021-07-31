using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MemberDirectory.Data.Repositories
{
    public class RepositoryBase
    {

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
    }
}
