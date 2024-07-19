using BussinessLogic.DTO.Blog;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _service;

        public VoteController(IVoteService service)
        {
            _service = service;
        }

        [HttpPost("AddVote/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult<Comment>> AddVote(int id, int userId, [FromBody] VoteDTO request)
        {
            request.BlogId = id;

            var newVote = await _service.AddVote(userId, request);

            if (newVote.Success == false && newVote.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when voting blog {request}");
                return StatusCode(500, ModelState);
            }

            if (newVote.Success == false && newVote.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when voting blog {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(newVote);
        }

        [HttpPut("UpdateVote/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> updateVote(int id, int userId, [FromBody] VoteDTO request)
        {
            request.VoteId = id;

            var updateVote = await _service.UpdateVote(userId, request);

            if (updateVote.Success == false && updateVote.Message == "Not Found")
            {
                ModelState.AddModelError("", "Vote Not found");
                return StatusCode(404, ModelState);
            }

            if (updateVote.Success == false && updateVote.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating vote {request}");
                return StatusCode(500, ModelState);
            }

            if (updateVote.Success == false && updateVote.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating vote {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateVote);
        }
    }
}
