using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.Blog
{
    public class VoteDTO
    {
        public int VoteId { get; set; }
        public bool VoteType { get; set; }

        public int BlogId { get; set; }
        public int UserId { get; set; }
    }
}
