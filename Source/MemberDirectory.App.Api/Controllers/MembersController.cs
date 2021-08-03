using EverlyHealth.Core.Common;
using MemberDirectory.App.Api.Models;
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
        public async Task<IEnumerable<Data.Models.DirectoryListMember>> Get()
        {
            return await _memberService.List();
        }

        /// <summary>
        /// Gets a member profile for the given Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MemberProfile>> GetById(int id)
        {
            var result = await _memberService.GetProfile(id);

            return result?.Id > 0 ? result : NotFound();
        }

        /// <summary>
        /// Creates a member.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Data.Models.Member>> Create(NewMember data)
        {
            BusinessResult<Data.Models.Member> result = await _memberService.Add(data);

            if (result.IsSuccess && result.Entity?.Id > 0)
            {
                // For success, return:
                //    201 "Created", a "Location" header with the URL to access the new record, and
                //    the new record model data in the response body.
                return CreatedAtAction(nameof(GetById), new { id = result.Entity.Id }, result.Entity);
            }
            else if (result.DbResultReason == Data.DbResultReason.DuplicateRecord)
            {
                // For duplicate records, return 409 "Conflict"
                return Conflict();
            }
            else if (result.ResultType == GenericEnums.ResultType.MissingRequiredInfo)
            {
                // If any required input data is missing, return 400 "Bad Request" and a detail message.
                // Note: .Net can send "Problem Details"
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
