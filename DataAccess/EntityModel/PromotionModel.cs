using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntityModel
{
    public class PromotionModel
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Promote { get; set; }
        public string? PromotionImg { get; set; }
        public List<ProductModel> Products { get; set; }
    }
}
