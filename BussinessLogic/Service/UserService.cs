using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Product;
using BussinessLogic.DTO.User;
using DataAccess.Models;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Service
{
    public interface IUserService
    {
        Task<ServiceResponse<List<UserModel>>> GetAllUser();
        Task<ServiceResponse<UserModel>> GetUserById(int id);
        Task<ServiceResponse<UserModel>> CreateStaff(UserModel request);
        Task<ServiceResponse<UserModel>> DisableUser(int id);
        Task<ServiceResponse<UserModel>> DeleteUser(int id);
        Task<ServiceResponse<AccountModel>> UpdateAccount(AccountModel account);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<UserModel>>> GetAllUser()
        {
            ServiceResponse<List<UserModel>> _response = new();

            try
            {
                var listAccount = await _repository.GetAllUser();
                var listAccountDto = new List<UserModel>();
                foreach (var account in listAccount)
                {
                    listAccountDto.Add(_mapper.Map<UserModel>(account));
                }
                _response.Success = true;
                _response.Data = listAccountDto;
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

        public async Task<ServiceResponse<UserModel>> GetUserById(int id)
        {
            ServiceResponse<UserModel> _response = new();

            try
            {
                var account = await _repository.GetUserById(id);
                if (account == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }

                var accountDto = _mapper.Map<UserModel>(account);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = accountDto;
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

        public async Task<ServiceResponse<UserModel>> CreateStaff(UserModel request)
        {
            ServiceResponse<UserModel> _response = new();
            try
            {
                User _newStaff = new User()
                {
                    Username = request.Username,
                    Password = request.Password,
                    FullName = request.FullName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Address = request.Address,
                    Phone = request.Phone,
                    Image = request.Image,
                    Email = request.Email,
                    status = "Available",
                    IsDisable = false,
                    RoleId = 2,
                };

                if (!await _repository.CreateStaff(_newStaff))
                {
                    _response.Message = "Repo Error";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<UserModel>(_newStaff);
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

        public async Task<ServiceResponse<UserModel>> DeleteUser(int id)
        {
            ServiceResponse<UserModel> _response = new();
            try
            {
                var existingAccount = await _repository.GetUserById(id);
                if (existingAccount == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _repository.DeleteUser(existingAccount))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var account = _mapper.Map<UserModel>(existingAccount);
                _response.Success = true;
                _response.Data = account;
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

        public async Task<ServiceResponse<UserModel>> DisableUser(int id)
        {
            ServiceResponse<UserModel> _response = new();
            try
            {
                var existingUser = await _repository.GetUserById(id);
                if (existingUser == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _repository.DisableUser(id))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var disableUser = await _repository.GetUserById(id);
                _response.Success = true;
                _response.Data = _mapper.Map<UserModel>(disableUser);
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

        public async Task<ServiceResponse<AccountModel>> UpdateAccount(AccountModel account)
        {
            ServiceResponse<AccountModel> _response = new();
            try
            {
                var existingAccount = await _repository.GetUserById(account.Id);

                if (existingAccount == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                existingAccount.Password = account.Password;
                existingAccount.FullName = account.FullName;
                existingAccount.DateOfBirth = account.DateOfBirth;
                existingAccount.Gender = account.Gender;
                existingAccount.Address = account.Address;
                existingAccount.Phone = account.Phone;
                existingAccount.Email = account.Email;
                existingAccount.Image = account.Image;

                if (!await _repository.UpdateAccount(existingAccount))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var updateAccount = _mapper.Map<AccountModel>(existingAccount);
                _response.Success = true;
                _response.Data = updateAccount;
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
