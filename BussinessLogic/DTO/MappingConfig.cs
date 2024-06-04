using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.ProductDTO;

namespace BussinessLogic.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<DataAccess.Models.Product, BussinessLogic.DTO.Product.ProductDTO>().ReverseMap();
            CreateMap<DataAccess.EntityModel.ProductModel, ProductModelDTO>().ReverseMap();
        }
        
    }
}
