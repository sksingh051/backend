using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CartDtos;

namespace Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;

public interface ICartRepository
{
    Task<ApiResponseResult<List<CartItemDto>>> GetCartByUserIdAsync(int userId);
    Task<ApiResponseResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto addToCartDto);
    Task<ApiResponseResult<bool?>> UpdateCartItemAsync(int itemId, int userId, UpdateCartItemDto updateCartItemDto);
    Task<ApiResponseResult<bool>> DeleteCartItemAsync(int itemId, int userId);

}