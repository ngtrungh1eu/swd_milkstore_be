using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BussinessLogic.DTO;
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
    public interface IPromotionService
    {
        Task<ServiceResponse<List<PromotionDTO>>> ListAllPromotion();
        Task<ServiceResponse<PromotionModelDTO>> GetPromotionModelById(int id);
        Task<ServiceResponse<PromotionDTO>> CreatePromotion(PromotionDTO promotion);
        Task<ServiceResponse<PromotionDTO>> UpdatePromotion(int promotionId,PromotionDTO promotion);
        Task<ServiceResponse<PromotionDTO>> DeletePromotion(int id);
        Task<ServiceResponse<PromotionDTO>> AddPromotionProduct(int promotionId,int productId);

    }
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public PromotionService(IPromotionRepository productRepository, IMapper mapper,IProductRepository productRepository1)
        {
            _promotionRepository = productRepository;
            _mapper = mapper;
            _productRepository = productRepository1;
        }

        async Task<ServiceResponse<List<PromotionDTO>>> IPromotionService.ListAllPromotion()
        {
            ServiceResponse<List<PromotionDTO>> _response = new();
            try
            {
                var listPromotion = await _promotionRepository.ListAllPromotion();
                var listPromotionDto = new List<PromotionDTO>();
                foreach (var promotion in listPromotion)
                {
                    listPromotionDto.Add(_mapper.Map<PromotionDTO>(promotion));
                }
                _response.Success = true;
                _response.Data = listPromotionDto;
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

        async Task<ServiceResponse<PromotionModelDTO>> IPromotionService.GetPromotionModelById(int id)
        {
            ServiceResponse<PromotionModelDTO> _response = new();
            try
            {
                PromotionModel promotion = await _promotionRepository.GetPromotionModelById(id);
                if (promotion == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                if(promotion.EndAt < DateTime.UtcNow)
                {
                    _response.Success = false;
                    _response.Message = "Coupon expire.";
                    return _response;
                }
                List<ProductDTO> listDto = new List<ProductDTO>();
                for(int i = 0; i < promotion.Products.Count; i++)
                {
                    listDto.Add(new ProductDTO
                    {
                        ProductId = promotion.Products[i].ProductId,
                        ProductName = promotion.Products[i].ProductName,
                        ProductTitle = promotion.Products[i].ProductTitle,
                        BrandId = promotion.Products[i].BrandId??0,
                        ByAge = promotion.Products[i].ByAge??0,
                        Quantity = promotion.Products[i].Quantity ?? 0,
                        ProductImg = promotion.Products[i].ProductImg,
                        ProductPrice = promotion.Products[i].ProductPrice ?? 0,
                        PreOrderAmount = promotion.Products[i].PreOrderAmount,
                        ProductDescription = promotion.Products[i].ProductDescription,
                        isPromote = promotion.Products[i].isPromote == 1 ? true : false,
                        isPreOrder = promotion.Products[i].isPreOrder ?? false,
                    });
                }
               // var promotionDto = _mapper.Map<PromotionModelDTO>(promotion);
                var promotionDto = new PromotionModelDTO
                {
                    PromotionId = promotion.PromotionId,
                    PromotionName = promotion.PromotionName,
                    PromotionImg = promotion.PromotionImg,
                    Promote = promotion.Promote,
                    StartAt = promotion.StartAt,
                    EndAt = promotion.EndAt,
                    Products = listDto,
                };


                _response.Success = true;
                _response.Message = "OK";
                _response.Data = promotionDto;

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

        async Task<ServiceResponse<PromotionDTO>> IPromotionService.CreatePromotion(PromotionDTO request)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
             
                Promotion _newProduct = new Promotion()
                {
                    PromotionName = request.PromotionName,
                    PromotionImg = request.PromotionImg,
                    StartAt = request.StartAt,
                    EndAt = request.EndAt,
                    Promote = request.Promote,
                };

                if (!await _promotionRepository.CreatePromotion(_newProduct))
                {
                    _response.Error = "ReporError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<PromotionDTO>(_newProduct);
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

        async Task<ServiceResponse<PromotionDTO>> IPromotionService.UpdatePromotion(int promotionId, PromotionDTO request)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
               
                var existingPromotion = await _promotionRepository.GetPromotionById(promotionId);
                if (existingPromotion == null)
                {
                    _response.Success = false;
                    _response.Message = "Error";
                    _response.Data = null;
                    return _response;
                }

                existingPromotion.PromotionName = request.PromotionName;
                existingPromotion.PromotionImg = request.PromotionImg;
                existingPromotion.StartAt = request.StartAt;
                existingPromotion.EndAt = request.EndAt;
                existingPromotion.Promote = request.Promote;

                if (!await _promotionRepository.UpdatePromotion(existingPromotion))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var promotionDto = _mapper.Map<PromotionDTO>(existingPromotion);
                _response.Success = true;
                _response.Data = promotionDto;
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

        async Task<ServiceResponse<PromotionDTO>> IPromotionService.DeletePromotion(int id)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
                var existingPromotion = await _promotionRepository.GetPromotionById(id);
                if (existingPromotion == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _promotionRepository.DeletePromotion(existingPromotion))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var _producDto = _mapper.Map<PromotionDTO>(existingPromotion);
                _response.Success = true;
                _response.Data = _producDto;
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

        public async Task<ServiceResponse<PromotionDTO>> AddPromotionProduct(int promotionId, int productId)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
                Product product =  await _productRepository.GetProductById(productId);
                if(product == null)
                {
                    _response.Error = "Product not found";
                    _response.Success = false;
                    return _response;
                }
                Promotion promotion = await _promotionRepository.GetPromotionById(promotionId);
                if (promotion == null)
                {
                    _response.Error = "Promotion not found";
                    _response.Success = false;
                    return _response;
                }

                await _promotionRepository.UpdateProductPromotion(promotion, product);
                promotion = await _promotionRepository.GetPromotionById(promotionId);

                _response.Success = true;
                _response.Data = _mapper.Map<PromotionDTO>(promotion);
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
    }
}
