using EverlyHealth.Core.Common;
using MemberDirectory.App.Api.Models;
using MemberDirectory.Data.Interfaces;
using MemberDirectory.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Services
{
    public class MemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
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

            Member member = new()
            {
                MemberName = data.Name,
                WebsiteUrl = data.WebsiteUrl
            };

            var result = await _memberRepository.Add<BusinessResult<Member>>(member);
            result.Entity = member;

            return result;
        }
    }
}
