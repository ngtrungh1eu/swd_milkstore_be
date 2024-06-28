using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Cart
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int TotalItem { get; set; }
        public double TotalPrice { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItemDTO> CartItems { get; set; }
    }
}
