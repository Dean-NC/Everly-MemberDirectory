using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Models
{
    public class MemberProfile : Data.Models.Member
    {
        public IEnumerable<string> WebsiteHeadings { get; set; }

        public IEnumerable<Friend> Friends { get; set; }
    }
}
