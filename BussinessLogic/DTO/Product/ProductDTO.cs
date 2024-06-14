using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Product
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductImg { get; set; }
        public double ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int ByAge { get; set; }
        public bool isPreOrder { get; set; }
        public int? PreOrderAmount { get; set; }
        public bool isPromote { get; set; }
        public bool isDisable {  get; set; }
        public int BrandId { get; set; }
    }
}
