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
using BussinessLogic.DTO.Promotion;

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

            CreateMap<Brand, BussinessLogic.DTO.BrandDTO.BrandDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Promotion, PromotionDTO>().ReverseMap();
            CreateMap<DataAccess.Models.Feedback, Feedback.FeedbackDTO>().ReverseMap();
        }
        
    }
}
