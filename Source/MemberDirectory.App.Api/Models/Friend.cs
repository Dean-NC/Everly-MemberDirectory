using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Models
{
    public class Friend
    {
        public int MemberId { get; set; }

        public string Name { get; set; }

        public string WebsiteUrl { get; set; }

        public DateTime StartDate { get; set; }
    }
}
