using EverlyHealth.Core.Common;
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
    }
}
