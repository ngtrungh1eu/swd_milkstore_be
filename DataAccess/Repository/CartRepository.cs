﻿using DataAccess.Data;
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
        Task<Cart> GetCart(int userId);
        Task<Cart> CreateCart(int userId);
        Task<Cart> AddToCart(int userId, int productId, int quantity);
        Task<Cart> UpdateProductQuantity(int userId, int productId, int quantity);
        Task<Cart> RemoveProduct(int userId, int productId);
    }
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;

        public CartRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Cart> AddToCart(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null;
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductPromotes)
                    .ThenInclude(pp => pp.Promotion)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            // Kiểm tra số lượng sản phẩm có sẵn
            if (product.isPreOrder && product.PreOrderAmount < quantity)
            {
                return null;
            }
            else if (!product.isPreOrder && product.Quantity < quantity)
            {
                return null;
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            // Cập nhật lại giá nếu có Promotion
            double unitPrice = product.ProductPrice;
            var promotion = product.ProductPromotes
                                .Select(pp => pp.Promotion)
                                .FirstOrDefault(pr => pr.StartAt <= DateTime.Now && pr.EndAt >= DateTime.Now);

            if (promotion != null)
            {
                unitPrice *= (1 - (promotion.Promote / 100.0));
            }

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice * quantity,
                    ProductName = product.ProductName,
                    BrandName = product.Brand.BrandName,
                    Image = product.ProductImg,
                };

                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Kiểm tra số lượng sản phẩm hiện có khi cập nhật giỏ hàng
                if (product.isPreOrder && product.PreOrderAmount < cartItem.Quantity + quantity)
                {
                    return null;
                }
                else if (!product.isPreOrder && product.Quantity < cartItem.Quantity + quantity)
                {
                    return null;
                }

                cartItem.Quantity += quantity;
                cartItem.UnitPrice = unitPrice * quantity;
            }

            cart.TotalItem = cart.TotalItem - cart.TotalItem + cart.CartItems.Sum(ci => ci.Quantity);
            cart.TotalPrice = cart.TotalPrice - cart.TotalPrice + cart.CartItems.Sum(ci => ci.UnitPrice);

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<Cart> CreateCart(int userId)
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

        public async Task<Cart> GetCart(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> RemoveProduct(int userId, int productId)
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

        public async Task<Cart> UpdateProductQuantity(int userId, int productId, int quantity)
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

            var product = await _context.Products
                                    .Include(p => p.ProductPromotes)
                                        .ThenInclude(pp => pp.Promotion)
                                    .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return null;
            }

            // Kiểm tra số lượng sản phẩm có sẵn khi cập nhật giỏ hàng
            if (product.isPreOrder)
            {
                if (product.PreOrderAmount < quantity)
                {
                    return null;
                }
            }
            else
            {
                if (product.Quantity < quantity)
                {
                    return null;
                }
            }

            // Cập nhật lại giá nếu có Promotion
            double unitPrice = product.ProductPrice;
            var promotion = product.ProductPromotes
                                .Select(pp => pp.Promotion)
                                .FirstOrDefault(pr => pr.StartAt <= DateTime.Now && pr.EndAt >= DateTime.Now);

            if (promotion != null)
            {
                unitPrice *= (1 - (promotion.Promote / 100.0));
            }

            cart.TotalItem += quantity - cartItem.Quantity;
            cart.TotalPrice += (quantity - cartItem.Quantity) * unitPrice;
            cartItem.Quantity = quantity;
            cartItem.UnitPrice = quantity * unitPrice;

            _context.CartItems.Update(cartItem);
            _context.Carts.Update(cart);

            await _context.SaveChangesAsync();

            return cart;
        }
    }
}
