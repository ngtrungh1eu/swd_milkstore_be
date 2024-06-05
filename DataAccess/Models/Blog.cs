using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? BlogId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? BlogImg {  get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? vote { get; set; }
        [StringLength(20)]
        public string? Tags { get; set; }
    }
}
