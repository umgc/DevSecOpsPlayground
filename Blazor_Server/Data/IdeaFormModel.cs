using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Blazor_Server.Data
{
    public class IdeaFormModel
    {
        private Contact Sponsor { get; set; } = new();

        private Contact Submitter { get; set; } = new();

        public bool IsDirty { get; set; }

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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
            }
        }


        [Required]
        [StringLength(50, ErrorMessage = "Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
        [DisplayName("Project Title")]
        public string ProjectTitle { get; set; } = string.Empty;

        [Required]
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; } = string.Empty;

        public IReadOnlyList<IProjectFile> Attachements { get; set; } = new List<IProjectFile>();

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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
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
                this.IsDirty = true;
            }
        }
    }
}
