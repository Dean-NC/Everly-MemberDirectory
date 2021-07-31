using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Models
{
    public class DirectoryListMember : Member
    {
        public int FriendCount { get; set; }
    }
}
