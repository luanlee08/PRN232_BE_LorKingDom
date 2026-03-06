using BLL.Events;
using BLL.Events.Order.Handlers;
using BLL.Helpers.Notification;
using BLL.Helpers.Order;
using BLL.Interfaces;
using BLL.Interfaces.Moderation;
using BLL.Interfaces.Notification;
using BLL.Interfaces.Order;
using BLL.Interfaces.Wallet;
using BLL.Services;
using BLL.Services.Moderation;
using BLL.Services.Notification;
using BLL.Services.Order;
using BLL.Services.Wallet;
using BLL.Validators.Address;
using BLL.Validators.Auth;
using BLL.Validators.SuperCategory;
using BLL.Worker;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class BLLServiceCollectionExtensions
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddScoped<ISuperCategoryService, SuperCategoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<IAgeService, AgeService>();
            services.AddScoped<IOriginService, OriginService>();
            services.AddScoped<IPriceRangeService, PriceRangeService>();
            services.AddScoped<ISexService, SexService>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // Account
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICustomerAccountService, CustomerAccountService>();

            // Profile
            services.AddScoped<IProfileService, ProfileService>();

            // Address
            services.AddScoped<IAddressServices, AddressServices>();

            // Blog 
            services.AddScoped<IReviewBlogService, ReviewBlogService>();
            services.AddScoped<IReviewBlogReactionService, ReviewBlogReactionService>();
            services.AddScoped<IReviewBlogReplyService, ReviewBlogReplyService>();

            // Cart
            services.AddScoped<ICartService, CartServices>();

            // Order - New CQRS pattern services
            services.AddScoped<IOrderQueryService, OrderQueryService>();
            services.AddScoped<IOrderCommandService, OrderCommandService>();
            services.AddScoped<IOrderPaymentService, OrderPaymentService>();
            services.AddScoped<IOrderWebhookService, OrderWebhookService>();
            services.AddScoped<IOrderRefundService, OrderRefundService>();
            services.AddScoped<IOrderExportService, OrderExportService>();

            // Order Helpers
            services.AddScoped<OrderMappingHelper>();
            services.AddScoped<OrderCalculationHelper>();
            services.AddScoped<OrderValidationHelper>();
            services.AddScoped<PaymentGatewayHelper>();

            // Order - Keep old service for backward compatibility (will be deprecated)
            services.AddScoped<IOrderService, OrderService>();

            // Payment Gateways
            services.AddScoped<IVNPayService, VNPayService>();
            services.AddScoped<IMoMoService, MoMoService>();
            services.AddScoped<ISepayService, SepayService>();

            // Wallet - CQRS pattern services
            services.AddScoped<IWalletQueryService, WalletQueryService>();
            services.AddScoped<IWalletCommandService, WalletCommandService>();

            // Shipping Providers
            services.AddScoped<IGHNService, GHNService>();
            services.AddScoped<ILocationService, LocationService>();

            // GHN Shipping Status — single source of truth for all status update logic
            services.AddScoped<IGHNShippingStatusService, GHNShippingStatusService>();

            // HttpClient for payment & shipping services
            services.AddHttpClient();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductImageService, ProductImageService>();

            // Voucher
            services.AddScoped<IVoucherService, VoucherService>();

            // Review product         
            services.AddScoped<IReviewProductService, ReviewProductService>();
            services.AddScoped<IReviewReactionService, ReviewProductReactionService>();

            // Cloudinary
            services.AddSingleton<ICloudinaryService, CloudinaryService>();

            // Moderate product
            services.AddScoped<IReviewModerationService, ReviewModerationService>();
            services.AddScoped<IModerationLayer1Service, ModerationLayer1Service>();
            services.AddScoped<IModerationLayer2Service, ModerationLayer2Service>();

            // Notification - New CQRS pattern services
            services.AddScoped<INotificationQueryService, NotificationQueryService>();
            services.AddScoped<INotificationCommandService, NotificationCommandService>();
            services.AddScoped<INotificationSchedulerService, NotificationSchedulerService>();

            // Notification Helpers
            services.AddScoped<NotificationHelper>();
            services.AddScoped<NotificationMapperHelper>();
            services.AddScoped<NotificationTargetHelper>();
            services.AddScoped<NotificationContentHelper>();



            // =========================================================
            // Domain Events
            // =========================================================
            // Dispatcher uses IServiceProvider to resolve handlers at runtime,
            // so new handlers just need to be registered here — no other changes.
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // Order domain event handlers (subscribe to order events → send notifications)
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.OrderCreatedEvent>, OrderCreatedNotificationHandler>();
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.OrderStatusChangedEvent>, OrderStatusChangedNotificationHandler>();
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.OrderCancelledEvent>, OrderCancelledNotificationHandler>();
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.OrderPaidEvent>, OrderPaidNotificationHandler>();

            // GHN shipping status domain event handlers
            // Push notification for intermediate GHN statuses (picking, transporting, delivering, return)
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.GHNShippingStatusChangedEvent>, GHNStatusNotificationHandler>();
            // Push realtime via IShippingRealtimeService (implemented by SignalR in Web layer)
            services.AddScoped<IDomainEventHandler<BLL.Events.Order.GHNShippingStatusChangedEvent>, GHNStatusRealtimeHandler>();
            // =========================================================

            // Template
            services.AddScoped<ITemplateService, TemplateService>();

            // Campaign
            services.AddScoped<ICampaignService, CampaignService>();


            // Workers
            services.AddScoped<NotificationWorker>();
            services.AddScoped<ShippingStatusSyncWorker>();
            services.AddScoped<DemoShippingFlowWorker>();

            // Đăng ký FluentValidation cho assembly (Đk 1 cái là đủ, mấy cái còn lại ăn theo)
            services.AddValidatorsFromAssemblyContaining<CreateSuperCategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

            // Blogs
            // Blog
            services.AddScoped<IBlogCategoryService, BlogCategoryService>();
            services.AddScoped<IBlogService, BlogService>();

            // Statistics
            services.AddScoped<IStatisticsService, StatisticsService>();

            return services;
        }
    }
}
