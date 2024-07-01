using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetAllUser();
        Task<User> GetUserById(int id);
        Task<bool> CreateStaff(User user);
        Task<bool> DisableUser(int id);
        Task<bool> DeleteUser(User user);
        Task<bool> UpdateAccount(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> CreateStaff(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteUser(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DisableUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || user.IsDisable == true)
            {
                return false;
            }

            user.IsDisable = true;
            _context.Users.Update(user);

            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<ICollection<User>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateAccount(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
