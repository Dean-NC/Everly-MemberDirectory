using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Interfaces
{
    public interface IFriendshipRepository
    {
        Task AddFriendsToMember(int memberId, IEnumerable<int> friendIds);

        Task<IEnumerable<Models.ExpertSearchResult>> MutualFriendSearch(int memberId, string websiteHeadingSearch);
    }
}
