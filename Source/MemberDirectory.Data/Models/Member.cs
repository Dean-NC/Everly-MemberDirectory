using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Models
{
    public class Member
    {
        public int Id { get; set; }

        public string MemberName { get; set; }

        public string WebsiteUrl { get; set; }

        public string WebsiteShortUrl { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
