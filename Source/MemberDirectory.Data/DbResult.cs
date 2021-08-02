using EverlyHealth.Core.Common;
using System;
using System.Collections.Generic;

namespace MemberDirectory.Data
{
    public class DbResult : GenericResult
    {
        public DbResultReason DbResultReason { get; internal set; }
    }

    public enum DbResultReason
    {
        None,
        DuplicateRecord
    }
}
