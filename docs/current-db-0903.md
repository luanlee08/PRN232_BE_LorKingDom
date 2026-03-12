USE [ASP_LorKingDom4]
GO
/**\*\*** Object: Schema [Notification] Script Date: 09/03/2026 7:20:57 AM **\*\***/
CREATE SCHEMA [Notification]
GO
/**\*\*** Object: Schema [System] Script Date: 09/03/2026 7:20:57 AM **\*\***/
CREATE SCHEMA [System]
GO
/**\*\*** Object: Table [dbo].[Accounts] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
[AccountID] [int] IDENTITY(1,1) NOT NULL,
[RoleID] [int] NULL,
[AccountName] [nvarchar](100) NOT NULL,
[PhoneNumber] [varchar](15) NULL,
[Email] [nvarchar](255) NOT NULL,
[Image] [nvarchar](500) NULL,
[Password] [nvarchar](255) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[Status] [nvarchar](10) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
[Provider] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED
(
[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Addresses] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addresses](
[AddressID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[AddressLine] [nvarchar](500) NOT NULL,
[City] [nvarchar](100) NOT NULL,
[Ward] [nvarchar](100) NULL,
[IsDefault] [bit] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
[District] [nvarchar](100) NULL,
[RecipientName] [nvarchar](100) NULL,
[PhoneNumber] [nvarchar](20) NULL,
[DistrictId] [int] NULL,
[WardCode] [varchar](20) NULL,
[ProvinceId] [int] NULL,
PRIMARY KEY CLUSTERED
(
[AddressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Ages] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ages](
[AgeID] [int] IDENTITY(1,1) NOT NULL,
[AgeRange] [nvarchar](50) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[AgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[BlogCategories] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogCategories](
[BlogCategoryID] [int] IDENTITY(1,1) NOT NULL,
[BlogCategoryName] [nvarchar](100) NOT NULL,
[Description] [nvarchar](500) NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[BlogCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[BlogPostCategories] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogPostCategories](
[BlogPostID] [int] NOT NULL,
[BlogCategoryID] [int] NOT NULL,
PRIMARY KEY CLUSTERED
(
[BlogPostID] ASC,
[BlogCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[BlogPosts] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogPosts](
[BlogPostID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[BlogTitle] [nvarchar](255) NOT NULL,
[BlogContent] [nvarchar](max) NOT NULL,
[BlogThumbnail] [nvarchar](500) NULL,
[BlogUrl] [nvarchar](max) NULL,
[IsPublished] [bit] NOT NULL,
[IsFeatured] [bit] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[BlogPostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Brands] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
[BrandID] [int] IDENTITY(1,1) NOT NULL,
[BrandName] [nvarchar](100) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Cart] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
[CartID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[CartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[CartItems] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
[CartItemID] [int] IDENTITY(1,1) NOT NULL,
[CartID] [int] NOT NULL,
[ProductID] [int] NOT NULL,
[Quantity] [int] NOT NULL,
[PriceAtThatTime] [decimal](12, 2) NOT NULL,
[Status] [nvarchar](15) NOT NULL,
[AddedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[CartItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Categories] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
[CategoryID] [int] IDENTITY(1,1) NOT NULL,
[SuperCategoryID] [int] NOT NULL,
[CategoryName] [nvarchar](255) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[EmailOtps] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailOtps](
[EmailOtpID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NULL,
[Email] [nvarchar](255) NULL,
[Purpose] [nvarchar](50) NOT NULL,
[OtpCode] [nvarchar](10) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[ExpiresAt] [datetime] NOT NULL,
[IsUsed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED
(
[EmailOtpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ExternalApiLogs] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalApiLogs](
[LogID] [bigint] IDENTITY(1,1) NOT NULL,
[Provider] [nvarchar](50) NOT NULL,
[Endpoint] [nvarchar](500) NULL,
[Method] [nvarchar](10) NULL,
[RequestPayload] [nvarchar](max) NULL,
[ResponsePayload] [nvarchar](max) NULL,
[StatusCode] [int] NULL,
[CreatedAt] [datetime] NOT NULL,
[CreatedBy] [int] NULL,
PRIMARY KEY CLUSTERED
(
[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ExternalLogins] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalLogins](
[ExternalLoginID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[Provider] [nvarchar](100) NOT NULL,
[ProviderKey] [nvarchar](255) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ExternalLoginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Materials] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Materials](
[MaterialID] [int] IDENTITY(1,1) NOT NULL,
[MaterialName] [nvarchar](255) NOT NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[MaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[OrderDetails] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
[OrderID] [int] NOT NULL,
[ProductID] [int] NOT NULL,
[Quantity] [int] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[UnitPrice] [decimal](12, 2) NOT NULL,
[Total] AS ([Quantity]\*[UnitPrice]) PERSISTED,
[Reviewed] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[OrderRefunds] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderRefunds](
[RefundID] [bigint] IDENTITY(1,1) NOT NULL,
[OrderID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[RequestedBy] [int] NULL,
[ApprovedBy] [int] NULL,
[WalletTransactionID] [bigint] NULL,
[RefundMode] [nvarchar](20) NOT NULL,
[RefundStatus] [nvarchar](20) NOT NULL,
[TotalAmount] [decimal](12, 2) NOT NULL,
[RefundAmount] [decimal](12, 2) NOT NULL,
[Reason] [nvarchar](500) NULL,
[CreatedAt] [datetime] NOT NULL,
[ApprovedAt] [datetime] NULL,
[ProcessedAt] [datetime] NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[RefundID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Orders] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
[OrderID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[VoucherID] [int] NULL,
[StatusID] [int] NOT NULL,
[ShippingName] [nvarchar](100) NULL,
[ShippingPhone] [nvarchar](20) NULL,
[ShippingAddressLine] [nvarchar](500) NULL,
[ShippingCity] [nvarchar](100) NULL,
[ShippingWard] [nvarchar](100) NULL,
[ShippingMethod] [nvarchar](100) NULL,
[ShippingFee] [decimal](12, 2) NOT NULL,
[OrderDate] [datetime] NOT NULL,
[TotalAmount] [decimal](12, 2) NOT NULL,
[PaidByWalletAmount] [decimal](14, 2) NOT NULL,
[PaidByExternalAmount] [decimal](14, 2) NOT NULL,
[PaymentCompletedAt] [datetime] NULL,
[RefundStatus] [nvarchar](15) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
[ShippingDistrict] [nvarchar](255) NULL,
[ShippingDistrictId] [int] NULL,
[ShippingWardCode] [varchar](20) NULL,
[ShippingProvinceId] [int] NULL,
PRIMARY KEY CLUSTERED
(
[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[OrderStatusHistory] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatusHistory](
[OrderStatusHistoryID] [int] IDENTITY(1,1) NOT NULL,
[OrderID] [int] NOT NULL,
[StatusID] [int] NULL,
[ChangedAt] [datetime] NOT NULL,
[ChangedBy] [int] NULL,
[Note] [nvarchar](500) NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[OrderStatusHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Origins] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Origins](
[OriginID] [int] IDENTITY(1,1) NOT NULL,
[OriginName] [nvarchar](255) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[OriginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[PaymentGatewayTransactions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentGatewayTransactions](
[GatewayTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
[PaymentHistoryID] [bigint] NOT NULL,
[Provider] [varchar](50) NOT NULL,
[TransactionID] [varchar](200) NULL,
[PaymentUrl] [varchar](1000) NULL,
[Status] [varchar](50) NOT NULL,
[Amount] [decimal](14, 2) NOT NULL,
[GatewayResponse] [nvarchar](max) NULL,
[CreatedAt] [datetime] NOT NULL,
[CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[GatewayTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[PaymentHistory] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentHistory](
[PaymentHistoryID] [bigint] IDENTITY(1,1) NOT NULL,
[OrderID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[PaymentMethod] [nvarchar](50) NOT NULL,
[PaymentStatus] [nvarchar](50) NOT NULL,
[TransactionCode] [nvarchar](100) NULL,
[Amount] [decimal](14, 2) NOT NULL,
[Currency] [nvarchar](3) NOT NULL,
[WalletTransactionID] [bigint] NULL,
[Note] [nvarchar](255) NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[PaymentHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[PriceRanges] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceRanges](
[PriceRangeID] [int] IDENTITY(1,1) NOT NULL,
[PriceRangeMin] [decimal](12, 2) NOT NULL,
[PriceRangeMax] [decimal](12, 2) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[PriceRangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ProductImages] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImages](
[ImageID] [int] IDENTITY(1,1) NOT NULL,
[ProductID] [int] NOT NULL,
[ImageUrl] [nvarchar](500) NOT NULL,
[IsMain] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Products] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
[Quantity] [int] NOT NULL,
[ProductStatus] [nvarchar](15) NOT NULL,
[Description] [nvarchar](max) NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewBlogReactions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogReactions](
[ReactionBlogID] [int] IDENTITY(1,1) NOT NULL,
[ReviewBlogID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[ReactionType] [nvarchar](10) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ReactionBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewBlogReplies] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogReplies](
[ReplyBlogID] [int] IDENTITY(1,1) NOT NULL,
[ReviewBlogID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[Content] [nvarchar](1000) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ReplyBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewBlogs] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogs](
[ReviewBlogID] [int] IDENTITY(1,1) NOT NULL,
[BlogPostID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[Rating] [int] NOT NULL,
[Comment] [nvarchar](max) NULL,
[IsBlocked] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ReviewBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewModerationLogs] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewModerationLogs](
[ModerationLogID] [int] IDENTITY(1,1) NOT NULL,
[ReviewProductID] [int] NOT NULL,
[Stage] [nvarchar](50) NOT NULL,
[Result] [nvarchar](50) NOT NULL,
[Score] [decimal](3, 2) NULL,
[Details] [nvarchar](max) NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ModerationLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewProductImages] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductImages](
[ReviewProductImageID] [int] IDENTITY(1,1) NOT NULL,
[ReviewProductID] [int] NOT NULL,
[ImageUrl] [nvarchar](500) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[ReviewProductImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewProductReactions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductReactions](
[ReactionProductID] [int] IDENTITY(1,1) NOT NULL,
[ReviewProductID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[ReactionType] [nvarchar](10) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ReactionProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewProductReplies] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductReplies](
[ReplyProductID] [int] IDENTITY(1,1) NOT NULL,
[ReviewProductID] [int] NOT NULL,
[AccountID] [int] NOT NULL,
[Content] [nvarchar](1000) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[ReplyProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ReviewProducts] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProducts](
[ReviewProductID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[ProductID] [int] NOT NULL,
[OrderDetailID] [int] NULL,
[Rating] [int] NOT NULL,
[Comment] [nvarchar](1000) NULL,
[IsVerifiedPurchase] [bit] NOT NULL,
[IsDeleted] [bit] NOT NULL,
[Status] [nvarchar](20) NOT NULL,
[Visibility] [nvarchar](20) NOT NULL,
[ModerationScore] [decimal](3, 2) NULL,
[ModerationDetail] [nvarchar](max) NULL,
[EditCount] [int] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[ReviewProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Roles] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
[RoleID] [int] IDENTITY(1,1) NOT NULL,
[RoleName] [nvarchar](50) NOT NULL,
[Description] [nvarchar](255) NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Sexes] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sexes](
[SexID] [int] IDENTITY(1,1) NOT NULL,
[SexName] [nvarchar](20) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[SexID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ShippingProviderTransactions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
[LastPolledAt] [datetime2](7) NULL,
[RowVersion] [timestamp] NOT NULL,
[RetryCount] [int] NOT NULL,
[LastErrorMessage] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED
(
[ShippingTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[ShippingStatusHistories] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingStatusHistories](
[HistoryId] [bigint] IDENTITY(1,1) NOT NULL,
[ShippingTxId] [bigint] NOT NULL,
[OrderId] [int] NOT NULL,
[PreviousStatus] [nvarchar](50) NOT NULL,
[NewStatus] [nvarchar](50) NOT NULL,
[Source] [nvarchar](20) NOT NULL,
[RawPayload] [nvarchar](max) NULL,
[ProcessedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED
(
[HistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[StatusOrders] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusOrders](
[StatusID] [int] IDENTITY(1,1) NOT NULL,
[StatusName] [nvarchar](50) NOT NULL,
[Description] [nvarchar](max) NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[SuperCategories] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuperCategories](
[SuperCategoryID] [int] IDENTITY(1,1) NOT NULL,
[SuperCategoryName] [nvarchar](255) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[SuperCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[SystemConfigs] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigs](
[ConfigKey] [nvarchar](100) NOT NULL,
[ConfigValue] [nvarchar](max) NOT NULL,
[Description] [nvarchar](255) NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[ConfigKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Voucher] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
[VoucherID] [int] IDENTITY(1,1) NOT NULL,
[VoucherTypeID] [int] NOT NULL,
[CreateBy] [int] NULL,
[VoucherCode] [nvarchar](50) NOT NULL,
[DiscountType] [nvarchar](10) NOT NULL,
[DiscountValue] [decimal](12, 2) NOT NULL,
[MaxDiscountAmount] [decimal](12, 2) NULL,
[MinOrderAmount] [decimal](12, 2) NULL,
[UsageLimitPerUser] [int] NULL,
[IsStackable] [bit] NOT NULL,
[StartDate] [datetime] NOT NULL,
[EndDate] [datetime] NOT NULL,
[Status] [nvarchar](15) NOT NULL,
[IsDeleted] [bit] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[VoucherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[VoucherTypes] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherTypes](
[VoucherTypeID] [int] IDENTITY(1,1) NOT NULL,
[VoucherTypeName] [nvarchar](255) NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[VoucherTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Wallets] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallets](
[WalletID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[Currency] [nvarchar](3) NOT NULL,
[Balance] [decimal](14, 2) NOT NULL,
[Status] [nvarchar](10) NOT NULL,
[LastTransactionAt] [datetime] NULL,
[CreatedAt] [datetime] NOT NULL,
[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[WalletID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[WalletTransactions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
[Status] [nvarchar](12) NOT NULL,
[Reason] [nvarchar](255) NULL,
[Metadata] [nvarchar](max) NULL,
[CreatedAt] [datetime] NOT NULL,
[CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED
(
[WalletTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[WebhookEvents] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebhookEvents](
[WebhookEventID] [bigint] IDENTITY(1,1) NOT NULL,
[Provider] [varchar](50) NOT NULL,
[EventType] [varchar](100) NOT NULL,
[Payload] [nvarchar](max) NOT NULL,
[Signature] [varchar](500) NULL,
[Status] [varchar](20) NOT NULL,
[ProcessedAt] [datetime] NULL,
[ErrorMessage] [nvarchar](max) NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[WebhookEventID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/**\*\*** Object: Table [dbo].[Wishlists] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wishlists](
[WishlistID] [int] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[ProductID] [int] NOT NULL,
[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED
(
[WishlistID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [Notification].[Campaigns] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[Campaigns](
[CampaignID] [int] IDENTITY(1,1) NOT NULL,
[CampaignName] [nvarchar](255) NOT NULL,
[TemplateCode] [varchar](50) NULL,
[TitleOverride] [nvarchar](255) NULL,
[MessageOverride] [nvarchar](500) NULL,
[SourceType] [varchar](10) NOT NULL,
[TargetType] [varchar](10) NOT NULL,
[Status] [varchar](15) NOT NULL,
[ScheduledAt] [datetime2](0) NULL,
[EventKey] [varchar](100) NULL,
[ImageUrl] [nvarchar](500) NULL,
[ActionType] [nvarchar](20) NULL,
[ActionTarget] [nvarchar](500) NULL,
[CreatedByAccountID] [int] NOT NULL,
[CreatedAt] [datetime2](0) NOT NULL,
[UpdatedAt] [datetime2](0) NULL,
PRIMARY KEY CLUSTERED
(
[CampaignID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [Notification].[CampaignTargets] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[CampaignTargets](
[CampaignTargetID] [int] IDENTITY(1,1) NOT NULL,
[CampaignID] [int] NOT NULL,
[TargetValue] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED
(
[CampaignTargetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [Notification].[Deliveries] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[Deliveries](
[DeliveryID] [bigint] IDENTITY(1,1) NOT NULL,
[AccountID] [int] NOT NULL,
[CreatedByJobID] [int] NULL,
[TemplateCode] [varchar](50) NOT NULL,
[Title] [nvarchar](255) NOT NULL,
[Message] [nvarchar](500) NOT NULL,
[Payload] [nvarchar](1000) NOT NULL,
[Status] [varchar](10) NOT NULL,
[CreatedAt] [datetime2](0) NOT NULL,
[ImageUrl] [nvarchar](500) NULL,
[ActionType] [nvarchar](20) NULL,
[ActionTarget] [nvarchar](500) NULL,
[CampaignID] [int] NULL,
PRIMARY KEY CLUSTERED
(
[DeliveryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [Notification].[DeliveryActions] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[DeliveryActions](
[ActionID] [bigint] IDENTITY(1,1) NOT NULL,
[DeliveryID] [bigint] NOT NULL,
[AccountID] [int] NOT NULL,
[ActionType] [varchar](10) NOT NULL,
[ActionTarget] [nvarchar](500) NULL,
[OccurredAt] [datetime2](0) NOT NULL,
PRIMARY KEY CLUSTERED
(
[ActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [Notification].[Templates] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[Templates](
[TemplateID] [smallint] IDENTITY(1,1) NOT NULL,
[TemplateCode] [varchar](50) NOT NULL,
[TitleTemplate] [nvarchar](255) NOT NULL,
[MessageTemplate] [nvarchar](500) NOT NULL,
[IsActive] [bit] NOT NULL,
[CreatedAt] [datetime2](0) NOT NULL,
[UpdatedAt] [datetime2](0) NULL,
PRIMARY KEY CLUSTERED
(
[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/**\*\*** Object: Table [System].[BackgroundJobs] Script Date: 09/03/2026 7:20:57 AM **\*\***/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System].[BackgroundJobs](
[JobID] [int] IDENTITY(1,1) NOT NULL,
[JobName] [nvarchar](100) NOT NULL,
[CronExpression] [nvarchar](100) NULL,
[IsEnabled] [bit] NOT NULL,
[LastRunTime] [datetime2](7) NULL,
[NextRunTime] [datetime2](7) NULL,
[LastRunStatus] [nvarchar](20) NULL,
[LastRunMessage] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED
(
[JobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Accounts] ON

INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Image], [Password], [IsDeleted], [Status], [CreatedAt], [UpdatedAt], [Provider]) VALUES (1, 2, N'staff01', N'0900000001', N'staff@lorkingdom.com', NULL, N'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', 0, N'Active', CAST(N'2026-03-04T18:51:25.603' AS DateTime), NULL, NULL)
INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Image], [Password], [IsDeleted], [Status], [CreatedAt], [UpdatedAt], [Provider]) VALUES (2, 3, N'warehouse01', N'0900000002', N'warehouse@lorkingdom.com', NULL, N'$2b$11$ACDZHTdYJs7tFYezEOO1VOJUilPqZv8aaLz28J5GWwCjSIsUlSKtS', 0, N'Active', CAST(N'2026-03-04T18:51:25.603' AS DateTime), NULL, NULL)
INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Image], [Password], [IsDeleted], [Status], [CreatedAt], [UpdatedAt], [Provider]) VALUES (3, 4, N'admin01', N'0900000003', N'admin@lorkingdom.com', NULL, N'$2b$11$puQ/vlg2ytfmh3VgmCK/wePQdbILFqu5/VkRByoQpHbvs91tHgFCG', 0, N'Active', CAST(N'2026-03-04T18:51:25.603' AS DateTime), NULL, NULL)
INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Image], [Password], [IsDeleted], [Status], [CreatedAt], [UpdatedAt], [Provider]) VALUES (4, 1, N'Khang', N'0382733605', N'khangtv1502@gmail.com', NULL, N'$2a$11$K2h8UjcDY.nEuXvosWBKD.nQKSJeyOcsSrEZeWxLcOFxVxoxxfPyC', 0, N'Active', CAST(N'2026-03-04T18:51:25.603' AS DateTime), NULL, NULL)
INSERT [dbo].[Accounts] ([AccountID], [RoleID], [AccountName], [PhoneNumber], [Email], [Image], [Password], [IsDeleted], [Status], [CreatedAt], [UpdatedAt], [Provider]) VALUES (5, 1, N'Ngọc Quỳnh', N'0912802301', N'vuquangduc1404@gmail.com', NULL, N'$2a$11$BNCVeMqbyurWfbINT.pTyOifrgaqwbd1ExVkP.Ml9NB7jlCgZceUW', 0, N'Active', CAST(N'2026-03-06T15:42:02.253' AS DateTime), NULL, N'Email')
SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO
SET IDENTITY_INSERT [dbo].[Addresses] ON

