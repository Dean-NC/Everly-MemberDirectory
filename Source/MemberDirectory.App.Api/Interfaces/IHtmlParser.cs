using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Interfaces
{
    public interface IHtmlParser
    {
        ICollection<string> GetTextForHeadingTags(System.IO.Stream htmlStream);
    }
}
