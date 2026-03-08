using BLL.DTOs;
using BLL.DTOs.Orders;
using BLL.Helpers.Order;
using BLL.Interfaces.Order;
using DAL.Interface;
using Microsoft.Extensions.Logging;

namespace BLL.Services.Order
{

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
            var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");

            return _mapper.MapToDto(order);
        }

        public async Task<OrderDto> GetOrderByIdForAccountAsync(int orderId, int accountId)
        {
            var order = await _orderRepo.GetByIdForAccountAsync(orderId, accountId);
            if (order == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");

            return _mapper.MapToDto(order);
        }

        public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(
            int accountId,
            int pageNumber = 1,
            int pageSize = 10,
            string? statusFilter = null)
        {
            var skip = (pageNumber - 1) * pageSize;

            var orders = await _orderRepo.GetOrdersByAccountIdAsync(accountId, skip, pageSize, statusFilter);
            var totalCount = await _orderRepo.GetOrdersCountByAccountIdAsync(accountId, statusFilter);

            return new PagedResult<OrderDto>
            {
                Items = orders.Select(o => _mapper.MapToDto(o)).ToList(),
                TotalCount = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? statusFilter = null)
        {
            var skip = (pageNumber - 1) * pageSize;

            var orders = await _orderRepo.GetAllOrdersAsync(skip, pageSize, statusFilter);
            var totalCount = await _orderRepo.GetTotalOrdersCountAsync(statusFilter);

            return new PagedResult<OrderDto>
            {
                Items = orders.Select(o => _mapper.MapToDto(o)).ToList(),
                TotalCount = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<OrderResponse>> GetAdminOrdersPagedAsync(OrderQuery query)
        {
            var (items, totalCount) = await _orderRepo.GetPagedAsync(
                query.Keyword,
                query.StatusId,
                query.FromDate,
                query.ToDate,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDesc);

            return new PagedResult<OrderResponse>
            {
                Items = items.Select(o => _mapper.MapToOrderResponse(o)).ToList(),
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<OrderDetailResponse> GetAdminOrderDetailAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdWithDetailsAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng");

            return _mapper.MapToOrderDetailResponse(order);
        }
    }
}
