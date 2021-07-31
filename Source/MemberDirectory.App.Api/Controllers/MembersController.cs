using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberDirectory.App.Api.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MembersController : ControllerBase
    {


        public MembersController()
        {

        }

        /// <summary>
        /// Gets a directory list of members.
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        /// <summary>
        /// Gets a member for the given Id.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Members/1
        /// </remarks>
        /// <param name="id">The Id of the member to get.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            return Ok();
        }
    }
}
