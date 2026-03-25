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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId)
        {
            var comments = await _commentService.GetAllAsyncByPostId(postId);
            if (comments == null || comments.Count == 0)
            {
                return null;
            }

            return Ok(comments);

        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCommentsByUserId(Guid userId)
        {
            var comments = await _commentService.GetAllAsyncByUserId(userId);
            if (comments == null || comments.Count == 0)
            {
                return NotFound("No comments found for this user.");
            }

            return Ok(comments);

        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentDto dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comment = await _commentService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetCommentsByPostId),
                new { postId = comment.PostId }, comment);
        }


        [Authorize]
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateComment(Guid id, UpdateCommentDto dto)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var comment = await _commentService.UpdateAsync(id, dto, userId);
            if (comment == null)
            {
                return NotFound("Comment not found or you are not the author.");
            }
            return Ok(comment);
        }

        [Authorize]
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _commentService.DeleteAsync(id, userId);
            if (!result)
            {
                return NotFound("Comment not found or you are not the author.");
            }
            return NoContent();

        }
    }
}
