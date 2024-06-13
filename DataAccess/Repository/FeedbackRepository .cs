using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback> GetFeedbackByIdAsync(int id);
        Task<bool> CreateFeedbackAsync(Feedback feedback);
        Task<bool> DeleteFeedbackAsync(Feedback feedback);
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;

        public FeedbackRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback> GetFeedbackByIdAsync(int id)
        {
            return await _context.Feedbacks.FindAsync(id);
        }

        public async Task<bool> CreateFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
