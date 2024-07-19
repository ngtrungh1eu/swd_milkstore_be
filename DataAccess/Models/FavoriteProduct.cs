using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class FavoriteProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavoriteProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int? Promote {  get; set; }

        public int FavoriteId { get; set; }
        public Favorite Favorite { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
