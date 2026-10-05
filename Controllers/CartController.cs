using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CartDtos;
using Phase_07_Poc_01.Infrastructure.Services.Interfaces;
using Phase_07_Poc_01.Static;
using UserProductCart.WebApi.ApiModels;
using Phase_07_Poc_01.Helper;


namespace Phase_07_Poc_01.Controllers;

[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    public CartController(ICartService cartService, ILogger<CartController> logger, IMapper mapper)
    {
        _cartService = cartService;
        _logger = logger;
        _mapper = mapper;
    }



    [HttpGet, Route(ApiRoutes.Cart.CartBase)]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Fetching cart for user {UserId}", userId);
        var result = await _cartService.GetCartAsync(userId);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to fetch cart for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }
        _logger.LogInformation("Successfully fetched cart for user {UserId}", userId);
        var cartApiModel = _mapper.Map<List<CartItemApiModel>>(result.Result);
        var response = new ApiResponseResult<List<CartItemApiModel>>
        {
            Success = result.Success,
            Message = result.Message,
            Result = cartApiModel
        };
        return Ok(response);
    }


    [HttpPost, Route(ApiRoutes.Cart.CartBase)]
    public async Task<IActionResult> AddToCartAsync(AddToCartDto addToCartDto)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Adding item to cart for user {UserId}", userId);
        var result = await _cartService.AddToCartAsync(userId, addToCartDto);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to add item to cart for user {UserId}: {Message}", userId, result.Message);
            return BadRequest(result);
        }
        _logger.LogInformation("Successfully added item to cart for user {UserId}", userId);
        var cartApiModel = _mapper.Map<CartItemApiModel>(result.Result);
        var response = new ApiResponseResult<CartItemApiModel>
        {
            Success = result.Success,
            Message = result.Message,
            Result = cartApiModel
        };
        return CreatedAtAction(nameof(GetCart), response);
    }

    [HttpPut, Route(ApiRoutes.Cart.GetCartById)]
    public async Task<IActionResult> UpdateCartItemAsync(
    int itemId,
    UpdateCartItemDto updateCartItemDto)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Updating cart item {ItemId} for user {UserId}", itemId, userId);
        var result = await _cartService.UpdateCartItemAsync(itemId, userId, updateCartItemDto);

        if (!result.Success)
        {
            _logger.LogWarning("Failed to update cart item {ItemId}: {Message}", itemId, result.Message);
            return BadRequest(result);
        }
        _logger.LogInformation("Successfully updated cart item {ItemId}", itemId);
        return NoContent();
    }

    [HttpDelete, Route(ApiRoutes.Cart.GetCartById)]
    public async Task<IActionResult> DeleteCartItemAsync(int itemId)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Deleting cart item {ItemId} for user {UserId}", itemId, userId);
        var result = await _cartService.DeleteCartItemAsync(itemId, userId);
        if (!result.Success)
        {
            _logger.LogWarning("Failed to delete cart item {ItemId}: {Message}", itemId, result.Message);
            return BadRequest(result);
        }
        _logger.LogInformation("Successfully deleted cart item {ItemId}", itemId);
        return NoContent();
    }

}