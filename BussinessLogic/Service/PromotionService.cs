using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Product;
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
        Task<ServiceResponse<PromotionDTO>> UpdatePromotion(int promotionId, PromotionDTO promotion);
        Task<ServiceResponse<PromotionDTO>> DeletePromotion(int id);
        Task<ServiceResponse<PromotionDTO>> AddPromotionProduct(int promotionId, int productId);
        Task<ServiceResponse<PromotionDTO>> RemoveProductFromPromotion(int promotionId, int productId);
    }
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public PromotionService(IPromotionRepository productRepository, IMapper mapper, IProductRepository productRepository1)
        {
            _promotionRepository = productRepository;
            _mapper = mapper;
            _productRepository = productRepository1;
        }

        public async Task<ServiceResponse<List<PromotionDTO>>> ListAllPromotion()
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

        public async Task<ServiceResponse<PromotionModelDTO>> GetPromotionModelById(int id)
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

                
                
                List<ProductDTO> listDto = new List<ProductDTO>();
                foreach (var products in promotion.Products)
                {
                    var product = await _productRepository.GetProductById(products.ProductId);
                        listDto.Add(new ProductDTO
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            BrandId = product.BrandId,
                            ByAge = product.ByAge,
                            Quantity = product.Quantity,
                            ProductImg = product.ProductImg,
                            ProductPrice = product.ProductPrice,
                            PreOrderAmount = product.PreOrderAmount,
                            ProductDescription = product.ProductDescription,
                            isPromote = product.isPromote,
                            isPreOrder = product.isPreOrder,
                            ProductBrand = product.Brand.BrandName,
                            Rate = product.Rate,
                            Discount = product.Discount,

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

                //kiem tra qua han 
                if (promotion.EndAt < DateTime.UtcNow)
                {
                    _response.Success = false;
                    _response.Message = "Coupon expire.";
                    return _response;
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

        public async Task<ServiceResponse<PromotionDTO>> CreatePromotion(PromotionDTO request)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
                if (request.Promote < 0 || request.Promote > 100)
                {
                    _response.Success = false;
                    _response.Message = "Promotion value must be between 0 and 100.";
                    _response.Data = null;
                    return _response;
                }

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(request);
                if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
                {
                    _response.Success = false;
                    _response.Message = "Validation Error";
                    _response.ErrorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    return _response;
                }
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

        public async Task<ServiceResponse<PromotionDTO>> UpdatePromotion(int promotionId, PromotionDTO request)
        {
            ServiceResponse<PromotionDTO> _response = new();
            try
            {
                if (request.Promote < 0 || request.Promote > 100)
                {
                    _response.Success = false;
                    _response.Message = "Promotion value must be between 0 and 100.";
                    _response.Data = null;
                    return _response;
                }

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(request);
                if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
                {
                    _response.Success = false;
                    _response.Message = "Validation Error";
                    _response.ErrorMessages = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    return _response;
                }

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

        public async Task<ServiceResponse<PromotionDTO>> DeletePromotion(int id)
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
                Product product = await _productRepository.GetProductById(productId);
                if (product == null)
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
                if(promotion.ProductPromotes != null)
                {
                    var existingProduct = promotion.ProductPromotes.FirstOrDefault(p => p.ProductId == productId);
                    if (existingProduct != null)
                    {
                        _response.Success = false;
                        _response.Data = null;
                        _response.Message = "Product was existed in promotion";
                    }
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

        public async Task<ServiceResponse<PromotionDTO>> RemoveProductFromPromotion(int promotionId, int productId)
        {
            ServiceResponse<PromotionDTO> response = new();
            try
            {
                Promotion promotion = await _promotionRepository.GetPromotionById(promotionId);
                if (promotion == null)
                {
                    response.Success = false;
                    response.Message = "Promotion not found";
                    return response;
                }

                await _promotionRepository.RemoveProductFromPromotion(promotionId, productId);

                promotion = await _promotionRepository.GetPromotionById(promotionId);
                response.Data = _mapper.Map<PromotionDTO>(promotion);
                response.Success = true;
                response.Message = "Product removed from promotion";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { ex.Message };
            }
            return response;
        }

    }
}
