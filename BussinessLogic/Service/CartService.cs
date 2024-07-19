using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Cart;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface ICartService
    {
        Task<ServiceResponse<CartDTO>> GetCart(int userId);
        Task<ServiceResponse<CartDTO>> CreateCart(int userId);
        Task<ServiceResponse<CartDTO>> AddToCart(int userId, int productId, int quantity);
        Task<ServiceResponse<CartDTO>> UpdateProductQuantity(int userId, int productId, int quantity);
        Task<ServiceResponse<CartDTO>> RemoveProduct(int userId, int productId);
    }
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<CartDTO>> AddToCart(int userId, int productId, int quantity)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                if (quantity <= 0)
                {
                    _response.Success = false;
                    _response.Message = "Product quantity has to be > 0";
                    return _response;
                }

                var addProduct = await _repository.AddToCart(userId, productId, quantity);

                if (addProduct == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Message = "Product Added";
                _response.Data = _mapper.Map<CartDTO>(addProduct);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<ServiceResponse<CartDTO>> CreateCart(int userId)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var createdCart = await _repository.CreateCart(userId);

                if (createdCart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found or Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Message = "Cart Created";
                _response.Data = _mapper.Map<CartDTO>(createdCart);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { ex.Message };
            }

            return _response;
        }

        public async Task<ServiceResponse<CartDTO>> GetCart(int userId)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var cart = await _repository.GetCart(userId);
               
                if (cart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                var cartDto = _mapper.Map<CartDTO>(cart);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = cartDto;
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = "Error";
                _response.Data = null;
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<ServiceResponse<CartDTO>> RemoveProduct(int userId, int productId)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var existingCart = await _repository.GetCart(userId);

                if (existingCart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                var removeProduct = await _repository.RemoveProduct(userId, productId);

                if (removeProduct == null)
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Message = "Product Removed";
                _response.Data = _mapper.Map<CartDTO>(removeProduct);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<ServiceResponse<CartDTO>> UpdateProductQuantity(int userId, int productId, int quantity)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                if (quantity <= 0)
                {
                    _response.Success = false;
                    _response.Message = "Product quantity has to be > 0";
                    return _response;
                }

                var existingCart = await _repository.GetCart(userId);

                if (existingCart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                var updateQuantity = await _repository.UpdateProductQuantity(userId, productId, quantity);

                if (updateQuantity == null)
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Message = "Product Quantity Updated";
                _response.Data = _mapper.Map<CartDTO>(updateQuantity);
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }
    }
}
