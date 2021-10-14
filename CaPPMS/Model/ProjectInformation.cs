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

        private string projectPlan = string.Empty;

        [Export(true)]
        [DisplayName("Project Plan")]
        [Browsable(true)]
        public string ProjectPlan
        {
            get
            {
                return this.projectPlan;
            }
            set
            {
                this.projectPlan = value;
                this.IsDirty = true;
            }
        }

        private string projectPlanLink = string.Empty;

        [Export(true)]
        [DisplayName("Project Plan Link")]
        [Browsable(true)]
        public string ProjectPlanLink
        {
            get
            {
                return this.projectPlanLink;
            }
            set
            {
                this.projectPlanLink = value;
                this.IsDirty = true;
            }
        }

        private string srs = string.Empty;

        [Export(true)]
        [DisplayName("Software Requirements Specification")]
        [Browsable(true)]
        public string SRS
        {
            get
            {
                return this.srs;
            }
            set
            {
                this.srs = value;
                this.IsDirty = true;
            }
        }

        private string srsLink = string.Empty;

        [Export(true)]
        [DisplayName("Software Requirements Specification Link")]
        [Browsable(true)]
        public string SRSLink
        {
            get
            {
                return this.srsLink;
            }
            set
            {
                this.srsLink = value;
                this.IsDirty = true;
            }
        }

        private string tdd = string.Empty;

        [Export(true)]
        [DisplayName("Technical Design Document")]
        [Browsable(true)]
        public string TDD
        {
            get
            {
                return this.tdd;
            }
            set
            {
                this.tdd = value;
                this.IsDirty = true;
            }
        }

        private string tddLink = string.Empty;

        [Export(true)]
        [DisplayName("Technical Design Document Link")]
        [Browsable(true)]
        public string TDDLink
        {
            get
            {
                return this.tddLink;
            }
            set
            {
                this.tddLink = value;
                this.IsDirty = true;
            }
        }

        private string runbook = string.Empty;

        [Export(true)]
        [DisplayName("Deployment and Operations Guide(Runbook)")]
        [Browsable(true)]
        public string Runbook
        {
            get
            {
                return this.runbook;
            }
            set
            {
                this.runbook = value;
                this.IsDirty = true;
            }
        }


        private string runbookLink = string.Empty;

        [Export(true)]
        [DisplayName("Deployment and Operations Guide(Runbook) Link")]
        [Browsable(true)]
        public string RunbookLink
        {
            get
            {
                return this.runbookLink;
            }
            set
            {
                this.runbookLink = value;
                this.IsDirty = true;
            }
        }

        private string programmersGuide = string.Empty;

        [Export(true)]
        [DisplayName("Programmers Guide")]
        [Browsable(true)]
        public string ProgrammersGuide
        {
            get
            {
                return this.programmersGuide;
            }
            set
            {
                this.programmersGuide = value;
                this.IsDirty = true;
            }
        }

        private string programmersGuideLink = string.Empty;

        [Export(true)]
        [DisplayName("Programmers Guide Link")]
        [Browsable(true)]
        public string ProgrammersGuideLink
        {
            get
            {
                return this.programmersGuideLink;
            }
            set
            {
                this.programmersGuideLink = value;
                this.IsDirty = true;
            }
        }

        private string usersGuide = string.Empty;

        [Export(true)]
        [DisplayName("Users Guide")]
        [Browsable(true)]
        public string UsersGuide
        {
            get
            {
                return this.usersGuide;
            }
            set
            {
                this.usersGuide = value;
                this.IsDirty = true;
            }
        }

        private string usersGuideLink = string.Empty;

        [Export(true)]
        [DisplayName("Users Guide Link")]
        [Browsable(true)]
        public string UsersGuideLink
        {
            get
            {
                return this.usersGuideLink;
            }
            set
            {
                this.usersGuideLink = value;
                this.IsDirty = true;
            }
        }

        private string testReport = string.Empty;

        [Export(true)]
        [DisplayName("Test Report")]
        [Browsable(true)]
        public string TestReport
        {
            get
            {
                return this.testReport;
            }
            set
            {
                this.testReport = value;
                this.IsDirty = true;
            }
        }

        private string testReportLink = string.Empty;

        [Export(true)]
        [DisplayName("Test Report Link")]
        [Browsable(true)]
        public string TestReportLink
        {
            get
            {
                return this.testReportLink;
            }
            set
            {
                this.testReportLink = value;
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
        [AttachmentsNumFilesValidator(10)]
        // Matches the config value. Value is in Mb
        [AttachmentsFileSizeValidator(10)]
        public IList<ProjectFile> Attachments { get; private set; } = new List<ProjectFile>();


        public IList<string> VideoLinks { get; private set; } = new List<string>();


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
