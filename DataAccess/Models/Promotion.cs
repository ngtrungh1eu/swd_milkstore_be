using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Promotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PromotionId { get; set; }

        [StringLength(50)]
        public string? PromotionName { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set;}
        public int? Promote {  get; set; }
        public string? PromotionImg {  get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
