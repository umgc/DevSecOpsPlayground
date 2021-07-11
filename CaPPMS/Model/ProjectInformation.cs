using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using CaPPMS.Attributes;
using Newtonsoft.Json;
using System.Reflection;

namespace CaPPMS.Model
{
    public class ProjectInformation
    {
        [Export]
        [Browsable(false)]
        public Guid ProjectID { get; set; } = Guid.NewGuid();

        [Export(true)]
        [Required]
        [StringLength(50, ErrorMessage = "Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
        [DisplayName("Project Title")]
        [Browsable(true)]
        [ColumnHeader]
        public string ProjectTitle { get; set; } = string.Empty;

        [Export(true)]
        [Required]
        [DisplayName("Project Description")]
        [Browsable(true)]
        [ColumnHeader]
        public string ProjectDescription { get; set; } = string.Empty;

        [Export(true)]
        [DisplayName("Project Website")]
        [Browsable(true)]
        [ColumnHeader]
        public string Url { get; set; } = string.Empty;

        [Export]
        [Browsable(false)]
        private Contact Sponsor { get; set; } = new();

        [Export]
        [Browsable(false)]
        private Contact Submitter { get; set; } = new();

        [Browsable(false)]
        [JsonIgnore]
        public bool IsDirty { get; set; } = false;

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

        public void SetAttachments(IList<IProjectFile> files)
        {
            this.Attachments.Clear();

            foreach(var file in files)
            {
                this.Attachments.Add(file as ProjectFile);
            }
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
