using BussinessLogic.DTO.Momo;
using BussinessLogic.Service;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MilkStore_BE.Controllers
{
    [Route("Checkout/PaymentCallBack")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        private readonly IMomoService _momoService;
        private readonly IOrderService _orderService;
        private readonly ILogger<MomoController> _logger;

        public MomoController(IMomoService momoService, ILogger<MomoController> logger, IOrderService orderService)
        {
            _momoService = momoService;
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] OrderInfoModel model)
        {
            try
            {
                var response = await _momoService.CreatePaymentAsync(model);

                if (response == null || string.IsNullOrEmpty(response.PayUrl))
                {
                    return BadRequest(new { message = "Failed to create payment URL" });
                }

                return Ok(new
                {
                    payUrl = response.PayUrl,
                    orderId = model.OrderId,
                    message = "Payment URL created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating payment: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallback([FromQuery] string orderId, [FromQuery] string errorCode)
        {
            try
            {
                
                // Ghi log các query parameters nhận được
                _logger.LogInformation($"Received Query Parameters: {string.Join(", ", Request.Query.Select(q => $"{q.Key}={q.Value}"))}");

                var response = _momoService.PaymentExecuteAsync(Request.Query);

                bool isSuccess = errorCode == "0";

                Match match = Regex.Match(response.OrderInfo, @"CartId:\s*(\d+)");

                string cartId = match.Success ? match.Groups[1].Value : "Không có CartId"; // Nếu tìm thấy, lấy giá trị CartId, nếu không thì thông báo lỗi
                string orderInfoWithoutCartId = Regex.Replace(response.OrderInfo, @"\s*CartId:\s*\d+", "").Trim();
                // Tạo result với các giá trị cần thiết
                var result = new
                {
                    success = isSuccess,
                    orderId = orderId,
                    amount = response.Amount,
                    cartId = cartId, // Gán giá trị CartId đã được cắt chuỗi
                    message = isSuccess ? "Payment successful" : "Payment failed",
                    orderInfo = orderInfoWithoutCartId
                };
                int cartIdInt;
                bool isCartIdValid = int.TryParse(result.cartId, out cartIdInt);


                // Tiến hành gọi CreateOrder với cartId đã chuyển thành int
                var newOrder = await _orderService.CreateOrder(cartIdInt);

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
                //var redirectUrl = $"http://localhost:7269/payment-result?success={isSuccess}&orderId={orderId}";
                //return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Missing query parameter: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing callback: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing the callback" });
            }
        }


    }
}