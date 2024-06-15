using BussinessLogic.DTO.Brand;
using BussinessLogic.DTO;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IAuthService
    {
        Task<User> AuthenticateUserAsync(string username, string password);
        Task<bool> RegisterUserAsync(User user, string password);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

    }
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        async Task<User> IAuthService.AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // Xác thực thành công
            return user;
        }

        async Task<bool> IAuthService.RegisterUserAsync(User user, string password)
        {
            return await _userRepository.AddUserAsync(user, password);
        }


        void IAuthService.CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }
            return true;
        }


    }
}
