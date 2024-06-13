using BussinessLogic.Service;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

namespace MilkStore_BE.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ICartRepository _cartRepository;

        public UserController(AuthService authService, ICartRepository cartRepository)
        {
            _authService = authService;
            _cartRepository = cartRepository;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserLoginRequest model)
        {
            var user = await _authService.AuthenticateUserAsync(model.UserName, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            // Xác thực thành công, trả về thông tin người dùng (ví dụ: JWT token)
            // Trong ví dụ này, chỉ trả về thông tin người dùng để minh họa
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequest model)
        {
            // Kiểm tra xác thực thông tin đăng ký
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.FullName))
                return BadRequest(new { message = "Username, password, and full name are required" });

            var user = new User
            {
                UserName = model.UserName,
                Password = model.Password,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                Phone = model.Phone,
                //Image = model.Image,
                //IsDisable = model.IsDisable
            };

            bool result = await _authService.RegisterUserAsync(user, model.Password);

            if (!result)
                return BadRequest(new { message = "Username already exists" });

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCartAsync([FromBody] AddToCartRequest model)
        {
            var cart = new Cart(_cartRepository);
            await cart.AddToCartAsync(model.UserId, model.ProductId, model.Quantity);
            return Ok();
        }

        [HttpDelete("deleteCartItem/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync(int cartItemId)
        {
            var cart = new Cart(_cartRepository);
            await cart.DeleteCartItemAsync(cartItemId);
            return Ok();
        }
    }

    public class UserLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class Cart
    {
        private readonly ICartRepository _cartRepository;

        public Cart(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            await _cartRepository.AddToCartAsync(userId, productId, quantity);
        }

        public async Task DeleteCartItemAsync(int cartItemId)
        {
            await _cartRepository.DeleteCartItemAsync(cartItemId);
        }
    }
}
