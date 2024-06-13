using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.BrandDTO;
using DataAccess.Models;
using DataAccess.Repository;

namespace BussinessLogic.Service
{
    public interface IBrandService
    {
        Task<ServiceResponse<List<BrandDTO>>> ListAllBrands();
        Task<ServiceResponse<BrandDTO>> GetBrandById(int id);
        Task<ServiceResponse<BrandDTO>> CreateBrand(BrandDTO brandDTO);
        Task<ServiceResponse<BrandDTO>> UpdateBrand(BrandDTO brandDTO);
        Task<ServiceResponse<bool>> DeleteBrand(int id);
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
                var brands = await _brandRepository.ListAllBrands();
                response.Data = _mapper.Map<List<BrandDTO>>(brands);
                response.Success = true;
                response.Message = "OK";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error occurred while retrieving brands.";
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
                    response.Message = "Brand not found.";
                    return response;
                }

                response.Data = _mapper.Map<BrandDTO>(brand);
                response.Success = true;
                response.Message = "OK";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error occurred while retrieving the brand.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> CreateBrand(BrandDTO brandDTO)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                //Brand brand = _mapper.Map<Brand>(brandDTO);
                Brand brand = new Brand
                {
                    BrandName = brandDTO.BrandName,
                    BrandImg = brandDTO.BrandImg,
                    MadeIn = brandDTO.MadeIn,
                };
                var isCreated = await _brandRepository.CreateBrand(brand);
                if (!isCreated)
                {
                    response.Success = false;
                    response.Message = "Failed to create brand.";
                    return response;
                }

                response.Data = new BrandDTO
                {
                    BrandId = brand.BrandId,
                    BrandName = brand.BrandName,
                    BrandImg = brand.BrandImg,
                    MadeIn = brand.MadeIn,
                };
                response.Success = true;
                response.Message = "Brand created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error occurred while creating the brand.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<BrandDTO>> UpdateBrand(BrandDTO brandDTO)
        {
            var response = new ServiceResponse<BrandDTO>();
            try
            {
                var brand = await _brandRepository.GetBrandById(brandDTO.BrandId);
                if (brand == null)
                {
                    response.Success = false;
                    response.Message = "Brand not found.";
                    return response;
                }

                brand.BrandName = brandDTO.BrandName;
                brand.BrandImg = brandDTO.BrandImg;
                brand.MadeIn = brandDTO.MadeIn;
                // Update other properties as needed

                var isUpdated = await _brandRepository.UpdateBrand(brand);
                if (!isUpdated)
                {
                    response.Success = false;
                    response.Message = "Failed to update brand.";
                    return response;
                }

               // response.Data = _mapper.Map<BrandDTO>(brand);
                response.Data = new BrandDTO
                {
                    BrandId = brand.BrandId,
                    BrandName = brand.BrandName,
                    BrandImg = brand.BrandImg,
                    MadeIn = brand.MadeIn,
                };
                response.Success = true;
                response.Message = "Brand updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error occurred while updating the brand.";
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteBrand(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var brand = await _brandRepository.GetBrandById(id);
                if (brand == null)
                {
                    response.Success = false;
                    response.Message = "Brand not found.";
                    response.Data = false;
                    return response;
                }

                var isDeleted = await _brandRepository.DeleteBrand(brand);
                if (!isDeleted)
                {
                    response.Success = false;
                    response.Message = "Failed to delete brand.";
                    response.Data = false;
                    return response;
                }

                response.Success = true;
                response.Message = "Brand deleted successfully.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error occurred while deleting the brand.";
                response.ErrorMessages = new List<string> { ex.Message };
                response.Data = false;
            }

            return response;
        }
    }
}
