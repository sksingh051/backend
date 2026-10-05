using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.ProductDtos;

namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<ApiResponseResult<PagedResponse<ProductDto>>> GetAllProductAsync(ProductQueryParametersDto query);
        Task<ApiResponseResult<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponseResult<ProductDto>> CreateProductAsync(CreateProductDto product);
        Task<ApiResponseResult<bool>> UpdateProductByIdAsync(int id, UpdateProductDto dto);
        Task<ApiResponseResult<bool>> DeleteProductByIdAsync(int id);
    }
}