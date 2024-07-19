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
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace BussinessLogic.Service
{
    public interface IFeedbackService
    {
        Task<ServiceResponse<FeedbackDTO>> CreateFeedback(FeedbackDTO feedback);
        Task<ServiceResponse<FeedbackDTO>> DeleteFeedback(int id);
        Task<ServiceResponse<FeedbackDTO>> GetFeedbackById(int id);
        Task<ServiceResponse<List<FeedbackDTO>>> ListAllFeedback(int productId);
        Task<ServiceResponse<FeedbackDTO>> UpdateFeedback(int id, int orderId, int productId, int feedbackId, FeedbackDTO feedback);
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

        public async Task<ServiceResponse<FeedbackDTO>> DeleteFeedback(int id)
        {
            ServiceResponse<FeedbackDTO> _response = new();
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackById(id);
                if (feedback == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _feedbackRepository.DeleteFeedback(feedback))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var feedbackDto = _mapper.Map<FeedbackDTO>(feedback);
                _response.Success = true;
                _response.Data = feedbackDto;
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


        public async Task<ServiceResponse<FeedbackDTO>> CreateFeedback(FeedbackDTO feedbackDto)
        {
            ServiceResponse<FeedbackDTO> _response = new();
            try
            {
                Feedback feedback = new Feedback
                {
                    Comment = feedbackDto.Comment,
                    Rate = feedbackDto.Rate,
                    CreateAt = DateTime.Now,
                    OrderId = feedbackDto.OrderId,
                    ProductId = feedbackDto.ProductId,
                    UserId = feedbackDto.UserId,
                };

                var result = await _feedbackRepository.CreateFeedback(feedback);

                if (result == null)
                {
                    _response.Message = "Repo Error ";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                var feedbackDtoResult = _mapper.Map<FeedbackDTO>(feedback);

                _response.Success = true;
                _response.Data = feedbackDtoResult;
                _response.Message = "Created";
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

        public async Task<ServiceResponse<List<FeedbackDTO>>> ListAllFeedback(int productId)
        {
            ServiceResponse<List<FeedbackDTO>> _response = new();

            try
            {
                var feedbackList = await _feedbackRepository.ListAllFeedback(productId);
                var feedbackListDto = new List<FeedbackDTO>();
                foreach (var feedback in feedbackList)
                {
                    feedbackListDto.Add(_mapper.Map<FeedbackDTO>(feedback));
                }

                _response.Success = true;
                _response.Data = feedbackListDto;
                _response.Message = "OK";
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

        public async Task<ServiceResponse<FeedbackDTO>> UpdateFeedback(int id, int orderId, int productId, int feedbackId, FeedbackDTO request)
        {
            ServiceResponse<FeedbackDTO> _response = new();
            try
            {
                var existingFeedback = await _feedbackRepository.GetFeedbackById(feedbackId);

                if (existingFeedback == null)
                {
                    _response.Success = false;
                    _response.Message = "Error";
                    _response.Data = null;
                    return _response;
                }

                if (existingFeedback.UserId != id || existingFeedback.OrderId != orderId || existingFeedback.ProductId != productId)
                {
                    _response.Success = false;
                    _response.Message = "Unauthorized";
                    _response.Data = null;
                    return _response;
                }

                existingFeedback.Comment = request.Comment;
                existingFeedback.Rate = request.Rate;
                existingFeedback.UpdateAt = DateTime.Now;

                if (!await _feedbackRepository.UpdateFeedback(id, orderId, productId, feedbackId, existingFeedback))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var feedbackDto = _mapper.Map<FeedbackDTO>(existingFeedback);

                _response.Success = true;
                _response.Data = feedbackDto;
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

        public async Task<ServiceResponse<FeedbackDTO>> GetFeedbackById(int id)
        {
            ServiceResponse<FeedbackDTO> _response = new();
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackById(id);
                if (feedback == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }

                var feedbackDto = _mapper.Map<FeedbackDTO>(feedback);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = feedbackDto;
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
