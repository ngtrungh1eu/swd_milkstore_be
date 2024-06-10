using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Order;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.ProductDTO;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IOrderService
    {
        Task<ServiceResponse<List<OrderDTO>>> GetOrderList();
        Task<ServiceResponse<OrderDTO>> GetOrderById(int id);
        Task<ServiceResponse<OrderDTO>> CreateOrder(int cartId);
        Task<ServiceResponse<OrderDTO>> UpdateProcess(int id, string status);
        Task<ServiceResponse<OrderDTO>> CancelOrder(int id);
    }
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        async Task<ServiceResponse<OrderDTO>> IOrderService.CancelOrder(int id)
        {
            ServiceResponse<OrderDTO> _response = new();
            try
            {
                var existingOrder = await _repository.GetOrderById(id);
                if (existingOrder == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }
                
                if(!await _repository.CancelOrder(id))
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Repo Error";
                    return _response;
                }

                if (existingOrder != null && existingOrder.Status == "Đang Xác Nhận")
                {
                    var cancelledOrder = await _repository.GetOrderById(id);
                    _response.Success = true;
                    _response.Data = _mapper.Map<OrderDTO>(cancelledOrder);
                    _response.Message = "Cancelled";
                }
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

        async Task<ServiceResponse<OrderDTO>> IOrderService.CreateOrder(int cartId)
        {
            ServiceResponse<OrderDTO> _response = new();
            try
            {
                bool isCreated = await _repository.CreateOrder(cartId);

                if (!isCreated)
                {
                    _response.Error = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                // Fetch the created order to return it in the response
                var createdOrder = await _repository.CreateOrder(cartId);

                if (createdOrder == null)
                {
                    _response.Error = "Order Not Found";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<OrderDTO>(createdOrder);
                _response.Message = "Order Created Successfully";
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

        async Task<ServiceResponse<OrderDTO>> IOrderService.GetOrderById(int id)
        {
            ServiceResponse<OrderDTO> _response = new();
            try
            {
                var order = await _repository.GetOrderById(id);
                if (order == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                var orderDto = _mapper.Map<OrderDTO>(order);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = orderDto;

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

        async Task<ServiceResponse<List<OrderDTO>>> IOrderService.GetOrderList()
        {
            ServiceResponse<List<OrderDTO>> _response = new();
            try
            {
                var orderList = await _repository.GetOrderList();
                var orderListDto = new List<OrderDTO>();
                foreach (var order in orderList)
                {
                    orderListDto.Add(_mapper.Map<OrderDTO>(order));
                }
                _response.Success = true;
                _response.Data = orderListDto;
                _response.Message = "OK";
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = "Error";
                _response.Data = null;
                _response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
            }
            return _response;
        }

        async Task<ServiceResponse<OrderDTO>> IOrderService.UpdateProcess(int id, string status)
        {
            ServiceResponse<OrderDTO> _response = new();
            try
            {
                var existingOrder = await _repository.GetOrderById(id);
                if (existingOrder == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _repository.UpdateProcess(id, status))
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Repo Error";
                    return _response;
                }

                if (existingOrder != null && existingOrder.Status != "Đã Hủy")
                {
                    var updateProcess = await _repository.GetOrderById(id);
                    _response.Success = true;
                    _response.Data = _mapper.Map<OrderDTO>(updateProcess);
                    _response.Message = "Updated";
                }
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
