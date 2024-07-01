using BussinessLogic.DTO.User;
using BussinessLogic.DTO;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace BussinessLogic.Service
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse<TokenModel>> Login(SignInModel model);
        Task<ServiceResponse<TokenModel>> RefreshToken(TokenModel model);
        Task<ServiceResponse<UserDTO>> Register(UserDTO user);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticaionRepository _repository;
        private readonly IMapper _mapper;

        public AuthenticationService(IAuthenticaionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<TokenModel>> Login(SignInModel model)
        {
            ServiceResponse<TokenModel> _response = new();
            try
            {
                var token = await _repository.Login(model);

                if (token == null)
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Invalid Username or Password!";
                    return _response;
                }

                _response.Success = true;
                _response.Data = token;
                _response.Message = "Login Successfully!";
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

        public async Task<ServiceResponse<TokenModel>> RefreshToken(TokenModel model)
        {
            ServiceResponse<TokenModel> _response = new();

            try
            {
                var tokenModel = await _repository.RefreshToken(model);

                if (tokenModel == null)
                {
                    _response.Success = false;
                    _response.Data = null;
                    _response.Message = "Invalid refreshToken or accessToken.";
                    return _response;
                }

                _response.Success = true;
                _response.Data = tokenModel;
                _response.Message = "Token refreshed successfully.";
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

        public async Task<ServiceResponse<UserDTO>> Register(UserDTO request)
        {
            ServiceResponse<UserDTO> _response = new();

            try
            {
                User _newUser = new User()
                {
                    Username = request.Username,
                    Password = request.Password,
                    FullName = request.FullName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Address = request.Address,
                    Phone = request.Phone,
                    Email = request.Email,
                    IsDisable = false,
                    RoleId = 3
                };

                if (!await _repository.Register(_newUser))
                {
                    _response.Message = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<UserDTO>(_newUser);
                _response.Message = "User registered successfully!";
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
