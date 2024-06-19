using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BussinessLogic.DTO.Promotion;
using DataAccess.Models;
using BussinessLogic.DTO.Feedback;

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

        [HttpGet]
        public async Task<ActionResult<List<Feedback>>> GetPromotionList(int? productId)
        {
            return Ok(await _service.ListAllFeedback(productId));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PromotionDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataAccess.Models.Feedback>> GetFeedbackById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var ServiceFound = await _service.GetFeedbackById(id);
            if (ServiceFound == null)
            {
                return NotFound();
            }
            return Ok(ServiceFound);
        }

        [HttpPost]
        public async Task<ActionResult<DataAccess.Models.Feedback>> AddService(FeedbackDTO request)
        {
           
            var newFeedback = await _service.CreateFeedback(request);
            if (newFeedback.Success == false && newFeedback.Message == "Existed")
            {
                return Ok(newFeedback);
            }

            if (newFeedback.Success == false && newFeedback.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding product {request}");
                return StatusCode(500, ModelState);
            }

            if (newFeedback.Success == false && newFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding product {request}");
                return StatusCode(500, ModelState);
            }
            return Ok(newFeedback.Data);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateFeedback(FeedbackDTO request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }

            var updateFeedback = await _service.UpdateFeedback(request);

            if (updateFeedback.Success == false && updateFeedback.Message == "NotFound")
            {
                return Ok(updateFeedback);
            }

            if (updateFeedback.Success == false && updateFeedback.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating product {request}");
                return StatusCode(500, ModelState);
            }

            if (updateFeedback.Success == false && updateFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating product {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateFeedback);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var deleteFeedback = await _service.DeleteFeedback(id);


            if (deleteFeedback.Success == false && deleteFeedback.Message == "NotFound")
            {
                ModelState.AddModelError("", "Service Not found");
                return StatusCode(404, ModelState);
            }

            if (deleteFeedback.Success == false && deleteFeedback.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting Feedback");
                return StatusCode(500, ModelState);
            }

            if (deleteFeedback.Success == false && deleteFeedback.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting Feedback");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
