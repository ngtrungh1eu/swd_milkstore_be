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
        Task<bool> CreateOrder (int cartId);
        Task<bool> UpdateProcess (int id, string status);
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
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status != "Đang Xác Nhận")
            {
                return false;
            }

            order.Status = "Đã Hủy";
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<bool> IOrderRepository.CreateOrder(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart == null)
            {
                return false;
            }

            var order = new Order
            {
                UserId = cart.UserId,
                DeliverAddress = cart.User.Address,
                Phone = cart.User.Phone,
                FullName = cart.User.FullName,
                PaymentMethod = "Thanh Toán Khi Nhận Hàng",
                Status = "Đang Xác Nhận",
                OrderDate = DateTime.Now,
                TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product.ProductPrice),
                ProductOrders = cart.CartItems.Select(item => new ProductOrder
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.ProductPrice
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.Carts.Remove(cart);

            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<Order?> IOrderRepository.GetOrderById(int id)
        {
            return await _context.Orders.Include(x => x.ProductOrders).FirstOrDefaultAsync(o => o.OrderId == id);
        }

        async Task<ICollection<Order>> IOrderRepository.GetOrderList()
        {
            return await _context.Orders.ToListAsync();
        }

        async Task<bool> IOrderRepository.UpdateProcess(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status == "Đã Hủy")
            {
                return false;
            }

            order.Status = status;
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
