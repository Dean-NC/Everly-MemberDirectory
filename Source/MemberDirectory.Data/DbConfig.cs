﻿using System;

namespace MemberDirectory.Data
{
    public class DbConfig
    {
        private readonly string _connectionString;

        public DbConfig(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
