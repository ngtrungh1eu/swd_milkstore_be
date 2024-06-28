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
    public interface IOrderRepository
    {
        Task<ICollection<Order>> GetOrderList();
        Task<Order> GetOrderById(int id);
        Task<Order> CreateOrder (int cartId);
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

        async Task<bool> IOrderRepository.CancelOrder(int id)
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
                    product.Quantity += productOrder.Quantity;
                    _context.Products.Update(product);
                }
            }

            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<Order> IOrderRepository.CreateOrder(int cartId)
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
                if (product == null || product.Quantity < item.Quantity)
                {
                    return null;
                }
            }

            // Choose a random available staff
            var staff = await _context.Users.FirstOrDefaultAsync(u => u.Role.Id == 2 && u.status == "Available" );

            var order = new Order
            {
                UserId = cart.UserId,
                DeliverAddress = cart.User.Address,
                StaffId = null,
                Phone = cart.User.Phone,
                FullName = cart.User.FullName,
                PaymentMethod = "Thanh Toán Khi Nhận Hàng",
                Status = "processing",
                OrderDate = DateTime.Now,
                TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product.ProductPrice),
                ProductOrders = cart.CartItems.Select(item => new ProductOrder
                {
                    ProductId = item.ProductId,
                    Image = item.Image,
                    ProductName = item.ProductName,                 
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.ProductPrice * item.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);

            // Update Product Quantity after Create Order
            foreach (var item in cart.CartItems)
            {
                var product = item.Product;
                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                    _context.Products.Update(product);
                }
            }

            // Detlete products in cart
            _context.CartItems.RemoveRange(cart.CartItems);

            // Clear totalItem in Cart
            cart.TotalItem = 0;

            await _context.SaveChangesAsync();
            return order;
        }

        async Task<Order> IOrderRepository.GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.ProductOrders)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        async Task<ICollection<Order>> IOrderRepository.GetOrderList()
        {
            return await _context.Orders.ToListAsync();
        }

        async Task<bool> IOrderRepository.UpdateProcess(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status == "cancelled")
            {
                return false;
            }

            switch (order.Status)
            {
                case "processing":
                    order.Status = "shipped";
                    _context.Orders.Update(order);
                    break;

                case "shipped":
                    order.Status = "completed";
                    _context.Orders.Update(order);
                    break;

            }
            
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
