using AutoMapper;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.ApiModels;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.ProductDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet, Route(ApiRoutes.Product.ProductBase)]
        public async Task<IActionResult> GetAllProductAsync([FromQuery] Phase_07_Poc_01.DTO.ProductDtos.ProductQueryParametersDto query)
        {
            if (query.Page < 1) query.Page = 1;
            if (query.Limit < 1 || query.Limit > 100) query.Limit = 10;

            _logger.LogInformation("Getting all products with pagination");
            var result = await _productService.GetAllProductAsync(query);

            if (!result.Success)
            {
                _logger.LogWarning("No products found");
                return BadRequest(result);
            }

            _logger.LogInformation("Products fetched successfully");
            
            // Map the PagedResponse items
            var productApiModelItems = _mapper.Map<List<ProductApiModel>>(result.Result.Items);
            var pagedResponse = new PagedResponse<ProductApiModel>
            {
                Items = productApiModelItems,
                TotalCount = result.Result.TotalCount,
                Page = result.Result.Page,
                PageSize = result.Result.PageSize
            };

            var response = new ApiResponseResult<PagedResponse<ProductApiModel>> { Message = result.Message, Success = result.Success, Result = pagedResponse };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet, Route(ApiRoutes.Product.GetProductById)]
        public async Task<IActionResult> GetProductById(int id)
        {
            _logger.LogInformation("Getting product with ID {ProductId}", id);
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return BadRequest(result);
            }

            _logger.LogInformation("Successfully fetched product with ID {ProductId}", id);
            var productApiModel = _mapper.Map<ProductApiModel>(result.Result);
            var response = new ApiResponseResult<ProductApiModel> { Message = result.Message, Success = result.Success, Result = productApiModel };
            return Ok(response);
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpPost, Route(ApiRoutes.Product.ProductBase)]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto product)
        {
            _logger.LogInformation("Creating product resource");
            var result = await _productService.CreateProductAsync(product);

            if (!result.Success)
            {
                _logger.LogWarning("Error while creating product");
                return BadRequest(result);
            }

            _logger.LogInformation("Product created with ID {ProductId}", result.Result!.Id);
            return CreatedAtAction(nameof(GetProductById), new { id = result.Result!.Id }, result);
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpPut, Route(ApiRoutes.Product.GetProductById)]
        public async Task<IActionResult> UpdateProductByIdAsync(UpdateProductDto product, int id)
        {
            _logger.LogInformation("Updating product resource for ID: {ProductId}", id);
            var result = await _productService.UpdateProductByIdAsync(product, id);
            if (!result.Success)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return BadRequest(result);

            }
            _logger.LogInformation("Product with ID {ProductId} updated successfully", id);
            return NoContent();
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpDelete, Route(ApiRoutes.Product.GetProductById)]
        public async Task<IActionResult> DeleteProductByIdAsync(int id)
        {
            _logger.LogInformation("Deleting product with ID {ProductId}", id);
            var result = await _productService.DeleteProductByIdAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                return NotFound(result);
            }

            _logger.LogInformation("Successfully deleted product with ID {ProductId}", id);
            return NoContent();
        }
    }
}