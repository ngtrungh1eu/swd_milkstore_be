using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntityModel
{
    public class UpdateCartAndProductsMessage
    {
        public int CartId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
