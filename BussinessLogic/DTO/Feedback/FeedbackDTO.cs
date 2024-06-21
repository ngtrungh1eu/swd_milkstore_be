using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Feedback
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        [Range(1, 5)]
        public int Rate { get; set; }
        public string Comment { get; set; }
        public int? ReplyId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
}
