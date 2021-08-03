using Dapper;
using EverlyHealth.Core.Common;
using MemberDirectory.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MemberDirectory.Data.Repositories
{
    /// <summary>
    /// Provides and manages data for directory members.
    /// </summary>
    public class FriendshipRepository : RepositoryBase, Interfaces.IFriendshipRepository
    {
        public FriendshipRepository(DbConfig dbConfig)
            : base(dbConfig)
        { }

        /// <summary>
        /// Searches for potential new friends for the given member, that share a mutual friend with the member,
        /// and have a search phrase in one of their website headings.
        /// </summary>
        /// <param name="id">The Id of the member.</param>
        /// <param name="websiteHeadingSearch">A word or phrase to search in the website headings of the potential friend.</param>
        public async Task<IEnumerable<ExpertSearchResult>> MutualFriendSearch(int memberId, string websiteHeadingSearch)
        {
            DynamicParameters theParams = new();
            theParams.Add("@MemberId", memberId, DbType.Int32);
            theParams.Add("@SearchPhrase", websiteHeadingSearch, DbType.String);

            using IDbConnection connection = await GetDbConnectionAsync();
            return await connection.QueryAsync<ExpertSearchResult>("Friendship_MutualSearch", theParams, commandType: CommandType.StoredProcedure);
        }
    }
}
