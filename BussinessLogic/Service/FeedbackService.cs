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
using BussinessLogic.DTO.ProductDTO;
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
        Task<List<Feedback>> ListAllFeedback();
        Task<ServiceResponse<Feedback>> UpdateFeedback(FeedbackDTO feedback);
    }
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackService(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }

        async Task<ServiceResponse<Feedback>> IFeedbackService.DeleteFeedback(int id)
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
    

    async Task<ServiceResponse<Feedback>> IFeedbackService.CreateFeedback(FeedbackDTO feedbackDto)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {
                Feedback feedback = new Feedback
                {
                    OrderId = feedbackDto.OrderId,
                    Comment = feedbackDto.Comment,
                    Rate = feedbackDto.Rate,
                    ReplyId = feedbackDto.ReplyId,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                };
                if (feedbackDto.PreOrderId != null)
                {
                    feedback.PreOrderId = feedbackDto.PreOrderId;
                }

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
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return _response;
        }

        async Task<List<Feedback>> IFeedbackService.ListAllFeedback()
        {
            return await _feedbackRepository.ListAllFeedback();
        }


        async Task<ServiceResponse<Feedback>> IFeedbackService.UpdateFeedback(FeedbackDTO request)
        {
            ServiceResponse<Feedback> _response = new();
            try
            {

                var existingFeedback = await _feedbackRepository.GetFeedbackById(request.FeedbackId??0);
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

        async Task<ServiceResponse<Feedback>> IFeedbackService.GetFeedbackById(int id)
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
