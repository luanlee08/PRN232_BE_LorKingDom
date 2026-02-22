using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Interface;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{
    /// <summary>
    /// Service for handling Order query operations (read-only)
    /// </summary>
    public class OrderQueryService : IOrderQueryService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly OrderMappingHelper _mapper;
        private readonly ILogger<OrderQueryService> _logger;

        public OrderQueryService(
            IOrderRepository orderRepo,
            OrderMappingHelper mapper,
            ILogger<OrderQueryService> logger)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                return _mapper.MapToDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // This will be implemented by controller passing accountId
                throw new NotImplementedException("Use GetOrdersAsync with userId parameter");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                throw;
            }
        }

        public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var skip = (pageNumber - 1) * pageSize;

                // Convert status to string if needed
                string? statusFilter = status?.ToString();

                var orders = await _orderRepo.GetAllOrdersAsync(skip, pageSize, statusFilter);
                var totalCount = await _orderRepo.GetTotalOrdersCountAsync(statusFilter);

                var orderDtos = orders.Select(o => _mapper.MapToDto(o)).ToList();

                return new PagedResult<OrderDto>
                {
                    Items = orderDtos,
                    TotalCount = totalCount,
                    Page = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                throw;
            }
        }

        public async Task<List<OrderDto>> GetOrdersAsync(
            int? userId = null,
            int? status = null,
            string? paymentMethod = null,
            string? paymentStatus = null)
        {
            try
            {
                // For now, get all orders and filter in memory
                // TODO: Add repository method that supports these filters
                var orders = await _orderRepo.GetAllOrdersAsync(0, 1000, status?.ToString());

                var filteredOrders = orders.AsEnumerable();

                if (userId.HasValue)
                {
                    filteredOrders = filteredOrders.Where(o => o.AccountId == userId.Value);
                }

                return filteredOrders.Select(o => _mapper.MapToDto(o)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders with filters");
                throw;
            }
        }

        public async Task<OrderDto> GetOrderDetailAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy đơn hàng");
                }

                return _mapper.MapToDto(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order detail {OrderId}", orderId);
                throw;
            }
        }
    }
}
