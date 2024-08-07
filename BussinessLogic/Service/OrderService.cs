﻿using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Order;
using BussinessLogic.DTO.Product;
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
        Task<ServiceResponse<List<OrderDTO>>> CreateOrder(int cartId);
        Task<ServiceResponse<OrderDTO>> UpdateProcess(int id);
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

        public async Task<ServiceResponse<OrderDTO>> CancelOrder(int id)
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

                if (existingOrder != null && existingOrder.Status == "Processing")
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

        public async Task<ServiceResponse<List<OrderDTO>>> CreateOrder(int cartId)
        {
            ServiceResponse<List<OrderDTO>> _response = new();
            try
            {
                var createdOrders = await _repository.CreateOrder(cartId);

                if (createdOrders == null || !createdOrders.Any())
                {
                    _response.Message = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<List<OrderDTO>>(createdOrders);
                _response.Message = "Orders Created Successfully";
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

        public async Task<ServiceResponse<OrderDTO>> GetOrderById(int id)
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

        public async Task<ServiceResponse<List<OrderDTO>>> GetOrderList()
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

        public async Task<ServiceResponse<OrderDTO>> UpdateProcess(int id)
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

                if (!await _repository.UpdateProcess(id))
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Repo Error";
                    return _response;
                }

                if (existingOrder != null && existingOrder.Status == "Cancelled")
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Order has been cancelled!";
                    return _response;
                }

                if (existingOrder != null && existingOrder.Status != "Cancelled")
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
