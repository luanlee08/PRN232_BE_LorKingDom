using BLL.DTOs;
using BLL.DTOs.Notifications;
using BLL.Helpers.Notification;
using BLL.Interfaces.Notification;
using DAL.Interface;

namespace BLL.Services.Notification
{

    public class NotificationQueryService : INotificationQueryService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly NotificationMapperHelper _mapperHelper;

        public NotificationQueryService(
            INotificationRepository notificationRepo,
            NotificationMapperHelper mapperHelper)
        {
            _notificationRepo = notificationRepo;
            _mapperHelper = mapperHelper;
        }


        public async Task<ApiResponse<PagedResult<DeliveryResponse>>> GetDeliveriesAsync(DeliveryQuery query)
        {
            var (items, total) = await _notificationRepo.GetDeliveriesAsync(
                query.AccountId,
                query.TemplateCode,
                query.Status,
                query.Keyword,
                query.FromDate,
                query.ToDate,
                query.Page,
                query.PageSize);

            return new ApiResponse<PagedResult<DeliveryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy danh sách deliveries thành công",
                Data = new PagedResult<DeliveryResponse>
                {
                    Items = items.Select(d => _mapperHelper.MapToResponse(d)).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }


        public async Task<ApiResponse<DeliveryResponse>> GetDeliveryByIdAsync(long id)
        {
            var delivery = await _notificationRepo.GetDeliveryByIdAsync(id);

            if (delivery == null)
            {
                return new ApiResponse<DeliveryResponse>
                {
                    Status = 404,
                    StatusMessage = "NOT_FOUND",
                    Message = "Không tìm thấy delivery"
                };
            }

            return new ApiResponse<DeliveryResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông tin delivery thành công",
                Data = _mapperHelper.MapToResponse(delivery)
            };
        }


        public async Task<ApiResponse<DeliveryStatsResponse>> GetStatsAsync()
        {
            var stats = await _notificationRepo.GetStatsAsync();

            return new ApiResponse<DeliveryStatsResponse>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thống kê thành công",
                Data = new DeliveryStatsResponse
                {
                    TotalDeliveries = stats.TotalDeliveries,
                    UnreadDeliveries = stats.UnreadDeliveries,
                    ReadDeliveries = stats.ReadDeliveries,
                    TodayDeliveries = stats.TodayDeliveries,
                    DeliveriesByTemplate = stats.DeliveriesByTemplate
                }
            };
        }


        public async Task<ApiResponse<PagedResult<DeliveryResponse>>> GetUserNotificationsAsync(int accountId, UserNotificationQuery query)
        {
            var (items, total) = await _notificationRepo.GetUserDeliveriesAsync(
                accountId, query.Status, query.TemplateCode, query.Keyword,
                query.FromDate, query.ToDate, query.Page, query.PageSize);

            return new ApiResponse<PagedResult<DeliveryResponse>>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy thông báo của user thành công",
                Data = new PagedResult<DeliveryResponse>
                {
                    Items = items.Select(d => _mapperHelper.MapToResponse(d)).ToList(),
                    TotalCount = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                }
            };
        }


        public async Task<ApiResponse<int>> GetUnreadCountAsync(int accountId)
        {
            var count = await _notificationRepo.GetUnreadCountAsync(accountId);

            return new ApiResponse<int>
            {
                Status = 200,
                StatusMessage = "SUCCESS",
                Message = "Lấy số lượng thông báo chưa đọc thành công",
                Data = count
            };
        }
    }
}
