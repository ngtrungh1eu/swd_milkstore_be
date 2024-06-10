﻿using System;
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
        public string? ProductTitle { get; set; }
        public string? ProductDescription { get; set; }
        public string ProductImg {  get; set; }
        public double ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int ByAge { get; set; }
        public bool? isPreOrder { get; set; }
        public int? PreOrderAmount { get; set; }
        public bool? isPromote { get; set; }
        
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public PreOrder PreOrder { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Promotion> Promotes { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}