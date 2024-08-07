﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Promotion
{
    public class PromotionDTO
    {
        public int PromotionId { get; set; }
        public string PromotionName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool Status => StartAt <= DateTime.UtcNow && DateTime.UtcNow <= EndAt;
        public int Promote { get; set; }
        public string? PromotionImg { get; set; }
    }
}
