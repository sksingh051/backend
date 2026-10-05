using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO.CategoryDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;

namespace Phase_07_Poc_01.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet, Route(Phase_07_Poc_01.Static.ApiRoutes.Category.CategoryBase)]
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("Fetching all categories");
            var result = await _categoryService.GetAllCategoriesAsync();
            if (!result.Success)
            {
                _logger.LogWarning("Failed to fetch categories: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully fetched all categories");
            return Ok(result);
        }

        [HttpGet, Route(Phase_07_Poc_01.Static.ApiRoutes.Category.GetCategoryById)]
        public async Task<IActionResult> GetCategory(int id)
        {
            _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.Success)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                return NotFound(result);
            }
            _logger.LogInformation("Successfully fetched category with ID: {CategoryId}", id);
            return Ok(result);
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpPost, Route(Phase_07_Poc_01.Static.ApiRoutes.Category.CategoryBase)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for category creation");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new category with name: {CategoryName}", dto.Name);
            var result = await _categoryService.CreateCategoryAsync(dto);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to create category: {Message}", result.Message);
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully created category");
            return CreatedAtAction(nameof(GetCategory), new { id = result.Result!.Id }, result);
        }

        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        [HttpDelete, Route(Phase_07_Poc_01.Static.ApiRoutes.Category.DeleteCategory)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to delete category with ID {CategoryId}: {Message}", id, result.Message);
                if (result.Message == Phase_07_Poc_01.Helper.ResponseMessages.CategoryNotFound)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            _logger.LogInformation("Successfully deleted category with ID: {CategoryId}", id);
            return NoContent();
        }
    }
}
