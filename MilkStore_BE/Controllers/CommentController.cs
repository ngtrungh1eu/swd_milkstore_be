using BussinessLogic.DTO.Blog;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetCommentById(int id)
        {
            var comment = await _service.GetCommentById(id);

            if (comment.Success == false && comment.Message == "Not Found")
            {
                ModelState.AddModelError("", "Comment Not found");
                return StatusCode(404, ModelState);
            }

            if (comment.Success == false && comment.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display comment");
                return StatusCode(500, ModelState);
            }

            return Ok(comment);
        }

        [HttpPost("CreateComment/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult<Comment>> CreateComment(int id, int userId, [FromBody] CommentDTO request)
        {
            request.BlogId = id;

            var newComment = await _service.CreateComment(userId, request);

            if (newComment.Success == false && newComment.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding comment {request}");
                return StatusCode(500, ModelState);
            }

            if (newComment.Success == false && newComment.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding comment {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(newComment);
        }

        [HttpPut("UpdateComment/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> UpdateComment(int id, int userId, [FromBody] CommentDTO request)
        {
            request.CommentId = id;

            var updateComment = await _service.UpdateComment(userId, request);

            if (updateComment.Success == false && updateComment.Message == "Not Found")
            {
                ModelState.AddModelError("", "Comment Not found");
                return StatusCode(404, ModelState);
            }

            if (updateComment.Success == false && updateComment.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating comment {request}");
                return StatusCode(500, ModelState);
            }

            if (updateComment.Success == false && updateComment.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating comment {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateComment);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var deleteComment = await _service.DeleteComment(id);


            if (deleteComment.Success == false && deleteComment.Message == "Not Found")
            {
                ModelState.AddModelError("", "Comment Not found");
                return StatusCode(404, ModelState);
            }

            if (deleteComment.Success == false && deleteComment.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting comment");
                return StatusCode(500, ModelState);
            }

            if (deleteComment.Success == false && deleteComment.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting comment");
                return StatusCode(500, ModelState);
            }

            return Ok(deleteComment);
        }
    }
}
