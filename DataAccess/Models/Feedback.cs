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
        public int FeedbackId {  get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public int ReplyId { get; set; }
        public DateTime CreateAT { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
