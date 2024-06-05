using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int StaffId { get; set; }
        public string DeliverAddress { get; set; }
        public string Phone {  get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime Date {  get; set; }
        public double TotalPrice { get; set; }
    }
}
