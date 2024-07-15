using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IBrandRepository
    {
        Task<ICollection<Brand>> ListAllBrands();
        Task<Brand> GetBrandById(int id);
        Task<bool> CreateBrand(Brand brand);
        Task<bool> UpdateBrand(Brand brand);
        Task<bool> DeleteBrand(Brand brand);
    }

    public class BrandRepository : IBrandRepository
    {
        private readonly DataContext _context;

        public BrandRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Brand>> ListAllBrands()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetBrandById(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
        }

        public async Task<bool> CreateBrand(Brand brand)
        {
            _context.Brands.Add(brand);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateBrand(Brand brand)
        {
            _context.Brands.Update(brand);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteBrand(Brand brand)
        {
            _context.Brands.Remove(brand);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
