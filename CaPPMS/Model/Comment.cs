using System;

namespace CaPPMS.Model
{
    public class Comment
    {
        public Comment(Guid projectId)
        {
            this.ProjectID = projectId;
            this.CommentId = Guid.NewGuid();
        }

        public Guid CommentId { get; set; }

        public string Comments { get; set; }

        public string UserEmail { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public Guid ProjectID { get; private set; }
    }
}