INSERT [dbo].[Addresses] ([AddressID], [AccountID], [AddressLine], [City], [Ward], [IsDefault], [IsDeleted], [CreatedAt], [UpdatedAt], [District], [RecipientName], [PhoneNumber], [DistrictId], [WardCode], [ProvinceId]) VALUES (1, 4, N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', 1, 0, CAST(N'2026-03-04T11:54:48.343' AS DateTime), CAST(N'2026-03-04T11:56:50.527' AS DateTime), N'Quận Tây Hồ', N'Quang Ducky', N'0912802301', NULL, NULL, 2002)
SET IDENTITY_INSERT [dbo].[Addresses] OFF
GO
SET IDENTITY_INSERT [dbo].[Brands] ON

INSERT [dbo].[Brands] ([BrandID], [BrandName], [IsDeleted], [CreatedAt]) VALUES (1, N'Lego', 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[Brands] ([BrandID], [BrandName], [IsDeleted], [CreatedAt]) VALUES (2, N'Hasbro', 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime))
SET IDENTITY_INSERT [dbo].[Brands] OFF
GO
SET IDENTITY_INSERT [dbo].[Cart] ON

INSERT [dbo].[Cart] ([CartID], [AccountID], [CreatedAt], [UpdatedAt]) VALUES (1, 4, CAST(N'2026-03-04T11:53:30.483' AS DateTime), CAST(N'2026-03-07T12:01:17.407' AS DateTime))
INSERT [dbo].[Cart] ([CartID], [AccountID], [CreatedAt], [UpdatedAt]) VALUES (2, 5, CAST(N'2026-03-06T15:42:06.483' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Cart] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON

INSERT [dbo].[Categories] ([CategoryID], [SuperCategoryID], [CategoryName], [IsDeleted], [CreatedAt]) VALUES (1, 1, N'Đồ chơi trẻ em', 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime))
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderDetails] ON

INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (1, 1, 1, 2, 0, CAST(350000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-04T11:58:11.523' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (2, 1, 2, 1, 0, CAST(420000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-04T11:58:11.570' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (5, 3, 3, 1, 0, CAST(850000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T12:40:24.027' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (6, 3, 4, 1, 0, CAST(650000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T12:40:24.033' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (9, 5, 5, 1, 0, CAST(250000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T13:05:40.790' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (10, 5, 2, 2, 0, CAST(420000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T13:05:40.817' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (11, 6, 2, 1, 0, CAST(420000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T13:08:58.453' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (12, 7, 2, 1, 0, CAST(420000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T13:19:13.503' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (13, 8, 3, 2, 0, CAST(850000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-06T15:18:18.783' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (14, 8, 4, 2, 0, CAST(650000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-06T15:18:18.797' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (15, 8, 5, 1, 0, CAST(250000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-06T15:18:18.803' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (16, 9, 4, 1, 0, CAST(650000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-07T11:12:33.767' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (17, 9, 5, 1, 0, CAST(250000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-07T11:12:33.777' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (18, 10, 5, 1, 0, CAST(250000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-07T11:49:00.947' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (19, 10, 2, 1, 0, CAST(420000.00 AS Decimal(12, 2)), 1, CAST(N'2026-03-07T11:49:00.957' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (20, 11, 5, 1, 0, CAST(250000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-07T12:00:55.653' AS DateTime))
INSERT [dbo].[OrderDetails] ([OrderDetailID], [OrderID], [ProductID], [Quantity], [IsDeleted], [UnitPrice], [Reviewed], [CreatedAt]) VALUES (21, 12, 2, 1, 0, CAST(420000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-07T12:02:37.383' AS DateTime))
SET IDENTITY_INSERT [dbo].[OrderDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON

INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (1, 4, NULL, 5, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-04T11:58:11.357' AS DateTime), CAST(1150000.00 AS Decimal(12, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(1150000.00 AS Decimal(14, 2)), NULL, N'None', 0, CAST(N'2026-03-04T11:58:11.357' AS DateTime), CAST(N'2026-03-04T12:10:21.477' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (3, 4, NULL, 6, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Express', CAST(50000.00 AS Decimal(12, 2)), CAST(N'2026-03-04T12:40:24.020' AS DateTime), CAST(1550000.00 AS Decimal(12, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(1550000.00 AS Decimal(14, 2)), NULL, N'None', 0, CAST(N'2026-03-04T12:40:24.020' AS DateTime), CAST(N'2026-03-06T11:40:07.010' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (5, 4, NULL, 1, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-04T13:05:40.683' AS DateTime), CAST(1120000.00 AS Decimal(12, 2)), CAST(1120000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-04T13:05:40.943' AS DateTime), N'None', 0, CAST(N'2026-03-04T13:05:40.683' AS DateTime), NULL, N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (6, 4, NULL, 1, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-04T13:08:58.443' AS DateTime), CAST(450000.00 AS Decimal(12, 2)), CAST(450000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-04T13:08:58.467' AS DateTime), N'None', 0, CAST(N'2026-03-04T13:08:58.443' AS DateTime), NULL, N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (7, 4, NULL, 1, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-04T13:19:13.500' AS DateTime), CAST(450000.00 AS Decimal(12, 2)), CAST(450000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-04T13:19:13.523' AS DateTime), N'None', 0, CAST(N'2026-03-04T13:19:13.500' AS DateTime), NULL, N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (8, 4, NULL, 5, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-06T15:18:18.760' AS DateTime), CAST(3280000.00 AS Decimal(12, 2)), CAST(3280000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-06T15:18:18.847' AS DateTime), N'None', 0, CAST(N'2026-03-06T15:18:18.760' AS DateTime), CAST(N'2026-03-06T15:31:12.393' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (9, 4, NULL, 5, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-07T11:12:33.713' AS DateTime), CAST(930000.00 AS Decimal(12, 2)), CAST(930000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-07T11:12:33.857' AS DateTime), N'None', 0, CAST(N'2026-03-07T11:12:33.713' AS DateTime), CAST(N'2026-03-07T12:03:16.963' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (10, 4, 2, 5, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-07T11:49:00.910' AS DateTime), CAST(566000.00 AS Decimal(12, 2)), CAST(566000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(N'2026-03-07T11:49:01.027' AS DateTime), N'None', 0, CAST(N'2026-03-07T11:49:00.910' AS DateTime), CAST(N'2026-03-07T14:08:25.280' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (11, 4, NULL, 6, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Standard', CAST(30000.00 AS Decimal(12, 2)), CAST(N'2026-03-07T12:00:55.597' AS DateTime), CAST(280000.00 AS Decimal(12, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(280000.00 AS Decimal(14, 2)), NULL, N'None', 0, CAST(N'2026-03-07T12:00:55.597' AS DateTime), CAST(N'2026-03-07T12:20:04.707' AS DateTime), N'Quận Tây Hồ', NULL, NULL, 2002)
INSERT [dbo].[Orders] ([OrderID], [AccountID], [VoucherID], [StatusID], [ShippingName], [ShippingPhone], [ShippingAddressLine], [ShippingCity], [ShippingWard], [ShippingMethod], [ShippingFee], [OrderDate], [TotalAmount], [PaidByWalletAmount], [PaidByExternalAmount], [PaymentCompletedAt], [RefundStatus], [IsDeleted], [CreatedAt], [UpdatedAt], [ShippingDistrict], [ShippingDistrictId], [ShippingWardCode], [ShippingProvinceId]) VALUES (12, 4, NULL, 1, N'Quang Ducky', N'0912802301', N'600 Nguyễn Văn Cừ', N'Thành phố Hà Nội', N'Phường Nhật Tân', N'Express', CAST(50000.00 AS Decimal(12, 2)), CAST(N'2026-03-07T12:02:37.377' AS DateTime), CAST(470000.00 AS Decimal(12, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), NULL, N'None', 0, CAST(N'2026-03-07T12:02:37.377' AS DateTime), NULL, N'Quận Tây Hồ', NULL, NULL, 2002)
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderStatusHistory] ON

INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (1, 1, 1, CAST(N'2026-03-04T11:58:11.573' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-04T11:58:11.573' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (2, 1, 2, CAST(N'2026-03-04T11:58:59.427' AS DateTime), 3, N'Đơn hàng đã được xác nhận.', CAST(N'2026-03-04T11:58:59.427' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (3, 1, 3, CAST(N'2026-03-04T12:01:55.297' AS DateTime), 3, N'Đơn hàng đang được giao.', CAST(N'2026-03-04T12:01:55.297' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (4, 1, 4, CAST(N'2026-03-04T12:02:48.207' AS DateTime), 3, N'Đơn hàng đã giao thành công.', CAST(N'2026-03-04T12:02:48.207' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (5, 1, 5, CAST(N'2026-03-04T12:10:21.477' AS DateTime), 3, N'Đơn hàng đã hoàn thành.', CAST(N'2026-03-04T12:10:21.477' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (7, 3, 1, CAST(N'2026-03-04T12:40:24.033' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-04T12:40:24.033' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (9, 5, 1, CAST(N'2026-03-04T13:05:40.820' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-04T13:05:40.820' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (10, 6, 1, CAST(N'2026-03-04T13:08:58.457' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-04T13:08:58.457' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (11, 7, 1, CAST(N'2026-03-04T13:19:13.510' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-04T13:19:13.510' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (12, 3, 6, CAST(N'2026-03-06T11:40:07.010' AS DateTime), NULL, N'Tự động hủy do quá thời gian thanh toán (15 phút)', CAST(N'2026-03-06T11:40:07.010' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (13, 8, 1, CAST(N'2026-03-06T15:18:18.807' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-06T15:18:18.807' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (14, 8, 2, CAST(N'2026-03-06T15:21:12.520' AS DateTime), 3, N'Đơn hàng đã được xác nhận.', CAST(N'2026-03-06T15:21:12.520' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (15, 8, 3, CAST(N'2026-03-06T15:23:00.317' AS DateTime), 3, N'Đơn hàng đang được giao.', CAST(N'2026-03-06T15:23:00.317' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (16, 8, 4, CAST(N'2026-03-06T15:23:34.880' AS DateTime), 3, N'Đơn hàng đã giao thành công.', CAST(N'2026-03-06T15:23:34.880' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (17, 8, 5, CAST(N'2026-03-06T15:31:12.393' AS DateTime), 3, N'Đơn hàng đã hoàn thành.', CAST(N'2026-03-06T15:31:12.393' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (18, 9, 1, CAST(N'2026-03-07T11:12:33.783' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-07T11:12:33.783' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (19, 10, 1, CAST(N'2026-03-07T11:49:00.967' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-07T11:49:00.967' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (20, 11, 1, CAST(N'2026-03-07T12:00:55.677' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-07T12:00:55.677' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (21, 12, 1, CAST(N'2026-03-07T12:02:37.387' AS DateTime), NULL, N'Đơn hàng được tạo', CAST(N'2026-03-07T12:02:37.387' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (22, 9, 5, CAST(N'2026-03-07T12:03:16.963' AS DateTime), 3, N'Đơn hàng đã hoàn thành.', CAST(N'2026-03-07T12:03:16.963' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (23, 11, 6, CAST(N'2026-03-07T12:20:04.707' AS DateTime), NULL, N'Tự động hủy do quá thời gian thanh toán (15 phút)', CAST(N'2026-03-07T12:20:04.707' AS DateTime), NULL)
INSERT [dbo].[OrderStatusHistory] ([OrderStatusHistoryID], [OrderID], [StatusID], [ChangedAt], [ChangedBy], [Note], [CreatedAt], [UpdatedAt]) VALUES (24, 10, 5, CAST(N'2026-03-07T14:08:25.280' AS DateTime), 3, N'Đơn hàng đã hoàn thành.', CAST(N'2026-03-07T14:08:25.280' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[OrderStatusHistory] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentGatewayTransactions] ON

INSERT [dbo].[PaymentGatewayTransactions] ([GatewayTransactionID], [PaymentHistoryID], [Provider], [TransactionID], [PaymentUrl], [Status], [Amount], [GatewayResponse], [CreatedAt], [CompletedAt]) VALUES (1, 1, N'Sepay', NULL, N'http://localhost:3000/payment/sepay-test?mock=true&order_id=1&amount=1150000', N'Pending', CAST(1150000.00 AS Decimal(14, 2)), NULL, CAST(N'2026-03-04T11:58:11.640' AS DateTime), NULL)
INSERT [dbo].[PaymentGatewayTransactions] ([GatewayTransactionID], [PaymentHistoryID], [Provider], [TransactionID], [PaymentUrl], [Status], [Amount], [GatewayResponse], [CreatedAt], [CompletedAt]) VALUES (2, 2, N'Sepay', NULL, N'http://localhost:3000/payment/sepay-test?mock=true&order_id=3&amount=1550000', N'Pending', CAST(1550000.00 AS Decimal(14, 2)), NULL, CAST(N'2026-03-04T12:40:24.040' AS DateTime), NULL)
INSERT [dbo].[PaymentGatewayTransactions] ([GatewayTransactionID], [PaymentHistoryID], [Provider], [TransactionID], [PaymentUrl], [Status], [Amount], [GatewayResponse], [CreatedAt], [CompletedAt]) VALUES (3, 9, N'Sepay', NULL, N'http://localhost:3000/payment/sepay-test?mock=true&order_id=11&amount=280000', N'Pending', CAST(280000.00 AS Decimal(14, 2)), NULL, CAST(N'2026-03-07T12:00:55.740' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[PaymentGatewayTransactions] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentHistory] ON

INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (1, 1, 4, N'Sepay', N'Pending', NULL, CAST(1150000.00 AS Decimal(14, 2)), N'VND', NULL, NULL, CAST(N'2026-03-04T11:58:11.600' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (2, 3, 4, N'Sepay', N'Expired', NULL, CAST(1550000.00 AS Decimal(14, 2)), N'VND', NULL, NULL, CAST(N'2026-03-04T12:40:24.037' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (3, 5, 4, N'Wallet', N'Success', N'5_wallet_639082263408654820', CAST(1120000.00 AS Decimal(14, 2)), N'VND', 5, NULL, CAST(N'2026-03-04T13:05:40.913' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (4, 6, 4, N'Wallet', N'Success', N'6_wallet_639082265384616021', CAST(450000.00 AS Decimal(14, 2)), N'VND', 6, NULL, CAST(N'2026-03-04T13:08:58.463' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (5, 7, 4, N'Wallet', N'Success', N'7_wallet_639082271535155674', CAST(450000.00 AS Decimal(14, 2)), N'VND', 7, NULL, CAST(N'2026-03-04T13:19:13.520' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (6, 8, 4, N'Wallet', N'Success', N'8_wallet_639084070988258921', CAST(3280000.00 AS Decimal(14, 2)), N'VND', 11, NULL, CAST(N'2026-03-06T15:18:18.830' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (7, 9, 4, N'Wallet', N'Success', N'9_wallet_639084787538220098', CAST(930000.00 AS Decimal(14, 2)), N'VND', 14, NULL, CAST(N'2026-03-07T11:12:33.830' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (8, 10, 4, N'Wallet', N'Success', N'10_wallet_639084809409961772', CAST(566000.00 AS Decimal(14, 2)), N'VND', 17, NULL, CAST(N'2026-03-07T11:49:01.003' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (9, 11, 4, N'Sepay', N'Expired', NULL, CAST(280000.00 AS Decimal(14, 2)), N'VND', NULL, NULL, CAST(N'2026-03-07T12:00:55.710' AS DateTime))
INSERT [dbo].[PaymentHistory] ([PaymentHistoryID], [OrderID], [AccountID], [PaymentMethod], [PaymentStatus], [TransactionCode], [Amount], [Currency], [WalletTransactionID], [Note], [CreatedAt]) VALUES (10, 12, 4, N'COD', N'Pending', NULL, CAST(470000.00 AS Decimal(14, 2)), N'VND', NULL, NULL, CAST(N'2026-03-07T12:02:37.393' AS DateTime))
SET IDENTITY_INSERT [dbo].[PaymentHistory] OFF
GO
SET IDENTITY_INSERT [dbo].[PriceRanges] ON

INSERT [dbo].[PriceRanges] ([PriceRangeID], [PriceRangeMin], [PriceRangeMax], [IsDeleted], [CreatedAt]) VALUES (1, CAST(100000.00 AS Decimal(12, 2)), CAST(500000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[PriceRanges] ([PriceRangeID], [PriceRangeMin], [PriceRangeMax], [IsDeleted], [CreatedAt]) VALUES (2, CAST(500000.00 AS Decimal(12, 2)), CAST(1000000.00 AS Decimal(12, 2)), 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime))
SET IDENTITY_INSERT [dbo].[PriceRanges] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON

INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID], [BrandID], [OriginID], [ProductName], [Price], [Quantity], [ProductStatus], [Description], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (1, N'SKU-LEGO-01', 1, NULL, NULL, NULL, 1, 1, NULL, N'Lego City Police', CAST(350000.00 AS Decimal(12, 2)), 52, N'Available', NULL, 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID], [BrandID], [OriginID], [ProductName], [Price], [Quantity], [ProductStatus], [Description], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (2, N'SKU-LEGO-02', 1, NULL, NULL, NULL, 1, 1, NULL, N'Lego Fire Station', CAST(420000.00 AS Decimal(12, 2)), 68, N'Available', NULL, 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID], [BrandID], [OriginID], [ProductName], [Price], [Quantity], [ProductStatus], [Description], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (3, N'SKU-HAS-01', 1, NULL, NULL, NULL, 2, 2, NULL, N'Transformers Optimus', CAST(850000.00 AS Decimal(12, 2)), 43, N'Available', NULL, 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID], [BrandID], [OriginID], [ProductName], [Price], [Quantity], [ProductStatus], [Description], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (4, N'SKU-HAS-02', 1, NULL, NULL, NULL, 2, 2, NULL, N'Monopoly Classic', CAST(650000.00 AS Decimal(12, 2)), 53, N'Available', NULL, 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime), NULL)
INSERT [dbo].[Products] ([ProductID], [SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID], [BrandID], [OriginID], [ProductName], [Price], [Quantity], [ProductStatus], [Description], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (5, N'SKU-HAS-03', 1, NULL, NULL, NULL, 1, 2, NULL, N'Play-Doh Color Set', CAST(250000.00 AS Decimal(12, 2)), 192, N'Available', NULL, 0, CAST(N'2026-03-04T18:51:25.607' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[ReviewModerationLogs] ON

INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (1, 1, N'Decision', N'Approved', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', CAST(N'2026-03-06T14:45:52.507' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (2, 2, N'Decision', N'Approved', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', CAST(N'2026-03-06T15:02:08.263' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (3, 3, N'Decision', N'Approved', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', CAST(N'2026-03-06T15:33:29.670' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (4, 4, N'Decision', N'Approved', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', CAST(N'2026-03-06T15:36:45.407' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (5, 5, N'Decision', N'Rejected', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', CAST(N'2026-03-06T15:37:12.717' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (6, 6, N'Decision', N'Rejected', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''cặc''', CAST(N'2026-03-07T12:04:28.377' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (7, 7, N'Decision', N'Rejected', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', CAST(N'2026-03-07T13:26:13.897' AS DateTime))
INSERT [dbo].[ReviewModerationLogs] ([ModerationLogID], [ReviewProductID], [Stage], [Result], [Score], [Details], [CreatedAt]) VALUES (8, 8, N'Decision', N'Rejected', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', CAST(N'2026-03-07T14:08:51.027' AS DateTime))
SET IDENTITY_INSERT [dbo].[ReviewModerationLogs] OFF
GO
SET IDENTITY_INSERT [dbo].[ReviewProductImages] ON

INSERT [dbo].[ReviewProductImages] ([ReviewProductImageID], [ReviewProductID], [ImageUrl], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (1, 1, N'https://res.cloudinary.com/dghnftz3b/image/upload/v1772808406/reviews/1/iykljjnuzifkynqccpwr.jpg', 0, CAST(N'2026-03-06T14:45:52.297' AS DateTime), NULL)
INSERT [dbo].[ReviewProductImages] ([ReviewProductImageID], [ReviewProductID], [ImageUrl], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (2, 1, N'https://res.cloudinary.com/dghnftz3b/image/upload/v1772808404/reviews/1/plkxok3gix2jf0gyzlxf.jpg', 0, CAST(N'2026-03-06T14:45:52.297' AS DateTime), NULL)
INSERT [dbo].[ReviewProductImages] ([ReviewProductImageID], [ReviewProductID], [ImageUrl], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (3, 3, N'https://res.cloudinary.com/dghnftz3b/image/upload/v1772811264/reviews/3/ytbluvhkr6hmzgyok27d.jpg', 0, CAST(N'2026-03-06T15:33:29.500' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[ReviewProductImages] OFF
GO
SET IDENTITY_INSERT [dbo].[ReviewProductReplies] ON

INSERT [dbo].[ReviewProductReplies] ([ReplyProductID], [ReviewProductID], [AccountID], [Content], [IsDeleted], [CreatedAt]) VALUES (1, 4, 3, N'Xin lỗi quý khách vì sự bất tiện này. Shop sẽ cố gắng nâng cao chất lượng. Hẹn gặp lại quý khách lần sau!', 0, CAST(N'2026-03-07T11:09:42.037' AS DateTime))
SET IDENTITY_INSERT [dbo].[ReviewProductReplies] OFF
GO
SET IDENTITY_INSERT [dbo].[ReviewProducts] ON

INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (1, 4, 1, 1, 4, N'Ok phết đấy', 0, 0, N'Approved', N'Public', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', 0, CAST(N'2026-03-06T14:45:52.240' AS DateTime), CAST(N'2026-03-06T14:45:52.477' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (2, 4, 2, 2, 5, N'Đồ chơi đẹp, bền lâu. Tôi sẽ ủng hộ tiếp ^^', 0, 0, N'Approved', N'Public', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', 0, CAST(N'2026-03-06T15:02:07.870' AS DateTime), CAST(N'2026-03-06T15:02:08.240' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (3, 4, 3, 13, 3, N'Tệ hơn lần trước, nhưng vì khách quen nên cho 3 sao', 0, 0, N'Approved', N'Public', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', 0, CAST(N'2026-03-06T15:33:29.460' AS DateTime), CAST(N'2026-03-06T15:33:29.650' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (4, 4, 4, 14, 2, N'Chưa chơi được gì đã hỏng, tôi mới mua cho cháu tầm vài tuần đổ lại', 0, 0, N'Approved', N'Public', CAST(0.00 AS Decimal(3, 2)), N'Đạt tiêu chuẩn cộng đồng', 0, CAST(N'2026-03-06T15:36:45.350' AS DateTime), CAST(N'2026-03-06T15:36:45.400' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (5, 4, 5, 15, 1, N'Địt con mẹ mày thg chó', 0, 0, N'Rejected', N'AuthorOnly', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', 0, CAST(N'2026-03-06T15:37:12.637' AS DateTime), CAST(N'2026-03-06T15:37:12.713' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (6, 4, 4, 16, 1, N'Như cái con cặc tao vậy', 0, 0, N'Rejected', N'AuthorOnly', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''cặc''', 0, CAST(N'2026-03-07T12:04:28.207' AS DateTime), CAST(N'2026-03-07T12:04:28.350' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (7, 4, 5, 17, 1, N'địt mẹ mày', 0, 0, N'Rejected', N'AuthorOnly', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', 0, CAST(N'2026-03-07T13:26:13.663' AS DateTime), CAST(N'2026-03-07T13:26:13.880' AS DateTime))
INSERT [dbo].[ReviewProducts] ([ReviewProductID], [AccountID], [ProductID], [OrderDetailID], [Rating], [Comment], [IsVerifiedPurchase], [IsDeleted], [Status], [Visibility], [ModerationScore], [ModerationDetail], [EditCount], [CreatedAt], [UpdatedAt]) VALUES (8, 4, 2, 19, 2, N'cái địt con mẹ nó chứ', 0, 0, N'Rejected', N'AuthorOnly', CAST(1.00 AS Decimal(3, 2)), N'Phát hiện từ ngữ không phù hợp: ''địt''', 0, CAST(N'2026-03-07T14:08:50.820' AS DateTime), CAST(N'2026-03-07T14:08:50.990' AS DateTime))
SET IDENTITY_INSERT [dbo].[ReviewProducts] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON

INSERT [dbo].[Roles] ([RoleID], [RoleName], [Description], [CreatedAt]) VALUES (1, N'Customer', N'Khách hàng', CAST(N'2026-03-04T18:51:25.603' AS DateTime))
INSERT [dbo].[Roles] ([RoleID], [RoleName], [Description], [CreatedAt]) VALUES (2, N'Staff', N'Nhân viên CSKH', CAST(N'2026-03-04T18:51:25.603' AS DateTime))
INSERT [dbo].[Roles] ([RoleID], [RoleName], [Description], [CreatedAt]) VALUES (3, N'Warehouse', N'Nhân viên kho', CAST(N'2026-03-04T18:51:25.603' AS DateTime))
INSERT [dbo].[Roles] ([RoleID], [RoleName], [Description], [CreatedAt]) VALUES (4, N'Admin', N'Quản trị hệ thống', CAST(N'2026-03-04T18:51:25.603' AS DateTime))
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[StatusOrders] ON

INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (1, N'Pending', N'Đơn hàng mới tạo, chưa xác nhận', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (2, N'Confirmed', N'Đơn hàng đã được xác nhận', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (3, N'Shipped', N'Đơn hàng đang được giao', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (4, N'Delivered', N'Đơn hàng đã giao thành công', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (5, N'Completed', N'Đơn hàng hoàn tất', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (6, N'Cancelled', N'Đơn hàng bị hủy', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
INSERT [dbo].[StatusOrders] ([StatusID], [StatusName], [Description], [CreatedAt]) VALUES (7, N'Refunded', N'Đã hoàn tiền', CAST(N'2026-03-04T18:51:25.607' AS DateTime))
SET IDENTITY_INSERT [dbo].[StatusOrders] OFF
GO
SET IDENTITY_INSERT [dbo].[SuperCategories] ON

INSERT [dbo].[SuperCategories] ([SuperCategoryID], [SuperCategoryName], [IsDeleted], [CreatedAt]) VALUES (1, N'Đồ chơi', 0, CAST(N'2026-03-04T18:51:25.603' AS DateTime))
SET IDENTITY_INSERT [dbo].[SuperCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Voucher] ON

INSERT [dbo].[Voucher] ([VoucherID], [VoucherTypeID], [CreateBy], [VoucherCode], [DiscountType], [DiscountValue], [MaxDiscountAmount], [MinOrderAmount], [UsageLimitPerUser], [IsStackable], [StartDate], [EndDate], [Status], [IsDeleted], [CreatedAt], [UpdatedAt]) VALUES (2, 1, NULL, N'MARCH20', N'Percentage', CAST(20.00 AS Decimal(12, 2)), CAST(200000.00 AS Decimal(12, 2)), CAST(100000.00 AS Decimal(12, 2)), NULL, 0, CAST(N'2026-03-01T11:11:00.000' AS DateTime), CAST(N'2026-03-31T11:11:00.000' AS DateTime), N'Active', 0, CAST(N'2026-03-07T11:11:25.157' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Voucher] OFF
GO
SET IDENTITY_INSERT [dbo].[VoucherTypes] ON

INSERT [dbo].[VoucherTypes] ([VoucherTypeID], [VoucherTypeName], [CreatedAt]) VALUES (1, N'Order', CAST(N'2026-03-07T11:10:43.417' AS DateTime))
INSERT [dbo].[VoucherTypes] ([VoucherTypeID], [VoucherTypeName], [CreatedAt]) VALUES (2, N'Shipping', CAST(N'2026-03-07T11:10:43.417' AS DateTime))
SET IDENTITY_INSERT [dbo].[VoucherTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Wallets] ON

INSERT [dbo].[Wallets] ([WalletID], [AccountID], [Currency], [Balance], [Status], [LastTransactionAt], [CreatedAt], [UpdatedAt]) VALUES (1, 4, N'VND', CAST(3342000.00 AS Decimal(14, 2)), N'Active', CAST(N'2026-03-07T14:07:45.817' AS DateTime), CAST(N'2026-03-04T12:35:54.843' AS DateTime), CAST(N'2026-03-07T14:07:45.817' AS DateTime))
SET IDENTITY_INSERT [dbo].[Wallets] OFF
GO
SET IDENTITY_INSERT [dbo].[WalletTransactions] ON

INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (1, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', NULL, N'TOPUP_4_b3d780602e4f410f94d473470c875479', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-04T12:35:54.863' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (2, 1, 4, N'TopUp', N'In', CAST(1000000.00 AS Decimal(14, 2)), CAST(0.00 AS Decimal(14, 2)), CAST(1000000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772628707378', N'TOPUP_4_069ced13cf724a72b7e14bd2afc70f71', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-04T12:51:43.867' AS DateTime), CAST(N'2026-03-04T12:51:47.423' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (3, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(1000000.00 AS Decimal(14, 2)), CAST(3000000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772628834938', N'TOPUP_4_045e8392147844f8aa627350004ac14e', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-04T12:53:51.663' AS DateTime), CAST(N'2026-03-04T12:53:54.957' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (5, 1, 4, N'Payment', N'Out', CAST(1120000.00 AS Decimal(14, 2)), CAST(3000000.00 AS Decimal(14, 2)), CAST(1880000.00 AS Decimal(14, 2)), 5, NULL, NULL, NULL, N'5_wallet_639082263408654820', N'Completed', N'Thanh toán đơn hàng #5', NULL, CAST(N'2026-03-04T13:05:40.867' AS DateTime), CAST(N'2026-03-04T13:05:40.867' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (6, 1, 4, N'Payment', N'Out', CAST(450000.00 AS Decimal(14, 2)), CAST(1880000.00 AS Decimal(14, 2)), CAST(1430000.00 AS Decimal(14, 2)), 6, NULL, NULL, NULL, N'6_wallet_639082265384616021', N'Completed', N'Thanh toán đơn hàng #6', NULL, CAST(N'2026-03-04T13:08:58.460' AS DateTime), CAST(N'2026-03-04T13:08:58.460' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (7, 1, 4, N'Payment', N'Out', CAST(450000.00 AS Decimal(14, 2)), CAST(1430000.00 AS Decimal(14, 2)), CAST(980000.00 AS Decimal(14, 2)), 7, NULL, NULL, NULL, N'7_wallet_639082271535155674', N'Completed', N'Thanh toán đơn hàng #7', NULL, CAST(N'2026-03-04T13:19:13.517' AS DateTime), CAST(N'2026-03-04T13:19:13.517' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (8, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(980000.00 AS Decimal(14, 2)), CAST(980000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_47e174562eab40e39d2eceb47528df6d', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-06T15:10:26.613' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (9, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(980000.00 AS Decimal(14, 2)), CAST(2980000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772809858499', N'TOPUP_4_35f17f7537b14768ab803bb55a8e3613', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-06T15:10:54.917' AS DateTime), CAST(N'2026-03-06T15:10:58.533' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (10, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(2980000.00 AS Decimal(14, 2)), CAST(4980000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772809868367', N'TOPUP_4_bc9049ff67164502bd0c41e551e7b70a', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-06T15:11:05.033' AS DateTime), CAST(N'2026-03-06T15:11:08.377' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (11, 1, 4, N'Payment', N'Out', CAST(3280000.00 AS Decimal(14, 2)), CAST(4980000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), 8, NULL, NULL, NULL, N'8_wallet_639084070988258921', N'Completed', N'Thanh toán đơn hàng #8', NULL, CAST(N'2026-03-06T15:18:18.827' AS DateTime), CAST(N'2026-03-06T15:18:18.827' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (12, 1, 4, N'TopUp', N'In', CAST(1000000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_c068eb2728a34b0c93a79c27f9f100e0', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T10:59:40.147' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (13, 1, 4, N'TopUp', N'In', CAST(500000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_c10cc03f60004807876219f1828e0bd0', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T11:00:56.877' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (14, 1, 4, N'Payment', N'Out', CAST(930000.00 AS Decimal(14, 2)), CAST(1700000.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), 9, NULL, NULL, NULL, N'9_wallet_639084787538220098', N'Completed', N'Thanh toán đơn hàng #9', NULL, CAST(N'2026-03-07T11:12:33.823' AS DateTime), CAST(N'2026-03-07T11:12:33.823' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (15, 1, 4, N'TopUp', N'In', CAST(1888888.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_9858733955194f5abae977b28de876c6', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T11:46:53.663' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (16, 1, 4, N'TopUp', N'In', CAST(500000.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_ce9db685dd6d453bb0d9abc2d411946b', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T11:47:25.357' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (17, 1, 4, N'Payment', N'Out', CAST(566000.00 AS Decimal(14, 2)), CAST(770000.00 AS Decimal(14, 2)), CAST(204000.00 AS Decimal(14, 2)), 10, NULL, NULL, NULL, N'10_wallet_639084809409961772', N'Completed', N'Thanh toán đơn hàng #10', NULL, CAST(N'2026-03-07T11:49:00.997' AS DateTime), CAST(N'2026-03-07T11:49:00.997' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (18, 1, 4, N'TopUp', N'In', CAST(2000000.00 AS Decimal(14, 2)), CAST(204000.00 AS Decimal(14, 2)), CAST(2204000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772884698970', N'TOPUP_4_ec696f7eba6f4f4ea92deaac93d4eef2', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-07T11:58:15.063' AS DateTime), CAST(N'2026-03-07T11:58:19.010' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (19, 1, 4, N'TopUp', N'In', CAST(500000.00 AS Decimal(14, 2)), CAST(2204000.00 AS Decimal(14, 2)), CAST(2704000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772884718877', N'TOPUP_4_4c9355ea9e39492dbd0a39d7d7f40f9e', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-07T11:58:35.430' AS DateTime), CAST(N'2026-03-07T11:58:38.887' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (20, 1, 4, N'TopUp', N'In', CAST(1000000.00 AS Decimal(14, 2)), CAST(2704000.00 AS Decimal(14, 2)), CAST(2704000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_866964382604462db92a9c6bf0692d89', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T13:21:49.860' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (21, 1, 4, N'TopUp', N'In', CAST(500000.00 AS Decimal(14, 2)), CAST(2704000.00 AS Decimal(14, 2)), CAST(3204000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772889739865', N'TOPUP_4_98270d9706d041debabb153051064526', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-07T13:22:16.507' AS DateTime), CAST(N'2026-03-07T13:22:19.903' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (22, 1, 4, N'TopUp', N'In', CAST(100000.00 AS Decimal(14, 2)), CAST(3204000.00 AS Decimal(14, 2)), CAST(3204000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_d21d6c2379d84a79a5dee82732100722', N'Failed', N'Hết thời gian chờ thanh toán', NULL, CAST(N'2026-03-07T14:04:55.793' AS DateTime), NULL)
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (23, 1, 4, N'TopUp', N'In', CAST(50000.00 AS Decimal(14, 2)), CAST(3204000.00 AS Decimal(14, 2)), CAST(3254000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772892400292', N'TOPUP_4_e643f840338745299a5d698725deec5d', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-07T14:06:36.683' AS DateTime), CAST(N'2026-03-07T14:06:40.327' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (24, 1, 4, N'TopUp', N'In', CAST(88000.00 AS Decimal(14, 2)), CAST(3254000.00 AS Decimal(14, 2)), CAST(3342000.00 AS Decimal(14, 2)), NULL, NULL, N'Sepay', N'MOCK-TXN-1772892465805', N'TOPUP_4_c457368da89648e9b7691acc823d26d6', N'Completed', N'Nạp tiền vào ví qua Sepay', NULL, CAST(N'2026-03-07T14:07:42.223' AS DateTime), CAST(N'2026-03-07T14:07:45.817' AS DateTime))
INSERT [dbo].[WalletTransactions] ([WalletTransactionID], [WalletID], [AccountID], [TxnType], [Direction], [Amount], [BalanceBefore], [BalanceAfter], [RelatedOrderID], [RelatedPaymentHistoryID], [Method], [ExternalRef], [IdempotencyKey], [Status], [Reason], [Metadata], [CreatedAt], [CompletedAt]) VALUES (25, 1, 4, N'TopUp', N'In', CAST(199000.00 AS Decimal(14, 2)), CAST(3342000.00 AS Decimal(14, 2)), CAST(3342000.00 AS Decimal(14, 2)), NULL, NULL, N'VNPay', NULL, N'TOPUP_4_9f6ba971f10f404d935f559a4c992f2b', N'Pending', N'Nạp tiền vào ví qua VNPay', NULL, CAST(N'2026-03-07T15:00:02.073' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[WalletTransactions] OFF
GO
SET IDENTITY_INSERT [Notification].[Deliveries] ON

INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (1, 4, NULL, N'ORDER_CONFIRMED', N'Đơn hàng #ORD000001 đã được xác nhận', N'Chào Quang Ducky, đơn hàng #ORD000001 của bạn đã được xác nhận.', N'{"type":"order","orderId":1,"orderCode":"ORD000001","status":"confirmed","link":"/orders/1"}', N'Read', CAST(N'2026-03-04T11:58:59.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (2, 4, NULL, N'ORDER_SHIPPED', N'Đơn hàng #ORD000001 đang được giao', N'Đơn hàng #ORD000001 đang được giao cho đơn vị vận chuyển .', N'{"type":"order","orderId":1,"orderCode":"ORD000001","status":"shipped","link":"/orders/1"}', N'Read', CAST(N'2026-03-04T12:01:55.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (3, 4, NULL, N'ORDER_DELIVERED', N'Đơn hàng #ORD000001 đã giao thành công', N'Cảm ơn bạn đã mua hàng!', N'{"type":"order","orderId":1,"orderCode":"ORD000001","status":"delivered","link":"/orders/1"}', N'Read', CAST(N'2026-03-04T12:02:48.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (4, 4, NULL, N'ORDER_CONFIRMED', N'Đơn hàng #ORD000008 đã được xác nhận', N'Chào Quang Ducky, đơn hàng #ORD000008 của bạn đã được xác nhận.', N'{"type":"order","orderId":8,"orderCode":"ORD000008","status":"confirmed","link":"/orders/8"}', N'Read', CAST(N'2026-03-06T15:21:13.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (5, 4, NULL, N'ORDER_SHIPPED', N'Đơn hàng #ORD000008 đang được giao', N'Đơn hàng #ORD000008 đang được giao cho đơn vị vận chuyển .', N'{"type":"order","orderId":8,"orderCode":"ORD000008","status":"shipped","link":"/orders/8"}', N'Read', CAST(N'2026-03-06T15:23:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (6, 4, NULL, N'ORDER_DELIVERED', N'Đơn hàng #ORD000008 đã giao thành công', N'Cảm ơn bạn đã mua hàng!', N'{"type":"order","orderId":8,"orderCode":"ORD000008","status":"delivered","link":"/orders/8"}', N'Read', CAST(N'2026-03-06T15:23:35.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [Notification].[Deliveries] ([DeliveryID], [AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status], [CreatedAt], [ImageUrl], [ActionType], [ActionTarget], [CampaignID]) VALUES (9, 4, NULL, N'REVIEW_REJECTED', N'Đánh giá bị từ chối: Lego Fire Station', N'Đánh giá của bạn về ''Lego Fire Station'' đã bị từ chối. Lý do: Phát hiện từ ngữ không phù hợp: ''địt''', N'{"reviewId":8,"productName":"Lego Fire Station","reason":"Phát hiện từ ngữ không phù hợp: \u0027địt\u0027"}', N'Unread', CAST(N'2026-03-07T14:08:51.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [Notification].[Deliveries] OFF
GO
SET IDENTITY_INSERT [Notification].[Templates] ON

INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (1, N'ORDER_CONFIRMED', N'Đơn hàng #{{orderCode}} đã được xác nhận', N'Chào {{customerName}}, đơn hàng #{{orderCode}} của bạn đã được xác nhận.', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (2, N'ORDER_SHIPPED', N'Đơn hàng #{{orderCode}} đang được giao', N'Đơn hàng #{{orderCode}} đang được giao cho đơn vị vận chuyển {{shippingUnit}}.', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (3, N'ORDER_DELIVERED', N'Đơn hàng #{{orderCode}} đã giao thành công', N'Cảm ơn bạn đã mua hàng!', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (4, N'PROMOTION', N'🎁 Khuyến mãi: {{promotionName}}', N'{{description}}', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (5, N'WELCOME', N'Chào mừng {{userName}} đến với LorKingDom!', N'Chúc mừng bạn đã tạo tài khoản thành công.', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (6, N'PAYMENT_SUCCESS', N'Thanh toán thành công #{{paymentId}}', N'Giao dịch thanh toán {{amount}} VNĐ thành công.', 1, CAST(N'2026-03-04T18:51:26.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (7, N'REVIEW_REJECTED', N'Đánh giá của bạn bị từ chối', N'Đánh giá của bạn về ''#{productName}'' đã bị từ chối. Lý do: #{reason}', 1, CAST(N'2026-03-07T21:03:21.0000000' AS DateTime2), NULL)
INSERT [Notification].[Templates] ([TemplateID], [TemplateCode], [TitleTemplate], [MessageTemplate], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (8, N'CUSTOM', N'#{title}', N'#{message}', 1, CAST(N'2026-03-07T21:03:21.0000000' AS DateTime2), NULL)
SET IDENTITY_INSERT [Notification].[Templates] OFF
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Accounts__A9D10534BA52CC81] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Accounts] ADD UNIQUE NONCLUSTERED
(
[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Ages__E0EBEE3851E4357C] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Ages] ADD UNIQUE NONCLUSTERED
(
[AgeRange] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__BlogCate__06725EA7DDEC02DE] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[BlogCategories] ADD UNIQUE NONCLUSTERED
(
[BlogCategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Brands__2206CE9B47DC91CB] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Brands] ADD UNIQUE NONCLUSTERED
(
[BrandName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Origins__636F5CFD4ED34B5B] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Origins] ADD UNIQUE NONCLUSTERED
(
[OriginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Products__CA1ECF0D17E14C64] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Products] ADD UNIQUE NONCLUSTERED
(
[SKU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Roles__8A2B6160C074306E] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED
(
[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Sexes__BA354290D90FBDCB] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Sexes] ADD UNIQUE NONCLUSTERED
(
[SexName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__SuperCat__3FA779DF6DB33AF1] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[SuperCategories] ADD UNIQUE NONCLUSTERED
(
[SuperCategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Voucher__7F0ABCA99BD70956] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [dbo].[Voucher] ADD UNIQUE NONCLUSTERED
(
[VoucherCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Template__0FDB508167916948] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [Notification].[Templates] ADD UNIQUE NONCLUSTERED
(
[TemplateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/**\*\*** Object: Index [UQ__Backgrou__F1AC1A956E07DD5A] Script Date: 09/03/2026 7:20:57 AM **\*\***/
ALTER TABLE [System].[BackgroundJobs] ADD UNIQUE NONCLUSTERED
(
[JobName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Accounts] ADD DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Accounts] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Addresses] ADD DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[Addresses] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Addresses] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Ages] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Ages] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlogCategories] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BlogCategories] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlogPosts] ADD DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[BlogPosts] ADD DEFAULT ((0)) FOR [IsFeatured]
GO
ALTER TABLE [dbo].[BlogPosts] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BlogPosts] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Brands] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Brands] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Cart] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CartItems] ADD DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[CartItems] ADD DEFAULT (getdate()) FOR [AddedAt]
GO
ALTER TABLE [dbo].[Categories] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Categories] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[EmailOtps] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[EmailOtps] ADD DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[ExternalApiLogs] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExternalLogins] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Materials] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Materials] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderDetails] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[OrderDetails] ADD DEFAULT ((0)) FOR [Reviewed]
GO
ALTER TABLE [dbo].[OrderDetails] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD DEFAULT ('Wallet') FOR [RefundMode]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD DEFAULT ('Requested') FOR [RefundStatus]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD DEFAULT ((0)) FOR [RefundAmount]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT ((0)) FOR [ShippingFee]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT ((0)) FOR [PaidByWalletAmount]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT ((0)) FOR [PaidByExternalAmount]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT ('None') FOR [RefundStatus]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Orders] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderStatusHistory] ADD DEFAULT (getdate()) FOR [ChangedAt]
GO
ALTER TABLE [dbo].[OrderStatusHistory] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Origins] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Origins] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD DEFAULT ('Failed') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD DEFAULT ('VND') FOR [Currency]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PriceRanges] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PriceRanges] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ProductImages] ADD DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [dbo].[ProductImages] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Products] ADD DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Products] ADD DEFAULT ('Available') FOR [ProductStatus]
GO
ALTER TABLE [dbo].[Products] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Products] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogReactions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogReplies] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogs] ADD DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[ReviewBlogs] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewModerationLogs] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductImages] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductImages] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductReactions] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductReactions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductReplies] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductReplies] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT ((0)) FOR [IsVerifiedPurchase]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT ('AuthorOnly') FOR [Visibility]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT ((0)) FOR [EditCount]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Roles] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Sexes] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Sexes] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] ADD DEFAULT ((0)) FOR [RetryCount]
GO
ALTER TABLE [dbo].[ShippingStatusHistories] ADD DEFAULT (getutcdate()) FOR [ProcessedAt]
GO
ALTER TABLE [dbo].[StatusOrders] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SuperCategories] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SuperCategories] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemConfigs] ADD DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Voucher] ADD DEFAULT ('Fixed') FOR [DiscountType]
GO
ALTER TABLE [dbo].[Voucher] ADD DEFAULT ((0)) FOR [IsStackable]
GO
ALTER TABLE [dbo].[Voucher] ADD DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Voucher] ADD DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Voucher] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[VoucherTypes] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Wallets] ADD DEFAULT ('VND') FOR [Currency]
GO
ALTER TABLE [dbo].[Wallets] ADD DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[Wallets] ADD DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Wallets] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD DEFAULT ('Completed') FOR [Status]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[WebhookEvents] ADD DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[WebhookEvents] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Wishlists] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [Notification].[Campaigns] ADD DEFAULT ('ADMIN') FOR [SourceType]
GO
ALTER TABLE [Notification].[Campaigns] ADD DEFAULT ('ALL') FOR [TargetType]
GO
ALTER TABLE [Notification].[Campaigns] ADD DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [Notification].[Campaigns] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [Notification].[Deliveries] ADD DEFAULT ('Unread') FOR [Status]
GO
ALTER TABLE [Notification].[Deliveries] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [Notification].[DeliveryActions] ADD DEFAULT (getdate()) FOR [OccurredAt]
GO
ALTER TABLE [Notification].[Templates] ADD DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Notification].[Templates] ADD DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [System].[BackgroundJobs] ADD DEFAULT ((1)) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[Accounts] WITH CHECK ADD CONSTRAINT [FK_Accounts_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Roles]
GO
ALTER TABLE [dbo].[Addresses] WITH CHECK ADD CONSTRAINT [FK_Addresses_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addresses] CHECK CONSTRAINT [FK_Addresses_Accounts]
GO
ALTER TABLE [dbo].[BlogPostCategories] WITH CHECK ADD CONSTRAINT [FK_BlogPostCategories_Categories] FOREIGN KEY([BlogCategoryID])
REFERENCES [dbo].[BlogCategories] ([BlogCategoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlogPostCategories] CHECK CONSTRAINT [FK_BlogPostCategories_Categories]
GO
ALTER TABLE [dbo].[BlogPostCategories] WITH CHECK ADD CONSTRAINT [FK_BlogPostCategories_Posts] FOREIGN KEY([BlogPostID])
REFERENCES [dbo].[BlogPosts] ([BlogPostID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlogPostCategories] CHECK CONSTRAINT [FK_BlogPostCategories_Posts]
GO
ALTER TABLE [dbo].[BlogPosts] WITH CHECK ADD CONSTRAINT [FK_BlogPosts_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[BlogPosts] CHECK CONSTRAINT [FK_BlogPosts_Accounts]
GO
ALTER TABLE [dbo].[Cart] WITH CHECK ADD CONSTRAINT [FK_Cart_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_Cart_Accounts]
GO
ALTER TABLE [dbo].[CartItems] WITH CHECK ADD CONSTRAINT [FK_CartItems_Cart] FOREIGN KEY([CartID])
REFERENCES [dbo].[Cart] ([CartID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Cart]
GO
ALTER TABLE [dbo].[CartItems] WITH CHECK ADD CONSTRAINT [FK_CartItems_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Products]
GO
ALTER TABLE [dbo].[Categories] WITH CHECK ADD CONSTRAINT [FK_Categories_SuperCategories] FOREIGN KEY([SuperCategoryID])
REFERENCES [dbo].[SuperCategories] ([SuperCategoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_SuperCategories]
GO
ALTER TABLE [dbo].[EmailOtps] WITH CHECK ADD CONSTRAINT [FK_EmailOtps_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailOtps] CHECK CONSTRAINT [FK_EmailOtps_Accounts]
GO
ALTER TABLE [dbo].[ExternalLogins] WITH CHECK ADD CONSTRAINT [FK_ExternalLogins_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExternalLogins] CHECK CONSTRAINT [FK_ExternalLogins_Accounts]
GO
ALTER TABLE [dbo].[OrderDetails] WITH CHECK ADD CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders]
GO
ALTER TABLE [dbo].[OrderDetails] WITH CHECK ADD CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Products]
GO
ALTER TABLE [dbo].[OrderRefunds] WITH CHECK ADD CONSTRAINT [FK_Refunds_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_Accounts]
GO
ALTER TABLE [dbo].[OrderRefunds] WITH CHECK ADD CONSTRAINT [FK_Refunds_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_Orders]
GO
ALTER TABLE [dbo].[Orders] WITH CHECK ADD CONSTRAINT [FK_Orders_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Accounts]
GO
ALTER TABLE [dbo].[Orders] WITH CHECK ADD CONSTRAINT [FK_Orders_StatusOrder] FOREIGN KEY([StatusID])
REFERENCES [dbo].[StatusOrders] ([StatusID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_StatusOrder]
GO
ALTER TABLE [dbo].[Orders] WITH CHECK ADD CONSTRAINT [FK_Orders_Vouchers] FOREIGN KEY([VoucherID])
REFERENCES [dbo].[Voucher] ([VoucherID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Vouchers]
GO
ALTER TABLE [dbo].[OrderStatusHistory] WITH CHECK ADD CONSTRAINT [FK_OSH_ChangedBy] FOREIGN KEY([ChangedBy])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_ChangedBy]
GO
ALTER TABLE [dbo].[OrderStatusHistory] WITH CHECK ADD CONSTRAINT [FK_OSH_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_Orders]
GO
ALTER TABLE [dbo].[OrderStatusHistory] WITH CHECK ADD CONSTRAINT [FK_OSH_StatusOrder] FOREIGN KEY([StatusID])
REFERENCES [dbo].[StatusOrders] ([StatusID])
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_StatusOrder]
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions] WITH CHECK ADD CONSTRAINT [FK_GatewayTxn_PaymentHistory] FOREIGN KEY([PaymentHistoryID])
REFERENCES [dbo].[PaymentHistory] ([PaymentHistoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions] CHECK CONSTRAINT [FK_GatewayTxn_PaymentHistory]
GO
ALTER TABLE [dbo].[PaymentHistory] WITH CHECK ADD CONSTRAINT [FK_PaymentHistory_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_Accounts]
GO
ALTER TABLE [dbo].[PaymentHistory] WITH CHECK ADD CONSTRAINT [FK_PaymentHistory_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_Orders]
GO
ALTER TABLE [dbo].[PaymentHistory] WITH CHECK ADD CONSTRAINT [FK_PaymentHistory_WalletTxn] FOREIGN KEY([WalletTransactionID])
REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID])
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_WalletTxn]
GO
ALTER TABLE [dbo].[ProductImages] WITH CHECK ADD CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductImages] CHECK CONSTRAINT [FK_ProductImages_Products]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Ages] FOREIGN KEY([AgeID])
REFERENCES [dbo].[Ages] ([AgeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Ages]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Brands] FOREIGN KEY([BrandID])
REFERENCES [dbo].[Brands] ([BrandID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Brands]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Materials] FOREIGN KEY([MaterialID])
REFERENCES [dbo].[Materials] ([MaterialID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Materials]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Origins] FOREIGN KEY([OriginID])
REFERENCES [dbo].[Origins] ([OriginID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Origins]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_PriceRanges] FOREIGN KEY([PriceRangeID])
REFERENCES [dbo].[PriceRanges] ([PriceRangeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_PriceRanges]
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CONSTRAINT [FK_Products_Sexes] FOREIGN KEY([SexID])
REFERENCES [dbo].[Sexes] ([SexID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Sexes]
GO
ALTER TABLE [dbo].[ReviewBlogReactions] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogReactions_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewBlogReactions] CHECK CONSTRAINT [FK_ReviewBlogReactions_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogReactions] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs] FOREIGN KEY([ReviewBlogID])
REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
GO
ALTER TABLE [dbo].[ReviewBlogReactions] CHECK CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs]
GO
ALTER TABLE [dbo].[ReviewBlogReplies] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogReplies_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewBlogReplies] CHECK CONSTRAINT [FK_ReviewBlogReplies_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogReplies] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs] FOREIGN KEY([ReviewBlogID])
REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
GO
ALTER TABLE [dbo].[ReviewBlogReplies] CHECK CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs]
GO
ALTER TABLE [dbo].[ReviewBlogs] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogs_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewBlogs] CHECK CONSTRAINT [FK_ReviewBlogs_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogs] WITH CHECK ADD CONSTRAINT [FK_ReviewBlogs_BlogPosts] FOREIGN KEY([BlogPostID])
REFERENCES [dbo].[BlogPosts] ([BlogPostID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewBlogs] CHECK CONSTRAINT [FK_ReviewBlogs_BlogPosts]
GO
ALTER TABLE [dbo].[ReviewModerationLogs] WITH CHECK ADD CONSTRAINT [FK_RML_ReviewProduct] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewModerationLogs] CHECK CONSTRAINT [FK_RML_ReviewProduct]
GO
ALTER TABLE [dbo].[ReviewProductImages] WITH CHECK ADD CONSTRAINT [FK_ReviewProdImages_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewProductImages] CHECK CONSTRAINT [FK_ReviewProdImages_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProductReactions] WITH CHECK ADD CONSTRAINT [FK_ReviewProdReactions_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewProductReactions] CHECK CONSTRAINT [FK_ReviewProdReactions_Accounts]
GO
ALTER TABLE [dbo].[ReviewProductReactions] WITH CHECK ADD CONSTRAINT [FK_ReviewProdReactions_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewProductReactions] CHECK CONSTRAINT [FK_ReviewProdReactions_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProductReplies] WITH CHECK ADD CONSTRAINT [FK_ReviewProdReplies_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewProductReplies] CHECK CONSTRAINT [FK_ReviewProdReplies_Accounts]
GO
ALTER TABLE [dbo].[ReviewProductReplies] WITH CHECK ADD CONSTRAINT [FK_ReviewProdReplies_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewProductReplies] CHECK CONSTRAINT [FK_ReviewProdReplies_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProducts] WITH CHECK ADD CONSTRAINT [FK_ReviewProducts_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_Accounts]
GO
ALTER TABLE [dbo].[ReviewProducts] WITH CHECK ADD CONSTRAINT [FK_ReviewProducts_OrderDetails] FOREIGN KEY([OrderDetailID])
REFERENCES [dbo].[OrderDetails] ([OrderDetailID])
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_OrderDetails]
GO
ALTER TABLE [dbo].[ReviewProducts] WITH CHECK ADD CONSTRAINT [FK_ReviewProducts_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_Products]
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] WITH CHECK ADD CONSTRAINT [FK_ShippingTxn_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] CHECK CONSTRAINT [FK_ShippingTxn_Orders]
GO
ALTER TABLE [dbo].[ShippingStatusHistories] WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[ShippingStatusHistories] WITH CHECK ADD FOREIGN KEY([ShippingTxId])
REFERENCES [dbo].[ShippingProviderTransactions] ([ShippingTransactionID])
GO
ALTER TABLE [dbo].[Voucher] WITH CHECK ADD CONSTRAINT [FK_Voucher_Accounts_CreateBy] FOREIGN KEY([CreateBy])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_Accounts_CreateBy]
GO
ALTER TABLE [dbo].[Voucher] WITH CHECK ADD CONSTRAINT [FK_Voucher_VoucherTypes] FOREIGN KEY([VoucherTypeID])
REFERENCES [dbo].[VoucherTypes] ([VoucherTypeID])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_VoucherTypes]
GO
ALTER TABLE [dbo].[Wallets] WITH CHECK ADD CONSTRAINT [FK_Wallets_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wallets] CHECK CONSTRAINT [FK_Wallets_Accounts]
GO
ALTER TABLE [dbo].[WalletTransactions] WITH CHECK ADD CONSTRAINT [FK_WalletTxn_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Accounts]
GO
ALTER TABLE [dbo].[WalletTransactions] WITH CHECK ADD CONSTRAINT [FK_WalletTxn_Orders] FOREIGN KEY([RelatedOrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Orders]
GO
ALTER TABLE [dbo].[WalletTransactions] WITH CHECK ADD CONSTRAINT [FK_WalletTxn_Wallets] FOREIGN KEY([WalletID])
REFERENCES [dbo].[Wallets] ([WalletID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Wallets]
GO
ALTER TABLE [dbo].[Wishlists] WITH CHECK ADD CONSTRAINT [FK_Wishlists_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlists] CHECK CONSTRAINT [FK_Wishlists_Accounts]
GO
ALTER TABLE [dbo].[Wishlists] WITH CHECK ADD CONSTRAINT [FK_Wishlists_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlists] CHECK CONSTRAINT [FK_Wishlists_Products]
GO
ALTER TABLE [Notification].[Campaigns] WITH CHECK ADD CONSTRAINT [FK_Campaigns_Accounts] FOREIGN KEY([CreatedByAccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [Notification].[Campaigns] CHECK CONSTRAINT [FK_Campaigns_Accounts]
GO
ALTER TABLE [Notification].[Campaigns] WITH CHECK ADD CONSTRAINT [FK_Campaigns_Templates] FOREIGN KEY([TemplateCode])
REFERENCES [Notification].[Templates] ([TemplateCode])
ON DELETE SET NULL
GO
ALTER TABLE [Notification].[Campaigns] CHECK CONSTRAINT [FK_Campaigns_Templates]
GO
ALTER TABLE [Notification].[CampaignTargets] WITH CHECK ADD CONSTRAINT [FK_CampaignTargets_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [Notification].[Campaigns] ([CampaignID])
ON DELETE CASCADE
GO
ALTER TABLE [Notification].[CampaignTargets] CHECK CONSTRAINT [FK_CampaignTargets_Campaigns]
GO
ALTER TABLE [Notification].[Deliveries] WITH CHECK ADD CONSTRAINT [FK_NotificationDeliveries_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_Accounts]
GO
ALTER TABLE [Notification].[Deliveries] WITH CHECK ADD CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs] FOREIGN KEY([CreatedByJobID])
REFERENCES [System].[BackgroundJobs] ([JobID])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs]
GO
ALTER TABLE [Notification].[Deliveries] WITH CHECK ADD CONSTRAINT [FK_NotificationDeliveries_Campaigns] FOREIGN KEY([CampaignID])
REFERENCES [Notification].[Campaigns] ([CampaignID])
ON DELETE SET NULL
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_Campaigns]
GO
ALTER TABLE [Notification].[Deliveries] WITH CHECK ADD CONSTRAINT [FK_NotificationDeliveries_Templates] FOREIGN KEY([TemplateCode])
REFERENCES [Notification].[Templates] ([TemplateCode])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_Templates]
GO
ALTER TABLE [Notification].[DeliveryActions] WITH CHECK ADD CONSTRAINT [FK_DeliveryActions_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [Notification].[DeliveryActions] CHECK CONSTRAINT [FK_DeliveryActions_Accounts]
GO
ALTER TABLE [Notification].[DeliveryActions] WITH CHECK ADD CONSTRAINT [FK_DeliveryActions_Deliveries] FOREIGN KEY([DeliveryID])
REFERENCES [Notification].[Deliveries] ([DeliveryID])
ON DELETE CASCADE
GO
ALTER TABLE [Notification].[DeliveryActions] CHECK CONSTRAINT [FK_DeliveryActions_Deliveries]
GO
ALTER TABLE [dbo].[CartItems] WITH CHECK ADD CHECK (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderDetails] WITH CHECK ADD CHECK (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CHECK (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Products] WITH CHECK ADD CHECK (([Quantity]>=(0)))
GO
ALTER TABLE [dbo].[ReviewBlogs] WITH CHECK ADD CHECK (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[ReviewProducts] WITH CHECK ADD CHECK (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[WalletTransactions] WITH CHECK ADD CHECK (([Amount]>(0)))
GO
