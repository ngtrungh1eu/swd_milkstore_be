﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.ProductDTO
{
    public class ProductModelDTO
    {
        public int ProductId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductPromoteId { get; set; }
        public string? ProductImg { get; set; }
        public string? ProductName { get; set; }
        public string? Brand { get; set; }
        public string? BrandImg { get; set; }
        public string? MadeIn { get; set; }
        public string? ProductTitle { get; set; }
        public string? ProductDescription { get; set; }
        public int? ByAge { get; set; }
        public double? ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public int? Rate { get; set; }
        public bool? isPreOrder { get; set; }
        public int? PreOrderAmount { get; set; }
        public int? isPromote { get; set; }
    }
}
