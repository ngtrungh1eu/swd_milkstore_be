using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.ProductDTO;
using BussinessLogic.DTO.Order;

namespace BussinessLogic.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<DataAccess.Models.Product, Product.ProductDTO>().ReverseMap();
            CreateMap<DataAccess.EntityModel.ProductModel, ProductModelDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Order, OrderDTO>().ReverseMap();
        }
        
    }
}
