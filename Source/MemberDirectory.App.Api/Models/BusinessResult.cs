using System;
using System.Collections.Generic;

namespace MemberDirectory.App.Api.Models
{
    /// <summary>
    /// This is a common result class to return from business services when both a DbResult and
    /// a model object are desired to be returned. It inherits from DbResult, which inherits from GenericResult.
    /// </summary>
    /// <typeparam name="T">Any class type.</typeparam>
    public class BusinessResult<T> : Data.DbResult where T : new()
    {
        public T Entity { get; set; }

        public BusinessResult()
        { }
    }
}
