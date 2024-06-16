using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Favorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavoriteId { get; set; }
        public bool isAvailable { get; set; }
        public bool isPromote { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<FavoriteProduct> FavoriteProducts { get; set; }
    }
}
