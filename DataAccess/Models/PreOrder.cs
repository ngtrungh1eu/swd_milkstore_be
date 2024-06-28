using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PreOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PreOrderId { get; set; }
        public int? StaffId { get; set; }
        public string DeliverAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string PreOrderDate { get; set; }
        public int Quantity { get; set; }
        public double? UnitPrice { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public virtual Feedback Feedback { get; set; }
    }
}
