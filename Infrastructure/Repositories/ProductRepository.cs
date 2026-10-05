using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.ProductDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Helper;
namespace Phase_07_Poc_01.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{

    private readonly AllDbContext _productDb;
    private readonly ILogger<ProductRepository> _logger;
    private readonly IMapper _mapper;

    public ProductRepository(AllDbContext productDb, ILogger<ProductRepository> logger, IMapper mapper)
    {
        _productDb = productDb;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponseResult<PagedResponse<ProductDto>>> GetAllProductAsync(ProductQueryParametersDto queryParam)
    {
        _logger.LogInformation("Fetching Products from Db with pagination");
        
        var query = _productDb.Product.AsNoTracking();
        
        if (queryParam.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == queryParam.CategoryId.Value);
        }

        if (!string.IsNullOrEmpty(queryParam.Search))
        {
            query = query.Where(p => p.Name.Contains(queryParam.Search) || p.Description.Contains(queryParam.Search));
        }

        var totalCount = await query.CountAsync();
        var productList = await query
            .Skip((queryParam.Page - 1) * queryParam.Limit)
            .Take(queryParam.Limit)
            .ToListAsync();

        var productDtoList = _mapper.Map<List<ProductDto>>(productList);
        var pagedResponse = new PagedResponse<ProductDto>
        {
            Items = productDtoList,
            TotalCount = totalCount,
            Page = queryParam.Page,
            PageSize = queryParam.Limit
        };

        _logger.LogInformation("Product successfully fetched");
        return ApiResponseResult<PagedResponse<ProductDto>>.SuccessResponse(pagedResponse, ResponseMessages.ProductsFetched);
    }

        public async Task<ApiResponseResult<ProductDto>> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Fetching product with ID {ProductId} from the database", id);
            var existingProduct = await _productDb.Product.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct is null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found in the database", id);
            return ApiResponseResult<ProductDto>.FailureResponse(ResponseMessages.ProductNotFound);
        }

        var productDto = _mapper.Map<ProductDto>(existingProduct);
        _logger.LogInformation("Successfully fetched product with ID {ProductId} from the database", id);
        return ApiResponseResult<ProductDto>.SuccessResponse(productDto, ResponseMessages.ProductsFetched);
    }

    public async Task<ApiResponseResult<ProductDto>> CreateProductAsync(CreateProductDto productDto)
    {
        _logger.LogInformation("Checking for existing productCode: {ProductCode}", productDto.ProductCode);
        var existing = await _productDb.Product
.FirstOrDefaultAsync(p => p.ProductCode == productDto.ProductCode.ToString());

        if (existing is not null)
        {
            _logger.LogWarning("Product already exists for productCode: {ProductCode}", productDto.ProductCode);
            return ApiResponseResult<ProductDto>.FailureResponse($"Product with code {productDto.ProductCode} already exists");
        }

        var productEntity = _mapper.Map<Infrastructure.Entities.Product>(productDto);

        await _productDb.Product.AddAsync(productEntity);
        await _productDb.SaveChangesAsync();

        var productdto = _mapper.Map<ProductDto>(productEntity);
        _logger.LogInformation("Product resource created Successfully");
        return ApiResponseResult<ProductDto>.SuccessResponse(productdto, ResponseMessages.ProductCreated);
    }

    public async Task<ApiResponseResult<bool>> UpdateProductByIdAsync(int id, UpdateProductDto dto)
    {
        _logger.LogInformation("Updating product {Id}", id);

        var existingProduct = await _productDb.Product.FirstOrDefaultAsync(p => p.Id == id);

        if (existingProduct is null)
        {
            _logger.LogWarning("Product {Id} not found for update", id);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.ProductNotFound);
        }

        // apply only fields that client sent
        if (dto.Name is not null) existingProduct.Name = dto.Name;
        if (dto.Description is not null) existingProduct.Description = dto.Description;
        if (dto.Price is not null) existingProduct.Price = dto.Price.Value;
        if (dto.Quantity is not null) existingProduct.Quantity = dto.Quantity.Value;

        await _productDb.SaveChangesAsync();

        _logger.LogInformation("Product {Id} updated successfully", id);
        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.ProductUpdated);
    }

    public async Task<ApiResponseResult<bool>> DeleteProductByIdAsync(int id)
    {
        _logger.LogInformation("Fetching product with ID {ProductId} for deletion", id);
        
        var result = await _productDb.Product.FirstOrDefaultAsync(p => p.Id == id);
        if (result is null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.ProductNotFound);
        }

        _productDb.Product.Remove(result);
        await _productDb.SaveChangesAsync();
        _logger.LogInformation("Successfully deleted product with ID {ProductId} from the database", id);
        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.ProductDeleted);
    }
}
