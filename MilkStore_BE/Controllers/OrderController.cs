using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetOrderList()
        {
            return Ok(await _service.GetOrderList());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(id);
            }
            var OrderFound = await _service.GetOrderById(id);
            if (OrderFound == null)
            {
                return NotFound();
            }
            return Ok(OrderFound);
        }

        [HttpPost("CreateOrder/{id}")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] int cartId)
        {

            var newOrder = await _service.CreateOrder(cartId);
            if (newOrder.Success == false && newOrder.Message == "Existed")
            {
                return Ok(newOrder);
            }

            if (newOrder.Success == false && newOrder.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create order");
                return StatusCode(500, ModelState);
            }

            if (newOrder.Success == false && newOrder.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when create order");
                return StatusCode(500, ModelState);
            }
            return Ok(newOrder.Data);
        }

        [HttpPut("UpdateProgress/{id}")]
        public async Task<ActionResult<Order>> UpdateProgress(int orderId, [FromBody] string status)
        {
            if (orderId == null || status == null)
            {
                return BadRequest(ModelState);
            }

            var updateProgress = await _service.UpdateProcess(orderId, status);

            if (updateProgress.Success == false && updateProgress.Message == "Not Found")
            {
                return Ok(updateProgress);
            }

            if (updateProgress.Success == false && updateProgress.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when update progress");
                return StatusCode(500, ModelState);
            }

            if (updateProgress.Success == false && updateProgress.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when update progress");
                return StatusCode(500, ModelState);
            }
            return Ok(updateProgress.Data);
        }
        
        [HttpPut("CancelOrder/{id}")]
        public async Task<ActionResult<Order>> CancelOrder(int orderId)
        {
            if (orderId == null)
            {
                return BadRequest(ModelState);
            }

            var cancelOrder = await _service.CancelOrder(orderId);

            if (cancelOrder.Success == false && cancelOrder.Message == "Not Found")
            {
                return Ok(cancelOrder);
            }

            if (cancelOrder.Success == false && cancelOrder.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when cancel order");
                return StatusCode(500, ModelState);
            }

            if (cancelOrder.Success == false && cancelOrder.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when cancel order");
                return StatusCode(500, ModelState);
            }
            return Ok(cancelOrder.Data);
        }
    }
}
