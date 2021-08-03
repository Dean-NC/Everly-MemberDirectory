using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Interfaces
{
    public interface IFriendshipRepository
    {


        Task<IEnumerable<Models.ExpertSearchResult>> MutualFriendSearch(int memberId, string websiteHeadingSearch);
    }
}
