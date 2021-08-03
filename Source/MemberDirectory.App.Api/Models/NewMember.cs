using System;
using System.ComponentModel.DataAnnotations;

namespace MemberDirectory.App.Api.Models
{
    public class NewMember
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string WebsiteUrl { get; set; }
    }
}
