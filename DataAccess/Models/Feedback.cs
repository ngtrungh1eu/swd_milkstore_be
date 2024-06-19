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
        // productId where? lấy thì đơn giản, vấn đề là lưu nó vào đâu? thêm fiel à, lưu cái productId cho cái feed back
        // Ở đây e có 1 có 1 danh sách sản phẩm trong cái order, em tạo feedback rồi cho cái product nào
        // 1 order - nhiều feedback => nhưng nhiều feedback lại không biết cho chính xác sản phẩm nào
        // VD: 1 order có 4 món hàng, feedback cho thằng nào cho 4 thằng đó
        // Thế lấy feedback: => Sản phẩm join Order_Product join Order join Feedback => lấy danh sách feedback à? chết query đấy
        // Vậy feedback cuối cùng thì cho thằng sản phẩm nào trong hóa đơn? lưu cái productId nào, vì order có nhiều order_product
        // ok, hiểu
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackId {  get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public int? ReplyId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int? PreOrderId { get; set; }
        public PreOrder PreOrder { get; set; }
    }
}
