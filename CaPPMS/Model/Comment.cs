using System;
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

        [StringLength(500, ErrorMessage = "Min: 20 characters. Max: 500 Characters", MinimumLength = 20)]
        public string Comments { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public DateTime UpdateDateTime { get; set; } = default;

        public Guid ProjectID { get; set; } = Guid.Empty;
    }
}
