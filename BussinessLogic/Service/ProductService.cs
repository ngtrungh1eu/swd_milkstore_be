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
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace BussinessLogic.Service
{
    public interface IProductService
    {
        Task<ServiceResponse<List<ProductDTO>>> ListAllProduct();
        Task<ServiceResponse<List<ProductDTO>>> GetListProductPromotion();
        Task<ServiceResponse<ProductModelDTO>> GetProductModelById(int id);
        Task<ServiceResponse<ProductDTO>> GetProductById(int id);
        Task<Product> GetProductEntityById(int id);
        Task<ServiceResponse<ProductDTO>> CreateProduct(ProductDTO product);
        Task<ServiceResponse<ProductDTO>> UpdateProduct(ProductDTO product);
        Task<ServiceResponse<ProductDTO>> DeleteProduct(int id);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        async Task<ServiceResponse<ProductDTO>> IProductService.CreateProduct(ProductDTO request)
        {
            ServiceResponse<ProductDTO> _response = new();
            try
            {
                Product _newProduct = new Product()
                {
                    ProductName = request.ProductName,
                    ProductImg = request.ProductImg,
                    ProductTitle = request.ProductTitle,
                    ProductDescription = request.ProductDescription,
                    ByAge = request.ByAge,
                    ProductPrice = request.ProductPrice,
                    Quantity = request.Quantity,
                    isPreOrder = request.isPreOrder,
                    PreOrderAmount = request.PreOrderAmount,
                    isPromote = request.isPromote,
                    BrandId = request.BrandId,
                };

                if (!await _productRepository.CreateProduct(_newProduct))
                {
                    _response.Error = "ReporError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<ProductDTO>(_newProduct);
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

        async Task<ServiceResponse<ProductDTO>> IProductService.DeleteProduct(int id)
        {
            ServiceResponse<ProductDTO> _response = new();
            try
            {
                var existingProduct = await _productRepository.GetProductById(id);
                if (existingProduct == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                if (!await _productRepository.DeleteProduct(existingProduct))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var _producDto = _mapper.Map<ProductDTO>(existingProduct);
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

        async Task<ServiceResponse<ProductDTO>> IProductService.GetProductById(int id)
        {
            ServiceResponse<ProductDTO> _response = new();
            try
            {
                var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                var prodcutDto = _mapper.Map<ProductDTO>(product);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = prodcutDto;

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

        async Task<ServiceResponse<ProductModelDTO>> IProductService.GetProductModelById(int id)
        {
            ServiceResponse<ProductModelDTO> _response = new();
            try
            {
                var product = await _productRepository.GetProductModelById(id);
                if (product == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    return _response;
                }
                var productDto = _mapper.Map<ProductModelDTO>(product);
                _response.Success = true;
                _response.Message = "OK";
                _response.Data = productDto;

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

        async Task<ServiceResponse<List<ProductDTO>>> IProductService.ListAllProduct()
        {
            ServiceResponse<List<ProductDTO>> _response = new();
            try
            {
                var listProduct = await _productRepository.ListAllProduct();
                var listProductDto = new List<ProductDTO>();
                foreach (var product in listProduct)
                {
                    listProductDto.Add(_mapper.Map<ProductDTO>(product));
                }
                _response.Success = true;
                _response.Data = listProductDto;
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

        async Task<ServiceResponse<List<ProductDTO>>> IProductService.GetListProductPromotion()
        {
            ServiceResponse<List<ProductDTO>> _response = new();
            try
            {
                var listProduct = await _productRepository.GetListProductPromotion();
                var listProductDto = new List<ProductDTO>();
                foreach (var product in listProduct)
                {
                    listProductDto.Add(_mapper.Map<ProductDTO>(product));
                }
                _response.Success = true;
                _response.Data = listProductDto;
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

        async Task<ServiceResponse<ProductDTO>> IProductService.UpdateProduct(ProductDTO request)
        {
            ServiceResponse<ProductDTO> _response = new();
            try
            {
                var existingProduct = await _productRepository.GetProductById(request.ProductId);
                if (existingProduct == null)
                {
                    _response.Success = false;
                    _response.Message = "Error";
                    _response.Data = null;
                    return _response;
                }

                existingProduct.ProductName = request.ProductName;
                existingProduct.ProductImg = request.ProductImg;
                existingProduct.ProductTitle = request.ProductTitle;
                existingProduct.ProductTitle = request.ProductTitle;
                existingProduct.ProductDescription = request.ProductDescription;
                existingProduct.ByAge = request.ByAge;
                existingProduct.ProductPrice = request.ProductPrice;
                existingProduct.Quantity = request.Quantity;
                existingProduct.isPreOrder = request.isPreOrder;
                existingProduct.PreOrderAmount = request.PreOrderAmount;
                existingProduct.isPromote = request.isPromote;
                existingProduct.BrandId = request.BrandId;


                if (!await _productRepository.UpdateProduct(existingProduct))
                {
                    _response.Success = false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var productDto = _mapper.Map<ProductDTO>(existingProduct);
                _response.Success = true;
                _response.Data = productDto;
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

        async Task<Product> IProductService.GetProductEntityById(int id)
        {
            return await _productRepository.GetProductById(id);
        }
    }
}
