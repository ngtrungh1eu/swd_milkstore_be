using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.BrandDTO;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Service
{
    public interface IBrandService
    {
        Task<ServiceResponse<List<BrandDTO>>> ListAllBrands();
        Task<ServiceResponse<BrandDTO>> GetBrandById(int id);
        Task<ServiceResponse<BrandDTO>> CreateBrand(BrandDTO brandDTO);
        Task<ServiceResponse<BrandDTO>> UpdateBrand(BrandDTO brandDTO);
        Task<ServiceResponse<BrandDTO>> DeleteBrand(int id);
    }

    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<BrandDTO>>> ListAllBrands()
        {
            var response = new ServiceResponse<List<BrandDTO>>();
            try
            {
                var listBrand = await _brandRepository.ListAllBrands();
                var listBrandDto = new List<BrandDTO>();
                foreach (var brand in listBrand)
                {
                    listBrandDto.Add(_mapper.Map<BrandDTO>(brand));
                }

                response.Success = true;
                response.Message = "OK";
                response.Data = listBrandDto;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.Data = null;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> GetBrandById(int id)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                var brand = await _brandRepository.GetBrandById(id);
                if (brand == null)
                {
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }

                response.Data = _mapper.Map<BrandDTO>(brand);
                response.Success = true;
                response.Message = "OK";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.Data = null;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> CreateBrand(BrandDTO brandDTO)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                Brand brand = new Brand()
                {
                    BrandName = brandDTO.BrandName,
                    BrandImg = brandDTO.BrandImg,
                    MadeIn = brandDTO.MadeIn,
                    description = brandDTO.description
                };

                if (!await _brandRepository.CreateBrand(brand))
                {
                    response.Success = false;
                    response.Message = "Repo Error";
                    response.Data = null;
                    return response;
                }

                response.Success = true;
                response.Message = "Brand created successfully.";
                response.Data = _mapper.Map<BrandDTO>(brand);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.Data = null;
                response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> UpdateBrand(BrandDTO brandDTO)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                var existingBrand = await _brandRepository.GetBrandById(brandDTO.BrandId);
                if (existingBrand == null)
                {
                    response.Success = false;
                    response.Message = "Not Found";
                    return response;
                }

                existingBrand.BrandName = brandDTO.BrandName;
                existingBrand.BrandImg = brandDTO.BrandImg;
                existingBrand.MadeIn = brandDTO.MadeIn;
                existingBrand.description = brandDTO.description;


                if (!await _brandRepository.UpdateBrand(existingBrand))
                {
                    response.Success = false;
                    response.Message = "Repo Error";
                    response.Data = null;
                    return response;
                }

                var brandDto = _mapper.Map<BrandDTO>(existingBrand);
                response.Data = brandDto;
                response.Success = true;
                response.Message = "Brand updated successfully";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.Data = null;
                response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> DeleteBrand(int id)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                var existingBrand = await _brandRepository.GetBrandById(id);
                if (existingBrand == null)
                {
                    response.Success = false;
                    response.Message = "Not Found";
                    response.Data = null;
                    return response;
                }

                if (!await _brandRepository.DeleteBrand(existingBrand))
                {
                    response.Success = false;
                    response.Message = "Repo Error";
                    response.Data = null;
                    return response;
                }

                var brandDto = _mapper.Map<BrandDTO>(existingBrand);

                response.Success = true;
                response.Message = "Brand deleted successfully.";
                response.Data = brandDto;
                response.Message = "Deleted";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.Data = null;
                response.ErrorMessages = new List<string> { Convert.ToString(ex.Message) };
            }

            return response;
        }
    }
}
