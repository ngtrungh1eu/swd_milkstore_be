using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.Promotion;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIlkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;
        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<PromotionDTO>>> GetPromotionList()
        {
            return Ok(await _service.ListAllPromotion());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PromotionDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataAccess.Models.Promotion>> GetPromotionModelById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var ServiceFound = await _service.GetPromotionModelById(id);
            if (ServiceFound == null)
            {
                return NotFound();
            }
            return Ok(ServiceFound);
        }

        [HttpPost("promotionId/productId")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<DataAccess.Models.Promotion>> AddProduct(int promotionId, int productId)
        {

            var newPromotion = await _service.AddPromotionProduct(promotionId, productId);
            if (newPromotion.Success == false)
            {
                return BadRequest(newPromotion);
            }

            if (newPromotion.Success == false && newPromotion.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding product ");
                return StatusCode(500, ModelState);
            }

            if (newPromotion.Success == false && newPromotion.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding product ");
                return StatusCode(500, ModelState);
            }
            return Ok(newPromotion.Data);
        }

        [HttpPost]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<DataAccess.Models.Promotion>> AddService(PromotionDTO request)
        {
            if (request.Promote < 0)
            {
                ModelState.AddModelError("", $"Promote cannot be < 0");
                return StatusCode(500, ModelState);
            }
            var newPromotion = await _service.CreatePromotion(request);
            if (newPromotion.Success == false && newPromotion.Message == "Existed")
            {
                return Ok(newPromotion);
            }

            if (newPromotion.Success == false && newPromotion.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding product {request}");
                return StatusCode(500, ModelState);
            }

            if (newPromotion.Success == false && newPromotion.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding product {request}");
                return StatusCode(500, ModelState);
            }
            return Ok(newPromotion.Data);
        }

        [HttpPut("{promotionId}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult> UpdatePromotion(int promotionId, PromotionDTO request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }

            if (request.Promote < 0)
            {
                ModelState.AddModelError("", $"Promote cannot be < 0");
                return StatusCode(500, ModelState);
            }


            var updatePromotion = await _service.UpdatePromotion(promotionId, request);

            if (updatePromotion.Success == false && updatePromotion.Message == "NotFound")
            {
                return Ok(updatePromotion);
            }

            if (updatePromotion.Success == false && updatePromotion.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating product {request}");
                return StatusCode(500, ModelState);
            }

            if (updatePromotion.Success == false && updatePromotion.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating product {request}");
                return StatusCode(500, ModelState);
            }


            return Ok(updatePromotion);

        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult> DeletePromotion(int id)
        {
            var deletePromotion = await _service.DeletePromotion(id);


            if (deletePromotion.Success == false && deletePromotion.Message == "NotFound")
            {
                ModelState.AddModelError("", "Service Not found");
                return StatusCode(404, ModelState);
            }

            if (deletePromotion.Success == false && deletePromotion.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting Promotion");
                return StatusCode(500, ModelState);
            }

            if (deletePromotion.Success == false && deletePromotion.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting Promotion");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete("{promotionId}/product/{productId}")] //bac xem cau ham nay tui lam ok ko dr bac xoa 1 product ra khoi promotion nhung ko phai xoa lun cai promotion
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult> RemoveProductFromPromotion(int promotionId, int productId)
        {
            var result = await _service.RemoveProductFromPromotion(promotionId, productId);

            if (!result.Success)
            {
                if (result.Message == "Promotion not found")
                {
                    return NotFound(result.Message);
                }
                return StatusCode(500, result.Message);
            }

            return Ok(result.Data);
        }
    }
}
