using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CaPPMS.Model
{
    public class Comments : IEnumerable<Comment>
    {
        private ConcurrentDictionary<Guid, Comment> comments = new ConcurrentDictionary<Guid, Comment>();

        public int Count => comments.Count;

        public void Add(Comment item)
        {
            if(item.ProjectID == Guid.Empty)
            {
                throw new ArgumentNullException("Project ID cannot be null on comments while adding.");
            }

            comments.AddOrUpdate(item.CommentId, item, (k, v) => v = item);
        }

        public void Clear()
        {
            comments.Clear();
        }

        public bool Contains(Comment item)
        {
            return comments.ContainsKey(item.CommentId);
        }

        public IEnumerator<Comment> GetEnumerator()
        {
            foreach(var item in comments.Values)
            {
                yield return item;
            }
        }

        public bool Remove(Comment item)
        {
            return comments.TryRemove(item.CommentId, out Comment _);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
