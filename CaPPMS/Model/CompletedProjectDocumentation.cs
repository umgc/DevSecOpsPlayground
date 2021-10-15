using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using CaPPMS.Attributes;

namespace CaPPMS.Model
{
    public class CompletedProjectDocumentation
    {
        private string websiteLink = string.Empty;

        [Export(true)]
        [DisplayName("Team Website Link")]
        [Browsable(true)]
        public string WebsiteLink
        {
            get
            {
                return this.websiteLink;
            }
            set
            {
                this.websiteLink = value;
                this.IsDirty = true;
            }
        }
        private string videoLink = string.Empty;

        [Export(true)]
        [DisplayName("Team Video Link")]
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
        private string githubLink = string.Empty;

        [Export(true)]
        [DisplayName("Team GitHub Link")]
        [Browsable(true)]
        public string GitHubLink
        {
            get
            {
                return this.githubLink;
            }
            set
            {
                this.githubLink = value;
                this.IsDirty = true;
            }
        }
        private string teamName = string.Empty;

        [Export(true)]
        [DisplayName("Team Name")]
        [Browsable(true)]
        public string TeamName
        {
            get
            {
                return this.teamName;
            }
            set
            {
                this.teamName = value;
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

        [Browsable(false)]
        [JsonIgnore]
        public bool IsDirty { get; set; } = false;
    }
}
