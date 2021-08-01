using EverlyHealth.Core.Common;
using MemberDirectory.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Repositories
{
    public class MemberRepository : RepositoryBase, Interfaces.IMemberRepository
    {
        private readonly DbConfig _dbConfig;

        public MemberRepository(DbConfig dbConfig)
        {
            _dbConfig = dbConfig;
        }

        public Task<IEnumerable<DirectoryListMember>> List()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResult> Add(Member member)
        {
            throw new NotImplementedException();
        }

        public Task<Member> Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
