using BussinessLogic.DTO.BrandDTO;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<BrandDTO>>> GetBrandList()
        {
            var response = await _service.ListAllBrands();
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response.Data);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BrandDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<BrandDTO>> GetBrandById(int id)
        {
            var brand = await _service.GetBrandById(id);

            if (brand.Success == false && brand.Message == "Not Found")
            {
                return BadRequest();
            }

            if (brand.Success == false && brand.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display brand");
                return StatusCode(500, ModelState);
            }

            return Ok(brand);
        }

        [HttpPost("CreateBrand")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<BrandDTO>> CreateBrand(BrandDTO request)
        {
            var newBrand = await _service.CreateBrand(request);

            if (newBrand.Success == false && newBrand.Message == "Existed")
            {
                return StatusCode(409, newBrand);
            }

            if (newBrand.Success == false && newBrand.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create brand {request}");
                return StatusCode(500, ModelState);
            }

            if (newBrand.Success == false && newBrand.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when create brand {request}");
                return StatusCode(500, ModelState);
            }
            return Ok(newBrand.Data);
        }

        [HttpPut("UpdateBrand/{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> UpdateBrand(int id, [FromBody] BrandDTO request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }

            request.BrandId = id;

            var updateBrand = await _service.UpdateBrand(request);

            if (updateBrand.Success == false && updateBrand.Message == "Not Found")
            {
                return StatusCode(404, updateBrand);
            }

            if (updateBrand.Success == false && updateBrand.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating brand {request}");
                return StatusCode(500, ModelState);
            }

            if (updateBrand.Success == false && updateBrand.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating brand {request}");
                return StatusCode(500, ModelState);
            }

            return Ok(updateBrand.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var deleteBrand = await _service.DeleteBrand(id);

            if (deleteBrand.Success == false && deleteBrand.Message == "Not Found")
            {
                ModelState.AddModelError("", "Brand Not found");
                return StatusCode(404, ModelState);
            }

            if (deleteBrand.Success == false && deleteBrand.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting Brrand");
                return StatusCode(500, ModelState);
            }

            if (deleteBrand.Success == false && deleteBrand.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting Brand");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
