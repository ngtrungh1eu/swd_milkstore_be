using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Comment { get; set; }
        public int? ReplyId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public int ProductId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
