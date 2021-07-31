using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlyHealth.Core.Common
{
    /// <summary>
    /// This class represents the result of some action and is usually returned by a method.
    /// It holds a generic "result type" (Information, InvalidData, Error, etc.), and a message.
    /// It's useful by itself, or could be used as a base class. For example: a ProcessFileResult class could
    /// derive from this and add a "LineCount" property.
    /// </summary>
    public class GenericResult
    {
        public GenericEnums.ResultType ResultType { get; set; }

        public string Message { get; set; }

        public bool IsSuccess
        {
            get
            {
                return ResultType == GenericEnums.ResultType.Information || ResultType == GenericEnums.ResultType.Warning;
            }
        }

        public void SetMissingRequired(string message)
        {
            ResultType = GenericEnums.ResultType.MissingRequiredInfo;
            Message = message;
        }

        public void SetInvalidData(string message)
        {
            ResultType = GenericEnums.ResultType.InvalidData;
            Message = message;
        }

        public void SetError(string message)
        {
            ResultType = GenericEnums.ResultType.Error;
            Message = message;
        }
    }
}
