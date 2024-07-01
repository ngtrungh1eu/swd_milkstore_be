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
        Task<List<Feedback>> ListAllFeedback();
        Task<Feedback> GetFeedbackById(int id);
        Task<bool> UpdateFeedback(Feedback existingFeedback);
        Task<bool> DeleteFeedback(Feedback existingFeedback);
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly DataContext _context;

        public FeedbackRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> DeleteFeedback(Feedback f)
        {
            _context.Feedbacks.Remove(f);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<bool> UpdateFeedback(Feedback existingFeedback)
        {
            _context.Feedbacks.Update(existingFeedback);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
        public async Task<Feedback> GetFeedbackById(int id)
        {
            return await _context.Feedbacks.FirstOrDefaultAsync(p => p.FeedbackId == id);
        }

        async Task<List<Feedback>> IFeedbackRepository.ListAllFeedback()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<bool> CreateFeedback(Feedback f)
        {
            _context.Feedbacks.Add(f);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
