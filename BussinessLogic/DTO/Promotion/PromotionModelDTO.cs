using BussinessLogic.DTO.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Promotion
{
    public class PromotionModelDTO
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public bool Status => StartAt <= DateTime.UtcNow && DateTime.UtcNow <= EndAt;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Promote { get; set; }
        public string? PromotionImg { get; set; }
        public List<Product.ProductDTO> Products { get; set; }
        //public List<ProductDTO> Products { get; set; }
    }
}

