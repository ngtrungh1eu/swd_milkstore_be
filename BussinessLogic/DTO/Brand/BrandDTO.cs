using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.BrandDTO
{
    public class BrandDTO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandImg { get; set; }
        public string MadeIn { get; set; }
        public string description { get; set;}
    }
}
