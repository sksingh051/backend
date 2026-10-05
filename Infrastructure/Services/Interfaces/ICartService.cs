using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CartDtos;
namespace Phase_07_Poc_01.Infrastructure.Services.Interfaces;

public interface ICartService
{
    Task<ApiResponseResult<List<CartItemDto>>> GetCartAsync(int userId);
    Task<ApiResponseResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto addToCartDto);
    Task<ApiResponseResult<bool?>> UpdateCartItemAsync(int itemId, int userId, UpdateCartItemDto updateCartItemDto);
    Task<ApiResponseResult<bool>> DeleteCartItemAsync(int itemId, int userId);

}