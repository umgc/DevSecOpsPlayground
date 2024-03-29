﻿using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace CaPPMS.Model
{
    public class ProjectFile : IProjectFile
    {
        private string location = string.Empty;

        // Parameterless contructor for Deserialization
        public ProjectFile() { }

        public ProjectFile(IBrowserFile browserFile)
        {
            this.BrowserFile = browserFile;
        }

        private long size;

        public Guid File_ID { get; set; } = Guid.NewGuid();

        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                this.location = value;
                this.BrowserFile = null;
            }
        }

        public string Name
        {
            get
            {
                if (BrowserFile != null)
                {
                    return BrowserFile.Name;
                }

                return Location?.Split(ProjectFileManager.Delimiter, StringSplitOptions.RemoveEmptyEntries).Last() ?? string.Empty;
            }
        }

        public long Size
        {
            get
            {
                if (BrowserFile != null)
                {
                    size = BrowserFile.Size;
                }

                return size;
            }
            set
            {
                size = value;
            }
        }
        
        [JsonIgnore]
        public IBrowserFile BrowserFile { get; private set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
