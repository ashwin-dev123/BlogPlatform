using BlogPlatform.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Application.Interfaces
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetAllAsyncByPostId(Guid postId);

        Task<List<CommentDto>> GetAllAsyncByUserId(Guid userId);

        Task<CommentDto> CreateAsync(CreateCommentDto dto, Guid userId);

        Task<CommentDto> UpdateAsync(Guid id, UpdateCommentDto dto, Guid userId);

        Task<bool> DeleteAsync(Guid id, Guid userId);

    }

}
