using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Feedback;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.Promotion;
using DataAccess.EntityModel;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace BussinessLogic.Service
{
    public interface IFeedbackService
    {
        Task<ServiceResponse<Feedback>> CreateFeedback(FeedbackDTO feedback);
        Task<ServiceResponse<Feedback>> DeleteFeedback(int id);
        Task<ServiceResponse<Feedback>> GetFeedbackById(int id);
        Task<List<Feedback>> ListAllFeedback(int? productId);
        Task<ServiceResponse<Feedback>> UpdateFeedback(FeedbackDTO feedback);
    }
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Feedback>> DeleteFeedback(int id)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {
                var f = await _feedbackRepository.GetFeedbackById(id);
                if (f == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _feedbackRepository.DeleteFeedback(f))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = f;
                _response.Message = "Deleted";
                return _response;
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


        public async Task<ServiceResponse<Feedback>> CreateFeedback(FeedbackDTO feedbackDto)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {
                var order = await _orderRepository.GetOrderById(feedbackDto.OrderId);
                
                if (order == null)
                {
                    _response.Error = "Order is not existed";
                    _response.Success = false;
                    return _response;
                }
                if (order.Status != "completed")
                {
                    _response.Error = $"Order is {order.Status}, can't not send feedback";
                    _response.Success = false;
                    return _response;
                }
                if (!order.ProductOrders.Any(x => x.ProductId == feedbackDto.ProductId))
                {
                    // Cái này là tránh trường hợp nó mua sản phẩm A nhưng đi feedback cho sản phẩm B nhé
                    _response.Error = $"This order can't has product: {feedbackDto.ProductId}!";
                    _response.Success = false;
                    return _response;
                }
                //fix
                if (feedbackDto.Rate < 1 || feedbackDto.Rate > 5)
                {
                    _response.Error = "Rate has to be between 1 and 5";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;

                }

                Feedback feedback = new Feedback
                {
                    Comment = feedbackDto.Comment,
                    Rate = feedbackDto.Rate,
                    ReplyId = order.UserId,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    OrderId = feedbackDto.OrderId,
                    ProductId = feedbackDto.ProductId,
                    UserId = order.UserId,
                };
               
                if (!await _feedbackRepository.CreateFeedback(feedback))
                {
                    _response.Error = "ReporError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }
                
                _response.Success = true;
                _response.Data = feedback;
                _response.Message = "Created";
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Product have feedback";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        public async Task<List<Feedback>> ListAllFeedback(int? productId)
        {
            if (productId.HasValue)
                return (await _feedbackRepository.ListAllFeedback()).Where(x => x.ProductId == productId).ToList();
            return await _feedbackRepository.ListAllFeedback();
        }


        public async Task<ServiceResponse<Feedback>> UpdateFeedback(FeedbackDTO request)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {

                var existingFeedback = await _feedbackRepository.GetFeedbackById(request.FeedbackId);
                if (existingFeedback == null)
                {
                    _response.Success = false;
                    _response.Message = "Error";
                    _response.Data = null;
                    return _response;
                }

                existingFeedback.Comment = request.Comment;
                existingFeedback.Rate = request.Rate;
                existingFeedback.UpdateAt = DateTime.Now;

                if (!await _feedbackRepository.UpdateFeedback(existingFeedback))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = existingFeedback;
                _response.Message = "Updated";
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

        public async Task<ServiceResponse<Feedback>> GetFeedbackById(int id)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {
                Feedback f = await _feedbackRepository.GetFeedbackById(id);
                

                _response.Success = true;
                _response.Message = "OK";
                _response.Data = f;
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
    }
}
