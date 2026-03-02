USE [ASP_LorKingDom]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* =============================================
   0. TẠO SCHEMA
   ============================================= */
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'System')
    EXEC('CREATE SCHEMA [System]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Notification')
    EXEC('CREATE SCHEMA [Notification]');
GO


/* =============================================
   1. TẠO CÁC BẢNG THEO THỨ TỰ DEPENDENCY
   ============================================= */


-- ============= LEVEL 0: Bảng không phụ thuộc =============
CREATE TABLE [dbo].[Roles](
    [RoleID] [int] IDENTITY(1,1) NOT NULL,
    [RoleName] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([RoleID] ASC),
UNIQUE NONCLUSTERED ([RoleName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[SuperCategories](
    [SuperCategoryID] [int] IDENTITY(1,1) NOT NULL,
    [SuperCategoryName] [nvarchar](255) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([SuperCategoryID] ASC),
UNIQUE NONCLUSTERED ([SuperCategoryName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Ages](
    [AgeID] [int] IDENTITY(1,1) NOT NULL,
    [AgeRange] [nvarchar](50) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([AgeID] ASC),
UNIQUE NONCLUSTERED ([AgeRange] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Sexes](
    [SexID] [int] IDENTITY(1,1) NOT NULL,
    [SexName] [nvarchar](20) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([SexID] ASC),
UNIQUE NONCLUSTERED ([SexName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Brands](
    [BrandID] [int] IDENTITY(1,1) NOT NULL,
    [BrandName] [nvarchar](100) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([BrandID] ASC),
UNIQUE NONCLUSTERED ([BrandName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Origins](
    [OriginID] [int] IDENTITY(1,1) NOT NULL,
    [OriginName] [nvarchar](255) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([OriginID] ASC),
UNIQUE NONCLUSTERED ([OriginName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Materials](
    [MaterialID] [int] IDENTITY(1,1) NOT NULL,
    [MaterialName] [nvarchar](255) NOT NULL,
    [Description] [nvarchar](max) NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([MaterialID] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[PriceRanges](
    [PriceRangeID] [int] IDENTITY(1,1) NOT NULL,
    [PriceRangeMin] [decimal](12, 2) NOT NULL,
    [PriceRangeMax] [decimal](12, 2) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([PriceRangeID] ASC),
CONSTRAINT [CK_PriceRanges_MinMax] CHECK (([PriceRangeMin]<=[PriceRangeMax]))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[StatusOrders](
    [StatusID] [int] IDENTITY(1,1) NOT NULL,
    [StatusName] [nvarchar](50) NOT NULL DEFAULT 'Pending',
    [Description] [nvarchar](max) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([StatusID] ASC),
CHECK (([StatusName]='Cancelled' OR [StatusName]='Confirmed' OR [StatusName]='Delivered' OR [StatusName]='Shipped' OR [StatusName]='Pending' OR [StatusName]='Completed' OR [StatusName]='Refunded'))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[VoucherTypes](
    [VoucherTypeID] [int] IDENTITY(1,1) NOT NULL,
    [VoucherTypeName] [nvarchar](255) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([VoucherTypeID] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[BlogCategories](
    [BlogCategoryID] [int] IDENTITY(1,1) NOT NULL,
    [BlogCategoryName] [nvarchar](100) NOT NULL,
    [Description] [nvarchar](500) NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([BlogCategoryID] ASC),
UNIQUE NONCLUSTERED ([BlogCategoryName] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[SystemConfigs](
    [ConfigKey] [nvarchar](100) NOT NULL,
    [ConfigValue] [nvarchar](max) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [UpdatedAt] [datetime] DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ConfigKey] ASC)
) ON [PRIMARY]
GO


CREATE TABLE [System].[BackgroundJobs] (
    [JobID]             INT IDENTITY(1,1) PRIMARY KEY,
    [JobName]           NVARCHAR(100)  NOT NULL UNIQUE,
    [CronExpression]    NVARCHAR(100)  NULL,
    [IsEnabled]         BIT            NOT NULL DEFAULT 1,
    [LastRunTime]       DATETIME2(7)   NULL,
    [NextRunTime]       DATETIME2(7)   NULL,
    [LastRunStatus]     NVARCHAR(20)   NULL,
    [LastRunMessage]    NVARCHAR(MAX)  NULL
);
GO


CREATE TABLE [Notification].[Templates] (
    [TemplateID] SMALLINT IDENTITY(1,1) PRIMARY KEY,
    [TemplateCode] VARCHAR(50) NOT NULL UNIQUE,
    [TitleTemplate] NVARCHAR(255) NOT NULL,
    [MessageTemplate] NVARCHAR(500) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME2(0) NULL
);
GO


-- ============= LEVEL 1: Bảng phụ thuộc vào Roles =============
CREATE TABLE [dbo].[Accounts](
    [AccountID] [int] IDENTITY(1,1) NOT NULL,
    [RoleID] [int] NULL,
    [AccountName] [nvarchar](100) NOT NULL,
    [PhoneNumber] [varchar](15) NULL,
    [Email] [nvarchar](255) NOT NULL,
    [Image] [nvarchar](500) NULL,
    [Password] [nvarchar](255) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [Status] [nvarchar](10) NOT NULL DEFAULT 'Active',
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [Provider] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED ([AccountID] ASC),
UNIQUE NONCLUSTERED ([Email] ASC),
CONSTRAINT [FK_Accounts_Roles] FOREIGN KEY([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE SET NULL,
CHECK (([Status]='Blocked' OR [Status]='Inactive' OR [Status]='Active'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Categories](
    [CategoryID] [int] IDENTITY(1,1) NOT NULL,
    [SuperCategoryID] [int] NOT NULL,
    [CategoryName] [nvarchar](255) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([CategoryID] ASC),
CONSTRAINT [FK_Categories_SuperCategories] FOREIGN KEY([SuperCategoryID]) REFERENCES [dbo].[SuperCategories] ([SuperCategoryID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


-- ============= LEVEL 2: Bảng phụ thuộc vào Accounts =============
CREATE TABLE [dbo].[Addresses](
    [AddressID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [AddressLine] [nvarchar](500) NOT NULL,
    [City] [nvarchar](100) NOT NULL,
    [District] [nvarchar](100) NULL,
    [Ward] [nvarchar](100) NULL,
    [IsDefault] [bit] NOT NULL DEFAULT 0,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [RecipientName] [nvarchar](100) NULL,        -- Tên người nhận hàng
    [PhoneNumber] [varchar](15) NULL,             -- SĐT người nhận
    [ProvinceId] [int] NULL,                      -- GHN Province ID
    [DistrictId] [int] NULL,                      -- GHN District ID
    [WardCode] [nvarchar](10) NULL,               -- GHN Ward Code (e.g. "21211")
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([AddressID] ASC),
CONSTRAINT [FK_Addresses_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[EmailOtps](
    [EmailOtpID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NULL,
    [Email] [nvarchar](255) NULL,
    [Purpose] [nvarchar](50) NOT NULL,
    [OtpCode] [nvarchar](10) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [ExpiresAt] [datetime] NOT NULL,
    [IsUsed] [bit] NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED ([EmailOtpID] ASC),
CONSTRAINT [FK_EmailOtps_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE,
CONSTRAINT [CK_EmailOtps_Target] CHECK (([AccountID] IS NOT NULL OR [Email] IS NOT NULL))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ExternalLogins](
    [ExternalLoginID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [Provider] [nvarchar](100) NOT NULL,
    [ProviderKey] [nvarchar](255) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ExternalLoginID] ASC),
CONSTRAINT [UQ_ExternalLogins] UNIQUE NONCLUSTERED ([Provider] ASC, [ProviderKey] ASC),
CONSTRAINT [FK_ExternalLogins_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
) ON [PRIMARY]
GO




CREATE TABLE [dbo].[Voucher](
    [VoucherID] [int] IDENTITY(1,1) NOT NULL,
    [VoucherTypeID] [int] NOT NULL,
    [CreateBy] [int] NULL,
    [VoucherCode] [nvarchar](50) NOT NULL,
    [DiscountValue] [decimal](12, 2) NOT NULL,
    [MinOrderAmount] [decimal](12, 2) NULL,
    [UsageLimitPerUser] [int] NULL,
    [IsStackable] [bit] NOT NULL DEFAULT 0,
    [StartDate] [datetime] NOT NULL,
    [EndDate] [datetime] NOT NULL,
    [Status] [nvarchar](15) NOT NULL DEFAULT 'Active',
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([VoucherID] ASC),
UNIQUE NONCLUSTERED ([VoucherCode] ASC),
CONSTRAINT [FK_Voucher_Accounts_CreateBy] FOREIGN KEY([CreateBy]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE SET NULL,
CONSTRAINT [FK_Voucher_VoucherTypes] FOREIGN KEY([VoucherTypeID]) REFERENCES [dbo].[VoucherTypes] ([VoucherTypeID]),
CONSTRAINT [CK_Voucher_Dates] CHECK (([StartDate]<[EndDate])),
CHECK (([DiscountValue]>=(0))),
CHECK (([MinOrderAmount] IS NULL OR [MinOrderAmount]>=(0))),
CHECK (([Status]='Expired' OR [Status]='Inactive' OR [Status]='Active')),
CHECK (([UsageLimitPerUser] IS NULL OR [UsageLimitPerUser]>(0)))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Wallets](
    [WalletID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'VND',
    [Balance] [decimal](14, 2) NOT NULL DEFAULT 0,
    [Status] [nvarchar](10) NOT NULL DEFAULT 'Active',
    [LastTransactionAt] [datetime] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([WalletID] ASC),
UNIQUE NONCLUSTERED ([AccountID] ASC),
CONSTRAINT [FK_Wallets_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE,
CHECK (([Balance]>=(0))),
CHECK (([Status]='Closed' OR [Status]='Frozen' OR [Status]='Active'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Cart](
    [CartID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([CartID] ASC),
CONSTRAINT [FK_Cart_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[BlogPosts](
    [BlogPostID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [BlogTitle] [nvarchar](255) NOT NULL,
    [BlogContent] [nvarchar](max) NOT NULL,
    [BlogThumbnail] [nvarchar](500) NULL,
    [BlogUrl] [nvarchar](max) NULL,
    [IsPublished] [bit] NOT NULL DEFAULT 0,
    [IsFeatured] [bit] NOT NULL DEFAULT 0,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([BlogPostID] ASC),
CONSTRAINT [FK_BlogPosts_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[BlogPostCategories](
    [BlogPostID] [int] NOT NULL,
    [BlogCategoryID] [int] NOT NULL,
PRIMARY KEY CLUSTERED ([BlogPostID] ASC, [BlogCategoryID] ASC),
CONSTRAINT [FK_BlogPostCategories_Posts] FOREIGN KEY([BlogPostID]) REFERENCES [dbo].[BlogPosts] ([BlogPostID]) ON DELETE CASCADE,
CONSTRAINT [FK_BlogPostCategories_Categories] FOREIGN KEY([BlogCategoryID]) REFERENCES [dbo].[BlogCategories] ([BlogCategoryID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ExternalApiLogs](
    [LogID] [bigint] IDENTITY(1,1) NOT NULL,
    [Provider] [nvarchar](50) NOT NULL,
    [Endpoint] [nvarchar](500) NULL,
    [Method] [nvarchar](10) NULL,
    [RequestPayload] [nvarchar](max) NULL,
    [ResponsePayload] [nvarchar](max) NULL,
    [StatusCode] [int] NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [CreatedBy] [int] NULL,
PRIMARY KEY CLUSTERED ([LogID] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[WebhookEvents](
    [WebhookEventID] [bigint] IDENTITY(1,1) NOT NULL,
    [Provider] [varchar](50) NOT NULL,
    [EventType] [varchar](100) NOT NULL,
    [Payload] [nvarchar](max) NOT NULL,
    [Signature] [varchar](500) NULL,
    [Status] [varchar](20) NOT NULL DEFAULT 'Pending',
    [ProcessedAt] [datetime] NULL,
    [ErrorMessage] [nvarchar](max) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([WebhookEventID] ASC),
CHECK (([Status]='Failed' OR [Status]='Processed' OR [Status]='Pending'))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [Notification].[Deliveries] (
    [DeliveryID] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [AccountID] INT NOT NULL,
    [CreatedByJobID] INT NULL, -- NULL = User/System trigger, có ID = Worker tự động tạo
    [TemplateCode] VARCHAR(50) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Message] NVARCHAR(500) NOT NULL,  
    [Payload] NVARCHAR(1000) NOT NULL, -- JSON Data
    [Status] VARCHAR(10) NOT NULL DEFAULT 'Unread',
    [CreatedAt] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_NotificationDeliveries_Accounts] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts]([AccountID]),
    CONSTRAINT [FK_NotificationDeliveries_Templates] FOREIGN KEY ([TemplateCode]) REFERENCES [Notification].[Templates]([TemplateCode]),
    CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs] FOREIGN KEY ([CreatedByJobID]) REFERENCES [System].[BackgroundJobs]([JobID])
);
GO


-- ============= LEVEL 3: Products và related =============
CREATE TABLE [dbo].[Products](
    [ProductID] [int] IDENTITY(1,1) NOT NULL,
    [SKU] [varchar](50) NOT NULL,
    [CategoryID] [int] NULL,
    [MaterialID] [int] NULL,
    [AgeID] [int] NULL,
    [SexID] [int] NULL,
    [PriceRangeID] [int] NULL,
    [BrandID] [int] NULL,
    [OriginID] [int] NULL,
    [ProductName] [nvarchar](255) NOT NULL,
    [Price] [decimal](12, 2) NOT NULL,
    [Quantity] [int] NOT NULL DEFAULT 0,
    [ProductStatus] [nvarchar](15) NOT NULL DEFAULT 'Available',
    [Description] [nvarchar](max) NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([ProductID] ASC),
UNIQUE NONCLUSTERED ([SKU] ASC),
CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID]) REFERENCES [dbo].[Categories] ([CategoryID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_Materials] FOREIGN KEY([MaterialID]) REFERENCES [dbo].[Materials] ([MaterialID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_Ages] FOREIGN KEY([AgeID]) REFERENCES [dbo].[Ages] ([AgeID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_Sexes] FOREIGN KEY([SexID]) REFERENCES [dbo].[Sexes] ([SexID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_PriceRanges] FOREIGN KEY([PriceRangeID]) REFERENCES [dbo].[PriceRanges] ([PriceRangeID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_Brands] FOREIGN KEY([BrandID]) REFERENCES [dbo].[Brands] ([BrandID]) ON DELETE SET NULL,
CONSTRAINT [FK_Products_Origins] FOREIGN KEY([OriginID]) REFERENCES [dbo].[Origins] ([OriginID]) ON DELETE SET NULL,
CHECK (([Price]>=(0))),
CHECK (([ProductStatus]='Discontinued' OR [ProductStatus]='OutOfStock' OR [ProductStatus]='Available')),
CHECK (([Quantity]>=(0)))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ProductImages](
    [ImageID] [int] IDENTITY(1,1) NOT NULL,
    [ProductID] [int] NOT NULL,
    [ImageUrl] [nvarchar](500) NOT NULL,
    [IsMain] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ImageID] ASC),
CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[CartItems](
    [CartItemID] [int] IDENTITY(1,1) NOT NULL,
    [CartID] [int] NOT NULL,
    [ProductID] [int] NOT NULL,
    [Quantity] [int] NOT NULL,
    [PriceAtThatTime] [decimal](12, 2) NOT NULL,
    [Status] [nvarchar](15) NOT NULL DEFAULT 'Active',
    [AddedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([CartItemID] ASC),
CONSTRAINT [FK_CartItems_Cart] FOREIGN KEY([CartID]) REFERENCES [dbo].[Cart] ([CartID]) ON DELETE CASCADE,
CONSTRAINT [FK_CartItems_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]),
CHECK (([PriceAtThatTime]>=(0))),
CHECK (([Quantity]>(0))),
CHECK (([Status]='Purchased' OR [Status]='Removed' OR [Status]='Active'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Wishlists](
    [WishlistID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [ProductID] [int] NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([WishlistID] ASC),
CONSTRAINT [UQ_Wishlists] UNIQUE NONCLUSTERED ([AccountID] ASC, [ProductID] ASC),
CONSTRAINT [FK_Wishlists_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE,
CONSTRAINT [FK_Wishlists_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


-- ============= LEVEL 4: Orders và related =============
CREATE TABLE [dbo].[Orders](
    [OrderID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [VoucherID] [int] NULL,
    [StatusID] [int] NOT NULL,
    [ShippingName] [nvarchar](100) NULL,
    [ShippingPhone] [nvarchar](20) NULL,
    [ShippingAddressLine] [nvarchar](500) NULL,
    [ShippingCity] [nvarchar](100) NULL,
    [ShippingDistrict] [nvarchar](100) NULL,
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
PRIMARY KEY CLUSTERED ([OrderID] ASC),
CONSTRAINT [FK_Orders_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_Orders_Vouchers] FOREIGN KEY([VoucherID]) REFERENCES [dbo].[Voucher] ([VoucherID]),
CONSTRAINT [FK_Orders_StatusOrder] FOREIGN KEY([StatusID]) REFERENCES [dbo].[StatusOrders] ([StatusID]),
CHECK (([RefundStatus]='Full' OR [RefundStatus]='Rejected' OR [RefundStatus]='Requested' OR [RefundStatus]='None')),
CHECK (([TotalAmount]>=(0)))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[OrderDetails](
    [OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
    [OrderID] [int] NOT NULL,
    [ProductID] [int] NOT NULL,
    [Quantity] [int] NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [UnitPrice] [decimal](12, 2) NOT NULL,
    [Total] AS ([Quantity] * [UnitPrice]) PERSISTED,
    [Reviewed] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([OrderDetailID] ASC),
CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE,
CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]),
CHECK (([Quantity]>(0))),
CHECK (([UnitPrice]>=(0)))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[OrderStatusHistory](
    [OrderStatusHistoryID] [int] IDENTITY(1,1) NOT NULL,
    [OrderID] [int] NOT NULL,
    [StatusID] [int] NULL,
    [ChangedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [ChangedBy] [int] NULL,
    [Note] [nvarchar](500) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([OrderStatusHistoryID] ASC),
CONSTRAINT [FK_OSH_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE,
CONSTRAINT [FK_OSH_StatusOrder] FOREIGN KEY([StatusID]) REFERENCES [dbo].[StatusOrders] ([StatusID]),
CONSTRAINT [FK_OSH_ChangedBy] FOREIGN KEY([ChangedBy]) REFERENCES [dbo].[Accounts] ([AccountID])
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[WalletTransactions](
    [WalletTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
    [WalletID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [TxnType] [nvarchar](20) NOT NULL,
    [Direction] [nvarchar](3) NOT NULL,
    [Amount] [decimal](14, 2) NOT NULL,
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
    [CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([WalletTransactionID] ASC),
CONSTRAINT [FK_WalletTxn_Wallets] FOREIGN KEY([WalletID]) REFERENCES [dbo].[Wallets] ([WalletID]) ON DELETE CASCADE,
CONSTRAINT [FK_WalletTxn_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_WalletTxn_Orders] FOREIGN KEY([RelatedOrderID]) REFERENCES [dbo].[Orders] ([OrderID]),
CHECK (([Amount]>(0))),
CHECK (([BalanceBefore]>=(0))),
CHECK (([BalanceAfter]>=(0))),
CHECK (([Direction]='DR' OR [Direction]='CR')),
CHECK (([Method] IS NULL OR ([Method]='Wallet' OR [Method]='Cash' OR [Method]='EWallet' OR [Method]='Bank'))),
CHECK (([Status]='Cancelled' OR [Status]='Failed' OR [Status]='Completed' OR [Status]='Pending')),
CHECK (([TxnType]='Refund' OR [TxnType]='Payment' OR [TxnType]='TopUp'))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[PaymentHistory](
    [PaymentHistoryID] [bigint] IDENTITY(1,1) NOT NULL,
    [OrderID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [PaymentMethod] [nvarchar](50) NOT NULL,
    [PaymentStatus] [nvarchar](50) NOT NULL DEFAULT 'Failed',
    [TransactionCode] [nvarchar](100) NULL,
    [Amount] [decimal](14, 2) NOT NULL DEFAULT 0,
    [Currency] [nvarchar](3) NOT NULL DEFAULT 'VND',
    [WalletTransactionID] [bigint] NULL,
    [Note] [nvarchar](255) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([PaymentHistoryID] ASC),
CONSTRAINT [FK_PaymentHistory_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE,
CONSTRAINT [FK_PaymentHistory_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_PaymentHistory_WalletTxn] FOREIGN KEY([WalletTransactionID]) REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID]),
CHECK (([PaymentStatus] = 'Pending' OR [PaymentStatus] = 'Failed' OR [PaymentStatus] = 'Paid'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[PaymentGatewayTransactions](
    [GatewayTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
    [PaymentHistoryID] [bigint] NOT NULL,
    [Provider] [varchar](50) NOT NULL,
    [TransactionID] [varchar](200) NULL,
    [PaymentUrl] [nvarchar](500) NULL,
    [Status] [varchar](50) NOT NULL,
    [Amount] [decimal](14, 2) NOT NULL,
    [GatewayResponse] [nvarchar](max) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([GatewayTransactionID] ASC),
CONSTRAINT [FK_GatewayTxn_PaymentHistory] FOREIGN KEY([PaymentHistoryID]) REFERENCES [dbo].[PaymentHistory] ([PaymentHistoryID]) ON DELETE CASCADE
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ShippingProviderTransactions](
    [ShippingTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
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
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([ShippingTransactionID] ASC),
CONSTRAINT [FK_ShippingTxn_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[OrderRefunds](
    [RefundID] [bigint] IDENTITY(1,1) NOT NULL,
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
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([RefundID] ASC),
CONSTRAINT [FK_Refunds_Orders] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Orders] ([OrderID]) ON DELETE CASCADE,
CONSTRAINT [FK_Refunds_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_Refunds_RequestedBy] FOREIGN KEY([RequestedBy]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_Refunds_ApprovedBy] FOREIGN KEY([ApprovedBy]) REFERENCES [dbo].[Accounts] ([AccountID]),
CONSTRAINT [FK_Refunds_WalletTxn] FOREIGN KEY([WalletTransactionID]) REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID]),
CONSTRAINT [CK_OrderRefunds_Amounts] CHECK (([RefundAmount]<=[TotalAmount])),
CONSTRAINT [CK_OrderRefunds_Mode] CHECK (([RefundMode]='Cash' OR [RefundMode]='BankTransfer' OR [RefundMode]='OriginalPayment' OR [RefundMode]='Wallet')),
CONSTRAINT [CK_OrderRefunds_RefundAmount] CHECK (([RefundAmount]>=(0))),
CONSTRAINT [CK_OrderRefunds_Status] CHECK (([RefundStatus]='Cancelled' OR [RefundStatus]='Rejected' OR [RefundStatus]='Refunded' OR [RefundStatus]='Processing' OR [RefundStatus]='Approved' OR [RefundStatus]='Requested')),
CONSTRAINT [CK_OrderRefunds_TotalAmount] CHECK (([TotalAmount]>=(0)))
) ON [PRIMARY]
GO


-- ============= LEVEL 5: Review tables =============
CREATE TABLE [dbo].[ReviewBlogs](
    [ReviewBlogID] [int] IDENTITY(1,1) NOT NULL,
    [BlogPostID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [Rating] [int] NOT NULL,
    [Comment] [nvarchar](max) NULL,
    [IsBlocked] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ReviewBlogID] ASC),
CONSTRAINT [UQ_ReviewBlogs_PostAccount] UNIQUE NONCLUSTERED ([AccountID] ASC, [BlogPostID] ASC),
CONSTRAINT [FK_ReviewBlogs_BlogPosts] FOREIGN KEY([BlogPostID]) REFERENCES [dbo].[BlogPosts] ([BlogPostID]) ON DELETE CASCADE,
CONSTRAINT [FK_ReviewBlogs_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE,
CHECK (([Rating]>=(1) AND [Rating]<=(5)))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewBlogReactions](
    [ReactionBlogID] [int] IDENTITY(1,1) NOT NULL,
    [ReviewBlogID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [ReactionType] [nvarchar](10) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ReactionBlogID] ASC),
CONSTRAINT [UQ_ReviewBlogReactions] UNIQUE NONCLUSTERED ([ReviewBlogID] ASC, [AccountID] ASC),
CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs] FOREIGN KEY([ReviewBlogID]) REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID]),
CONSTRAINT [FK_ReviewBlogReactions_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CHECK (([ReactionType]='Dislike' OR [ReactionType]='Like'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewBlogReplies](
    [ReplyBlogID] [int] IDENTITY(1,1) NOT NULL,
    [ReviewBlogID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [Content] [nvarchar](1000) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ReplyBlogID] ASC),
CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs] FOREIGN KEY([ReviewBlogID]) REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID]),
CONSTRAINT [FK_ReviewBlogReplies_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewProducts](
    [ReviewProductID] [int] IDENTITY(1,1) NOT NULL,
    [AccountID] [int] NOT NULL,
    [ProductID] [int] NOT NULL,
    [OrderDetailID] [int] NULL,
    [Rating] [int] NOT NULL,
    [Comment] [nvarchar](1000) NULL,
    [IsVerifiedPurchase] [bit] NOT NULL DEFAULT 0,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [Status] [nvarchar](20) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected, UnderReview
    [Visibility] [nvarchar](20) NOT NULL DEFAULT 'AuthorOnly', -- AuthorOnly, Public
    [ModerationScore] [decimal](3, 2) NULL, -- 0.00 - 1.00
    [ModerationDetail] [nvarchar](MAX) NULL, -- JSON từ AI
    [EditCount] [int] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([ReviewProductID] ASC),
CONSTRAINT [FK_ReviewProducts_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]) ON DELETE CASCADE,
CONSTRAINT [FK_ReviewProducts_Products] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Products] ([ProductID]),
CONSTRAINT [FK_ReviewProducts_OrderDetails] FOREIGN KEY([OrderDetailID]) REFERENCES [dbo].[OrderDetails] ([OrderDetailID]),
CHECK (([Rating]>=(1) AND [Rating]<=(5)))
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewProductImages](
    [ReviewProductImageID] [int] IDENTITY(1,1) NOT NULL,
    [ReviewProductID] [int] NOT NULL,
    [ImageUrl] [nvarchar](500) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED ([ReviewProductImageID] ASC),
CONSTRAINT [FK_ReviewProdImages_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID]) ON DELETE CASCADE
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewProductReactions](
    [ReactionProductID] [int] IDENTITY(1,1) NOT NULL,
    [ReviewProductID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [ReactionType] [nvarchar](10) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ReactionProductID] ASC),
CONSTRAINT [UQ_ReviewProductReactions] UNIQUE NONCLUSTERED ([ReviewProductID] ASC, [AccountID] ASC),
CONSTRAINT [FK_ReviewProdReactions_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID]),
CONSTRAINT [FK_ReviewProdReactions_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID]),
CHECK (([ReactionType]='Dislike' OR [ReactionType]='Like'))
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReviewProductReplies](
    [ReplyProductID] [int] IDENTITY(1,1) NOT NULL,
    [ReviewProductID] [int] NOT NULL,
    [AccountID] [int] NOT NULL,
    [Content] [nvarchar](1000) NOT NULL,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
PRIMARY KEY CLUSTERED ([ReplyProductID] ASC),
CONSTRAINT [FK_ReviewProdReplies_ReviewProducts] FOREIGN KEY([ReviewProductID]) REFERENCES [dbo].[ReviewProducts] ([ReviewProductID]),
CONSTRAINT [FK_ReviewProdReplies_Accounts] FOREIGN KEY([AccountID]) REFERENCES [dbo].[Accounts] ([AccountID])
) ON [PRIMARY]
GO


-- Bảng log chi tiết moderation
CREATE TABLE dbo.ReviewModerationLogs (
    ModerationLogID INT IDENTITY(1,1) PRIMARY KEY,
    ReviewProductID INT NOT NULL,
    Stage NVARCHAR(50) NOT NULL, -- Stage1, Stage2, Stage3
    Result NVARCHAR(50) NOT NULL, -- Passed, Rejected, Flagged
    Score DECIMAL(3,2) NULL,
    Details NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_RML_ReviewProduct FOREIGN KEY (ReviewProductID) REFERENCES dbo.ReviewProducts(ReviewProductID)
);
GO




/* =============================================
   2. TẠO INDEX ĐỂ TĂNG PERFORMANCE
   ============================================= */


-- Index cho Foreign Keys thường query
CREATE NONCLUSTERED INDEX [IX_Accounts_RoleID] ON [dbo].[Accounts]([RoleID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_Products_CategoryID] ON [dbo].[Products]([CategoryID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_Orders_AccountID] ON [dbo].[Orders]([AccountID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_Orders_StatusID] ON [dbo].[Orders]([StatusID]);
GO
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderID] ON [dbo].[OrderDetails]([OrderID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductID] ON [dbo].[OrderDetails]([ProductID]);
GO


-- Index cho Notification queries
CREATE NONCLUSTERED INDEX [IX_Deliveries_AccountID_Status] ON [Notification].[Deliveries]([AccountID], [Status]) INCLUDE ([Title], [Message], [CreatedAt]);
GO




-- Index cho Product search/filter
CREATE NONCLUSTERED INDEX [IX_Products_Price] ON [dbo].[Products]([Price]) WHERE [IsDeleted] = 0 AND [ProductStatus] = 'Available';
GO
CREATE NONCLUSTERED INDEX [IX_Products_BrandID] ON [dbo].[Products]([BrandID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_Products_CreatedAt] ON [dbo].[Products]([CreatedAt] DESC) WHERE [IsDeleted] = 0;
GO


-- Index cho Cart operations
CREATE NONCLUSTERED INDEX [IX_Cart_AccountID] ON [dbo].[Cart]([AccountID]);
GO
CREATE NONCLUSTERED INDEX [IX_CartItems_CartID_Status] ON [dbo].[CartItems]([CartID], [Status]);
GO


-- Index cho Payment/Wallet
CREATE NONCLUSTERED INDEX [IX_PaymentHistory_OrderID] ON [dbo].[PaymentHistory]([OrderID]);
GO
CREATE NONCLUSTERED INDEX [IX_WalletTransactions_WalletID] ON [dbo].[WalletTransactions]([WalletID], [CreatedAt] DESC);
GO
CREATE NONCLUSTERED INDEX [IX_WalletTransactions_AccountID] ON [dbo].[WalletTransactions]([AccountID], [CreatedAt] DESC);
GO


-- Index cho Shipping/Gateway transactions
CREATE NONCLUSTERED INDEX [IX_ShippingTxn_OrderID] ON [dbo].[ShippingProviderTransactions]([OrderID]);
GO
CREATE NONCLUSTERED INDEX [IX_ShippingTxn_Provider_Status] ON [dbo].[ShippingProviderTransactions]([Provider], [Status]);
GO
CREATE NONCLUSTERED INDEX [IX_GatewayTxn_PaymentHistoryID] ON [dbo].[PaymentGatewayTransactions]([PaymentHistoryID]);
GO


-- Index cho Webhook processing
CREATE NONCLUSTERED INDEX [IX_WebhookEvents_Status_CreatedAt] ON [dbo].[WebhookEvents]([Status], [CreatedAt]) WHERE [Status] = 'Pending';
GO


-- Index cho Review queries
CREATE NONCLUSTERED INDEX [IX_ReviewProducts_ProductID] ON [dbo].[ReviewProducts]([ProductID]) WHERE [IsDeleted] = 0;
GO
CREATE NONCLUSTERED INDEX [IX_ReviewProducts_AccountID] ON [dbo].[ReviewProducts]([AccountID]) WHERE [IsDeleted] = 0;
GO


-- Index cho Blog
CREATE NONCLUSTERED INDEX [IX_BlogPosts_IsPublished_CreatedAt] ON [dbo].[BlogPosts]([IsPublished], [CreatedAt] DESC) WHERE [IsDeleted] = 0;
GO


-- Index cho Voucher
CREATE NONCLUSTERED INDEX [IX_Voucher_Status_Dates] ON [dbo].[Voucher]([Status], [StartDate], [EndDate]);
GO





