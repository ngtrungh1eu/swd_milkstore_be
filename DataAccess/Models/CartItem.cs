﻿using System;
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

        public int Quantity { get; set; }
    }
}