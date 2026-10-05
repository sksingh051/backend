using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CategoryDtos;

namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponseResult<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResponseResult<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponseResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
        Task<ApiResponseResult<bool>> DeleteCategoryAsync(int id);
    }
}
