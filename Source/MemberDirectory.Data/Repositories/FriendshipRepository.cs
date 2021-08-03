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

        /// <summary>
        /// Adds friends to a given member.
        /// </summary>
        /// <param name="memberId">The Id of the member to add friends to.</param>
        /// <param name="friendIds">A list of member Ids to add as friends to the given member.</param>
        /// <returns></returns>
        public async Task AddFriendsToMember(int memberId, IEnumerable<int> friendIds)
        {
            // Friendships are stored in a 2-way/2-record pattern, for example,
            // adding friend Id 7 to member Id 1 will result in 2 records:
            //  (member id) 1           (friend member id) 7
            //  (friend member id) 7    (member id) 1
            //
            // This is done for performance in finding friends for a member, and searching for experts.

            // The UpdLock/HoldLock table hints combo will ensure that no duplicates are entered,
            // by putting a lock on the "potential new record" that will be inserted.

            string sql =
               @"Begin Transaction

                Insert Into Friendship (MemberId, FriendMemberId)
	                Select
		                @memberId, Item
	                From
		                @friendIds
	                Where Not Exists (
		                Select 1
		                From Friendship WITH (UPDLOCK, HOLDLOCK)
		                Where MemberId = @memberId And FriendMemberId = Item
                    );

                Insert Into Friendship (MemberId, FriendMemberId)
	                Select
		                Item, @memberId
	                From
		                @friendIds
	                Where Not Exists (
		                Select 1
		                From Friendship WITH (UPDLOCK, HOLDLOCK)
		                Where MemberId = Item And FriendMemberId = @memberId
                    );

                Commit
	            ";

            var sqlRecords = MakeSqlDataRecords(friendIds);

            using IDbConnection connection = await GetDbConnectionAsync();
            await connection.ExecuteAsync(sql,
                new
                {
                    memberId,
                    friendIds = sqlRecords.AsTableValuedParameter("IntValuesType")
                }
            );
        }
    }
}
