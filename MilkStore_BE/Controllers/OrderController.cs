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
            var order = await _service.GetOrderById(id);

            if (order.Success == false && order.Message == "Not Found")
            {
                return BadRequest();
            }

            if (order.Success == false && order.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when display order");
                return StatusCode(500, ModelState);
            }

            return Ok(order);
        }

        [HttpPost("CreateOrder/{id}")]
        public async Task<ActionResult<Order>> CreateOrder(int id)
        {

            var newOrder = await _service.CreateOrder(id);
            if (newOrder.Success == false && newOrder.Message == "Existed")
            {
                return StatusCode(409, newOrder);
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
        public async Task<ActionResult<Order>> UpdateProgress(int id,[FromBody] string status)
        {
            if (id == null || status == null)
            {
                return BadRequest(ModelState);
            }

            var updateProgress = await _service.UpdateProcess(id);

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
        public async Task<ActionResult<Order>> CancelOrder(int id)
        {
            if (id == null)
            {
                return BadRequest(ModelState);
            }

            var cancelOrder = await _service.CancelOrder(id);

            if (cancelOrder.Success == false && cancelOrder.Message == "Not Found")
            {
                return StatusCode(404, cancelOrder);
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
