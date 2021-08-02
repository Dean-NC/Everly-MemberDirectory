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

        public async Task<DbResult> Add(Member member)
        {
            DbResult result = new();

            string sql =
                "Insert Into Member (MemberName, WebsiteUrl, WebsiteShortUrl) " +
                "Values (@MemberName, @WebsiteUrl, @WebsiteShortUrl)";

            using IDbConnection connection = await GetDbConnectionAsync();

            try
            {
                await connection.ExecuteAsync(sql, member);
            }
            catch (SqlException ex) when (IsDuplicateError(ex.Number))
            {
                result.ResultType = GenericEnums.ResultType.InvalidData;
                result.DbResultReason = DbResultReason.DuplicateRecord;
            }

            return result;
        }

        public async Task<Member> Get(int id)
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
            return await connection.QuerySingleOrDefaultAsync<Member>(sql, theParams);
        }
    }
}
