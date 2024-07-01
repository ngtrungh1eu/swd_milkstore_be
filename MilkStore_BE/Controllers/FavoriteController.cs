using BussinessLogic.DTO.Product;
using BussinessLogic.Service;
using DataAccess.Data;
using DataAccess.Models;
using DataAccess.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MIlkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly IProductService _productService;
        public FavoriteController(IProductService productService, DataContext context)
        {
            _productService = productService;
            _context = context;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> GetAllFavourite(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var favoriteProducts = await _context.Favorites
             .Where(f => f.UserId == user.Id)
             .Include(f => f.FavoriteProducts)
             .SelectMany(f => f.FavoriteProducts)
             .ToListAsync();


                return Ok(favoriteProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("/productId/userId/")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> AddFavorite(int productId, int userId)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (product == null || user == null)
                {
                    return BadRequest("Invalid product or user.");
                }
                var favoriteProduct = new FavoriteProduct
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice
                };
                var favoriteExistUser = await _context.Favorites
                                    .Include(f => f.FavoriteProducts)
                                    .FirstOrDefaultAsync(u => u.UserId == user.Id);
                var favoriteExistProduct = await _context.FavoriteProducts
                    .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                if (favoriteExistProduct != null) return StatusCode(300, $"Product {favoriteExistProduct.ProductName} was exist in favorite");
                if (favoriteExistUser == null)
                {
                    var favoriteModel = new Favorite
                    {
                        isAvailable = true,
                        isPromote = true,
                        UserId = user.Id,
                        FavoriteProducts = new List<FavoriteProduct> { favoriteProduct }
                    };
                    _context.Favorites.Add(favoriteModel);
                }
                else
                {
                    favoriteExistUser.FavoriteProducts.Add(favoriteProduct);
                }
                await _context.SaveChangesAsync();

                return Ok("Product added to favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("/productId/userId")]
        [Authorize(Policy = "Customer")]
        public async Task<ActionResult> DeleteFavorite(int productId, int userId)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (product == null || user == null)
                {
                    return BadRequest("Invalid product or user.");
                }
                var favoriteProduct = await _context.FavoriteProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                if (favoriteProduct == null)
                {
                    return NotFound("Favorite product not found.");
                }
                _context.FavoriteProducts.Remove(favoriteProduct);
                await _context.SaveChangesAsync();
                return Ok("Product delete out of favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        /*
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAllFavourite(int id)
        {
            try
            {
                var favouriteList = await _userService.GetFavoriteUserById(id);

                var json = JsonSerializer.Serialize(favouriteList, JsonSerializerOptionsProvider.Options);
                return new ContentResult
                {
                    Content = json,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return Ok(favouriteList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteFavorite(AddFavoriteDTO favoriteDto)
        {
            try
            {
                Product product = await _productService.GetProductEntityById(favoriteDto.ProductId);
                User user = await _userService.GetUserById(favoriteDto.UserId);

                if (product == null || user == null)
                {
                    return BadRequest("Invalid product or user.");
                }

                await _userService.DeleteFavorite(user, product);

                return Ok("Product delete out of favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }*/
    }
}
