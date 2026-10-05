using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.CartDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AllDbContext _db;
    private readonly ILogger<CartRepository> _logger;
    private readonly IMapper _mapper;

    public CartRepository(AllDbContext db, ILogger<CartRepository> logger, IMapper mapper)
    {
        _db = db;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResponseResult<List<CartItemDto>>> GetCartByUserIdAsync(int userId)
    {
        _logger.LogInformation("{Repository} - Fetching cart item {UserId}", nameof(CartRepository), userId);
        var cartItems = await _db.CartItems.AsNoTracking()
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var cartItemDto = _mapper.Map<List<CartItemDto>>(cartItems);
        _logger.LogInformation("Fetching cart item from DB {userId}", userId);
        return ApiResponseResult<List<CartItemDto>>.SuccessResponse(cartItemDto, ResponseMessages.CartFetchSuccess);
    }


    public async Task<ApiResponseResult<CartItemDto>> AddToCartAsync(int userId, AddToCartDto addToCartDto)
    {
        _logger.LogInformation("executing CartRepository ");
        
        _logger.LogInformation("Validating product existence for {ProductId}", addToCartDto.ProductId);
        var productExists = await _db.Product.AnyAsync(p => p.Id == addToCartDto.ProductId);
        if (!productExists)
        {
            return ApiResponseResult<CartItemDto>.FailureResponse(ResponseMessages.ProductNotFound);
        }

        _logger.LogInformation("checking existing cart for {UserId}", userId);
        var existing = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId
                                 && c.ProductId == addToCartDto.ProductId);

        if (existing is not null)
        {
            existing.Quantity += addToCartDto.Quantity;
            await _db.SaveChangesAsync();

            var updatedQuantityCart = _mapper.Map<CartItemDto>(existing);
            _logger.LogInformation("Updated existing cart for {UserId}", userId);
            return ApiResponseResult<CartItemDto>.SuccessResponse(updatedQuantityCart, ResponseMessages.CartItemAdded);
        }

        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = addToCartDto.ProductId,
            Quantity = addToCartDto.Quantity
        };

        await _db.CartItems.AddAsync(cartItem);
        await _db.SaveChangesAsync();

        await _db.Entry(cartItem).Reference(c => c.Product).LoadAsync();

        var newCartDto = _mapper.Map<CartItemDto>(cartItem);
        _logger.LogInformation("Created new Cart for {UserId}", userId);
        return ApiResponseResult<CartItemDto>.SuccessResponse(newCartDto, ResponseMessages.CartItemAdded);
    }


    public async Task<ApiResponseResult<bool?>> UpdateCartItemAsync(int itemId, int userId, UpdateCartItemDto updateCartItemDto)
    {
        _logger.LogInformation("Fetching cart item for update, ItemId={ItemId}, UserId={UserId}", itemId, userId);
        var cartItem = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId);

        if (cartItem is null)
        {
            return ApiResponseResult<bool?>.FailureResponse(ResponseMessages.CartItemNotFound);
        }

        cartItem.Quantity = updateCartItemDto.Quantity;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Updated cart item {Itemid}", itemId);
        return ApiResponseResult<bool?>.SuccessResponse(true, ResponseMessages.CartItemUpdated);
    }


    public async Task<ApiResponseResult<bool>> DeleteCartItemAsync(int itemId, int userId)
    {
        _logger.LogInformation("Fetching cart item for deletion {ItemId}", itemId);
        var cartItem = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId);

        if (cartItem is null)
        {
            _logger.LogWarning("Cart item {ItemId} not found for user {UserId}", itemId, userId);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.CartItemNotFound);
        }
        _db.CartItems.Remove(cartItem);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Item successfully deleted {ItemId}", itemId);
        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.CartItemDeleted);
    }

}