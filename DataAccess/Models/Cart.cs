using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        public int TotalItem { get; set; }
        public int TotalPrice { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
