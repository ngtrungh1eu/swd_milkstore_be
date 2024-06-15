using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.ProductDTO;
using BussinessLogic.DTO.Promotion;
using DataAccess.EntityModel;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace BussinessLogic.Service
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task UpdateFavoriteUser(User user, Product product);
        Task<List<User>> ListAllUser();
        Task DeleteFavorite(User user, Product product);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        async Task<List<User>> IUserService.ListAllUser()
        {
            return await _userRepository.ListAllUser();
        }

        async Task<User> IUserService.GetUserById(int id)
        {
            var u = await _userRepository.GetUserById(id);
            return u;
        }

        async Task IUserService.UpdateFavoriteUser(User user, Product product)
        {
            await _userRepository.UpdateFavoriteUser(user, product);
        }

        async Task IUserService.DeleteFavorite(User user, Product product)
        {
            await _userRepository.DeleteFavorite(user, product);
        }
    }
}
