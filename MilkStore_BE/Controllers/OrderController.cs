﻿using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Policy = "AllRoles")]
        public async Task<ActionResult<List<ProductDTO>>> GetOrderList()
        {
            return Ok(await _service.GetOrderList());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AllRoles")]
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
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult<List<Order>>> CreateOrder(int id)
        {
            var newOrder = await _service.CreateOrder(id);

            if (newOrder.Success == false && newOrder.Message == "Existed")
            {
                return StatusCode(409, newOrder);
            }

            if (newOrder.Success == false && newOrder.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Cart is empty, cannot create order");
                return StatusCode(400, ModelState);
            }

            if (newOrder.Success == false && newOrder.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in service layer when create order");
                return StatusCode(500, ModelState);
            }
            return Ok(newOrder.Data);
        }

        [HttpPut("UpdateProgress/{id}")]
        [Authorize(Policy = "Manager")]
        public async Task<ActionResult<Order>> UpdateProgress(int id,[FromBody] string status)
        {
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
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult<Order>> CancelOrder(int id)
        {
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
