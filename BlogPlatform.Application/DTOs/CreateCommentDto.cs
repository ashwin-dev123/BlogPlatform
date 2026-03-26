using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Application.DTOs
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentCommentId { get; set; }


    }
}
