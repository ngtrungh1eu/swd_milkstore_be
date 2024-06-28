using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetCart(int id)
        {
            var cart = await _service.GetCart(id);

            if (cart.Success == false && cart.Message == "Not Found")
            {
                return BadRequest();
            }

            if (cart.Success == false && cart.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display cart");
                return StatusCode(500, ModelState);
            }

            return Ok(cart);
        }

        [HttpPost("CreateCart/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateCart(int id)
        {
            var result = await _service.CreateCart(id);

            if (result.Success == false && result.Message == "Not Found")
            {
                return BadRequest();
            }

            if (result.Success == false && result.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create cart");
                return StatusCode(500, ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when create cart");
                return StatusCode(500, ModelState);
            }

            return Ok(result.Data);
        }

        [HttpPost("AddToCart/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart(int id, int productId, int quantity)
        {
            var result = await _service.AddToCart(id, productId, quantity);

            if (result.Success == false && result.Message == "Not Found")
            {
                return BadRequest();
            }

            if (result.Success == false && result.Message == "Product quantity has to be > 0")
            {
                ModelState.AddModelError("", $"Product quantity has to be > 0");
                return StatusCode(403, ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when adding item");
                return StatusCode(500, ModelState);
            }

            return Ok(result.Data);
        }

        [HttpPut("UpdateQuantity/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> EditProductQuantity(int id, int productId, int quantity)
        {
            var result = await _service.UpdateProductQuantity(id, productId, quantity);

            if (result.Success == false && result.Message == "Not Found")
            {
                return BadRequest();
            }

            if (result.Success == false && result.Message == "Product quantity has to be > 0")
            {
                ModelState.AddModelError("", $"Product quantity has to be > 0");
                return StatusCode(403, ModelState);
            }

            if (result.Success == false && result.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when update item");
                return StatusCode(500, ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when update item");
                return StatusCode(500, ModelState);
            }

            return Ok(result.Data);
        }

        [HttpDelete("RemoveProduct/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveProductFromCart(int id, int productId)
        {
            var result = await _service.RemoveProduct(id, productId);

            if (result.Success == false && result.Message == "Not Found")
            {
                return BadRequest();
            }

            if (result.Success == false && result.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when remove item");
                return StatusCode(500, ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when remove item");
                return StatusCode(500, ModelState);
            }

            return Ok(result.Data);
        }
    }
}
