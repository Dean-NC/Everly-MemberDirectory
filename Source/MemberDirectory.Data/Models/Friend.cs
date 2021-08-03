using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Models
{
    public class Friend
    {
        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public string WebsiteUrl { get; set; }

        public DateTime StartDate { get; set; }
    }
}
