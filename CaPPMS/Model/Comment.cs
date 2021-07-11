using System;

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

        public string Comments { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public DateTime UpdateDateTime { get; set; } = default;

        public Guid ProjectID { get; private set; } = Guid.Empty;
    }
}
