using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IFeedbackRepository
    {
        Task<bool> CreateFeedback(Feedback feedback);
        Task<ICollection<Feedback>> ListAllFeedback(int productId);
        Task<Feedback> GetFeedbackById(int id);
        Task<bool> UpdateFeedback(int id, int orderId, int productId, int feedbackId, Feedback existingFeedback);
        Task<bool> DeleteFeedback(Feedback existingFeedback);
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;

        public FeedbackRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteFeedback(Feedback existingFeedback)
        {
            _context.Feedbacks.Remove(existingFeedback);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateFeedback(int id, int orderId, int productId, int feedbackId, Feedback existingFeedback)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.UserId == id && f.FeedbackId == feedbackId);
            if (feedback == null)
            {
                return false;
            }

            feedback.Comment = existingFeedback.Comment;
            feedback.Rate = existingFeedback.Rate;
            feedback.UpdateAt = DateTime.Now;

            _context.Feedbacks.Update(existingFeedback);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<Feedback> GetFeedbackById(int id)
        {
            return await _context.Feedbacks.FirstOrDefaultAsync(p => p.FeedbackId == id);
        }

        public async Task<ICollection<Feedback>> ListAllFeedback(int productId)
        {
            return await _context.Feedbacks.Where(f => f.ProductId == productId).ToListAsync();
        }

        public async Task<bool> CreateFeedback(Feedback feedback)
        {
            // Check if order exists and belongs to user
            var order = await _context.Orders
                                      .Include(o => o.ProductOrders)
                                      .FirstOrDefaultAsync(o => o.OrderId == feedback.OrderId && o.UserId == feedback.UserId);

            if (order == null)
            {
                return false;
            }

            // Check if order has product
            var productOrder = order.ProductOrders.FirstOrDefault(po => po.ProductId == feedback.ProductId);

            if (productOrder == null)
            {
                return false;
            }

            // Check if product has been feedback by user
            var existingFeedback = await _context.Feedbacks
                                                 .FirstOrDefaultAsync(f => (f.UserId == feedback.UserId) && (f.OrderId == feedback.OrderId) && (f.ProductId == feedback.ProductId));

            if (existingFeedback != null)
            {
                return false;
            }

            _context.Feedbacks.Add(feedback);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
