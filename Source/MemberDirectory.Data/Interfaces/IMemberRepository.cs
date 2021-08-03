﻿using EverlyHealth.Core.Common;
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

        Task<Member> Get(int id);
    }
}
