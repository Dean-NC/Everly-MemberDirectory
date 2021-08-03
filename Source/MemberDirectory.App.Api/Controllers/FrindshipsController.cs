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
    }
}
