﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class Comment
    {
        // Default Constructor to read from Json File.
        public Comment() { }

        // Constructor to attach to a specific Project.
        public Comment(Guid projectId)
        {
            this.ProjectID = projectId;
        }

        public Guid CommentId { get; set; } = Guid.NewGuid();

        [Required]
        public string Comments { get; set; } = string.Empty;

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public Contact Contact { get; set; } = new Contact();

        public DateTime UpdateDateTime { get; set; } = default;

        public Guid ProjectID { get; set; } = Guid.Empty;
    }
}
