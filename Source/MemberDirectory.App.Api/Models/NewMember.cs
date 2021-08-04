using System;
using System.ComponentModel.DataAnnotations;

namespace MemberDirectory.App.Api.Models
{
    /// <summary>
    /// The data model provided to the API that represents info needed to add a new member.
    /// </summary>
    public class NewMember
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string WebsiteUrl { get; set; }
    }
}
