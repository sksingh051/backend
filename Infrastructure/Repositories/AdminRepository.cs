using Microsoft.EntityFrameworkCore;
using Phase_07_Poc_01.Data;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.DTO.AdminDtos;
using Phase_07_Poc_01.DTO.UserDtos;
using Phase_07_Poc_01.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AllDbContext _db;
        private readonly ILogger<AdminRepository> _logger;

        public AdminRepository(AllDbContext db, ILogger<AdminRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ApiResponseResult<PagedResponse<UserDto>>> GetUsersAsync(UserQueryParametersDto queryParam)
        {
            //var a = 0;var b=1;var c = b / a;
            var query = _db.Users.AsNoTracking();
            _logger.LogInformation("Fetching users from database with search={Search}, page={Page}, limit={Limit}", queryParam.Search, queryParam.Page, queryParam.Limit);

            if (queryParam.StartDate.HasValue)
            {
                var startDateUtc = queryParam.StartDate.Value.ToUniversalTime();
                query = query.Where(u => u.CreatedAt >= startDateUtc);
            }
            if (queryParam.EndDate.HasValue)
            {
                var endDateUtc = queryParam.EndDate.Value.ToUniversalTime();
                query = query.Where(u => u.CreatedAt <= endDateUtc);
            }

            if (!string.IsNullOrWhiteSpace(queryParam.Search))
            {
                var search = queryParam.Search.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(search) || u.Email.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            // Sorting
            bool isDesc = queryParam.Order?.ToLower() == "desc";
            query = queryParam.SortBy?.ToLower() switch
            {
                "name" => isDesc ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
                "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => isDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id), // default sort by Id
            };

            // Pagination
            var users = await query
                .Skip((queryParam.Page - 1) * queryParam.Limit)
                .Take(queryParam.Limit)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            var pagedResponse = new PagedResponse<UserDto>
            {
                Items = users,
                TotalCount = totalCount,
                Page = queryParam.Page,
                PageSize = queryParam.Limit
            };

            _logger.LogInformation("Successfully fetched {Count} users from database", users.Count);
            return ApiResponseResult<PagedResponse<UserDto>>.SuccessResponse(pagedResponse, ResponseMessages.UsersFetched);
        }

        public async Task<ApiResponseResult<OrderSummaryDto>> GetOrderSummaryAsync(SummaryQueryParametersDto queryParam)
        {
            _logger.LogInformation("Fetching order summary from database with startDate={StartDate}, endDate={EndDate}", queryParam.StartDate, queryParam.EndDate);
            var query = _db.Orders.AsNoTracking();

            if (queryParam.StartDate.HasValue)
            {
                var startDateUtc = queryParam.StartDate.Value.ToUniversalTime();
                query = query.Where(o => o.CreatedAt >= startDateUtc);
            }
            if (queryParam.EndDate.HasValue)
            {
                var endDateUtc = queryParam.EndDate.Value.ToUniversalTime();
                query = query.Where(o => o.CreatedAt <= endDateUtc);
            }

            var totalOrders = await query.CountAsync();
            var totalRevenue = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var ordersByStatusList = await query
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var ordersByStatus = ordersByStatusList.ToDictionary(x => x.Status, x => x.Count);

            var summary = new OrderSummaryDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                OrdersByStatus = ordersByStatus
            };

            _logger.LogInformation("Successfully fetched order summary: {TotalOrders} orders, {TotalRevenue} revenue", totalOrders, totalRevenue);
            return ApiResponseResult<OrderSummaryDto>.SuccessResponse(summary, ResponseMessages.SummaryFetched);
        }
  
        public async Task<ApiResponseResult<ProductSummaryDto>> GetProductSummaryAsync(SummaryQueryParametersDto queryParam)
        {
            _logger.LogInformation("Fetching product summary from database with startDate={StartDate}, endDate={EndDate}", queryParam.StartDate, queryParam.EndDate);
            var productQuery = _db.Product.AsNoTracking().AsQueryable();
            if (queryParam.StartDate.HasValue) 
            {
                var startDateUtc = queryParam.StartDate.Value.ToUniversalTime();
                productQuery = productQuery.Where(p => p.CreatedAt >= startDateUtc);
            }
            if (queryParam.EndDate.HasValue) 
            {
                var endDateUtc = queryParam.EndDate.Value.ToUniversalTime();
                productQuery = productQuery.Where(p => p.CreatedAt <= endDateUtc);
            }

            var totalProducts = await productQuery.CountAsync();
            var outOfStockCount = await productQuery.CountAsync(p => p.Quantity <= 0);

            var orderItemsQuery = _db.OrderItems.AsQueryable();
            if (queryParam.StartDate.HasValue || queryParam.EndDate.HasValue)
            {
                var startDateUtc = queryParam.StartDate?.ToUniversalTime();
                var endDateUtc = queryParam.EndDate?.ToUniversalTime();

                // To filter order items by date, we'd need to join with Orders.
                // For MostOrderedProducts, let's join to ensure accurate date range representation.
                orderItemsQuery = orderItemsQuery.Where(oi => 
                    _db.Orders.Any(o => o.Id == oi.OrderId && 
                        (!startDateUtc.HasValue || o.CreatedAt >= startDateUtc.Value) && 
                        (!endDateUtc.HasValue || o.CreatedAt <= endDateUtc.Value)));
            }

            var mostOrdered = await orderItemsQuery
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new MostOrderedProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantityOrdered = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantityOrdered)
                .Take(5)
                .ToListAsync();

            var summary = new ProductSummaryDto
            {
                TotalProducts = totalProducts,
                OutOfStockCount = outOfStockCount,
                MostOrderedProducts = mostOrdered
            };

            _logger.LogInformation("Successfully fetched product summary: {TotalProducts} products, {OutOfStock} out of stock", totalProducts, outOfStockCount);
            return ApiResponseResult<ProductSummaryDto>.SuccessResponse(summary, ResponseMessages.SummaryFetched);
        }
        public async Task<ApiResponseResult<PagedResponse<Phase_07_Poc_01.DTO.OrderDtos.OrderDto>>> GetOrdersAsync(Phase_07_Poc_01.DTO.OrderDtos.OrderQueryParametersDto queryParam)
        {
            _logger.LogInformation("Fetching all orders from database with page={Page}, limit={Limit}", queryParam.Page, queryParam.Limit);
            var query = _db.Orders.AsNoTracking()
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt);

            var totalCount = await query.CountAsync();

            var orders = await query
                .Skip((queryParam.Page - 1) * queryParam.Limit)
                .Take(queryParam.Limit)
                .Select(o => new Phase_07_Poc_01.DTO.OrderDtos.OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderItems = o.OrderItems.Select(oi => new Phase_07_Poc_01.DTO.OrderDtos.OrderItemDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        TotalPrice = oi.Price * oi.Quantity
                    }).ToList()
                })
                .ToListAsync();

            var pagedResponse = new PagedResponse<Phase_07_Poc_01.DTO.OrderDtos.OrderDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = queryParam.Page,
                PageSize = queryParam.Limit
            };

            _logger.LogInformation("Successfully fetched {Count} orders from database", orders.Count);
            return ApiResponseResult<PagedResponse<Phase_07_Poc_01.DTO.OrderDtos.OrderDto>>.SuccessResponse(pagedResponse, "Orders fetched successfully.");
        }

        public async Task<ApiResponseResult<bool>> UpdateUserRoleAsync(int userId, string newRole)
        {
            _logger.LogInformation("Updating role for user {UserId} to {NewRole}", userId, newRole);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return ApiResponseResult<bool>.FailureResponse("User not found.");
            }

            user.Role = newRole;
            await _db.SaveChangesAsync();
            _logger.LogInformation("Successfully updated role for user {UserId} to {NewRole}", userId, newRole);

            return ApiResponseResult<bool>.SuccessResponse(true, "User role updated successfully.");
        }
    }
}
