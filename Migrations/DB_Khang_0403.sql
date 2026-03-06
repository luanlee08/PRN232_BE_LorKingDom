USE [master]
GO

-- 1. TẠO DATABASE (Tự động lấy đường dẫn mặc định của máy)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ASP_LorKingDom4')
BEGIN
    CREATE DATABASE [ASP_LorKingDom4]
END
GO

USE [ASP_LorKingDom4]
GO

-- 2. TẠO SCHEMAS
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Notification') EXEC('CREATE SCHEMA [Notification]')
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'System') EXEC('CREATE SCHEMA [System]')
GO

-- 3. TẠO BẢNG (TABLES)
-- [Accounts]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Accounts]') AND type in (N'U'))
CREATE TABLE [dbo].[Accounts](
	[AccountID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[RoleID] [int] NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [varchar](15) NULL,
	[Email] [nvarchar](255) NOT NULL UNIQUE, -- Added UNIQUE directly for simplicity
	[Image] [nvarchar](500) NULL,
	[Password] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[Status] [nvarchar](10) NOT NULL DEFAULT 'Active',
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL,
	[Provider] [nvarchar](255) NULL
)
GO

-- [Roles]
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[RoleName] [nvarchar](50) NOT NULL UNIQUE,
	[Description] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
GO

-- [Products] & Related Tables
CREATE TABLE [dbo].[Brands](
	[BrandID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[BrandName] [nvarchar](100) NOT NULL UNIQUE,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[SuperCategories](
	[SuperCategoryID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SuperCategoryName] [nvarchar](255) NOT NULL UNIQUE,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SuperCategoryID] [int] NOT NULL,
	[CategoryName] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Materials](
	[MaterialID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MaterialName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Ages](
	[AgeID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AgeRange] [nvarchar](50) NOT NULL UNIQUE,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Sexes](
	[SexID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SexName] [nvarchar](20) NOT NULL UNIQUE,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[PriceRanges](
	[PriceRangeID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PriceRangeMin] [decimal](12, 2) NOT NULL,
	[PriceRangeMax] [decimal](12, 2) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Origins](
	[OriginID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OriginName] [nvarchar](255) NOT NULL UNIQUE,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SKU] [varchar](50) NOT NULL UNIQUE,
	[CategoryID] [int] NULL,
	[MaterialID] [int] NULL,
	[AgeID] [int] NULL,
	[SexID] [int] NULL,
	[PriceRangeID] [int] NULL,
	[BrandID] [int] NULL,
	[OriginID] [int] NULL,
	[ProductName] [nvarchar](255) NOT NULL,
	[Price] [decimal](12, 2) NOT NULL CHECK ([Price]>=0),
	[Quantity] [int] NOT NULL DEFAULT 0 CHECK ([Quantity]>=0),
	[ProductStatus] [nvarchar](15) NOT NULL DEFAULT 'Available',
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[ProductImages](
	[ImageID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ProductID] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[IsMain] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)

-- [Orders] & [Cart] Tables
CREATE TABLE [dbo].[Cart](
	[CartID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[CartItems](
	[CartItemID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CartID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL CHECK ([Quantity]>0),
	[PriceAtThatTime] [decimal](12, 2) NOT NULL,
	[Status] [nvarchar](15) NOT NULL DEFAULT 'Active',
	[AddedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[StatusOrders](
	[StatusID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[StatusName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[VoucherTypes](
	[VoucherTypeID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[VoucherTypeName] [nvarchar](255) NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Voucher](
	[VoucherID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[VoucherTypeID] [int] NOT NULL,
	[CreateBy] [int] NULL,
	[VoucherCode] [nvarchar](50) NOT NULL UNIQUE,
	[DiscountType] [nvarchar](10) NOT NULL DEFAULT 'Fixed',
	[DiscountValue] [decimal](12, 2) NOT NULL,
	[MaxDiscountAmount] [decimal](12, 2) NULL,
	[MinOrderAmount] [decimal](12, 2) NULL,
	[UsageLimitPerUser] [int] NULL,
	[IsStackable] [bit] NOT NULL DEFAULT 0,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Status] [nvarchar](15) NOT NULL DEFAULT 'Active',
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[VoucherID] [int] NULL,
	[StatusID] [int] NOT NULL,
	[ShippingName] [nvarchar](100) NULL,
	[ShippingPhone] [nvarchar](20) NULL,
	[ShippingAddressLine] [nvarchar](500) NULL,
	[ShippingCity] [nvarchar](100) NULL,
	[ShippingWard] [nvarchar](100) NULL,
	[ShippingMethod] [nvarchar](100) NULL,
	[ShippingFee] [decimal](12, 2) NOT NULL DEFAULT 0,
	[OrderDate] [datetime] NOT NULL DEFAULT GETDATE(),
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[PaidByWalletAmount] [decimal](14, 2) NOT NULL DEFAULT 0,
	[PaidByExternalAmount] [decimal](14, 2) NOT NULL DEFAULT 0,
	[PaymentCompletedAt] [datetime] NULL,
	[RefundStatus] [nvarchar](15) NOT NULL DEFAULT 'None',
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL,
	[ShippingDistrict] [nvarchar](255) NULL,
	[ShippingDistrictId] [int] NULL,
	[ShippingWardCode] [varchar](20) NULL,
	[ShippingProvinceId] [int] NULL
)
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL CHECK ([Quantity]>0),
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[UnitPrice] [decimal](12, 2) NOT NULL,
	[Total]  AS ([Quantity]*[UnitPrice]) PERSISTED,
	[Reviewed] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[OrderStatusHistory](
	[OrderStatusHistoryID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OrderID] [int] NOT NULL,
	[StatusID] [int] NULL,
	[ChangedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[ChangedBy] [int] NULL,
	[Note] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)

-- [Payment] & [Wallet] Tables
CREATE TABLE [dbo].[Wallets](
	[WalletID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[Currency] [nvarchar](3) NOT NULL DEFAULT 'VND',
	[Balance] [decimal](14, 2) NOT NULL DEFAULT 0,
	[Status] [nvarchar](10) NOT NULL DEFAULT 'Active',
	[LastTransactionAt] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[WalletTransactions](
	[WalletTransactionID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[WalletID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[TxnType] [nvarchar](20) NOT NULL,
	[Direction] [nvarchar](3) NOT NULL,
	[Amount] [decimal](14, 2) NOT NULL CHECK ([Amount]>0),
	[BalanceBefore] [decimal](14, 2) NOT NULL,
	[BalanceAfter] [decimal](14, 2) NOT NULL,
	[RelatedOrderID] [int] NULL,
	[RelatedPaymentHistoryID] [bigint] NULL,
	[Method] [nvarchar](30) NULL,
	[ExternalRef] [nvarchar](100) NULL,
	[IdempotencyKey] [nvarchar](100) NULL,
	[Status] [nvarchar](12) NOT NULL DEFAULT 'Completed',
	[Reason] [nvarchar](255) NULL,
	[Metadata] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[CompletedAt] [datetime] NULL
)
CREATE TABLE [dbo].[PaymentHistory](
	[PaymentHistoryID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OrderID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[PaymentStatus] [nvarchar](50) NOT NULL DEFAULT 'Failed',
	[TransactionCode] [nvarchar](100) NULL,
	[Amount] [decimal](14, 2) NOT NULL DEFAULT 0,
	[Currency] [nvarchar](3) NOT NULL DEFAULT 'VND',
	[WalletTransactionID] [bigint] NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[PaymentGatewayTransactions](
	[GatewayTransactionID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PaymentHistoryID] [bigint] NOT NULL,
	[Provider] [varchar](50) NOT NULL,
	[TransactionID] [varchar](200) NULL,
	[PaymentUrl] [varchar](1000) NULL,
	[Status] [varchar](50) NOT NULL,
	[Amount] [decimal](14, 2) NOT NULL,
	[GatewayResponse] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[CompletedAt] [datetime] NULL
)

-- [Other Tables]
CREATE TABLE [dbo].[Addresses](
	[AddressID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[AddressLine] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[Ward] [nvarchar](100) NULL,
	[IsDefault] [bit] NOT NULL DEFAULT 0,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL,
	[District] [nvarchar](100) NULL,
	[RecipientName] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[DistrictId] [int] NULL,
	[WardCode] [varchar](20) NULL,
	[ProvinceId] [int] NULL
)
CREATE TABLE [dbo].[BlogCategories](
	[BlogCategoryID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[BlogCategoryName] [nvarchar](100) NOT NULL UNIQUE,
	[Description] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[BlogPosts](
	[BlogPostID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[BlogTitle] [nvarchar](255) NOT NULL,
	[BlogContent] [nvarchar](max) NOT NULL,
	[BlogThumbnail] [nvarchar](500) NULL,
	[BlogUrl] [nvarchar](max) NULL,
	[IsPublished] [bit] NOT NULL DEFAULT 0,
	[IsFeatured] [bit] NOT NULL DEFAULT 0,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[BlogPostCategories](
	[BlogPostID] [int] NOT NULL,
	[BlogCategoryID] [int] NOT NULL,
	PRIMARY KEY ([BlogPostID], [BlogCategoryID])
)
CREATE TABLE [dbo].[EmailOtps](
	[EmailOtpID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NULL,
	[Email] [nvarchar](255) NULL,
	[Purpose] [nvarchar](50) NOT NULL,
	[OtpCode] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[ExpiresAt] [datetime] NOT NULL,
	[IsUsed] [bit] NOT NULL DEFAULT 0
)
CREATE TABLE [dbo].[ExternalLogins](
	[ExternalLoginID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[Provider] [nvarchar](100) NOT NULL,
	[ProviderKey] [nvarchar](255) NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[OrderRefunds](
	[RefundID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OrderID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[RequestedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[WalletTransactionID] [bigint] NULL,
	[RefundMode] [nvarchar](20) NOT NULL DEFAULT 'Wallet',
	[RefundStatus] [nvarchar](20) NOT NULL DEFAULT 'Requested',
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[RefundAmount] [decimal](12, 2) NOT NULL DEFAULT 0,
	[Reason] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[ApprovedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[ShippingProviderTransactions](
	[ShippingTransactionID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[OrderID] [int] NOT NULL,
	[Provider] [varchar](50) NOT NULL,
	[ProviderOrderCode] [varchar](100) NULL,
	[TrackingNumber] [varchar](100) NULL,
	[ServiceType] [nvarchar](100) NULL,
	[Status] [varchar](50) NULL,
	[ShippingFee] [decimal](12, 2) NULL,
	[EstimatedDelivery] [datetime] NULL,
	[ActualDelivery] [datetime] NULL,
	[Metadata] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[SystemConfigs](
	[ConfigKey] [nvarchar](100) NOT NULL PRIMARY KEY,
	[ConfigValue] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[Wishlists](
	[WishlistID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
-- [Review] Tables
CREATE TABLE [dbo].[ReviewProducts](
	[ReviewProductID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[OrderDetailID] [int] NULL,
	[Rating] [int] NOT NULL CHECK ([Rating]>=1 AND [Rating]<=5),
	[Comment] [nvarchar](1000) NULL,
	[IsVerifiedPurchase] [bit] NOT NULL DEFAULT 0,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[Status] [nvarchar](20) NOT NULL DEFAULT 'Pending',
	[Visibility] [nvarchar](20) NOT NULL DEFAULT 'AuthorOnly',
	[ModerationScore] [decimal](3, 2) NULL,
	[ModerationDetail] [nvarchar](max) NULL,
	[EditCount] [int] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[ReviewProductImages](
	[ReviewProductImageID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewProductID] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime] NULL
)
CREATE TABLE [dbo].[ReviewProductReactions](
	[ReactionProductID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewProductID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[ReactionType] [nvarchar](10) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[ReviewProductReplies](
	[ReplyProductID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewProductID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[IsDeleted] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[ReviewBlogs](
	[ReviewBlogID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[BlogPostID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Rating] [int] NOT NULL CHECK ([Rating]>=1 AND [Rating]<=5),
	[Comment] [nvarchar](max) NULL,
	[IsBlocked] [bit] NOT NULL DEFAULT 0,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[ReviewBlogReactions](
	[ReactionBlogID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewBlogID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[ReactionType] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[ReviewBlogReplies](
	[ReplyBlogID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewBlogID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
CREATE TABLE [dbo].[ReviewModerationLogs](
	[ModerationLogID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ReviewProductID] [int] NOT NULL,
	[Stage] [nvarchar](50) NOT NULL,
	[Result] [nvarchar](50) NOT NULL,
	[Score] [decimal](3, 2) NULL,
	[Details] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)
-- [Logs] & [Webhook]
CREATE TABLE [dbo].[ExternalApiLogs](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Provider] [nvarchar](50) NOT NULL,
	[Endpoint] [nvarchar](500) NULL,
	[Method] [nvarchar](10) NULL,
	[RequestPayload] [nvarchar](max) NULL,
	[ResponsePayload] [nvarchar](max) NULL,
	[StatusCode] [int] NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
	[CreatedBy] [int] NULL
)
CREATE TABLE [dbo].[WebhookEvents](
	[WebhookEventID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Provider] [varchar](50) NOT NULL,
	[EventType] [varchar](100) NOT NULL,
	[Payload] [nvarchar](max) NOT NULL,
	[Signature] [varchar](500) NULL,
	[Status] [varchar](20) NOT NULL DEFAULT 'Pending',
	[ProcessedAt] [datetime] NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETDATE()
)


-- [Notification] Tables
CREATE TABLE [Notification].[Templates](
	[TemplateID] [smallint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TemplateCode] [varchar](50) NOT NULL UNIQUE,
	[TitleTemplate] [nvarchar](255) NOT NULL,
	[MessageTemplate] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT 1,
	[CreatedAt] [datetime2](0) NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime2](0) NULL
)
CREATE TABLE [Notification].[Campaigns](
	[CampaignID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CampaignName] [nvarchar](255) NOT NULL,
	[TemplateCode] [varchar](50) NULL,
	[TitleOverride] [nvarchar](255) NULL,
	[MessageOverride] [nvarchar](500) NULL,
	[SourceType] [varchar](10) NOT NULL DEFAULT 'ADMIN',
	[TargetType] [varchar](10) NOT NULL DEFAULT 'ALL',
	[Status] [varchar](15) NOT NULL DEFAULT 'Draft',
	[ScheduledAt] [datetime2](0) NULL,
	[EventKey] [varchar](100) NULL,
	[ImageUrl] [nvarchar](500) NULL,
	[ActionType] [nvarchar](20) NULL,
	[ActionTarget] [nvarchar](500) NULL,
	[CreatedByAccountID] [int] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL DEFAULT GETDATE(),
	[UpdatedAt] [datetime2](0) NULL
)
CREATE TABLE [Notification].[CampaignTargets](
	[CampaignTargetID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CampaignID] [int] NOT NULL,
	[TargetValue] [varchar](200) NOT NULL
)
CREATE TABLE [Notification].[Deliveries](
	[DeliveryID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountID] [int] NOT NULL,
	[CreatedByJobID] [int] NULL,
	[TemplateCode] [varchar](50) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[Payload] [nvarchar](1000) NOT NULL,
	[Status] [varchar](10) NOT NULL DEFAULT 'Unread',
	[CreatedAt] [datetime2](0) NOT NULL DEFAULT GETDATE(),
	[ImageUrl] [nvarchar](500) NULL,
	[ActionType] [nvarchar](20) NULL,
	[ActionTarget] [nvarchar](500) NULL,
	[CampaignID] [int] NULL
)
CREATE TABLE [Notification].[DeliveryActions](
	[ActionID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[DeliveryID] [bigint] NOT NULL,
	[AccountID] [int] NOT NULL,
	[ActionType] [varchar](10) NOT NULL,
	[ActionTarget] [nvarchar](500) NULL,
	[OccurredAt] [datetime2](0) NOT NULL DEFAULT GETDATE()
)

-- [System] Tables
CREATE TABLE [System].[BackgroundJobs](
	[JobID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[JobName] [nvarchar](100) NOT NULL UNIQUE,
	[CronExpression] [nvarchar](100) NULL,
	[IsEnabled] [bit] NOT NULL DEFAULT 1,
	[LastRunTime] [datetime2](7) NULL,
	[NextRunTime] [datetime2](7) NULL,
	[LastRunStatus] [nvarchar](20) NULL,
	[LastRunMessage] [nvarchar](max) NULL
)
GO

-- 4. TẠO KHÓA NGOẠI (FOREIGN KEYS) - (Giữ nguyên cấu trúc nhưng gom gọn)
ALTER TABLE [dbo].[Accounts] ADD CONSTRAINT [FK_Accounts_Roles] FOREIGN KEY([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Addresses] ADD CONSTRAINT [FK_Addresses_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[BlogPostCategories] ADD CONSTRAINT [FK_BlogPostCategories_Categories] FOREIGN KEY([BlogCategoryID]) REFERENCES [dbo].[BlogCategories] ([BlogCategoryID]) ON DELETE CASCADE
ALTER TABLE [dbo].[BlogPostCategories] ADD CONSTRAINT [FK_BlogPostCategories_Posts] FOREIGN KEY([BlogPostID]) REFERENCES [dbo].[BlogPosts] ([BlogPostID]) ON DELETE CASCADE
ALTER TABLE [dbo].[BlogPosts] ADD CONSTRAINT [FK_BlogPosts_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [FK_Cart_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[CartItems] ADD CONSTRAINT [FK_CartItems_Cart] FOREIGN KEY([CartID]) REFERENCES [dbo].[Cart] ([CartID]) ON DELETE CASCADE
ALTER TABLE [dbo].[CartItems] ADD CONSTRAINT [FK_CartItems_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID])
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [FK_Categories_SuperCategories] FOREIGN KEY([SuperCategoryID]) REFERENCES [dbo].[SuperCategories] ([SuperCategoryID]) ON DELETE CASCADE
ALTER TABLE [dbo].[EmailOtps] ADD CONSTRAINT [FK_EmailOtps_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ExternalLogins] ADD CONSTRAINT [FK_ExternalLogins_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID])
ALTER TABLE [dbo].[OrderRefunds] ADD CONSTRAINT [FK_Refunds_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[OrderRefunds] ADD CONSTRAINT [FK_Refunds_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_StatusOrder] FOREIGN KEY([StatusID]) REFERENCES [dbo].[StatusOrders] ([StatusID])
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Orders_Vouchers] FOREIGN KEY([VoucherID]) REFERENCES [dbo].[Voucher] ([VoucherID])
ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OSH_ChangedBy] FOREIGN KEY([ChangedBy]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OSH_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
ALTER TABLE [dbo].[OrderStatusHistory] ADD CONSTRAINT [FK_OSH_StatusOrder] FOREIGN KEY([StatusID]) REFERENCES [dbo].[StatusOrders] ([StatusID])
ALTER TABLE [dbo].[PaymentGatewayTransactions] ADD CONSTRAINT [FK_GatewayTxn_PaymentHistory] FOREIGN KEY([PaymentHistoryID]) REFERENCES [dbo].[PaymentHistory] ([PaymentHistoryID]) ON DELETE CASCADE
ALTER TABLE [dbo].[PaymentHistory] ADD CONSTRAINT [FK_PaymentHistory_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[PaymentHistory] ADD CONSTRAINT [FK_PaymentHistory_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
ALTER TABLE [dbo].[PaymentHistory] ADD CONSTRAINT [FK_PaymentHistory_WalletTxn] FOREIGN KEY([WalletTransactionID]) REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID])
ALTER TABLE [dbo].[ProductImages] ADD CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Ages] FOREIGN KEY([AgeID]) REFERENCES [dbo].[Ages] ([AgeID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Brands] FOREIGN KEY([BrandID]) REFERENCES [dbo].[Brands] ([BrandID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Materials] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Materials] ([MaterialID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Origins] FOREIGN KEY([OriginID]) REFERENCES [dbo].[Origins] ([OriginID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_PriceRanges] FOREIGN KEY([PriceRangeID]) REFERENCES [dbo].[PriceRanges] ([PriceRangeID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Products_Sexes] FOREIGN KEY([SexID]) REFERENCES [dbo].[Sexes] ([SexID]) ON DELETE SET NULL
ALTER TABLE [dbo].[ReviewBlogReactions] ADD CONSTRAINT [FK_ReviewBlogReactions_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[ReviewBlogReactions] ADD CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs] FOREIGN KEY([ReviewBlogID]) REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
ALTER TABLE [dbo].[ReviewBlogReplies] ADD CONSTRAINT [FK_ReviewBlogReplies_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[ReviewBlogReplies] ADD CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs] FOREIGN KEY([ReviewBlogID]) REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
ALTER TABLE [dbo].[ReviewBlogs] ADD CONSTRAINT [FK_ReviewBlogs_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ReviewBlogs] ADD CONSTRAINT [FK_ReviewBlogs_BlogPosts] FOREIGN KEY([BlogPostID]) REFERENCES [dbo].[BlogPosts] ([BlogPostID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ReviewModerationLogs] ADD CONSTRAINT [FK_RML_ReviewProduct] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
ALTER TABLE [dbo].[ReviewProductImages] ADD CONSTRAINT [FK_ReviewProdImages_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ReviewProductReactions] ADD CONSTRAINT [FK_ReviewProdReactions_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[ReviewProductReactions] ADD CONSTRAINT [FK_ReviewProdReactions_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
ALTER TABLE [dbo].[ReviewProductReplies] ADD CONSTRAINT [FK_ReviewProdReplies_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[ReviewProductReplies] ADD CONSTRAINT [FK_ReviewProdReplies_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
ALTER TABLE [dbo].[ReviewProducts] ADD CONSTRAINT [FK_ReviewProducts_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[ReviewProducts] ADD CONSTRAINT [FK_ReviewProducts_OrderDetails] FOREIGN KEY([OrderDetailID]) REFERENCES [dbo].[OrderDetails] ([OrderDetailID])
ALTER TABLE [dbo].[ReviewProducts] ADD CONSTRAINT [FK_ReviewProducts_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID])
ALTER TABLE [dbo].[ShippingProviderTransactions] ADD CONSTRAINT [FK_ShippingTxn_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Voucher] ADD CONSTRAINT [FK_Voucher_Accounts_CreateBy] FOREIGN KEY([CreateBy]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE SET NULL
ALTER TABLE [dbo].[Voucher] ADD CONSTRAINT [FK_Voucher_VoucherTypes] FOREIGN KEY([VoucherTypeID]) REFERENCES [dbo].[VoucherTypes] ([VoucherTypeID])
ALTER TABLE [dbo].[Wallets] ADD CONSTRAINT [FK_Wallets_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[WalletTransactions] ADD CONSTRAINT [FK_WalletTxn_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [dbo].[WalletTransactions] ADD CONSTRAINT [FK_WalletTxn_Orders] FOREIGN KEY([RelatedOrderID]) REFERENCES [dbo].[Orders] ([OrderID])
ALTER TABLE [dbo].[WalletTransactions] ADD CONSTRAINT [FK_WalletTxn_Wallets] FOREIGN KEY([WalletID]) REFERENCES [dbo].[Wallets] ([WalletID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Wishlists] ADD CONSTRAINT [FK_Wishlists_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Wishlists] ADD CONSTRAINT [FK_Wishlists_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE CASCADE
ALTER TABLE [Notification].[Campaigns] ADD CONSTRAINT [FK_Campaigns_Accounts] FOREIGN KEY([CreatedByAccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [Notification].[Campaigns] ADD CONSTRAINT [FK_Campaigns_Templates] FOREIGN KEY([TemplateCode]) REFERENCES [Notification].[Templates] ([TemplateCode]) ON DELETE SET NULL
ALTER TABLE [Notification].[CampaignTargets] ADD CONSTRAINT [FK_CampaignTargets_Campaigns] FOREIGN KEY([CampaignID]) REFERENCES [Notification].[Campaigns] ([CampaignID]) ON DELETE CASCADE
ALTER TABLE [Notification].[Deliveries] ADD CONSTRAINT [FK_NotificationDeliveries_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [Notification].[Deliveries] ADD CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs] FOREIGN KEY([CreatedByJobID]) REFERENCES [System].[BackgroundJobs] ([JobID])
ALTER TABLE [Notification].[Deliveries] ADD CONSTRAINT [FK_NotificationDeliveries_Campaigns] FOREIGN KEY([CampaignID]) REFERENCES [Notification].[Campaigns] ([CampaignID]) ON DELETE SET NULL
ALTER TABLE [Notification].[Deliveries] ADD CONSTRAINT [FK_NotificationDeliveries_Templates] FOREIGN KEY([TemplateCode]) REFERENCES [Notification].[Templates] ([TemplateCode])
ALTER TABLE [Notification].[DeliveryActions] ADD CONSTRAINT [FK_DeliveryActions_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
ALTER TABLE [Notification].[DeliveryActions] ADD CONSTRAINT [FK_DeliveryActions_Deliveries] FOREIGN KEY([DeliveryID]) REFERENCES [Notification].[Deliveries] ([DeliveryID]) ON DELETE CASCADE
GO

-- 5. INSERT DỮ LIỆU CƠ BẢN (Essential Data)
-- [Roles]
SET IDENTITY_INSERT [dbo].[Roles] ON 
INSERT [dbo].[Roles] ([RoleID], [RoleName], [Description]) VALUES 
(1, N'Customer', N'Khách hàng'),
(2, N'Staff', N'Nhân viên CSKH'),
(3, N'Warehouse', N'Nhân viên kho'),
(4, N'Admin', N'Quản trị hệ thống')
SET IDENTITY_INSERT [dbo].[Roles] OFF

-- [Accounts] (Admin, Staff, Warehouse, Customer Demo)
SET IDENTITY_INSERT [dbo].[Accounts] ON 
INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Password], [Status]) VALUES 
(1, 2, N'staff01', N'0900000001', N'staff@lorkingdom.com', N'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', N'Active'),
(2, 3, N'warehouse01', N'0900000002', N'warehouse@lorkingdom.com', N'$2b$11$ACDZHTdYJs7tFYezEOO1VOJUilPqZv8aaLz28J5GWwCjSIsUlSKtS', N'Active'),
(3, 4, N'admin01', N'0900000003', N'admin@lorkingdom.com', N'$2b$11$puQ/vlg2ytfmh3VgmCK/wePQdbILFqu5/VkRByoQpHbvs91tHgFCG', N'Active'),
(4, 1, N'Khang', N'0382733605', N'khangtv1502@gmail.com', N'$2a$11$K2h8UjcDY.nEuXvosWBKD.nQKSJeyOcsSrEZeWxLcOFxVxoxxfPyC', N'Active')
SET IDENTITY_INSERT [dbo].[Accounts] OFF

-- [Categories]
SET IDENTITY_INSERT [dbo].[SuperCategories] ON 
INSERT [dbo].[SuperCategories] ([SuperCategoryID], [SuperCategoryName]) VALUES (1, N'Đồ chơi')
SET IDENTITY_INSERT [dbo].[SuperCategories] OFF

SET IDENTITY_INSERT [dbo].[Categories] ON 
INSERT [dbo].[Categories] ([CategoryID], [SuperCategoryID], [CategoryName]) VALUES (1, 1, N'Đồ chơi trẻ em')
SET IDENTITY_INSERT [dbo].[Categories] OFF

-- [Brands] & [PriceRanges]
SET IDENTITY_INSERT [dbo].[Brands] ON 
INSERT [dbo].[Brands] ([BrandID], [BrandName]) VALUES (1, N'Lego'), (2, N'Hasbro')
SET IDENTITY_INSERT [dbo].[Brands] OFF

SET IDENTITY_INSERT [dbo].[PriceRanges] ON 
INSERT [dbo].[PriceRanges] ([PriceRangeID], [PriceRangeMin], [PriceRangeMax]) VALUES 
(1, 100000.00, 500000.00),
(2, 500000.00, 1000000.00)
SET IDENTITY_INSERT [dbo].[PriceRanges] OFF

-- [Products]
SET IDENTITY_INSERT [dbo].[Products] ON 
INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [PriceRangeID], [BrandID], [ProductName], [Price], [Quantity]) VALUES 
(1, N'SKU-LEGO-01', 1, 1, 1, N'Lego City Police', 350000.00, 54),
(2, N'SKU-LEGO-02', 1, 1, 1, N'Lego Fire Station', 420000.00, 75),
(3, N'SKU-HAS-01', 1, 2, 2, N'Transformers Optimus', 850000.00, 45),
(4, N'SKU-HAS-02', 1, 2, 2, N'Monopoly Classic', 650000.00, 56),
(5, N'SKU-HAS-03', 1, 1, 2, N'Play-Doh Color Set', 250000.00, 196)
SET IDENTITY_INSERT [dbo].[Products] OFF

-- [StatusOrders]
SET IDENTITY_INSERT [dbo].[StatusOrders] ON 
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description]) VALUES 
(1, N'Pending', N'Đơn hàng mới tạo, chưa xác nhận'),
(2, N'Confirmed', N'Đơn hàng đã được xác nhận'),
(3, N'Shipped', N'Đơn hàng đang được giao'),
(4, N'Delivered', N'Đơn hàng đã giao thành công'),
(5, N'Completed', N'Đơn hàng hoàn tất'),
(6, N'Cancelled', N'Đơn hàng bị hủy'),
(7, N'Refunded', N'Đã hoàn tiền')

SET IDENTITY_INSERT [dbo].[StatusOrders] OFF

-- [Notification.Templates]
SET IDENTITY_INSERT [Notification].[Templates] ON 
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate]) VALUES 
(1, N'ORDER_CONFIRMED', N'Đơn hàng #{{orderCode}} đã được xác nhận', N'Chào {{customerName}}, đơn hàng #{{orderCode}} của bạn đã được xác nhận.'),
(2, N'ORDER_SHIPPED', N'Đơn hàng #{{orderCode}} đang được giao', N'Đơn hàng #{{orderCode}} đang được giao cho đơn vị vận chuyển {{shippingUnit}}.'),
(3, N'ORDER_DELIVERED', N'Đơn hàng #{{orderCode}} đã giao thành công', N'Cảm ơn bạn đã mua hàng!'),
(4, N'PROMOTION', N'🎁 Khuyến mãi: {{promotionName}}', N'{{description}}'),
(5, N'WELCOME', N'Chào mừng {{userName}} đến với LorKingDom!', N'Chúc mừng bạn đã tạo tài khoản thành công.'),
(6, N'PAYMENT_SUCCESS', N'Thanh toán thành công #{{paymentId}}', N'Giao dịch thanh toán {{amount}} VNĐ thành công.')
SET IDENTITY_INSERT [Notification].[Templates] OFF

GO