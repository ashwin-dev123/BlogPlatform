using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get ; set; }

        public Guid? ParentCommentId { get; set; }

        public List<CommentDto> Replies { get; set; }

        public string AuthorUsername { get; set; }

    }
}
