using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Blog;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IVoteService
    {
        Task<ServiceResponse<VoteDTO>> AddVote (int userId, VoteDTO response);
        Task<ServiceResponse<VoteDTO>> UpdateVote(int userId, VoteDTO response);
    }
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _repository;
        private readonly IMapper _mapper;

        public VoteService(IVoteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<VoteDTO>> AddVote(int userId, VoteDTO response)
        {
            ServiceResponse<VoteDTO> _response = new();

            try
            {
                Vote voteBlog = _mapper.Map<Vote>(response);

                if (!await _repository.AddVote(userId, voteBlog))
                {
                    _response.Message = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<VoteDTO>(voteBlog);
                _response.Message = "Voted";
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

        public async Task<ServiceResponse<VoteDTO>> UpdateVote(int userId, VoteDTO response)
        {
            ServiceResponse<VoteDTO> _response = new();
            try
            {
                var existingVote = await _repository.GetVoteById(response.VoteId);

                if (existingVote == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                Vote updatedVote = _mapper.Map<Vote>(response);

                if (!await _repository.UpdateVote(userId, updatedVote))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<VoteDTO>(existingVote);
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
