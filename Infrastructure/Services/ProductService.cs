using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.ProductDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
namespace Phase_07_Poc_01.Infrastructure.Services;

public class ProductService : IProductService
{

    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ApiResponseResult<PagedResponse<ProductDto>>> GetAllProductAsync(ProductQueryParametersDto query)
    {
        _logger.LogInformation("Product fetching execution from Database begin");
        var result = await _productRepository.GetAllProductAsync(query);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to fetch products: {Message}", result.Message);
            return result;
        }
        _logger.LogInformation("Successfully fetched all products");
        return result;
    }

    public async Task<ApiResponseResult<ProductDto>> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);
        var result = await _productRepository.GetProductByIdAsync(id);
        if (!result.Success)
        {
            _logger.LogWarning("Product with ID {ProductId} not found: {Message}", id, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully fetched product with ID: {ProductId}", id);
        return result;
    }

    public async Task<ApiResponseResult<ProductDto>> CreateProductAsync(CreateProductDto productDto)
    {
        _logger.LogInformation("Creating product resource");
        var result = await _productRepository.CreateProductAsync(productDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to create product: {Message}", result.Message);
            return result;
        }
        _logger.LogInformation("Successfully created product with ID: {ProductId}", result.Result?.Id);
        return result;
    }

    public async Task<ApiResponseResult<bool>> UpdateProductByIdAsync(UpdateProductDto updateProductDto, int id)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        var result = await _productRepository.UpdateProductByIdAsync(id, updateProductDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update product with ID {ProductId}: {Message}", id, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully updated product with ID: {ProductId}", id);
        return result;
    }

    public async Task<ApiResponseResult<bool>> DeleteProductByIdAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        var result = await _productRepository.DeleteProductByIdAsync(id);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to delete product with ID {ProductId}: {Message}", id, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully deleted product with ID: {ProductId}", id);
        return result;
    }
}