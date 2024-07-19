using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set;}

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
