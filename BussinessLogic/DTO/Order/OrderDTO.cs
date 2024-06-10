using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Order
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int? StaffId { get; set; }
        public string DeliverAddress { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }

        public int UserId { get; set; }
    }
}
