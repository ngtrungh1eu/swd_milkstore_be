using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.NewFolder
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int TotalItem { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
}
