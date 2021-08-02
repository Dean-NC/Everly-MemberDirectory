using System;

namespace MemberDirectory.Data
{
    public class DbConfig
    {
        public readonly string ConnectionString;

        public DbConfig(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(nameof(connectionString) + " must have a value.");
            }

            ConnectionString = connectionString;
        }
    }
}
