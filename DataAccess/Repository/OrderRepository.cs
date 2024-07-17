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
    public interface IOrderRepository
    {
        Task<ICollection<Order>> GetOrderList();
        Task<Order> GetOrderById(int id);
        Task<List<Order>> CreateOrder (int cartId);
        Task<bool> UpdateProcess (int id);
        Task<bool> CancelOrder (int id);
    }
    public class OrderRepository : IOrderRepository
    {

        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ProductOrders)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null || order.Status != "processing")
            {
                return false;
            }

            order.Status = "cancelled";
            _context.Orders.Update(order);

            foreach(var productOrder in order.ProductOrders)
            {
                var product = await _context.Products.FindAsync(productOrder.ProductId);
                if (product != null)
                {
                    if (product.isPreOrder == true)
                    {
                        product.PreOrderAmount += productOrder.Quantity;
                    }
                    else
                    {
                        product.Quantity += productOrder.Quantity;
                    }
                    
                    _context.Products.Update(product);
                }
            }

            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<List<Order>> CreateOrder(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart == null)
            {
                return null;
            }

            var disabledItems = cart.CartItems.Where(item => item.Product.isDisable == true).ToList();
            if (disabledItems.Any())
            {
                _context.CartItems.RemoveRange(disabledItems);
                cart.CartItems = cart.CartItems.Except(disabledItems).ToList();
                return null;
            }

            // Check available product quantity
            foreach (var item in cart.CartItems)
            {
                var product = item.Product;

                // Kiểm tra nếu sản phẩm là PreOrder
                if (product.isPreOrder)
                {
                    if (product == null || product.PreOrderAmount < item.Quantity)
                    {
                        return null;
                    }
                }
                else
                {
                    // Kiểm tra nếu sản phẩm không phải là PreOrder
                    if (product == null || product.Quantity < item.Quantity)
                    {
                        return null;
                    }
                }
            }

            // Choose a random available staff
            var staff = await _context.Users.FirstOrDefaultAsync(u => u.Role.Id == 2 && u.status == "Available");

            // Separate cart items into preOrder and non-preOrder
            var preOrderItems = cart.CartItems.Where(item => item.Product.isPreOrder == true).ToList();
            var nonPreOrderItems = cart.CartItems.Where(item => item.Product.isPreOrder == false).ToList();

            var orders = new List<Order>();

            // Create preOrder order
            if (preOrderItems.Any())
            {
                var preOrder = new Order
                {
                    UserId = cart.UserId,
                    DeliverAddress = cart.User.Address,
                    StaffId = staff?.Id,
                    Phone = cart.User.Phone,
                    FullName = cart.User.FullName,
                    PaymentMethod = "Thanh Toán Khi Nhận Hàng",
                    Status = "processing",
                    OrderDate = DateTime.Now,
                    TotalPrice = preOrderItems.Sum(item => item.UnitPrice),
                    ProductOrders = preOrderItems.Select(item => new ProductOrder
                    {
                        ProductId = item.ProductId,
                        Image = item.Image,
                        ProductName = item.ProductName,
                        BrandName = item.BrandName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                };

                orders.Add(preOrder);
                _context.Orders.Add(preOrder);

                // Update Product PreOrderAmount
                foreach (var item in preOrderItems)
                {
                    var product = item.Product;
                    if (product != null)
                    {
                        product.PreOrderAmount -= item.Quantity;
                        _context.Products.Update(product);
                    }
                }
            }

            // Create nonPreOrder order
            if (nonPreOrderItems.Any())
            {
                var nonPreOrder = new Order
                {
                    UserId = cart.UserId,
                    DeliverAddress = cart.User.Address,
                    StaffId = staff?.Id,
                    Phone = cart.User.Phone,
                    FullName = cart.User.FullName,
                    PaymentMethod = "Thanh Toán Khi Nhận Hàng",
                    Status = "processing",
                    OrderDate = DateTime.Now,
                    TotalPrice = nonPreOrderItems.Sum(item => item.UnitPrice),
                    ProductOrders = nonPreOrderItems.Select(item => new ProductOrder
                    {
                        ProductId = item.ProductId,
                        Image = item.Image,
                        ProductName = item.ProductName,
                        BrandName = item.BrandName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                };

                orders.Add(nonPreOrder);
                _context.Orders.Add(nonPreOrder);

                // Update Product Quantity
                foreach (var item in nonPreOrderItems)
                {
                    var product = item.Product;
                    if (product != null)
                    {
                        product.Quantity -= item.Quantity;
                        _context.Products.Update(product);
                    }
                }
            }

            // Remove products from cart
            _context.CartItems.RemoveRange(cart.CartItems);

            // Clear totalItem in Cart
            cart.TotalItem = 0;
            cart.TotalPrice = 0;

            await _context.SaveChangesAsync();
            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.ProductOrders)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        async Task<ICollection<Order>> IOrderRepository.GetOrderList()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<bool> UpdateProcess(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status == "cancelled")
            {
                return false;
            }

            switch (order.Status)
            {
                case "processing":
                    order.Status = "shipping";
                    _context.Orders.Update(order);
                    break;

                case "shipping":
                    order.Status = "completed";
                    _context.Orders.Update(order);
                    break;

            }
            
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
