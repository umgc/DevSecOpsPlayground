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

        private string videoLink = string.Empty;

        [Export(true)]
        [DisplayName("Video Link")]
        [Browsable(true)]
        public string VideoLink
        {
            get
            {
                return this.videoLink;
            }
            set
            {
                this.videoLink = value;
                this.IsDirty = true;
            }
        }

        [Export(true)]
        [Required]
        [StringLength(256, ErrorMessage = "Title is either too short or too long. We have confidence you can figure out which.", MinimumLength = 5)]
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

        private string semesterTerm = string.Empty;

        [Export(true)]
        [DisplayName("Semester Term")]
        [Browsable(true)]
        public string SemesterTerm
        {
            get
            {
                return this.semesterTerm;
            }
            set
            {
                this.semesterTerm = value;
                this.IsDirty = true;
            }
        }

        private string semesterYear = string.Empty;

        [Export(true)]
        [DisplayName("Semester Year")]
        [Browsable(true)]
        public string SemesterYear
        {
            get
            {
                return this.semesterYear;
            }
            set
            {
                this.semesterYear = value;
                this.IsDirty = true;
            }
        }
      
        private string url = string.Empty;

        [Export(true)]
        [DisplayName("Project Website")]
        [Browsable(true)]
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

        private string gitUrl = string.Empty;

        [Export(true)]
        [DisplayName("GitHub")]
        [Browsable(true)]
        [SpanIcon("fab fa-github", true)]
        public string Github
        {
            get
            {
                return this.gitUrl;
            }
            set
            {
                this.gitUrl = value;
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
        [StringLength(50, ErrorMessage = "First name is too long.")]
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
        [StringLength(50, ErrorMessage = "Last name is too long.")]
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
        // Matches the config.
        [AttachmentsNumFilesValidator(50)]
        // Matches the config value. Value is in Mb
        [AttachmentsFileSizeValidator(10)]
        public IList<ProjectFile> Attachments { get; private set; } = new List<ProjectFile>();
        public List<CompletedProjectDocumentation> CompletedDocuments { get; set; } = new List<CompletedProjectDocumentation>();
        

        [DisplayName("Are you the sponsor")]
        [Browsable(true)]
        public bool IsSponsor { get; set; } = true;

        [Export(true)]
        [Required]
        [StringLength(255, ErrorMessage = "Sponsor first name is too long.")]
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
        [StringLength(255, ErrorMessage = "Sponsor last name is too long.")]
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

        public void AddAttachments(IList<IProjectFile> files)
        {
            foreach (var file in files)
            {
                this.Attachments.Add(file as ProjectFile);
            }

            this.IsDirty = true;
        }

        public IEnumerable<Tuple<string, object>> GetExportableInformation()
        {
            foreach(var prop in this.GetType().GetProperties())
            {
                var canExport = prop.GetCustomAttribute<ExportAttribute>()?.CanExport ?? false;

                if (canExport)
                {
                    string name = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;

                    yield return Tuple.Create(name, prop.GetValue(this));
                }
            }
        }
    }
}
