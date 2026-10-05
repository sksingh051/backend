using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CartDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
namespace Phase_07_Poc_01.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly ILogger<CartService> _logger;

    public CartService(ICartRepository cartRepo, ILogger<CartService> logger)
    {
        _cartRepo = cartRepo;
        _logger = logger;
    }

    public async Task<ApiResponseResult<List<CartItemDto>>> GetCartAsync(int userId)
    {
        _logger.LogInformation("Fetching cart for user {UserId}", userId);
        var result = await _cartRepo.GetCartByUserIdAsync(userId);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to fetch cart for user {UserId}: {Message}", userId, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully fetched cart for user {UserId}", userId);
        return result;
    }

    public async Task<ApiResponseResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto addToCartDto)
    {
        _logger.LogInformation("Adding item to cart for user {UserId}, product {ProductId}", userId, addToCartDto.ProductId);
        var result = await _cartRepo.AddToCartAsync(userId, addToCartDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to add item to cart for user {UserId}: {Message}", userId, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully added item to cart for user {UserId}", userId);
        return result;
    }

    public async Task<ApiResponseResult<bool?>> UpdateCartItemAsync(int itemId, int userId, UpdateCartItemDto updateCartItemDto)
    {
        _logger.LogInformation("Updating cart item {ItemId} for user {UserId}", itemId, userId);
        var result = await _cartRepo.UpdateCartItemAsync(itemId, userId, updateCartItemDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to update cart item {ItemId} for user {UserId}: {Message}", itemId, userId, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully updated cart item {ItemId} for user {UserId}", itemId, userId);
        return result;
    }


    public async Task<ApiResponseResult<bool>> DeleteCartItemAsync(int itemId, int userId)
    {
        _logger.LogInformation("Deleting cart item {ItemId} for user {UserId}", itemId, userId);
        var result = await _cartRepo.DeleteCartItemAsync(itemId, userId);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to delete cart item {ItemId}: {Message}", itemId, result.Message);
            return result;
        }
        _logger.LogInformation("Successfully deleted cart item {ItemId}", itemId);
        return result;
    }

}