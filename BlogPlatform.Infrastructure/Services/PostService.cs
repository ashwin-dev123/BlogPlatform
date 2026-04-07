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
    public class PostService : IPostService
    {
        private readonly BlogDbContext _context;

        public PostService(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<List<PostDto>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorId = p.AuthorId,
                    AuthorUsername = p.Author.Username,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PostDto?> GetByIdAsync(Guid id)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return null;

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorId = post.AuthorId,
                AuthorUsername = post.Author.Username,
                CreatedAt = post.CreatedAt
            };
        }

        public async Task<PostDto> CreateAsync(CreatePostDto dto, Guid userId)
        {
            var post = new Post(dto.Title, dto.Content,dto.CoverImageUrl, userId);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                AuthorId = post.AuthorId,
                AuthorUsername = "",
                CoverImageUrl = dto.CoverImageUrl,
                CreatedAt = post.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(Guid id,UpdatePostDto dto,Guid userId)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null || post.AuthorId != userId)
                return false;

            post.Update(dto.Title, dto.Content);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)   
                return false;

            // ✅ ownership check
            if (post.AuthorId != userId)
                return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
