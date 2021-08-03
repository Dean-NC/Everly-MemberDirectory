using EverlyHealth.Core.Common;
using MemberDirectory.App.Api.Interfaces;
using MemberDirectory.App.Api.Models;
using MemberDirectory.Data.Interfaces;
using MemberDirectory.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace MemberDirectory.App.Api.Services
{
    public class MemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IHtmlParser _htmlParser;
        private readonly IUrlShortener _urlShortener;
        private readonly ILogger<MemberService> _logger;

        public MemberService(
            IMemberRepository memberRepository,
            IHtmlParser htmlParser,
            IUrlShortener urlShortener,
            ILogger<MemberService> logger)
        {
            _memberRepository = memberRepository;
            _htmlParser = htmlParser;
            _urlShortener = urlShortener;
            _logger = logger;
        }

        public async Task<IEnumerable<DirectoryListMember>> List()
        {
            return await _memberRepository.List();
        }

        public async Task<BusinessResult<Member>> Add(NewMember data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Name) || string.IsNullOrWhiteSpace(data.WebsiteUrl))
            {
                return new()
                {
                    ResultType = GenericEnums.ResultType.MissingRequiredInfo,
                    Message = "Name and website are required to create a new member."
                };
            }

            try
            {
                // The values of any H1 through H3 tags in the person's website should be captured and stored.
                var headingTexts = await GetWebsiteHeadings(data.WebsiteUrl);

                // The domain data model. Website URL should be shortened if possible.
                Member member = new()
                {
                    MemberName = data.Name,
                    WebsiteUrl = data.WebsiteUrl,
                    WebsiteShortUrl = await _urlShortener.Shorten(data.WebsiteUrl)
                };

                // Start a .Net transaction that can be committed. If not committed, all database-related
                // actions will automatically be rolled back.
                using (TransactionScope trans = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Add member record to the database
                    var memberAddResult = await _memberRepository.Add<BusinessResult<Member>>(member);
                    if (!memberAddResult.IsSuccess)
                    {
                        return memberAddResult;
                    }

                    // Add website headings to the database
                    await _memberRepository.AddWebsiteHeadings(memberAddResult.RecordId, headingTexts);

                    trans.Complete();
                    memberAddResult.Entity = member;
                    return memberAddResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating member");

                return new()
                {
                    ResultType = GenericEnums.ResultType.Error,
                    Message = "There was a problem adding the member, try again."
                };
            }
        }

        /// <summary>
        /// Returns a simple member object for the given Id.
        /// </summary>
        /// <param name="id">The Id of the member</param>
        public async Task<Member> Get(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _memberRepository.Get<Member>(id);
        }

        /// <summary>
        /// Returns a member profile object, which contains things like friend list, etc. for the given member.
        /// </summary>
        /// <param name="id">The Id of the member</param>
        public async Task<MemberProfile> GetProfile(int id)
        {
            if (id <= 0)
            {
                return null;

            }

            // Get the member record from the database.
            MemberProfile result = await _memberRepository.Get<MemberProfile>(id);
            if (result?.Id <= 0) return result;

            // Get the website headings for the member.
            result.WebsiteHeadings = await _memberRepository.GetWebsiteHeadings(id);

            // Get the friends (as member models) of the member.
            var friendMemberRecords = await _memberRepository.GetFriends(id);
            
            // Map member models to friend models.
            if (friendMemberRecords != null)
            {
                result.Friends =
                    from x in friendMemberRecords
                    where x != null
                    select new Models.Friend()
                    {
                        MemberId = x.Id,
                        Name = x.MemberName,
                        WebsiteUrl = x.WebsiteUrl,
                        StartDate = x.DateCreated
                    };
            }

            return result;
        }

        private async Task<IEnumerable<string>> GetWebsiteHeadings(string url)
        {
            using var response = await SharedHttpClient.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode) return null;

            return _htmlParser.GetTextForHeadingTags(await response.Content.ReadAsStreamAsync());
        }
    }
}
