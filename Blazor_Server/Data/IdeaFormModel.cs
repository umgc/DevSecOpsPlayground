using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor_Server.Data
{
    public class IdeaFormModel
    {
        private Contact Sponsor { get; set; } = new();

        private Contact Submitter { get; set; } = new();

        [Required]
        [StringLength(255, ErrorMessage = "First Name is too long")]
        [DisplayName("First Name")]
        public string FirstName
        {
            get
            {
                return this.Submitter.FirstName;
            }
            set
            {
                this.Submitter.FirstName = value;
            }
        }

        [Required]
        [StringLength(255, ErrorMessage = "Last Name is too long")]
        [DisplayName("Last Name")]
        public string LastName
        {
            get
            {
                return this.Submitter.LastName;
            }
            set
            {
                this.Submitter.LastName = value;
            }
        }

        [EmailAddress]
        public string Email
        {
            get
            {
                return this.Submitter.Email;
            }
            set
            {
                this.Submitter.Email = value;
            }
        }

        [Phone]
        public string Phone
        {
            get
            {
                return this.Submitter.Phone;
            }
            set
            {
                this.Submitter.Phone = value;
            }
        }


        [DisplayName("Your Website")]
        public string Url
        {
            get
            {
                return this.Submitter.Url;
            }
            set
            {
                this.Submitter.Url = value;
            }
        }


        [Required]
        [StringLength(50, ErrorMessage = "Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
        [DisplayName("Project Title")]
        public string ProjectTilte { get; set; } = string.Empty;

        [Required]
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; } = string.Empty;

        public IReadOnlyList<IBrowserFile> Attachements { get; set; } = new List<IBrowserFile>();

        [DisplayName("Are you the sponsor")]
        public bool IsSponsor { get; set; } = true;

        [Required]
        [StringLength(255, ErrorMessage = "First Name is too long")]
        [DisplayName("Sponsor First Name")]
        public string SponsorFirstName
        {
            get
            {
                return this.Sponsor.FirstName;
            }
            set
            {
                this.Sponsor.FirstName = value;
            }
        }

        [Required]
        [StringLength(255, ErrorMessage = "Last Name is too long")]
        [DisplayName("Sponsor Last Name")]
        public string SponsorLastName
        {
            get
            {
                return this.Sponsor.LastName;
            }
            set
            {
                this.Sponsor.LastName = value;
            }
        }

        [EmailAddress]
        [DisplayName("Sponsor Email")]
        public string SponsorEmail
        {
            get
            {
                return this.Sponsor.Email;
            }
            set
            {
                this.Sponsor.Email = value;
            }
        }
        
        [Phone]
        [DisplayName("Sponsor Phone")]
        public string SponsorPhone
        {
            get
            {
                return this.Sponsor.Phone;
            }
            set
            {
                this.Sponsor.Phone = value;
            }

        }
        
        [DisplayName("Sponsor's Website")]
        public string SponsorUrl
        {
            get
            {
                return this.Sponsor.Url;
            }
            set
            {
                this.Sponsor.Url = value;
            }
        }
    }
}
