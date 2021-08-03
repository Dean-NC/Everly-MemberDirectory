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
    public class MemberRepository : RepositoryBase, Interfaces.IMemberRepository
    {
        public MemberRepository(DbConfig dbConfig)
            : base(dbConfig)
        { }

        /// <summary>
        /// Gets a directory listing of members.
        /// </summary>
        public async Task<IEnumerable<DirectoryListMember>> List()
        {
            string sql = @"
                Select
	                Id, MemberName, DateCreated,
	                Case
		                When Member.WebsiteShortUrl Is Null Then Member.WebsiteUrl Else Member.WebsiteShortUrl
	                End As WebsiteUrl,
	                (
		                Select Count(1)
		                From Friendship
		                Where MemberId = Member.Id
	                ) As FriendCount
                From
	                Member
                Order By
                    MemberName
            ";

            using IDbConnection connection = await GetDbConnectionAsync();
            return await connection.QueryAsync<DirectoryListMember>(sql);
        }

        /// <summary>
        /// Adds a new member.
        /// </summary>
        /// <typeparam name="T">Any type that derives from DbResult.</typeparam>
        /// <param name="member">A data model with member information.</param>
        /// <returns>A DbResult or child class of DbResult.</returns>
        public async Task<T> Add<T>(Member member) where T : DbResult, new()
        {
            T result = new();

            string sql =
                "Insert Into Member (MemberName, WebsiteUrl, WebsiteShortUrl) " +
                "Values (@MemberName, @WebsiteUrl, @WebsiteShortUrl); " +
                "Select SCOPE_IDENTITY()";

            using IDbConnection connection = await GetDbConnectionAsync();

            try
            {
                member.Id = await connection.ExecuteScalarAsync<int>(sql, member);
                result.RecordId = member.Id;
            }
            catch (SqlException ex) when (IsDuplicateError(ex.Number))
            {
                result.ResultType = GenericEnums.ResultType.InvalidData;
                result.DbResultReason = DbResultReason.DuplicateRecord;
            }

            return result;
        }

        /// <summary>
        /// Adds website headings for the given member.
        /// </summary>
        /// <param name="memberId">The Id of the member to add the headings to.</param>
        /// <param name="headings">A list of strings for the headings text.</param>
        public async Task AddWebsiteHeadings(int memberId, IEnumerable<string> headings)
        {
            string sql =
               @"Insert Into WebsiteHeading (MemberId, HeadingText)
	                Select
		                @memberId, Item
	                From
		                @headings
	                ";

            var sqlRecords = MakeSqlDataRecords(headings);

            using IDbConnection connection = await GetDbConnectionAsync();
            await connection.ExecuteAsync(sql,
                new
                {
                    memberId,
                    headings = sqlRecords.AsTableValuedParameter("StringValuesType")
                }
            );
        }

        /// <summary>
        /// Gets website headings for the given member.
        /// </summary>
        /// <param name="memberId">The Id of the member.</param>
        public async Task<IEnumerable<string>> GetWebsiteHeadings(int memberId)
        {
            string sql = @"
                Select HeadingText
                From WebsiteHeading
                Where MemberId = @id
            ";

            DynamicParameters theParams = new();
            theParams.Add("@id", memberId, DbType.Int32);

            using IDbConnection connection = await GetDbConnectionAsync();
            return await connection.QueryAsync<string>(sql, theParams);
        }

        /// <summary>
        /// Gets a member by Id.
        /// </summary>
        /// <param name="id">The Id of the member to get.</param>
        public async Task<T> Get<T>(int id) where T : Member, new()
        {
            string sql = @"
                Select
                    Id, MemberName, WebsiteUrl, WebsiteShortUrl, DateCreated
                From
                    Member
                Where
                    Id = @id
            ";

            DynamicParameters theParams = new();
            theParams.Add("@id", id, DbType.Int32);

            using IDbConnection connection = await GetDbConnectionAsync();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, theParams);
        }

        /// <summary>
        /// Gets friends (as member records) for the given member.
        /// </summary>
        /// <param name="id">The Id of the member.</param>
        public async Task<IEnumerable<Member>> GetFriends(int memberId)
        {
            string sql = @"
                Select
	                Member.Id,
                    Member.MemberName,
	                Case
		                When Member.WebsiteShortUrl Is Null Then Member.WebsiteUrl
		                Else Member.WebsiteShortUrl
	                End As WebsiteUrl,
	                Friendship.DateCreated
                From
	                Friendship
                Inner Join
	                Member On Friendship.FriendMemberId = Member.Id
                Where
	                Friendship.MemberId = @memberId
                ";

            DynamicParameters theParams = new();
            theParams.Add("@memberId", memberId, DbType.Int32);

            using IDbConnection connection = await GetDbConnectionAsync();
            return await connection.QueryAsync<Member>(sql, theParams);
        }
    }
}
