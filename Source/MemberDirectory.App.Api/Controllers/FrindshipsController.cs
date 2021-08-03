using MemberDirectory.App.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FriendshipsController : ControllerBase
    {
        private readonly Services.FriendshipService _friendshipService;

        public FriendshipsController(Services.FriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }

        /// <summary>
        /// Searches for potential new friends for the given member, that share a mutual friend
        /// and are "experts" (have a search phrase in their website headings).
        /// </summary>
        [HttpGet("mutualsearch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<Data.Models.ExpertSearchResult>> MutualFriendSearch(int memberId, string headingText)
        {
            return await _friendshipService.MutualFriendSearch(memberId, headingText);
        }

        /// <summary>
        /// Adds friends to a given member.
        /// </summary>
        /// <param name="memberId">The Id of the member to add friends to.</param>
        /// <param name="friendIds">A list of member Ids to add as friends to the given member.</param>
        [HttpPost]
        public async Task AddToMember(int memberId, IEnumerable<int> friendIds)
        {
            await _friendshipService.AddFriendsToMember(memberId, friendIds);
        }
    }
}
