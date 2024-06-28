using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }
        [StringLength(50)]
        public string BrandName { get; set; }
        public string? BrandImg {  get; set; }
        public string? MadeIn { get; set; }
        public string description { get; set; }
        public virtual ICollection<Product> Products  { get; set; }
    }
}
