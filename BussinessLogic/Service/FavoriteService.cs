using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IFavoriteService
    {
        Task AddProductToFavorite(int userId, int productId);
        Task<List<FavoriteProduct>> GetFavoriteProducts(int userId);
        Task RemoveProductFromFavorite(int userId, int productId);
    }

    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IProductRepository _productRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository, IProductRepository productRepository)
        {
            _favoriteRepository = favoriteRepository;
            _productRepository = productRepository;
        }

        public async Task AddProductToFavorite(int userId, int productId)
        {
            var favorite = await _favoriteRepository.GetFavoriteByUserId(userId);
            if (favorite == null)
            {
                favorite = new Favorite { UserId = userId, isAvailable = true, isPromote = false };
                await _favoriteRepository.AddFavorite(favorite);
            }

            var product = await _productRepository.GetProductById(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var favoriteProduct = new FavoriteProduct
            {
                FavoriteId = favorite.FavoriteId,
                ProductId = productId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                Promote = (bool)product.isPromote ? 1 : 0
            };

            await _favoriteRepository.AddFavoriteProduct(favoriteProduct);
        }

        public async Task<List<FavoriteProduct>> GetFavoriteProducts(int userId)
        {
            var favorite = await _favoriteRepository.GetFavoriteByUserId(userId);
            return favorite?.FavoriteProducts.ToList() ?? new List<FavoriteProduct>();
        }

        public async Task RemoveProductFromFavorite(int userId, int productId)
        {
            var favorite = await _favoriteRepository.GetFavoriteByUserId(userId);
            if (favorite == null)
            {
                throw new Exception("Favorite not found");
            }

            var favoriteProduct = await _favoriteRepository.GetFavoriteProduct(favorite.FavoriteId, productId);
            if (favoriteProduct == null)
            {
                throw new Exception("Favorite product not found");
            }

            await _favoriteRepository.RemoveFavoriteProduct(favoriteProduct);
        }
    }
}
