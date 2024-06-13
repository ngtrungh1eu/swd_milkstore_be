using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MilkStore_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : ControllerBase
    {
        private readonly IRateService _rateService;

        public RateController(IRateService rateService)
        {
            _rateService = rateService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddRate(int orderId, int rate, string comment)
        {
            var response = await _rateService.AddRate(orderId, rate, comment);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("delete/{feedbackId}")]
        public async Task<IActionResult> DeleteRate(int feedbackId)
        {
            var response = await _rateService.DeleteRate(feedbackId);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
