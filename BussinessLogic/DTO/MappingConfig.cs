using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using BussinessLogic.DTO.User;
using BussinessLogic.DTO.Order;
using BussinessLogic.DTO.Cart;

namespace BussinessLogic.DTO
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<DataAccess.Models.User, UserDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Product, Product.ProductDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Order, OrderDTO>().ReverseMap();
            CreateMap<ProductOrder, ProductOrderDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Cart, CartDTO>().ReverseMap();
            CreateMap<CartItem, CartItemDTO>().ReverseMap();
        }
        
    }
}
