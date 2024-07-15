using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
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
    public interface IAuthenticaionRepository
    {
        Task<TokenModel> Login(SignInModel model);
        Task<TokenModel> RefreshToken(TokenModel model);
        Task<bool> Register(User user);
    }
    public class AuthenticationRepository : IAuthenticaionRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<bool> Register(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<TokenModel> Login(SignInModel model)
        {
            var user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password && u.IsDisable == false);

            if (user == null)
            {
                return null;
            }

            var authenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("Role", user.Role.RoleType)
            };

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: authenClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsExpired = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<TokenModel> RefreshToken(TokenModel model)
        {
            // Lấy refreshToken từ cơ sở dữ liệu dựa trên refreshToken
            var refreshTokenEntity = await _context.RefreshTokens.Include(rt => rt.User)
                                            .SingleOrDefaultAsync(rt => rt.Token == model.RefreshToken && !rt.IsUsed && !rt.IsExpired);

            if (refreshTokenEntity == null)
            {
                throw new Exception("Invalid refreshToken.");
            }

            // Cập nhật refreshToken đã được sử dụng
            refreshTokenEntity.IsUsed = true;
            _context.RefreshTokens.Update(refreshTokenEntity);
            await _context.SaveChangesAsync();

            // Tạo AccessToken mới từ thông tin của accessToken
            var authenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", refreshTokenEntity.UserId.ToString()),
                new Claim("RoleId", refreshTokenEntity.User.RoleId.ToString()),
                new Claim(ClaimTypes.Role, refreshTokenEntity.User.Role.RoleType)
            };

            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(24),
                claims: authenClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha256)
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Trả về token mới và refreshToken
            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = GenerateRefreshToken()
            };
        }
    }
}
