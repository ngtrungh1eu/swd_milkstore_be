using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }
        [NotMapped]
        public string? ProductBrand { get; set; }
        [NotMapped]
        public double? Rate {  get; set; }
        public int Discount { get; set; }
        public string? ProductDescription { get; set; }
        public string ProductImg {  get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Product price cannot be negative.")]
        public double ProductPrice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Product quantity cannot be negative.")]
        public int? Quantity { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Age cannot be negative.")]
        public int ByAge { get; set; }
        public bool isPreOrder { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Pre-Order amount cannot be negative.")]
        public int? PreOrderAmount { get; set; }
        public bool isPromote { get; set; }
        public bool isDisable { get; set; }
        
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        public virtual ICollection<FavoriteProduct> FavoriteProducts { get; set;}
        public virtual ICollection<ProductPromote> ProductPromotes { get; set;}
    }
}
