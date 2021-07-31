using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlyHealth.Core.Common
{
    public static class GenericEnums
    {
        /// <summary>
        /// A simple enum representing common types of results.
        /// </summary>
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
