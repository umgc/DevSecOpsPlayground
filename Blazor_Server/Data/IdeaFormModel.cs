using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace Blazor_Server.Data
{
    public class IdeaFormModel
    {
        [Required]
        [StringLength(255, ErrorMessage = "First Name is too long")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Last Name is too long")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(50, ErrorMessage ="Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
        [DisplayName("Project Title")]
        public string ProjectTilte { get; set; }

        [Required]
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; }

        public Stream Attachement { get; set; }

        [DisplayName("Your Website")]
        public string Url { get; set; }

        [DisplayName("Are you the sponsor")]
        public bool IsSponsor { get; set; } = true;
    }
}
