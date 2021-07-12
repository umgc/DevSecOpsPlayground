using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CaPPMS.Model
{
    [JsonDictionary("CommentEnumeration")]
    public class Comments : IDictionary<Guid, Comment>
    {
        private ConcurrentDictionary<Guid, Comment> comments { get; set; } = new ConcurrentDictionary<Guid, Comment>();

        public Comments() { }

        public int Count => comments.Count;

        public ICollection<Guid> Keys => this.comments.Keys;

        public ICollection<Comment> Values => this.comments.Values;

        public bool IsReadOnly => false;

        public Comment this[Guid key]
        {
            get
            {
                return this[key];
            }
            set
            {
                this.comments.AddOrUpdate(key, value, (k, v) => v = value);
            }
        }


        public void Add(Comment item)
        {
            if(item.ProjectID == Guid.Empty)
            {
                throw new ArgumentNullException("Project ID cannot be null on comments while adding.");
            }

            comments.AddOrUpdate(item.CommentId, item, (k, v) => v = item);
        }

        public void Add(Guid key, Comment value)
        {
            _ = this.comments.TryAdd(key, value);
        }

        public bool ContainsKey(Guid key)
        {
            return this.comments.ContainsKey(key);
        }

        public bool Remove(Guid key)
        {
            return this.comments.TryRemove(key, out Comment _);
        }

        public bool TryGetValue(Guid key, [MaybeNullWhen(false)] out Comment value)
        {
            return this.comments.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<Guid, Comment> item)
        {
            this.comments.AddOrUpdate(item.Key, item.Value, (k, v) => v = item.Value);
        }

        public bool Contains(KeyValuePair<Guid, Comment> item)
        {
            return this.comments.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<Guid, Comment>[] array, int arrayIndex)
        {
            for(int i = arrayIndex; i < array.Length; i++)
            {
                this.Add(array[i]);
            }
        }

        public bool Remove(KeyValuePair<Guid, Comment> item)
        {
            return this.Remove(item.Key);
        }

        IEnumerator<KeyValuePair<Guid, Comment>> IEnumerable<KeyValuePair<Guid, Comment>>.GetEnumerator()
        {
            return this.comments.GetEnumerator();
        }

        public void Clear()
        {
            this.comments.Clear();
        }

        public IEnumerator GetEnumerator()
        {
            return this.comments.GetEnumerator();
        }
    }
}
