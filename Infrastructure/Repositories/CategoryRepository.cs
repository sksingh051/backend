using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CategoryDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AllDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(AllDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponseResult<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Fetching all categories from the database");
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = _context.Product.Count(p => p.CategoryId == c.Id)
                })
                .ToListAsync();

            _logger.LogInformation("Successfully fetched {Count} categories from the database", categories.Count);
            return ApiResponseResult<List<CategoryDto>>.SuccessResponse(categories, ResponseMessages.CategoriesFetched);
        }

        public async Task<ApiResponseResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            _logger.LogInformation("Fetching category with ID {CategoryId} from the database", id);
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = _context.Product.Count(p => p.CategoryId == c.Id)
                })
                .FirstOrDefaultAsync();

            if (category is null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found in the database", id);
                return ApiResponseResult<CategoryDto>.FailureResponse(ResponseMessages.CategoryNotFound);
            }

            _logger.LogInformation("Successfully fetched category with ID {CategoryId} from the database", id);
            return ApiResponseResult<CategoryDto>.SuccessResponse(category, ResponseMessages.CategoriesFetched);
        }

        public async Task<ApiResponseResult<CategoryDto>> AddCategoryAsync(CreateCategoryDto dto)
        {
            _logger.LogInformation("Adding new category with name: {CategoryName} to the database", dto.Name);
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            _logger.LogInformation("Successfully added category with ID {CategoryId} to the database", category.Id);
            return ApiResponseResult<CategoryDto>.SuccessResponse(categoryDto, ResponseMessages.CategoryCreated);
        }

        public async Task<ApiResponseResult<bool>> DeleteCategoryAsync(int id)
        {
            _logger.LogInformation("Deleting category with ID {CategoryId} from the database", id);
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion", id);
                return ApiResponseResult<bool>.FailureResponse(ResponseMessages.CategoryNotFound);
            }

            // Check if any products are linked to this category
            var productCount = await _context.Product.CountAsync(p => p.CategoryId == id);
            if (productCount > 0)
            {
                _logger.LogWarning("Cannot delete category with ID {CategoryId} because {Count} products are linked to it", id, productCount);
                return ApiResponseResult<bool>.FailureResponse($"Cannot delete category '{category.Name}' because it has {productCount} product{(productCount > 1 ? "s" : "")} linked to it. Please reassign or delete the products first.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted category with ID {CategoryId} from the database", id);
            return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.CategoryDeleted);
        }
    }
}
