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
        Task<ICollection<Product>> DisplayProduct();
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
            var products = await _context.Products
                                .Include(p => p.Brand)
                                .Include(p => p.ProductPromotes)
                                    .ThenInclude(pp => pp.Promotion)
                                .ToListAsync();

            foreach (var product in products)
            {
                var feedback = await _context.Feedbacks
                                .Where(f => f.ProductId == product.ProductId)
                                .ToListAsync();

                if (feedback.Count > 0)
                {
                    product.Rate = feedback.Average(f => f.Rate);
                }
                else
                {
                    product.Rate = 0;
                }

                product.ProductBrand = product.Brand?.BrandName ?? "Unknown";

                product.Discount = product.Discount; 
            }

            return products;
        }

        public async Task<ICollection<Product>> DisplayProduct()
        {
            var products = await _context.Products
                                .Include(p => p.Brand)
                                .Include(p => p.ProductPromotes)
                                    .ThenInclude(pp => pp.Promotion)
                                .Where(p => !p.isDisable)
                                .ToListAsync();

            foreach (var product in products)
            {
                var feedback = await _context.Feedbacks
                                .Where(f => f.ProductId == product.ProductId)
                                .ToListAsync();

                if (feedback.Count > 0)
                {
                    product.Rate = feedback.Average(f => f.Rate);
                }
                else
                {
                    product.Rate = 0;
                }

                product.ProductBrand = product.Brand?.BrandName ?? "Unknown";

                // Kiểm tra nếu ProductPromotes không null trước khi truy cập
                if (product.ProductPromotes != null)
                {
                    var promotion = product.ProductPromotes
                        .Select(pp => pp.Promotion)
                        .FirstOrDefault(p => p.StartAt <= DateTime.Now && p.EndAt >= DateTime.Now);

                    product.Discount = product.Discount + promotion?.Promote ?? product.Discount;
                }
            }

            return products;
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
            var product = await  _context.Products
                            .Include(p => p.Brand)
                            .Include(p => p.ProductPromotes) // 
                                .ThenInclude(pp => pp.Promotion) //
                            .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                var feedbacks = await _context.Feedbacks
                                    .Where(f => f.ProductId == product.ProductId)
                                    .ToListAsync();

                if (feedbacks.Count > 0)
                {
                    product.Rate = feedbacks.Average(f => f.Rate);
                }
                else
                {
                    product.Rate = 0;
                }

                product.ProductBrand = product.Brand.BrandName;
                //
                if (product.ProductPromotes != null)
                {
                    var promotion = product.ProductPromotes
                        .Select(pp => pp.Promotion)
                        .FirstOrDefault(p => p.StartAt <= DateTime.Now && p.EndAt >= DateTime.Now);

                    product.Discount = product.Discount + (promotion?.Promote ?? 0); // Mapping Discount
                }
                //
            }

            return product;
        }

        public async Task<bool> DisableProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            product.isDisable = !product.isDisable;

            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
