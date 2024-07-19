using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public int? StaffId { get; set; }
        public string DeliverAddress { get; set; }
        public string Phone {  get; set; }
        public string FullName { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate {  get; set; }
        public double TotalPrice { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
    }
}
