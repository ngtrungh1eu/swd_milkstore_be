using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BussinessLogic.DTO;
using BussinessLogic.DTO.Product;
using DataAccess.Models;
using DataAccess.Repository;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace BussinessLogic.Service
{
    public interface IProductService
    {
        Task<ServiceResponse<List<ProductDTO>>> ListAllProduct();
        Task<ServiceResponse<List<ProductDTO>>> DisplayProduct();
        Task<ServiceResponse<ProductDTO>> GetProductById(int id);
        Task<ServiceResponse<ProductModel>> CreateProduct(ProductModel product);
        Task<ServiceResponse<ProductModel>> UpdateProduct(ProductModel product);
        Task<ServiceResponse<ProductDTO>> DisableProduct(int id);
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

        public async Task<ServiceResponse<ProductModel>> CreateProduct(ProductModel request)
        {
            ServiceResponse<ProductModel> _response = new();
            try
            {
                if (request.ProductPrice < 0 || request.Quantity < 0 || request.ByAge < 0 || request.PreOrderAmount < 0)
                {
                    _response.Success = false;
                    _response.Message = "Negative value does not allowed";
                    return _response;
                }
                if(request.Discount > 100 || request.Discount < 0)
                {
                    _response.Success = false;
                    _response.Message = "Discount can't more 100 or less 0";
                    return _response;
                }
                Product _newProduct = new Product()
                {
                    ProductName = request.ProductName,
                    ProductImg = request.ProductImg,
                    Discount = request.Discount,
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
                    _response.Message = "RepoError";
                    _response.Success = false;
                    _response.Data = null;
                    return _response;
                }

                _response.Success = true;
                _response.Data = _mapper.Map<ProductModel>(_newProduct);
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

        public async Task<ServiceResponse<ProductDTO>> DeleteProduct(int id)
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

        public async Task<ServiceResponse<ProductDTO>> DisableProduct(int id)
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

                if (!await _productRepository.DisableProduct(id))
                {
                    _response.Success=false;
                    _response.Message = "Repo Error";
                    _response.Data = null;
                    return _response;
                }

                var disableProduct = await _productRepository.GetProductById(id);
                _response.Success = true;
                _response.Data = _mapper.Map<ProductDTO>(disableProduct);
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

        public async Task<ServiceResponse<List<ProductDTO>>> DisplayProduct()
        {
            ServiceResponse<List<ProductDTO>> _response = new();
            try
            {
                var listProduct = await _productRepository.DisplayProduct();
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
            return _response; ;
        }

        public async Task<ServiceResponse<ProductDTO>> GetProductById(int id)
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

        public async Task<ServiceResponse<List<ProductDTO>>> ListAllProduct()
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

        public async Task<ServiceResponse<ProductModel>> UpdateProduct(ProductModel request)
        {
            ServiceResponse<ProductModel> _response = new();
            try
            {
                var existingProduct = await _productRepository.GetProductById(request.ProductId);

                if (request.ProductPrice < 0 || request.Quantity < 0 || request.ByAge < 0 || request.PreOrderAmount < 0)
                {
                    _response.Success = false;
                    _response.Message = "Negative value does not allowed";
                    return _response;
                }

                if (existingProduct == null)
                {
                    _response.Success = false;
                    _response.Message = "Not Found";
                    _response.Data = null;
                    return _response;
                }

                existingProduct.ProductName = request.ProductName;
                existingProduct.ProductImg = request.ProductImg;
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

                var productDto = _mapper.Map<ProductModel>(existingProduct);
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
    }
}
