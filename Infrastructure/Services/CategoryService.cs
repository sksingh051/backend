using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CategoryDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponseResult<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories");
            var result = await _categoryRepository.GetAllCategoriesAsync();
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch categories: {Message}", result.Message);
            }
            else
            {
                _logger.LogInformation("Successfully fetched all categories");
            }
            return result;
        }

        public async Task<ApiResponseResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
            var result = await _categoryRepository.GetCategoryByIdAsync(id);
            if (!result.Success)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
            }
            else
            {
                _logger.LogInformation("Successfully fetched category with ID: {CategoryId}", id);
            }
            return result;
        }

        public async Task<ApiResponseResult<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            _logger.LogInformation("Creating new category with name: {CategoryName}", dto.Name);
            var result = await _categoryRepository.AddCategoryAsync(dto);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create category: {Message}", result.Message);
            }
            else
            {
                _logger.LogInformation("Successfully created category");
            }
            return result;
        }

        public async Task<ApiResponseResult<bool>> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            var result = await _categoryRepository.DeleteCategoryAsync(id);
            if (!result.Success)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion", id);
            }
            else
            {
                _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", id);
            }
            return result;
        }
    }
}
