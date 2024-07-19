using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Blog;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IBlogService
    {
        Task<ServiceResponse<ICollection<BlogDTO>>> GetAllBlog();
        Task<ServiceResponse<BlogDTO>> GetBlogById(int id);
        Task<ServiceResponse<ICollection<CommentDTO>>> GetCommentByBlog(int blogId);
        Task<ServiceResponse<BlogDTO>> CreateBlog(BlogDTO blog);
        Task<ServiceResponse<BlogDTO>> UpdateBlog(BlogDTO blog);
        Task<ServiceResponse<BlogDTO>> DeleteBlog(int id);
    }
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public BlogService(IBlogRepository blogRepository, ICommentRepository commentRepository, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<BlogDTO>> CreateBlog(BlogDTO request)
        {
            ServiceResponse<BlogDTO> _response = new();
            try
            {

                Blog _newBlog = new Blog()
                {
                    Title = request.Title,
                    BlogImg = request.BlogImg,
                    Content = request.Content,
                    CreateAt = DateTime.Now,
                    Tags = request.Tags,
                    UserId = request.UserId
                };

                if (!await _blogRepository.CreateBlog(_newBlog))
                {
                    _response.Success = false;
                    _response.Error = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                // Retrieve the vote counts after creating the blog
                _newBlog.UsefulVote = await _blogRepository.CountUseful(_newBlog.BlogId);
                _newBlog.NotUsefulVote = await _blogRepository.CountUseful(_newBlog.BlogId);

                _response.Success = true;
                _response.Data = _mapper.Map<BlogDTO>(_newBlog);
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

        public async Task<ServiceResponse<BlogDTO>> DeleteBlog(int id)
        {
            ServiceResponse<BlogDTO> _response = new();

            try
            {
                var existingBlog = await _blogRepository.GetBlogById(id);

                var existingComment = await _commentRepository.GetCommentByBlog(id);

                if (existingBlog == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _blogRepository.DeleteBlog(existingBlog) && !await _commentRepository.DeleteComment((Comment)existingComment))
                {
                    _response.Success = false;
                    _response.Error = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<BlogDTO>(existingBlog);
                _response.Message = "Deleted";
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

        public async Task<ServiceResponse<ICollection<BlogDTO>>> GetAllBlog()
        {
            ServiceResponse<ICollection<BlogDTO>> _response = new();
            try
            {
                var ListBlog = await _blogRepository.GetAllBlog();

                var ListBlogDTO = new List<BlogDTO>();

                foreach (var blog in ListBlog)
                {
                    ListBlogDTO.Add(_mapper.Map<BlogDTO>(blog));
                }

                _response.Success = true;
                _response.Message = "OK";
                _response.Data = ListBlogDTO;
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

        public async Task<ServiceResponse<BlogDTO>> GetBlogById(int id)
        {
            ServiceResponse<BlogDTO> _response = new();
            try
            {
                var blog = await _blogRepository.GetBlogById(id);

                if (blog == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }

                _response.Success = true;
                _response.Message = "OK";
                _response.Data = _mapper.Map<BlogDTO>(blog);
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

        public async Task<ServiceResponse<ICollection<CommentDTO>>> GetCommentByBlog(int blogId)
        {
            ServiceResponse<ICollection<CommentDTO>> _response = new();
            try
            {
                var blog = await _blogRepository.GetBlogById(blogId);
                var comments = await _commentRepository.GetCommentByBlog(blogId);
                var commentDtos = new List<CommentDTO>();
                foreach (var comment in comments)
                {
                    commentDtos.Add(_mapper.Map<CommentDTO>(comment));
                    _response.Success = true;
                    _response.Message = "OK";
                    _response.Data = commentDtos;
                }


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

        public async Task<ServiceResponse<BlogDTO>> UpdateBlog(BlogDTO blog)
        {
            ServiceResponse<BlogDTO> _response = new();
            try
            {
                var existingBlog = await _blogRepository.GetBlogById(blog.BlogId);

                if (existingBlog == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                Blog updatedBlog = _mapper.Map<Blog>(blog);

                if (!await _blogRepository.UpdateBlog(updatedBlog))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<BlogDTO>(updatedBlog);
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
    }
}
