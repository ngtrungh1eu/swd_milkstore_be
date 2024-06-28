using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ProductName { get; set; }
        public string Image {  get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Item in Cart quantity must exists.")]
        public int Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "UnitPrice cannot be negative.")]
        public double UnitPrice { get; set; }
    }
}
