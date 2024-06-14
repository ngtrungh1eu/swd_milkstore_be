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

        async Task<ICollection<Product>> IProductRepository.ListAllProduct()
        {
            return await _context.Products.ToListAsync();
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

        async Task<bool> IProductRepository.DisableProduct(int id)
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
