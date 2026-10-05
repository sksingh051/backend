using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.OrderDtos;
using Phase_07_Poc_01.Infrastructure.Entities;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using MediatR;
using Phase_07_Poc_01.Infrastructure.Events;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AllDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderRepository> _logger;
    private readonly IMediator _mediator;

    public OrderRepository(AllDbContext db, IMapper mapper, ILogger<OrderRepository> logger, IMediator mediator)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    // POST /orders — create order from cart atomically
    public async Task<ApiResponseResult<OrderDto>> CreateOrderAsync(int userId)
    {
        // begin transaction — order creation and cart clear must happen together
        await using var transaction = await _db.Database.BeginTransactionAsync();

        _logger.LogInformation("{Repository} - Creating order for user {UserId}",
            nameof(OrderRepository), userId);

        // fetch user's cart with product details
        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            _logger.LogWarning("Cart empty for user {UserId}", userId);
            return ApiResponseResult<OrderDto>.FailureResponse(ResponseMessages.CartEmptyForOrder);
        }

        // calculate total amount
        var totalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);

        // create order entity
        var order = new Order
        {
            UserId = userId,
            TotalAmount = totalAmount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();  // save to get order Id

        // create order items from cart — snapshot product name and price
        var orderItems = cartItems.Select(c => new OrderItem
        {
            OrderId = order.Id,
            ProductId = c.ProductId,
            ProductName = c.Product.Name,   // snapshot
            Price = c.Product.Price,         // snapshot
            Quantity = c.Quantity
        }).ToList();

        await _db.OrderItems.AddRangeAsync(orderItems);
        await _db.SaveChangesAsync();

        // reduce product stock for each ordered item
        foreach (var cartItem in cartItems)
        {
            var product = cartItem.Product;
            if (product.Quantity < cartItem.Quantity)
            {
                _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Available}, Requested: {Requested}",
                    product.Id, product.Quantity, cartItem.Quantity);
                await transaction.RollbackAsync();
                return ApiResponseResult<OrderDto>.FailureResponse(
                    $"Insufficient stock for '{product.Name}'. Only {product.Quantity} left.");
            }
            product.Quantity -= cartItem.Quantity;
        }
        await _db.SaveChangesAsync();

        // clear cart atomically — same transaction
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();

        // commit transaction — all or nothing
        await transaction.CommitAsync();

        _logger.LogInformation("Order {OrderId} created for user {UserId}",
            order.Id, userId);

        // Publish event using MediatR
        await _mediator.Publish(new OrderPlacedEvent 
        { 
            OrderId = order.Id, 
            UserId = userId,
            TotalAmount = totalAmount 
        });

        // reload order with items for mapping
        var createdOrder = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        var orderDto = _mapper.Map<OrderDto>(createdOrder);
        return ApiResponseResult<OrderDto>.SuccessResponse(orderDto, ResponseMessages.OrderCreated);
    }

    // GET /orders — get all orders for logged in user
    public async Task<ApiResponseResult<List<OrderDto>>> GetOrdersByUserIdAsync(int userId)
    {
        _logger.LogInformation("{Repository} - Fetching orders for user {UserId}",
            nameof(OrderRepository), userId);

        var orders = await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var orderDtos = _mapper.Map<List<OrderDto>>(orders);
        return ApiResponseResult<List<OrderDto>>.SuccessResponse(orderDtos, ResponseMessages.OrdersFetched);
    }

    // GET /orders/{id} — get specific order for logged in user
    public async Task<ApiResponseResult<OrderDto>> GetOrderByIdAsync(int orderId, int userId)
    {
        _logger.LogInformation("{Repository} - Fetching order {OrderId} for user {UserId}",
            nameof(OrderRepository), orderId, userId);

        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
            return ApiResponseResult<OrderDto>.FailureResponse(ResponseMessages.OrderNotFound);
        }

        var orderDto = _mapper.Map<OrderDto>(order);
        return ApiResponseResult<OrderDto>.SuccessResponse(orderDto, ResponseMessages.OrdersFetched);
    }

    // PATCH /orders/{id}/status — admin only
    public async Task<ApiResponseResult<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
    {
        _logger.LogInformation("{Repository} - Updating status for order {OrderId}",
            nameof(OrderRepository), orderId);

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found for status update", orderId);
            return ApiResponseResult<bool>.FailureResponse(ResponseMessages.OrderNotFound);
        }

        order.Status = dto.Status;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} status updated to {Status}",
            orderId, dto.Status);

        // Publish event so the handler sends email + in-app notification
        await _mediator.Publish(new OrderStatusChangedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            NewStatus = dto.Status,
            TotalAmount = order.TotalAmount
        });

        return ApiResponseResult<bool>.SuccessResponse(true, ResponseMessages.OrderStatusUpdated);
    }
}