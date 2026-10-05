using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.ProductDtos;

namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces;

public interface IProductService
{

    Task<ApiResponseResult<PagedResponse<ProductDto>>> GetAllProductAsync(ProductQueryParametersDto query);
    Task<ApiResponseResult<ProductDto>> GetProductByIdAsync(int id);
    Task<ApiResponseResult<ProductDto>> CreateProductAsync(CreateProductDto product);
    Task<ApiResponseResult<bool>> UpdateProductByIdAsync(UpdateProductDto product, int id);
    Task<ApiResponseResult<bool>> DeleteProductByIdAsync(int id);
}