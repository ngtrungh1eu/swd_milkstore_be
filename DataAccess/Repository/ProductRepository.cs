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
    public interface IProductRepository
    {
        Task<ICollection<Product>> ListAllProduct();
        Task<Product> GetProductById(int id);
        Task<bool> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DisableProduct(int id);
        Task<bool> DeleteProduct(Product product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;
        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Product>> ListAllProduct()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<bool> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<bool> DisableProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null || product.isDisable == true)
            {
                return false;
            }

            product.isDisable = true;
            _context.Products.Update(product);

            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
