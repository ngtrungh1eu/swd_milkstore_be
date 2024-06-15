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
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> AddUserAsync(User user, string password);
        void Update(User user);
        Task<bool> SaveChangesAsync();
        Task<User> GetUserById(int id);
        Task<List<User>> ListAllUser();
        Task UpdateFavoriteUser(User user, Product product);
        Task DeleteFavorite(User user, Product product);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> AddUserAsync(User user, string password)
        {
            if (await GetUserByUsernameAsync(user.UserName) != null)
                return false; // Người dùng đã tồn tại

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            return await SaveChangesAsync();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        async Task<User> IUserRepository.GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<List<User>> ListAllUser()
        {
            // return await _context.Users.ToListAsync();
            return await _context.Users
                .Include(u => u.Products) // Eager load Products collection
                .ToListAsync();
        }

        async Task IUserRepository.UpdateFavoriteUser(User u, Product p)
        {
            if (u.Products == null)
            {
                u.Products = new List<Product>();
            }
            foreach(Product item in u.Products)
            {
                if(p.ProductId == item.ProductId)
                {
                    return;
                }
            }
            u.Products.Add(p);
            await _context.SaveChangesAsync();
        }

        async Task IUserRepository.DeleteFavorite(User user, Product product)
        {
            if (user.Products == null)
            {
                user = await _context.Users
                    .Include(u => u.Products)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);
            }
            var favoriteToRemove = user.Products.FirstOrDefault(p => p.ProductId == product.ProductId);

            if (favoriteToRemove != null)
            {
                user.Products.Remove(favoriteToRemove);
                await _context.SaveChangesAsync();
            }
        }
    }
}
