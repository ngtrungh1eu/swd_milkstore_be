using BussinessLogic.DTO.User;
using BussinessLogic.Service;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDTO user)
        {
            var newAccount = await _service.Register(user);

            if (newAccount.Success == false && newAccount.Message == "Repo Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create Account");
                return StatusCode(500, ModelState);
            }

            if (newAccount.Success == false && newAccount.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when create Account");
                return StatusCode(500, ModelState);
            }

            return Ok(newAccount.Data);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] SignInModel model)
        {
            var result = await _service.Login(model);

            if (result.Success == false && result.Message == "Invalid Username or Password!")
            {
                ModelState.AddModelError("", $"Invalid Username or Password!");
                return Unauthorized(ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Some thing went wrong in respository layer when Login");
                return StatusCode(500, ModelState);
            }

            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel request)
        {
            var result = await _service.RefreshToken(request);

            if (result.Success == false && result.Message == "Invalid refreshToken or accessToken.")
            {
                ModelState.AddModelError("", $"Invalid refreshToken or accessToken.");
                return Unauthorized(ModelState);
            }

            if (result.Success == false && result.Message == "Error")
            {
                ModelState.AddModelError("", $"Something went wrong while refreshing token.");
                return StatusCode(500, ModelState);
            }

            return Ok(result);
        }
    }
}
