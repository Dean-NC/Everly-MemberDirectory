using MemberDirectory.Data.Interfaces;
using MemberDirectory.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Services
{
    public class FriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<FriendshipService> _logger;

        public FriendshipService(
            IFriendshipRepository friendshipRepository,
            ILogger<FriendshipService> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        /// <summary>
        /// Searches for potential new friends for the given member, that share a mutual friend
        /// and are "experts" (have a search phrase in their website headings).
        /// </summary>
        /// <param name="memberId">The id of the member to find friends for.</param>
        /// <param name="headingTextSearch">A word or phrase to search in the new friend's website headings.</param>
        public async Task<IEnumerable<ExpertSearchResult>> MutualFriendSearch(int memberId, string headingTextSearch)
        {
            if (memberId <= 0 || string.IsNullOrWhiteSpace(headingTextSearch))
            {
                return null;
            }

            return await _friendshipRepository.MutualFriendSearch(memberId, headingTextSearch);
        }

        /// <summary>
        /// Adds friends to a given member.
        /// </summary>
        /// <param name="memberId">The Id of the member to add friends to.</param>
        /// <param name="friendIds">A list of member Ids to add as friends to the given member.</param>
        public async Task AddFriendsToMember(int memberId, IEnumerable<int> friendIds)
        {
            await _friendshipRepository.AddFriendsToMember(memberId, friendIds);
        }
    }
}
