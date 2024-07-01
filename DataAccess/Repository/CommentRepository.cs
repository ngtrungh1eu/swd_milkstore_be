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
        Task<bool> CreateComment(int userId, Comment comment);
        Task<bool> UpdateComment(int userId, Comment comment);
        Task<bool> DeleteComment(Comment comment);
    }
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;
        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateComment(int userId, Comment comment)
        {
            var commentBlog = new Comment
            {
                UserId = userId,
                BlogId = comment.BlogId,
                Content = comment.Content,
                CreatedDate = DateTime.Now,
            };

            _context.Comments.Add(commentBlog);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<Comment> GetCommentById(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task<ICollection<Comment>> GetCommentByBlog(int blogId)
        {
            return await _context.Comments.Where(c => c.BlogId == blogId).ToListAsync();
        }

        public async Task<bool> UpdateComment(int userId, Comment comment)
        {
            var existingComment = _context.Comments.FirstOrDefault(c => (c.UserId == userId) && (c.CommentId == comment.CommentId));

            if (existingComment == null)
            {
                return false;
            }

            existingComment.Content = comment.Content;
            existingComment.UpdatedDate = DateTime.Now;

            _context.Comments.Update(existingComment);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
