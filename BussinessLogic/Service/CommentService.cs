using AutoMapper;
using BussinessLogic.DTO.Blog;
using BussinessLogic.DTO;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface ICommentService
    {

        Task<ServiceResponse<CommentDTO>> GetCommentById(int id);
        Task<ServiceResponse<CommentDTO>> CreateComment(CommentDTO request);
        Task<ServiceResponse<CommentDTO>> UpdateComment(CommentDTO request);
        Task<ServiceResponse<CommentDTO>> DeleteComment(int id);

    }
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;


        private readonly IMapper _mapper;
        public CommentService(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;

        }

        async Task<ServiceResponse<CommentDTO>> ICommentService.DeleteComment(int id)
        {
            ServiceResponse<CommentDTO> _response = new();
            try
            {
                var existingComment = await _repository.GetCommentById(id);
                if (existingComment == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _repository.DeleteComment(existingComment))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var _commentDTO = _mapper.Map<CommentDTO>(existingComment);
                _response.Success = true;
                _response.Data = _commentDTO;
                _response.Message = "Deleted";

            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Data = null;
                _response.Message = "Error";
                _response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }
            return _response; ;
        }

        async Task<ServiceResponse<CommentDTO>> ICommentService.UpdateComment(CommentDTO request)
        {
            ServiceResponse<CommentDTO> _response = new();
            try
            {
                var existingComment = await _repository.GetCommentById(request.CommentId);
                if (existingComment == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }
                existingComment.CommentId = request.CommentId;
                existingComment.Content = request.Content;
                existingComment.UpdatedDate = request.UpdatedDate;


                if (!await _repository.UpdateComment(existingComment))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var _commentDTO = _mapper.Map<CommentDTO>(existingComment);
                _response.Success = true;
                _response.Data = _commentDTO;
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

        async Task<ServiceResponse<CommentDTO>> ICommentService.CreateComment(CommentDTO request)
        {
            ServiceResponse<CommentDTO> _response = new();
            try
            {

                Comment _newComment = new Comment()
                {
                    Content = request.Content,
                    BlogId = request.BlogId,
                    CreatedDate = DateTime.Now
                };

                if (!await _repository.CreateComment(_newComment))
                {
                    _response.Error = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<CommentDTO>(_newComment);
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

        async Task<ServiceResponse<CommentDTO>> ICommentService.GetCommentById(int id)
        {
            ServiceResponse<CommentDTO> _response = new();
            try
            {
                var comment = await _repository.GetCommentById(id);
                if (comment == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                var commentdto = _mapper.Map<CommentDTO>(comment);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = commentdto;

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
