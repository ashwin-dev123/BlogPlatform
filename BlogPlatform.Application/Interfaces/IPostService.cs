using BlogPlatform.Application.DTOs;
using BlogPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPlatform.Application.Interfaces
{
    public  interface IPostService
    {
        Task<List<PostDto>> GetAllAsync();
        Task<PostDto?> GetByIdAsync(Guid id);
        Task<PostDto> CreateAsync(CreatePostDto dto, Guid userId);

        Task<bool> UpdateAsync(Guid id, UpdatePostDto dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);

    }
}
