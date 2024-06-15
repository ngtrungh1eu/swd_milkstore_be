using BussinessLogic.DTO.Brand;
using BussinessLogic.Service;
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
        public async Task<ActionResult<List<BrandDTO>>> GetBrandList()
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
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var response = await _service.GetBrandById(id);
            if (!response.Success)
            {
                if (response.Message == "NotFound")
                {
                    return NotFound(response);
                }

                return StatusCode(500, response);
            }

            return Ok(response.Data);
        }

        [HttpPost("CreateBrand")]
        public async Task<ActionResult<BrandDTO>> CreateBrand(BrandDTO brandDTO)
        {
            if (brandDTO == null)
            {
                return BadRequest("Brand data is null");
            }

            var response = await _service.CreateBrand(brandDTO);
            if (!response.Success)
            {
                return StatusCode(500, response);
            }

            return Ok(response.Data);
        }

        [HttpPut("UpdateBrand")]
        public async Task<ActionResult> UpdateBrand(BrandDTO brandDTO)
        {
            if (brandDTO == null)
            {
                return BadRequest("Brand data is null");
            }

            var response = await _service.UpdateBrand(brandDTO);
            if (!response.Success)
            {
                if (response.Message == "NotFound")
                {
                    return NotFound(response);
                }

                return StatusCode(500, response);
            }

            return Ok(response.Data);
        }

        [HttpDelete("{id}/DeleteBrand")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var response = await _service.DeleteBrand(id);
            if (!response.Success)
            {
                if (response.Message == "NotFound")
                {
                    return NotFound(response);
                }

                return StatusCode(500, response);
            }

            return NoContent();
        }
    }
}
