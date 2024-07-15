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
    public interface IFavoriteRepository
    {
        Task<Favorite> GetFavoriteByUserId(int userId);
        Task<Favorite> CreateFavorite(int userId);
        Task AddFavorite(Favorite favorite);
        Task AddFavoriteProduct(FavoriteProduct favoriteProduct);
        Task RemoveFavoriteProduct(FavoriteProduct favoriteProduct);
        Task<FavoriteProduct> GetFavoriteProduct(int favoriteId, int productId);
    }

    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly DataContext _context;


        public async Task<Favorite> GetFavoriteByUserId(int userId)
        {
            return await _context.Favorites.Include(f => f.FavoriteProducts)
                                            .FirstOrDefaultAsync(f => f.UserId == userId);
        }

        public async Task<Favorite> CreateFavorite(int userId)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                isAvailable = false,
                isPromote = false,
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return favorite;
        }

        public async Task AddFavorite(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task AddFavoriteProduct(FavoriteProduct favoriteProduct)
        {
            _context.FavoriteProducts.Add(favoriteProduct);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteProduct(FavoriteProduct favoriteProduct)
        {
            _context.FavoriteProducts.Remove(favoriteProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<FavoriteProduct> GetFavoriteProduct(int favoriteId, int productId)
        {
            return await _context.FavoriteProducts.FirstOrDefaultAsync(fp => fp.FavoriteId == favoriteId && fp.ProductId == productId);
        }
    }
}
