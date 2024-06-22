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
    public interface ICommentRepository
    {
        Task<Comment> GetCommentById(int id);
        Task<ICollection<Comment>> GetCommentByBlog(int blogId);
        Task<bool> CreateComment (Comment comment);
        Task<bool> UpdateComment (Comment comment);
        Task<bool> DeleteComment(Comment comment);
    }
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        async Task<bool> ICommentRepository.CreateComment(Comment comment)
        {
            _context.Comments.Add(comment);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<bool> ICommentRepository.DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<Comment> ICommentRepository.GetCommentById(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
        }

        async Task<ICollection<Comment>> ICommentRepository.GetCommentByBlog(int blogId)
        {
            return await _context.Comments.Where(c => c.BlogId == blogId).ToListAsync();
        }

        async Task<bool> ICommentRepository.UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
