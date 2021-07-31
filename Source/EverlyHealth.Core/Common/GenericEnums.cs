using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlyHealth.Core.Common
{
    public static class GenericEnums
    {
        public enum ResultType
        {
            Information,
            Warning,
            MissingRequiredInfo,
            InvalidData,
            Error
        }
    }
}
