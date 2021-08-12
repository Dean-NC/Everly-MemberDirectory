using EverlyHealth.Core.Common;
using MemberDirectory.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<DirectoryListMember>> List();

        Task<T> Add<T>(Member member) where T : DbResult, new();

        Task AddWebsiteHeadings(int memberId, ICollection<string> headings);

        Task<IEnumerable<string>> GetWebsiteHeadings(int memberId);

        Task<IEnumerable<Member>> GetFriends(int memberId);

        Task<T> Get<T>(int id) where T : Member, new();
    }
}
