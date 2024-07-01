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
    public interface IBlogRepository
    {
        Task<ICollection<Blog>> GetAllBlog();
        Task<Blog> GetBlogById(int id);
        Task<bool> CreateBlog(Blog blog);
        Task<bool> UpdateBlog(Blog blog, int userId);
        Task<bool> DeleteBlog(Blog blog);
        Task<int> CountUseful(int blogId);
        Task<int> CountNotUseful(int blogId);
    }
    public class BlogRepository : IBlogRepository
    {
        private readonly DataContext _context;

        public BlogRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> CountNotUseful(int blogId)
        {
            return await _context.Votes.CountAsync(v => v.BlogId == blogId && v.VoteType == false);
        }

        public async Task<int> CountUseful(int blogId)
        {
            return await _context.Votes.CountAsync(v => v.BlogId == blogId && v.VoteType == true);
        }

        public async Task<bool> CreateBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteBlog(Blog blog)
        {
            _context.Blogs.Remove(blog);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<ICollection<Blog>> IBlogRepository.GetAllBlog()
        {
            return await _context.Blogs.ToListAsync();
        }

        async Task<Blog> IBlogRepository.GetBlogById(int id)
        {
            var blogDetail = await _context.Blogs
                .Where(b => b.BlogId == id)
                .Select(b => new Blog
                {
                    BlogId = b.BlogId,
                    Title = b.Title,
                    Content = b.Content,
                    BlogImg = b.BlogImg,
                    CreateAt = b.CreateAt,
                    UpdateAt = b.UpdateAt,
                    UsefulVote = _context.Votes.Count(v => v.BlogId == id && v.VoteType == true),
                    NotUsefulVote = _context.Votes.Count(v => v.BlogId == id && v.VoteType == false),
                    Tags = b.Tags,
                })
                .FirstOrDefaultAsync();

            return blogDetail;
        }

        public async Task<bool> UpdateBlog(Blog blog, int userId)
        {
            var existingBlog = await _context.Blogs.FirstOrDefaultAsync(b => (b.BlogId == blog.BlogId) && (b.UserId == userId));

            if (existingBlog == null)
            {
                return false;
            }

            existingBlog.BlogImg = blog.BlogImg;
            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;
            existingBlog.UpdateAt = DateTime.Now;
            existingBlog.Tags = blog.Tags;

            _context.Blogs.Update(existingBlog);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
