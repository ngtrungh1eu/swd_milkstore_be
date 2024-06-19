using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface ICartRepository
    {
        Task<Cart> GetCart(int cartId);
        Task<Cart> CreateCart(int userId);
        Task<Cart> AddToCart(int userId, int productId, int quantity);
        Task<Cart> UpdateProductQuantity(int userId, int productId, int quantity);
        Task<Cart> RemoveProduct(int cartId, int productId);
    }
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;

        public CartRepository(DataContext context)
        {
            _context = context;
        }

        async Task<Cart> ICartRepository.AddToCart(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null;
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return null;
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.ProductPrice * quantity,
                    ProductName = product.ProductName,
                    Image = product.ProductImg
                };

                _context.CartItems.Add(cartItem);
            }

            cart.TotalItem = cart.TotalItem - cart.TotalItem + cart.CartItems.Sum(ci => ci.Quantity);
            cart.TotalPrice = cart.TotalPrice - cart.TotalPrice + cart.CartItems.Sum(ci => ci.UnitPrice);

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        async Task<Cart> ICartRepository.CreateCart(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                TotalItem = 0,
                TotalPrice = 0,
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        async Task<Cart> ICartRepository.GetCart(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        async Task<Cart> ICartRepository.RemoveProduct(int userId, int productId)
        {
           var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null;
            }

            var cartItem  = await _context.CartItems.FirstOrDefaultAsync(ci => (ci.CartId == cart.CartId) && (ci.ProductId == productId));

            if (cartItem == null)
            {
                return null;
            }

            cart.TotalItem -= cartItem.Quantity;
            cart.TotalPrice -= cartItem.UnitPrice;

            _context.CartItems.Remove(cartItem);
            _context.Carts.Update(cart);

             await _context.SaveChangesAsync();

            return cart;
        }

        async Task<Cart> ICartRepository.UpdateProductQuantity(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null;
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => (ci.CartId == cart.CartId) && (ci.ProductId == productId));

            if (cartItem == null || quantity < 0)
            {
                return null;
            }

            var product = await _context.Products.FindAsync(productId);

            if (product == null || product.Quantity < quantity - cartItem.Quantity)
            {
                return null;
            }

            cart.TotalItem += quantity - cartItem.Quantity;
            cart.TotalPrice += (quantity - cartItem.Quantity) * cartItem.Product.ProductPrice;
            cartItem.Quantity = quantity;
            cartItem.UnitPrice = quantity * cartItem.Product.ProductPrice;

            _context.CartItems.Update(cartItem);
            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            return cart;

        }
    }
}
