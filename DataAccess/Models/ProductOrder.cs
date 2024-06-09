using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ProductOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductOrderId { get; set; }

        public int OrderId {  get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product {  get; set; }
        
        public int Quantity { get; set; }
        public double? UnitPrice { get; set; }
    }
}
