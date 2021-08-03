using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Models
{
    public class ExpertSearchResult
    {
        public string MutualFriend { get; set; }

        public int PotentialFriendId { get; set; }

        public string PotentialFriend { get; set; }

        public string HeadingText { get; set; }
    }
}
