using DataAccess.Data;
using DataAccess.EntityModel;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IProductRepository
    {
        Task<ICollection<Product>> ListAllProduct();
        Task<ProductModel> GetProductModelById(int id);
        Task<Product> GetProductById(int id);
        Task<bool> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(Product product);
        Task<ICollection<Product>> GetListProductPromotion();
    }

    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        async Task<ICollection<Product>> IProductRepository.ListAllProduct()
        {
            return await _context.Products.ToListAsync();
        }

        async Task<ProductModel> IProductRepository.GetProductModelById(int id)
        {
            var query = from p in _context.Products
                        join b in _context.Brands on p.BrandId equals b.BrandId
                        where p.ProductId == id
                        select new ProductModel
                        {
                           ProductId = p.ProductId,
                           BrandId = b.BrandId,
                           ProductImg = p.ProductImg,
                           ProductName = p.ProductName,
                           Brand = b.BrandName,
                           BrandImg = b.BrandImg,
                           MadeIn = b.MadeIn,
                           ProductTitle = p.ProductTitle,
                           ProductDescription = p.ProductDescription,
                           ByAge = p.ByAge,
                           ProductPrice = p.ProductPrice,
                           Quantity = p.Quantity,
                           isPreOrder = p.isPreOrder,
                           PreOrderAmount = p.PreOrderAmount,
                           isPromote = 1
                        };
            return await query.FirstOrDefaultAsync();
        }

        async Task<bool> IProductRepository.CreateProduct(Product product)
        {
            _context.Products.Add(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<bool> IProductRepository.UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<bool> IProductRepository.DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<Product> IProductRepository.GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }


        async Task<ICollection<Product>> IProductRepository.GetListProductPromotion()
        {
            List<Product> productsInPromotion = await _context.Products
                .Where(p => p.Promotes.Any()) // Chỉ lấy những sản phẩm có ít nhất một khuyến mãi
                .ToListAsync();

            return productsInPromotion;
        }
    }
}
