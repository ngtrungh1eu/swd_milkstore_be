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
        Task<ServiceResponse<CartDTO>> GetCart(int cartId);
        Task<ServiceResponse<CartDTO>> CreateCart(int userId);
        Task<ServiceResponse<CartDTO>> AddToCart(int cartId, int productId, int quantity);
        Task<ServiceResponse<CartDTO>> UpdateProductQuantity(int cartId, int productId, int quantity);
        Task<ServiceResponse<CartDTO>> RemoveProduct(int cartId, int productId);
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

        async Task<ServiceResponse<CartDTO>> ICartService.AddToCart(int cartId, int productId, int quantity)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var addProduct = await _repository.AddToCart(cartId, productId, quantity);

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

        async Task<ServiceResponse<CartDTO>> ICartService.CreateCart(int userId)
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

        async Task<ServiceResponse<CartDTO>> ICartService.GetCart(int cartId)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var cart = await _repository.GetCart(cartId);
               
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

        async Task<ServiceResponse<CartDTO>> ICartService.RemoveProduct(int cartId, int productId)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var existingCart = await _repository.GetCart(cartId);

                if (existingCart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                var removeProduct = await _repository.RemoveProduct(cartId, productId);

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

        async Task<ServiceResponse<CartDTO>> ICartService.UpdateProductQuantity(int cartId, int productId, int quantity)
        {
            ServiceResponse<CartDTO> _response = new();
            try
            {
                var existingCart = await _repository.GetCart(cartId);

                if (existingCart == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                var updateQuantity = await _repository.UpdateProductQuantity(cartId, productId, quantity);

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
