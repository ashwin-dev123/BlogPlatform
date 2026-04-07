using BlogPlatform.Application.DTOs;
using BlogPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // Public
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            return Ok(await _postService.GetAllAsync());
        }

        // Public
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        // ✅ Must be logged  in
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var post = await _postService.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetPost),
                new { id = post.Id }, post);
        }

        // ✅ Must be logged in
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _postService.UpdateAsync(id, dto, userId);

            if (!result)
                return Forbid();

            return NoContent();
        }

        // ✅ Must be logged in
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _postService.DeleteAsync(id, userId);

            if (!result) // if result is false, it means either post doesn't exist or user is not the author
                return Forbid();

            return NoContent();
        }
    }
}
