using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BussinessLogic.DTO.Promotion;
using DataAccess.Models;
using BussinessLogic.DTO.Feedback;
using Microsoft.AspNetCore.Authorization;

namespace MIlkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;
        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        [HttpGet("product/{id}")]
        public async Task<ActionResult<List<Feedback>>> GetFeedbackList(int id)
        {
            return Ok(await _service.ListAllFeedback(id));
        }

        //lay id theo feedback id
        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PromotionDTO))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesDefaultResponseType]
        //public async Task<ActionResult<DataAccess.Models.Feedback>> GetFeedbackById(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return BadRequest(id);
        //    }
        //    var ServiceFound = await _service.GetFeedbackById(id);
        //    if (ServiceFound == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(ServiceFound);
        //}

        [HttpPost("AddFeedback/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult<FeedbackDTO>> AddFeedback(int id, [FromQuery] int orderId, [FromQuery] int productId, [FromBody] FeedbackDTO request)
        {
            request.UserId = id;
            request.ProductId = productId;
            request.OrderId = orderId;

            var newFeedback = await _service.CreateFeedback(request);
            if (newFeedback.Success == false)
            {
                return BadRequest();
            }

            if (newFeedback.Success == false && newFeedback.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding feedback {request}");
                return StatusCode(500, ModelState);
            }

            if (newFeedback.Success == false && newFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding feedback {request}");
                return StatusCode(500, ModelState);
            }
            return Ok("Add feedback success");
        }

        [HttpPut("UpdateFeedback/{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> UpdateFeedback(int id, [FromQuery] int orderId, [FromQuery] int productId, [FromQuery] int feedbackId, [FromBody] FeedbackDTO request)
        {
            request.UserId = id;
            request.FeedbackId = feedbackId;
            request.OrderId = orderId;
            request.ProductId = productId;

            var updateFeedback = await _service.UpdateFeedback(id, orderId, productId, feedbackId, request);

            if (updateFeedback.Success == false && updateFeedback.Message == "Not Found")
            {
                return BadRequest();
            }

            if (updateFeedback.Success == false && updateFeedback.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating feedback {request}");
                return StatusCode(500, ModelState);
            }

            if (updateFeedback.Success == false && updateFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating feedback {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateFeedback);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var deleteFeedback = await _service.DeleteFeedback(id);


            if (deleteFeedback.Success == false && deleteFeedback.Message == "Not Found")
            {
                ModelState.AddModelError("", $"Feedback Not found");
                return StatusCode(404, ModelState);
            }

            if (deleteFeedback.Success == false && deleteFeedback.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting feedback");
                return StatusCode(500, ModelState);
            }

            if (deleteFeedback.Success == false && deleteFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting feedback");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
