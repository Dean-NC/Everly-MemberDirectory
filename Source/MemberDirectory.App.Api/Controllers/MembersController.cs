using EverlyHealth.Core.Common;
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
        private readonly Services.MemberService _memberService;

        public MembersController(Services.MemberService memberService)
        {
            _memberService = memberService;
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
        [HttpGet("{id:int}", Name = "GetById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Data.Models.Member> GetById(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Creates a member.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Data.Models.Member>> Create(Models.NewMember data)
        {
            var result = await _memberService.Add(data);

            if (result.IsSuccess && result.RecordId > 0)
            {
                return CreatedAtAction(nameof(GetById), new { id = 1 }, result.Entity);
            }
            else if (result.DbResultReason == Data.DbResultReason.DuplicateRecord)
            {
                return Conflict();
            }
            else if (result.ResultType == GenericEnums.ResultType.MissingRequiredInfo)
            {
                return Problem(
                    title: "Missing required information",
                    detail: result.Message
                );
            }
            else
            {
                return Problem(
                    title: "Unknown problem",
                    detail: "The member could not be created, try again."
                );
            }
        }
    }
}
