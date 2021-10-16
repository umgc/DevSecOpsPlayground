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

        [StringLength(500, ErrorMessage = "Comment must be at least 20 characters, at most 500 characters", MinimumLength = 20)]
        public string Comments { get; set; } = string.Empty;

        [StringLength(80, ErrorMessage = "You must include a valid contact email address (Min Length: 10)", MinimumLength =10)]
        public string UserEmail { get; set; } = string.Empty;

        public Contact Contact { get; set; } = new Contact();

        public DateTime UpdateDateTime { get; set; } = default;

        public Guid ProjectID { get; set; } = Guid.Empty;
    }
}
