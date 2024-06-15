﻿using DataAccess.Data;
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
    public interface IPromotionRepository
    {
        Task<ICollection<Promotion>> ListAllPromotion();
        Task<PromotionModel> GetPromotionModelById(int id);
        Task<bool> CreatePromotion(Promotion promotion);
        Task<bool> UpdatePromotion(Promotion promotion);
        Task<Promotion> GetPromotionById(int id);
        Task<bool> DeletePromotion(Promotion promotion);
    }

    public class PromotionRepository : IPromotionRepository
    {
        private readonly DataContext _context;
        public PromotionRepository(DataContext context)
        {
            _context = context;
        }

        async Task<ICollection<Promotion>> IPromotionRepository.ListAllPromotion()
        {
            return await _context.Promotions.ToListAsync();
        }

        async Task<bool> IPromotionRepository.CreatePromotion(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<bool> IPromotionRepository.UpdatePromotion(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        async Task<Promotion> IPromotionRepository.GetPromotionById(int id)
        {
            return await _context.Promotions.FirstOrDefaultAsync(p => p.PromotionId == id);
        }

        async Task<PromotionModel> IPromotionRepository.GetPromotionModelById(int id)
        {
            var query = from pm in _context.Promotions
                        where pm.PromotionId == id
                        select new PromotionModel
                        {
                            PromotionId = pm.PromotionId,
                            PromotionName = pm.PromotionName,
                            PromotionImg = pm.PromotionImg,
                            StartAt = pm.StartAt,
                            EndAt = pm.EndAt,
                            Promote = pm.Promote,
                        };
            PromotionModel promotionModel = await query.FirstOrDefaultAsync();

            List<ProductModel> productsOfPromotion = _context.Promotions
                .Where(p => p.PromotionId == promotionModel.PromotionId)
                .SelectMany(p => p.Products)
                .Select(p => new ProductModel
                {
                    ProductId = p.ProductId,
                    BrandId = p.BrandId,
                    ProductImg = p.ProductImg,
                    ProductName = p.ProductName,
                    Brand = p.Brand.BrandName, // Giả sử bạn có một navigation property là Brand
                    BrandImg = p.Brand.BrandImg, // Tương tự như trên
                    MadeIn = p.Brand.MadeIn, // Tương tự như trên
                    ProductTitle = p.ProductTitle,
                    ProductDescription = p.ProductDescription,
                    ByAge = p.ByAge,
                    ProductPrice = p.ProductPrice,
                    Quantity = p.Quantity,
                    isPreOrder = p.isPreOrder,
                    PreOrderAmount = p.PreOrderAmount,
                    isPromote = 1 // Bạn có thể cần thiết lập giá trị isPromote dựa trên logic cụ thể
                })
                .ToList();
            promotionModel.Products = productsOfPromotion;
            return promotionModel;
        }
        async Task<bool> IPromotionRepository.DeletePromotion(Promotion promotion)
        {
            _context.Promotions.Remove(promotion);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}