using BussinessLogic.DTO.Blog;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;

        public BlogController(IBlogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<BlogDTO>>> GetAllBlog()
        {
            return Ok(await _service.GetAllBlog());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BlogDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Blog>> GetByBlogId(int id)
        {
            var blog = await _service.GetBlogById(id);

            if (blog.Success == false && blog.Message == "Not Found")
            {
                return BadRequest();
            }

            if (blog.Success == false && blog.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display blog");
                return StatusCode(500, ModelState);
            }

            return Ok(blog);
        }

        [HttpPost("CreateBlog")]
        public async Task<ActionResult<Blog>> CreateBlog(BlogDTO request)
        {
            var newBlog = await _service.CreateBlog(request);

            if (newBlog.Success == false && newBlog.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create blog {request}");
                return StatusCode(500, ModelState);
            }

            if (newBlog.Success == false && newBlog.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when create blog {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(newBlog.Data);
        }

        [HttpPut("UpdateBlog/{id}")]
        public async Task<ActionResult> UpdateBlog(int id, int userId, [FromBody] BlogDTO request)
        {
            request.BlogId = id;

            var updateBlog = await _service.UpdateBlog(request, userId);

            if (updateBlog.Success == false && updateBlog.Message == "Not Found")
            {
                return StatusCode(404, updateBlog);
            }

            if (updateBlog.Success == false && updateBlog.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating blog {request}");
                return StatusCode(500, ModelState);
            }

            if (updateBlog.Success == false && updateBlog.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating blog {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateBlog.Data);
        }

        [HttpDelete("DeleteBlog/{id}")]
        public async Task<ActionResult> DeleteBlog(int id)
        {
            var deleteBlog = await _service.DeleteBlog(id);

            if (deleteBlog.Success == false && deleteBlog.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting Blog");
                return StatusCode(500, ModelState);
            }

            if (deleteBlog.Success == false && deleteBlog.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting Blog");
                return StatusCode(500, ModelState);
            }

            return Ok(deleteBlog);
        }
    }
}
