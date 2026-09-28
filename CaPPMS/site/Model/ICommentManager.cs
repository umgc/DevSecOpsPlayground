using System;
using System.Collections.Generic;

namespace CaPPMS.Model
{
    public interface ICommentManager
    {
        void AddComment(Comment comment);
        void DeleteComment(Comment comment);

        IEnumerable<Comment> GetComments(Guid ProjectID);
    }
}
