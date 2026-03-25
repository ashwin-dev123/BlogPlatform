using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comment> Comments { get; private set; } = new List<Comment>();

        private Post() { } // For EF Core

        public Post(string title, string content, Guid authorId)
        {
            Title = title;
            Content = content;
            AuthorId = authorId;
        }

        public void Update(string title, string content)
        {
            Title = title;
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
