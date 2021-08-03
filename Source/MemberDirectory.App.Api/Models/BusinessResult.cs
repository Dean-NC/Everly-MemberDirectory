using System;
using System.Collections.Generic;

namespace MemberDirectory.App.Api.Models
{
    public class BusinessResult<T> : Data.DbResult where T : new()
    {
        public T Entity { get; set; }

        public BusinessResult()
        { }

        public BusinessResult(Data.DbResult dbResult)
        {
            if (dbResult != null)
            {
                ResultType = dbResult.ResultType;
            }
        }
    }
}
