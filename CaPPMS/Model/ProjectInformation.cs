using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CaPPMS.Attributes;

namespace CaPPMS.Model
{
    public class ProjectInformation
    {        
        [Export]
        [Browsable(false)]
        public Guid ProjectID { get; set; } = Guid.NewGuid();

        private string projectTitle = string.Empty;

        [Export(true)]
        [Required]
        [StringLength(25, ErrorMessage = "Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
        [DisplayName("Project Title")]
        [Browsable(true)]
        [ColumnHeader]
        public string ProjectTitle
        {
            get
            {
                return this.projectTitle;
            }
            set
            {
                this.projectTitle = value;
                this.IsDirty = true;
            }
        }

        private string projectDescription = string.Empty;

        [Export(true)]
        [Required]
        [DisplayName("Project Description")]
        [Browsable(true)]
        [ColumnHeader]
        public string ProjectDescription
        {
            get
            {
                return this.projectDescription;
            }
            set
            {
                this.projectDescription = value;
                this.IsDirty = true;
            }
        }

        private string url = string.Empty;

        [Export(true)]
        [DisplayName("Project Website")]
        [Browsable(true)]
        [ColumnHeader]
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
                this.IsDirty = true;
            }
        }

        [Export]
        [Browsable(false)]
        private Contact Sponsor { get; set; } = new();

        [Export]
        [Browsable(false)]
        private Contact Submitter { get; set; } = new();

        [Export(true)]
        [Required]
        [StringLength(50, ErrorMessage = "First Name is too long")]
        [DisplayName("First Name")]
        [Browsable(true)]
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

        [Export(true)]
        [Required]
        [StringLength(50, ErrorMessage = "Last Name is too long")]
        [DisplayName("Last Name")]
        [Browsable(true)]
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

        [Export(true)]
        [EmailAddress]
        [Browsable(true)]
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

        [Export(true)]
        [Phone]
        [Browsable(true)]
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

        // We will export attachments manually.
        [Browsable(true)]
        public IList<ProjectFile> Attachments { get; private set; } = new List<ProjectFile>();

        [DisplayName("Are you the sponsor")]
        [Browsable(true)]
        public bool IsSponsor { get; set; } = true;

        [Export(true)]
        [Required]
        [StringLength(255, ErrorMessage = "First Name is too long")]
        [DisplayName("Sponsor First Name")]
        [Browsable(true)]
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

        [Export(true)]
        [Required]
        [StringLength(255, ErrorMessage = "Last Name is too long")]
        [DisplayName("Sponsor Last Name")]
        [Browsable(true)]
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

        [Export(true)]
        [EmailAddress]
        [DisplayName("Sponsor Email")]
        [Browsable(true)]
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

        [Export(true)]
        [Phone]
        [DisplayName("Sponsor Phone")]
        [Browsable(true)]
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

        private string status = "New";

        [ColumnHeader]
        [Browsable(true)]
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                this.IsDirty = true;
            }
        }

        [Export(false)]
        [DisplayName("Comments:")]
        public Comments Comments { get; } = new Comments();

        [Browsable(false)]
        [JsonIgnore]
        public bool IsDirty { get; set; } = false;

        public void SetAttachments(IList<IProjectFile> files)
        {
            this.Attachments.Clear();

            foreach(var file in files)
            {
                this.Attachments.Add(file as ProjectFile);
            }

            this.IsDirty = true;
        }

        public IEnumerable<Tuple<string, object>> GetExportableInformation()
        {
            foreach(var prop in this.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute<ExportAttribute>() != null)
                {
                    string name = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;

                    yield return Tuple.Create(name, prop.GetValue(this));
                }
            }
        }
    }
}
