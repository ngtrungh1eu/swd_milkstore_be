using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MIlkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetProductList()
        {
            return Ok(await _service.ListAllProduct());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataAccess.Models.Product>> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var ServiceFound = await _service.GetProductById(id);
            if (ServiceFound == null)
            {
                return NotFound();
            }
            return Ok(ServiceFound);
        }
        
        [HttpGet("Model/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<DataAccess.Models.Product>> GetProductModelById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var ServiceFound = await _service.GetProductModelById(id);
            if (ServiceFound == null)
            {
                return NotFound();
            }
            return Ok(ServiceFound);
        }

        [HttpPost]
        public async Task<ActionResult<DataAccess.Models.Product>> AddService(ProductDTO request)
        {


            var newProduct = await _service.CreateProduct(request);
            if (newProduct.Success == false && newProduct.Message == "Existed")
            {
                return Ok(newProduct);
            }

            if (newProduct.Success == false && newProduct.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when adding product {request}");
                return StatusCode(500, ModelState);
            }

            if (newProduct.Success == false && newProduct.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding product {request}");
                return StatusCode(500, ModelState);
            }
            return Ok(newProduct.Data);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateProduct(ProductDTO request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }


            var updateProduct = await _service.UpdateProduct(request);

            if (updateProduct.Success == false && updateProduct.Message == "NotFound")
            {
                return Ok(updateProduct);
            }

            if (updateProduct.Success == false && updateProduct.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when updating product {request}");
                return StatusCode(500, ModelState);
            }

            if (updateProduct.Success == false && updateProduct.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when updating product {request}");
                return StatusCode(500, ModelState);
            }


            return Ok(updateProduct);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var deleteProduct = await _service.DeleteProduct(id);


            if (deleteProduct.Success == false && deleteProduct.Message == "NotFound")
            {
                ModelState.AddModelError("", "Service Not found");
                return StatusCode(404, ModelState);
            }

            if (deleteProduct.Success == false && deleteProduct.Message == "RepoError")
            {
                ModelState.AddModelError("", $"Some thing went wrong in Repository when deleting Product");
                return StatusCode(500, ModelState);
            }

            if (deleteProduct.Success == false && deleteProduct.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when deleting Product");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }
    }
}
