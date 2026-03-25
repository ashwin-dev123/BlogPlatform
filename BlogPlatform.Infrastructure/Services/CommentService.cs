using BlogPlatform.Application.DTOs;
using BlogPlatform.Application.Interfaces;
using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly BlogDbContext _context;

        public CommentService(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommentDto>> GetAllAsyncByPostId(Guid postId)
        {
            var comments = await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
            List<CommentDto> result = new List<CommentDto>();   
            foreach (var comment in comments)
            {
                result.Add(new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    CreatedAt = comment.CreatedAt,
                    AuthorUsername = _context.Users.FirstOrDefault(u => u.Id == comment.UserId).Username
                });
            }

            return result;
        }

        public async Task<List<CommentDto>> GetAllAsyncByUserId(Guid userId)
        {
            var comments = await _context.Comments.Where(c => c.UserId == userId).ToListAsync();
            List<CommentDto> result = new List<CommentDto>();
            foreach (var comment in comments)
            {
                result.Add(new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    PostId = comment.PostId,
                    UserId = comment.UserId,
                    CreatedAt = comment.CreatedAt,
                   AuthorUsername = (await _context.Users.FindAsync(comment.UserId))?.Username

                });
            }

            return result;
        }

        public async Task<CommentDto> CreateAsync(CreateCommentDto dto, Guid userId)
        {
            bool postExist = await _context.Posts.AnyAsync(p => p.Id == dto.PostId);
            if (!postExist)
            {
                throw new Exception("Post not found");
            }

            var comment = new Comment(dto.Content, dto.PostId, userId);
            
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            var commentDto = new CommentDto()
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                AuthorUsername = (await _context.Users.FindAsync(comment.UserId))?.Username
            };

            return commentDto;

        }

        public async Task<CommentDto> UpdateAsync(Guid id, UpdateCommentDto dto, Guid userId)
        { 
           var comment = await _context.Comments.FindAsync(id);
            if(comment == null)
            {
                return null;
            }
            if (comment.UserId != userId)
            {
                return null;
            }
            comment.Content = dto.Content;

            await _context.SaveChangesAsync();

            return new CommentDto()
            {
                Id = comment.Id,
                Content = dto.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                AuthorUsername = (await _context.Users.FindAsync(comment.UserId))?.Username

            };
        
        }

        public async Task<bool> DeleteAsync(Guid id , Guid userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }
            if (comment.UserId != userId)
            {
                return false;
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
