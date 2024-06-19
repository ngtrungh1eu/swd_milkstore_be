using BussinessLogic.DTO.Favorite;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.Promotion;
using BussinessLogic.DTO.Promotion;
using BussinessLogic.Service;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilkStore_BE;
using System.Text.Json;

namespace MIlkStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly DataContext _context;

        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public FavoriteController(IProductService productService, IUserService userService, DataContext context)
        {
            _productService = productService;
            _userService = userService;
            _context = context;
        }



        [HttpPost]
        public async Task<ActionResult> AddFavorite(AddFavoriteDTO favoriteDto)
        {
            try
            {
                Product product = await _productService.GetProductEntityById(favoriteDto.ProductId);
                User user = await _userService.GetUserById(favoriteDto.UserId);

                if (product == null || user == null)
                {
                    return BadRequest("Invalid product or user.");
                }

                await _userService.UpdateFavoriteUser(user, product);

                return Ok("Product added to favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
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
        }
    }
}
