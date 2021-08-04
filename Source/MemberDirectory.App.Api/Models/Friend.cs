using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Models
{
    /// <summary>
    /// Used as a "view model" for showing friends of a member.
    /// </summary>
    public class Friend
    {
        // MemberId is the member Id of this friend.
        public int MemberId { get; set; }

        public string Name { get; set; }

        public string WebsiteUrl { get; set; }

        public DateTime StartDate { get; set; }
    }
}
